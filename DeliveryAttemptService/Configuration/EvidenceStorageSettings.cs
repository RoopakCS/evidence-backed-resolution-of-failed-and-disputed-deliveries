namespace DeliveryAttemptService.Configuration;

public sealed class EvidenceStorageSettings
{
    public const string SectionName = "EvidenceStorage";

    public string BucketName { get; set; } = "docket-evidence-dev";

    public string Region { get; set; } = "us-east-1";

    public int UploadUrlExpiryMinutes { get; set; } = 15;

    public int DownloadUrlExpiryMinutes { get; set; } = 15;
}
