// HireZ API Client
// Centralized API communication module

const API_CONFIG = {
    // Use relative path since frontend is served by the same backend
    baseURL: '/api',
    // For development with separate servers, uncomment and use:
    // baseURL: 'http://localhost:5179/api',
};

/**
 * Get stored authentication token
 */
function getAuthToken() {
    return localStorage.getItem('authToken');
}

/**
 * Store authentication token
 */
function setAuthToken(token) {
    localStorage.setItem('authToken', token);
}

/**
 * Remove authentication token (logout)
 */
function removeAuthToken() {
    localStorage.removeItem('authToken');
    localStorage.removeItem('userEmail');
}

/**
 * Check if user is authenticated
 */
function isAuthenticated() {
    return !!getAuthToken();
}

/**
 * Make authenticated API request
 */
async function apiRequest(endpoint, options = {}) {
    const token = getAuthToken();
    const url = `${API_CONFIG.baseURL}${endpoint}`;

    const defaultHeaders = {
        'Content-Type': 'application/json',
    };

    // Add authorization header if token exists
    if (token) {
        defaultHeaders['Authorization'] = `Bearer ${token}`;
    }

    // Merge headers
    const headers = {
        ...defaultHeaders,
        ...(options.headers || {}),
    };

    // Remove Content-Type for FormData (browser will set it automatically)
    if (options.body instanceof FormData) {
        delete headers['Content-Type'];
    }

    const config = {
        ...options,
        headers,
    };

    try {
        const response = await fetch(url, config);
        
        // Handle 401 Unauthorized - token expired or invalid
        if (response.status === 401) {
            removeAuthToken();
            if (window.location.pathname !== '/login.html' && !window.location.pathname.includes('login.html')) {
                window.location.href = 'login.html';
            }
            throw new Error('Authentication required. Please login again.');
        }

        // Parse JSON response if content-type is JSON
        const contentType = response.headers.get('content-type');
        let data;
        
        if (contentType && contentType.includes('application/json')) {
            data = await response.json();
        } else {
            data = await response.text();
        }

        if (!response.ok) {
            const errorMessage = data?.message || data?.error || `API Error: ${response.status} ${response.statusText}`;
            throw new Error(errorMessage);
        }

        return data;
    } catch (error) {
        // Handle network errors
        if (error.name === 'TypeError' && error.message.includes('fetch')) {
            throw new Error('Network error. Please check if the API server is running.');
        }
        throw error;
    }
}

/**
 * API Methods
 */
const API = {
    // Authentication
    auth: {
        async login(email, password) {
            const response = await apiRequest('/auth/login', {
                method: 'POST',
                body: JSON.stringify({ email, password }),
            });
            
            if (response.token) {
                setAuthToken(response.token);
                localStorage.setItem('userEmail', response.email || email);
            }
            
            return response;
        },

        async register(email, password) {
            return await apiRequest('/auth/register', {
                method: 'POST',
                body: JSON.stringify({ email, password }),
            });
        },

        logout() {
            removeAuthToken();
        },
    },

    // Profile
    profile: {
        async get() {
            return await apiRequest('/profile');
        },
    },

    // Resume
    resume: {
        async upload(file) {
            const formData = new FormData();
            formData.append('file', file);

            return await apiRequest('/resume/upload', {
                method: 'POST',
                body: formData,
            });
        },

        async get(id) {
            return await apiRequest(`/resume/${id}`);
        },

        async getAll() {
            return await apiRequest('/resume');
        },

        async reprocess(id) {
            return await apiRequest(`/resume/${id}/reprocess`, {
                method: 'POST',
            });
        },
    },

    // Jobs
    jobs: {
        async create(jobData) {
            return await apiRequest('/jobs', {
                method: 'POST',
                body: JSON.stringify(jobData),
            });
        },

        async get(id) {
            return await apiRequest(`/jobs/${id}`);
        },

        async matchResumeToJob(jobId, resumeId) {
            return await apiRequest(`/jobs/${jobId}/match/${resumeId}`, {
                method: 'POST',
            });
        },
    },

    // Interview
    interview: {
        async createSession(sessionData) {
            return await apiRequest('/interview/sessions', {
                method: 'POST',
                body: JSON.stringify(sessionData),
            });
        },

        async getSession(id) {
            return await apiRequest(`/interview/sessions/${id}`);
        },
    },

    // Analytics
    analytics: {
        async getOverview() {
            return await apiRequest('/analytics/overview');
        },

        async getTrends(days = 30) {
            return await apiRequest(`/analytics/trends?days=${days}`);
        },
    },

    // Feedback
    feedback: {
        async getForResume(resumeId) {
            return await apiRequest(`/resume/${resumeId}/feedback`);
        },

        async getById(resumeId, feedbackId) {
            return await apiRequest(`/resume/${resumeId}/feedback/${feedbackId}`);
        },

        async create(resumeId, feedbackData) {
            return await apiRequest(`/resume/${resumeId}/feedback`, {
                method: 'POST',
                body: JSON.stringify(feedbackData),
            });
        },
    },
};

// Export API client
window.API = API;
window.isAuthenticated = isAuthenticated;
window.getAuthToken = getAuthToken;

