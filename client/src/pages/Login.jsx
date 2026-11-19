import { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import {
  loginUser,
  getStoredSession,
  clearAuthSession,
} from '../services/authService'
import './Login.css'

export default function Login({ onLoginSuccess }) {
  const navigate = useNavigate()
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [showPassword, setShowPassword] = useState(false)
  const [isLoading, setIsLoading] = useState(false)
  const [errorMessage, setErrorMessage] = useState('')
  const [successMessage, setSuccessMessage] = useState('')
  const [fieldErrors, setFieldErrors] = useState({})

  const [currentUser, setCurrentUser] = useState(() => {
    const session = getStoredSession()
    return session ? session.user : null
  })

  const validateForm = () => {
    const errors = {}
    const trimmedEmail = email.trim()

    if (!trimmedEmail) {
      errors.email = 'Email address is required'
    } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(trimmedEmail)) {
      errors.email = 'Please enter a valid email address'
    }

    if (!password) {
      errors.password = 'Password is required'
    }

    setFieldErrors(errors)
    return Object.keys(errors).length === 0
  }

  const handleSubmit = async (e) => {
    e.preventDefault()
    setErrorMessage('')
    setSuccessMessage('')

    if (!validateForm()) {
      return
    }

    setIsLoading(true)

    try {
      const result = await loginUser({ email, password })

      if (result.success) {
        setSuccessMessage(result.message || 'Login successful. Redirecting...')
        setCurrentUser(result.user)

        if (onLoginSuccess) {
          onLoginSuccess(result)
        }
        navigate('/home')
      } else {
        setErrorMessage(
          result.message || 'Authentication failed. Please check your credentials.'
        )
      }
    } catch {
      setErrorMessage(
        'Unable to reach server. Please ensure the backend is running on http://localhost:5077.'
      )
    } finally {
      setIsLoading(false)
    }
  }

  const handleLogout = () => {
    clearAuthSession()
    setCurrentUser(null)
    setSuccessMessage('')
    setErrorMessage('')
  }

  return (
    <div className="login-page-container">
      <div className="login-card-wrapper">
        <div className="login-header-group">
          <h1 className="login-headline">Welcome back</h1>
          <p className="login-subheadline">
            Track and manage your incoming packages.
          </p>
        </div>

        {/* Feedback Messages */}
        {errorMessage && (
          <div className="feedback-banner error" role="alert">
            <svg
              className="feedback-icon"
              width="16"
              height="16"
              viewBox="0 0 24 24"
              fill="none"
              stroke="currentColor"
              strokeWidth="2"
              strokeLinecap="round"
              strokeLinejoin="round"
              aria-hidden="true"
            >
              <circle cx="12" cy="12" r="10" />
              <line x1="12" y1="8" x2="12" y2="12" />
              <line x1="12" y1="16" x2="12.01" y2="16" />
            </svg>
            <span>{errorMessage}</span>
          </div>
        )}

        {successMessage && (
          <div className="feedback-banner success" role="status">
            <svg
              className="feedback-icon"
              width="16"
              height="16"
              viewBox="0 0 24 24"
              fill="none"
              stroke="currentColor"
              strokeWidth="2"
              strokeLinecap="round"
              strokeLinejoin="round"
              aria-hidden="true"
            >
              <path d="M22 11.08V12a10 10 0 1 1-5.93-9.14" />
              <polyline points="22 4 12 14.01 9 11.01" />
            </svg>
            <span>{successMessage}</span>
          </div>
        )}

        {currentUser ? (
          <div className="authenticated-session-box">
            <div className="user-profile-badge">
              <div className="user-avatar-circle">
                {(currentUser.firstname?.[0] || 'U') +
                  (currentUser.lastname?.[0] || '')}
              </div>
              <div className="user-text-info">
                <span className="user-name">
                  {currentUser.firstname} {currentUser.lastname}
                </span>
                <span className="user-email">{currentUser.email}</span>
              </div>
            </div>

            <button
              type="button"
              className="secondary-action-btn"
              onClick={handleLogout}
            >
              Sign out
            </button>
          </div>
        ) : (
          <form onSubmit={handleSubmit} className="login-form" noValidate>
            {/* Email Address */}
            <div className="form-field-group">
              <label htmlFor="email" className="field-label">
                Email address
              </label>
              <div className="input-container">
                <input
                  id="email"
                  type="email"
                  autoComplete="email"
                  value={email}
                  onChange={(e) => {
                    setEmail(e.target.value)
                    if (fieldErrors.email) {
                      setFieldErrors((prev) => ({ ...prev, email: null }))
                    }
                  }}
                  placeholder="name@example.com"
                  disabled={isLoading}
                  className={`styled-input ${fieldErrors.email ? 'input-error' : ''}`}
                  required
                />
              </div>
              {fieldErrors.email && (
                <span className="inline-error">{fieldErrors.email}</span>
              )}
            </div>

            {/* Password */}
            <div className="form-field-group">
              <div className="field-label-row">
                <label htmlFor="password" className="field-label">
                  Password
                </label>
                <a
                  href="#forgot-password"
                  className="forgot-password-link"
                  onClick={(e) => {
                    e.preventDefault()
                    setErrorMessage(
                      'Please check your email or contact support to reset your password.'
                    )
                  }}
                >
                  Forgot password?
                </a>
              </div>

              <div className="input-container">
                <input
                  id="password"
                  type={showPassword ? 'text' : 'password'}
                  autoComplete="current-password"
                  value={password}
                  onChange={(e) => {
                    setPassword(e.target.value)
                    if (fieldErrors.password) {
                      setFieldErrors((prev) => ({ ...prev, password: null }))
                    }
                  }}
                  placeholder="••••••••••••"
                  disabled={isLoading}
                  className={`styled-input with-toggle-btn ${
                    fieldErrors.password ? 'input-error' : ''
                  }`}
                  required
                />
                <button
                  type="button"
                  className="password-visibility-toggle"
                  onClick={() => setShowPassword(!showPassword)}
                  aria-label={showPassword ? 'Hide password' : 'Show password'}
                  tabIndex={-1}
                >
                  {showPassword ? (
                    <svg
                      width="20"
                      height="20"
                      viewBox="0 0 24 24"
                      fill="none"
                      stroke="currentColor"
                      strokeWidth="1.8"
                      strokeLinecap="round"
                      strokeLinejoin="round"
                    >
                      <path d="M17.94 17.94A10.07 10.07 0 0 1 12 20c-7 0-11-8-11-8a18.45 18.45 0 0 1 5.06-5.94M9.9 4.24A9.12 9.12 0 0 1 12 4c7 0 11 8 11 8a18.5 18.5 0 0 1-2.16 3.19m-6.72-1.07a3 3 0 1 1-4.24-4.24" />
                      <line x1="1" y1="1" x2="23" y2="23" />
                    </svg>
                  ) : (
                    <svg
                      width="20"
                      height="20"
                      viewBox="0 0 24 24"
                      fill="none"
                      stroke="currentColor"
                      strokeWidth="1.8"
                      strokeLinecap="round"
                      strokeLinejoin="round"
                    >
                      <path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z" />
                      <circle cx="12" cy="12" r="3" />
                    </svg>
                  )}
                </button>
              </div>
              {fieldErrors.password && (
                <span className="inline-error">{fieldErrors.password}</span>
              )}
            </div>

            {/* Primary Submit Button */}
            <button
              type="submit"
              className="login-submit-button"
              disabled={isLoading}
            >
              {isLoading ? (
                <>
                  <span className="button-spinner" aria-hidden="true"></span>
                  <span>Logging in...</span>
                </>
              ) : (
                <>
                  <span>Log in</span>
                  <svg
                    className="button-arrow"
                    width="16"
                    height="16"
                    viewBox="0 0 24 24"
                    fill="none"
                    stroke="currentColor"
                    strokeWidth="2.2"
                    strokeLinecap="round"
                    strokeLinejoin="round"
                    aria-hidden="true"
                  >
                    <line x1="5" y1="12" x2="19" y2="12" />
                    <polyline points="12 5 19 12 12 19" />
                  </svg>
                </>
              )}
            </button>
          </form>
        )}

        {/* Footer switch to registration */}
        <div className="login-footer-section">
          <p className="register-prompt">
            New to Docket?{' '}
            <Link to="/register" className="register-link">
              Register as Customer
            </Link>
          </p>
        </div>
      </div>
    </div>
  )
}
