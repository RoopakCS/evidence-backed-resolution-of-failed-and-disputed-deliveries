// Authentication API service for Docket
const API_BASE = import.meta.env.VITE_API_URL || '/auth'
const DIRECT_BACKEND = 'http://localhost:5077/auth'

/**
 * Normalizes API endpoint URL, trying relative proxy first with direct fallback
 */
async function apiRequest(endpoint, options = {}) {
  const headers = {
    'Content-Type': 'application/json',
    Accept: 'application/json',
    ...options.headers,
  }

  // First try the configured API base (works with Vite proxy or custom env)
  try {
    const response = await fetch(`${API_BASE}${endpoint}`, {
      ...options,
      headers,
    })
    return response
  } catch (err) {
    // If relative request failed (e.g. dev server without proxy running), attempt direct call
    if (API_BASE.startsWith('/')) {
      return await fetch(`${DIRECT_BACKEND}${endpoint}`, {
        ...options,
        headers,
      })
    }
    throw err
  }
}

/**
 * Authenticate with email & password against POST /auth/login
 */
export async function loginUser({ email, password }) {
  try {
    const response = await apiRequest('/login', {
      method: 'POST',
      body: JSON.stringify({
        email: email.trim(),
        password,
      }),
    })

    const data = await response.json().catch(() => null)

    if (response.ok && data?.success) {
      if (data.token) {
        localStorage.setItem('docket_token', data.token)
      }
      const userInfo = {
        userId: data.user_id,
        firstname: data.firstname,
        lastname: data.lastname,
        email: data.email,
        tokenExpiresAt: data.token_expires_at,
      }
      localStorage.setItem('docket_user', JSON.stringify(userInfo))

      return {
        success: true,
        user: userInfo,
        token: data.token,
        message: data.message || 'Login successful',
      }
    }

    // Extract error message from API response
    let errorMessage = data?.message
    if (!errorMessage && data?.errors) {
      // Model validation errors (e.g. from ASP.NET Core ModelState)
      const errorList = Object.values(data.errors).flat()
      errorMessage = errorList.join('. ')
    }

    if (!errorMessage) {
      errorMessage =
        response.status === 401
          ? 'Invalid email or password.'
          : `Authentication failed (Status ${response.status}).`
    }

    return {
      success: false,
      message: errorMessage,
    }
  } catch {
    return {
      success: false,
      message:
        'Unable to reach the authentication server. Please ensure the backend is running on http://localhost:5077.',
    }
  }
}

/**
 * Retrieve current authenticated user profile via GET /auth/me
 */
export async function fetchCurrentUser() {
  const token = localStorage.getItem('docket_token')
  if (!token) return null

  try {
    const response = await apiRequest('/me', {
      method: 'GET',
      headers: {
        Authorization: `Bearer ${token}`,
      },
    })

    if (!response.ok) {
      if (response.status === 401) {
        clearAuthSession()
      }
      return null
    }

    const user = await response.json().catch(() => null)
    return user
  } catch {
    return null
  }
}

/**
 * Reads local session state without network call
 */
export function getStoredSession() {
  try {
    const token = localStorage.getItem('docket_token')
    const userStr = localStorage.getItem('docket_user')
    if (token && userStr) {
      return {
        token,
        user: JSON.parse(userStr),
      }
    }
  } catch {
    // Ignore corrupted storage
  }
  return null
}

/**
 * Clears authentication tokens and cached user
 */
export function clearAuthSession() {
  localStorage.removeItem('docket_token')
  localStorage.removeItem('docket_user')
}
