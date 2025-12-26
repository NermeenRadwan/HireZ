// Authentication utilities
// Handles authentication state and protected routes

/**
 * Check if user is authenticated and redirect if not
 */
function requireAuth() {
    if (!isAuthenticated()) {
        const currentPage = window.location.pathname.split('/').pop();
        // Don't redirect if already on login/register/index pages
        if (currentPage !== 'login.html' && currentPage !== 'register.html' && currentPage !== 'index.html' && currentPage !== '') {
            window.location.href = 'login.html';
            return false;
        }
    }
    return true;
}

/**
 * Redirect authenticated users away from auth pages
 */
function redirectIfAuthenticated() {
    if (isAuthenticated()) {
        window.location.href = 'dashboard.html';
    }
}

/**
 * Initialize authentication checks on page load
 */
function initAuth() {
    // Check authentication status on page load
    const currentPage = window.location.pathname.split('/').pop();
    const authPages = ['login.html', 'register.html', 'index.html'];
    const isAuthPage = authPages.includes(currentPage) || currentPage === '';

    if (isAuthPage) {
        // Redirect to dashboard if already logged in
        redirectIfAuthenticated();
    } else {
        // Require auth for protected pages
        requireAuth();
    }
}

/**
 * Update user display name in navigation
 */
function updateUserDisplay() {
    const userEmail = localStorage.getItem('userEmail');
    const userDropdowns = document.querySelectorAll('#navbarDropdown, .user-name');
    
    userDropdowns.forEach(element => {
        if (userEmail) {
            // Extract name from email (first part before @)
            const displayName = userEmail.split('@')[0];
            element.textContent = displayName;
            // If it's a dropdown toggle, update the text inside
            if (element.tagName === 'A') {
                const icon = element.querySelector('i');
                if (icon) {
                    element.innerHTML = `<i class="${icon.className}"></i>${displayName}`;
                }
            }
        }
    });
}

// Initialize auth on DOM ready
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', () => {
        initAuth();
        updateUserDisplay();
    });
} else {
    initAuth();
    updateUserDisplay();
}

// Export functions
window.requireAuth = requireAuth;
window.redirectIfAuthenticated = redirectIfAuthenticated;
window.updateUserDisplay = updateUserDisplay;

