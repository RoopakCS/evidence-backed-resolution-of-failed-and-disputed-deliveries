using System.Net;
using System.Security.Cryptography;
using Amazon.S3;
using Amazon.S3.Model;
using DeliveryAttemptService.Configuration;
using DeliveryAttemptService.DTOs.Requests;
using DeliveryAttemptService.DTOs.Responses;
using DeliveryAttemptService.Exceptions;
using DeliveryAttemptService.Models;
using DeliveryAttemptService.Repositories.Interfaces;
using DeliveryAttemptService.Services.Interfaces;
using DeliveryAttemptService.Services.Mapping;
using Microsoft.Extensions.Options;

namespace DeliveryAttemptService.Services.Implementations;

public class EvidenceService : IEvidenceService
{
    private readonly IDeliveryAttemptRepository _attempts;
    private readonly IAssignmentRepository _assignments;
    private readonly IAmazonS3 _s3;
    private readonly EvidenceStorageSettings _storage;

    public EvidenceService(
        IDeliveryAttemptRepository attempts,
        IAssignmentRepository assignments,
        IAmazonS3 s3,
        IOptions<EvidenceStorageSettings> storageOptions)
    {
        _attempts = attempts;
        _assignments = assignments;
        _s3 = s3;
        _storage = storageOptions.Value;
    }

    public async Task<CreateEvidenceUploadUrlResponse> CreateUploadUrlAsync(
        Guid driverId,
        CreateEvidenceUploadUrlRequest request,
        CancellationToken ct = default)
    {
        var attempt = await _attempts.GetAttemptByIdAsync(request.AttemptId, includeEvidence: false, ct)
            ?? throw new ServiceException(StatusCodes.Status404NotFound, "Delivery attempt not found.");

        if (attempt.DriverId != driverId)
        {
            throw new ServiceException(
                StatusCodes.Status403Forbidden,
                "You can only upload evidence for your own delivery attempts.");
        }

        var evidenceId = Guid.NewGuid();
        var safeFileName = SanitizeFileName(request.FileName);
        var objectKey = $"attempts/{attempt.Id}/{evidenceId}-{safeFileName}";
        var now = DateTime.UtcNow;
        var expiration = now.AddMinutes(_storage.UploadUrlExpiryMinutes);

        var evidence = new Evidence
        {
            Id = evidenceId,
            AttemptId = attempt.Id,
            ObjectKey = objectKey,
            ContentHash = string.Empty,
            MediaType = request.MediaType.Trim(),
            FileSize = request.FileSize,
            UploadedAt = now,
            CapturedAt = attempt.CapturedAt,
            EvidenceType = request.EvidenceType,
            CreatedByDriverId = driverId,
            IsConfirmed = false
        };

        var uploadUrl = BuildPresignedUrl(objectKey, HttpVerb.PUT, expiration, evidence.MediaType);
        await _attempts.AddEvidenceAsync(evidence, ct);
        await _attempts.SaveChangesAsync(ct);

        return new CreateEvidenceUploadUrlResponse
        {
            EvidenceId = evidence.Id,
            UploadUrl = uploadUrl,
            ObjectKey = objectKey,
            Expiration = expiration,
            RequiredUploadHeaders = new Dictionary<string, string>
            {
                ["Content-Type"] = evidence.MediaType
            }
        };
    }

    public async Task<EvidenceResponse> ConfirmUploadAsync(
        Guid driverId,
        ConfirmEvidenceUploadRequest request,
        CancellationToken ct = default)
    {
        var evidence = await _attempts.GetEvidenceByIdAsync(request.EvidenceId, ct)
            ?? throw new ServiceException(StatusCodes.Status404NotFound, "Evidence record not found.");

        if (evidence.CreatedByDriverId != driverId)
        {
            throw new ServiceException(
                StatusCodes.Status403Forbidden,
                "You can only confirm evidence you initiated.");
        }

        var normalizedHash = request.ContentHash.Trim().ToLowerInvariant();
        if (normalizedHash.Length != 64 || normalizedHash.Any(c => !Uri.IsHexDigit(c)))
        {
            throw new ServiceException(
                StatusCodes.Status400BadRequest,
                "ContentHash must be a 64-character SHA-256 hex string.");
        }

        if (evidence.IsConfirmed)
        {
            if (string.Equals(evidence.ContentHash, normalizedHash, StringComparison.OrdinalIgnoreCase))
            {
                var existingDownloadUrl = BuildPresignedUrl(
                    evidence.ObjectKey,
                    HttpVerb.GET,
                    DateTime.UtcNow.AddMinutes(_storage.DownloadUrlExpiryMinutes));
                return AttemptMapper.ToResponse(evidence, existingDownloadUrl);
            }

            throw new ServiceException(
                StatusCodes.Status409Conflict,
                "Evidence upload has already been confirmed with a different content hash.");
        }

        if (request.UploadConfirmation)
        {
            try
            {
                using var storedObject = await _s3.GetObjectAsync(new GetObjectRequest
                {
                    BucketName = _storage.BucketName,
                    Key = evidence.ObjectKey
                }, ct);

                if (storedObject.ContentLength != evidence.FileSize)
                {
                    throw new ServiceException(
                        StatusCodes.Status409Conflict,
                        "Uploaded file size does not match the requested size.");
                }

                var actualHash = Convert.ToHexStringLower(
                    await SHA256.HashDataAsync(storedObject.ResponseStream, ct));
                if (!string.Equals(actualHash, normalizedHash, StringComparison.Ordinal))
                {
                    throw new ServiceException(
                        StatusCodes.Status409Conflict,
                        "Uploaded file content does not match the supplied SHA-256 hash.");
                }
            }
            catch (AmazonS3Exception ex) when (
                ex.StatusCode == HttpStatusCode.NotFound ||
                string.Equals(ex.ErrorCode, "NoSuchKey", StringComparison.Ordinal))
            {
                throw new ServiceException(
                    StatusCodes.Status409Conflict,
                    "The evidence object has not been uploaded to storage.");
            }

            evidence.ContentHash = normalizedHash;
            evidence.IsConfirmed = true;
            evidence.ConfirmedAt = DateTime.UtcNow;
        }

        await _attempts.SaveChangesAsync(ct);

        var downloadUrl = evidence.IsConfirmed
            ? BuildPresignedUrl(
                evidence.ObjectKey,
                HttpVerb.GET,
                DateTime.UtcNow.AddMinutes(_storage.DownloadUrlExpiryMinutes))
            : null;

        return AttemptMapper.ToResponse(evidence, downloadUrl);
    }

    public async Task<List<EvidenceResponse>> GetEvidenceByAttemptAsync(
        Guid attemptId,
        Guid callerId,
        string callerRole,
        CancellationToken ct = default)
    {
        var attempt = await _attempts.GetAttemptByIdAsync(attemptId, includeEvidence: false, ct)
            ?? throw new ServiceException(StatusCodes.Status404NotFound, "Delivery attempt not found.");

        if (string.Equals(callerRole, "Driver", StringComparison.OrdinalIgnoreCase) && attempt.DriverId != callerId)
        {
            throw new ServiceException(
                StatusCodes.Status403Forbidden,
                "You can only view evidence for your own delivery attempts.");
        }
        else if (string.Equals(callerRole, "Customer", StringComparison.OrdinalIgnoreCase) &&
                 await _assignments.GetByShipmentAndCustomerAsync(attempt.ShipmentId, callerId, ct) is null)
        {
            throw new ServiceException(
                StatusCodes.Status403Forbidden,
                "You can only view evidence for your own shipments.");
        }

        var items = await _attempts.GetEvidenceByAttemptIdAsync(attemptId, ct);
        var expiry = DateTime.UtcNow.AddMinutes(_storage.DownloadUrlExpiryMinutes);

        return items.Select(e => AttemptMapper.ToResponse(
            e,
            e.IsConfirmed ? BuildPresignedUrl(e.ObjectKey, HttpVerb.GET, expiry) : null)).ToList();
    }

    private string BuildPresignedUrl(
        string objectKey,
        HttpVerb verb,
        DateTime expiresAtUtc,
        string? contentType = null)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = _storage.BucketName,
            Key = objectKey,
            Verb = verb,
            Expires = expiresAtUtc
        };

        if (!string.IsNullOrWhiteSpace(contentType))
            request.ContentType = contentType;

        return _s3.GetPreSignedURL(request);
    }

    private static string SanitizeFileName(string fileName)
    {
        var name = Path.GetFileName(fileName.Trim());
        if (string.IsNullOrWhiteSpace(name))
            return "evidence.bin";

        var invalid = Path.GetInvalidFileNameChars();
        var cleaned = new string(name.Select(c => invalid.Contains(c) || char.IsWhiteSpace(c) ? '_' : c).ToArray());
        return cleaned.Length > 100 ? cleaned[..100] : cleaned;
    }
}
