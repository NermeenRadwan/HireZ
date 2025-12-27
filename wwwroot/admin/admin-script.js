// HireZ Admin - Admin Panel JavaScript
// Administrative functionality for the HireZ platform

document.addEventListener('DOMContentLoaded', function() {
    // Initialize admin components
    initializeAdminComponents();
});

// Initialize all admin components
function initializeAdminComponents() {
    initializeAdminAuth();
    initializeAdminDashboard();
    initializeAdminUsers();
    initializeAdminFeedback();
    initializeAdminSettings();
    initializeAdminNavigation();
    initializeAdminCharts();
}

// Admin Authentication
function initializeAdminAuth() {
    const adminLoginForm = document.getElementById('adminLoginForm');
    if (adminLoginForm) {
        adminLoginForm.addEventListener('submit', handleAdminLogin);
    }

    const toggleAdminPassword = document.getElementById('toggleAdminPassword');
    if (toggleAdminPassword) {
        toggleAdminPassword.addEventListener('click', function() {
            togglePasswordVisibility('adminPassword', this);
        });
    }
}

// Handle admin login
function handleAdminLogin(e) {
    e.preventDefault();
    
    const formData = new FormData(e.target);
    const email = formData.get('adminEmail');
    const password = formData.get('adminPassword');
    
    // Show loading state
    const submitBtn = e.target.querySelector('button[type="submit"]');
    const originalText = submitBtn.innerHTML;
    submitBtn.innerHTML = '<i class="fas fa-spinner fa-spin me-2"></i>Accessing...';
    submitBtn.disabled = true;
    
    // Simulate admin authentication
    setTimeout(() => {
        if (email && password) {
            showAlert('success', 'Admin access granted! Redirecting to dashboard...');
            setTimeout(() => {
                window.location.href = 'admin-dashboard.html';
            }, 1500);
        } else {
            showAlert('danger', 'Invalid admin credentials. Please try again.');
            submitBtn.innerHTML = originalText;
            submitBtn.disabled = false;
        }
    }, 2000);
}

// Admin Dashboard
function initializeAdminDashboard() {
    // Initialize system activity chart
    initializeSystemActivityChart();
    
    // Initialize system status monitoring
    initializeSystemStatus();
    
    // Initialize recent activity
    initializeRecentActivity();
}

// Initialize system activity chart
function initializeSystemActivityChart() {
    const systemActivityChart = document.getElementById('systemActivityChart');
    if (systemActivityChart && typeof Chart !== 'undefined') {
        const ctx = systemActivityChart.getContext('2d');
        new Chart(ctx, {
            type: 'line',
            data: {
                labels: ['00:00', '04:00', '08:00', '12:00', '16:00', '20:00'],
                datasets: [{
                    label: 'User Activity',
                    data: [12, 19, 3, 5, 2, 3],
                    borderColor: '#dc3545',
                    backgroundColor: 'rgba(220, 53, 69, 0.1)',
                    tension: 0.4
                }, {
                    label: 'System Load',
                    data: [8, 15, 2, 4, 1, 2],
                    borderColor: '#6c757d',
                    backgroundColor: 'rgba(108, 117, 125, 0.1)',
                    tension: 0.4
                }, {
                    label: 'API Requests',
                    data: [3, 8, 1, 2, 1, 1],
                    borderColor: '#198754',
                    backgroundColor: 'rgba(25, 135, 84, 0.1)',
                    tension: 0.4
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        position: 'top',
                    }
                },
                scales: {
                    y: {
                        beginAtZero: true
                    }
                }
            }
        });
    }
}

// Initialize system status
function initializeSystemStatus() {
    // Simulate real-time status updates
    setInterval(updateSystemStatus, 30000); // Update every 30 seconds
}

// Update system status
function updateSystemStatus() {
    const statusIndicators = document.querySelectorAll('.status-indicator .badge');
    statusIndicators.forEach(indicator => {
        // Simulate status changes (in real app, this would be API calls)
        const statuses = ['Online', 'Degraded', 'Offline'];
        const colors = ['success', 'warning', 'danger'];
        const randomStatus = Math.floor(Math.random() * statuses.length);
        
        if (Math.random() < 0.1) { // 10% chance of status change
            indicator.textContent = statuses[randomStatus];
            indicator.className = `badge bg-${colors[randomStatus]}`;
        }
    });
}

// Initialize recent activity
function initializeRecentActivity() {
    // Add click handlers for recent user items
    const recentUserItems = document.querySelectorAll('.recent-user-item');
    recentUserItems.forEach(item => {
        item.addEventListener('click', function() {
            // Navigate to user details
            window.location.href = 'admin-users.html';
        });
    });
}

// Admin Users Management
function initializeAdminUsers() {
    // Initialize user search
    initializeUserSearch();
    
    // Initialize user actions
    initializeUserActions();
    
    // Initialize user filters
    initializeUserFilters();
}

// Initialize user search
function initializeUserSearch() {
    const searchInput = document.getElementById('searchUsers');
    if (searchInput) {
        searchInput.addEventListener('input', function() {
            const searchTerm = this.value.toLowerCase();
            const userRows = document.querySelectorAll('tbody tr');
            
            userRows.forEach(row => {
                const text = row.textContent.toLowerCase();
                if (text.includes(searchTerm)) {
                    row.style.display = '';
                } else {
                    row.style.display = 'none';
                }
            });
        });
    }
}

// Initialize user actions
function initializeUserActions() {
    // Add user button
    const addUserBtn = document.querySelector('[data-bs-target="#addUserModal"]');
    if (addUserBtn) {
        addUserBtn.addEventListener('click', function() {
            // Reset form when opening modal
            const form = document.getElementById('addUserForm');
            if (form) {
                form.reset();
            }
        });
    }
    
    // User action buttons
    const actionButtons = document.querySelectorAll('.btn-group .btn');
    actionButtons.forEach(button => {
        button.addEventListener('click', function(e) {
            e.stopPropagation();
            const action = this.title;
            handleUserAction(action, this);
        });
    });
}

// Handle user actions
function handleUserAction(action, button) {
    const row = button.closest('tr');
    const userName = row.querySelector('h6').textContent;
    
    switch (action) {
        case 'View Profile':
            showAlert('info', `Viewing profile for ${userName}`);
            break;
        case 'Edit User':
            showAlert('info', `Editing user ${userName}`);
            break;
        case 'Suspend User':
        case 'Reactivate User':
            const actionText = action.includes('Suspend') ? 'suspend' : 'reactivate';
            if (confirm(`Are you sure you want to ${actionText} ${userName}?`)) {
                showAlert('success', `User ${userName} has been ${actionText}ed`);
                // Update UI
                const statusBadge = row.querySelector('.badge');
                if (actionText === 'suspend') {
                    statusBadge.textContent = 'Suspended';
                    statusBadge.className = 'badge bg-danger';
                } else {
                    statusBadge.textContent = 'Active';
                    statusBadge.className = 'badge bg-success';
                }
            }
            break;
        case 'Delete User':
            if (confirm(`Are you sure you want to delete ${userName}? This action cannot be undone.`)) {
                showAlert('success', `User ${userName} has been deleted`);
                row.remove();
            }
            break;
        case 'Approve User':
            showAlert('success', `User ${userName} has been approved`);
            const statusBadge = row.querySelector('.badge');
            statusBadge.textContent = 'Active';
            statusBadge.className = 'badge bg-success';
            break;
        case 'Reject User':
            if (confirm(`Are you sure you want to reject ${userName}?`)) {
                showAlert('success', `User ${userName} has been rejected`);
                row.remove();
            }
            break;
    }
}

// Initialize user filters
function initializeUserFilters() {
    const filters = ['userStatusFilter', 'userRoleFilter'];
    filters.forEach(filterId => {
        const filter = document.getElementById(filterId);
        if (filter) {
            filter.addEventListener('change', applyUserFilters);
        }
    });
}

// Apply user filters
function applyUserFilters() {
    const statusFilter = document.getElementById('userStatusFilter').value;
    const roleFilter = document.getElementById('userRoleFilter').value;
    const userRows = document.querySelectorAll('tbody tr');
    
    userRows.forEach(row => {
        let showRow = true;
        
        if (statusFilter) {
            const statusBadge = row.querySelector('.badge');
            const status = statusBadge.textContent.toLowerCase();
            if (statusFilter === 'active' && status !== 'active') showRow = false;
            if (statusFilter === 'pending' && status !== 'pending') showRow = false;
            if (statusFilter === 'suspended' && status !== 'suspended') showRow = false;
            if (statusFilter === 'inactive' && status !== 'inactive') showRow = false;
        }
        
        if (roleFilter) {
            const roleBadge = row.querySelectorAll('.badge')[1];
            const role = roleBadge.textContent.toLowerCase();
            if (roleFilter === 'admin' && !role.includes('admin')) showRow = false;
            if (roleFilter === 'hr-manager' && !role.includes('hr')) showRow = false;
            if (roleFilter === 'recruiter' && !role.includes('recruiter')) showRow = false;
            if (roleFilter === 'hiring-manager' && !role.includes('hiring')) showRow = false;
        }
        
        row.style.display = showRow ? '' : 'none';
    });
}

// Clear user filters
function clearUserFilters() {
    const filters = ['searchUsers', 'userStatusFilter', 'userRoleFilter'];
    filters.forEach(id => {
        const element = document.getElementById(id);
        if (element) {
            element.value = '';
        }
    });
    applyUserFilters();
    showAlert('info', 'Filters cleared');
}

// Add user function
function addUser() {
    const form = document.getElementById('addUserForm');
    const formData = new FormData(form);
    
    const firstName = formData.get('newUserFirstName');
    const lastName = formData.get('newUserLastName');
    const email = formData.get('newUserEmail');
    const company = formData.get('newUserCompany');
    const role = formData.get('newUserRole');
    
    if (!firstName || !lastName || !email || !company || !role) {
        showAlert('warning', 'Please fill in all required fields');
        return;
    }
    
    showAlert('success', `User ${firstName} ${lastName} has been added successfully`);
    
    // Close modal
    const modal = bootstrap.Modal.getInstance(document.getElementById('addUserModal'));
    modal.hide();
    
    // Reset form
    form.reset();
}

// Admin Feedback Management
function initializeAdminFeedback() {
    // Initialize feedback charts
    initializeFeedbackCharts();
    
    // Initialize feedback search
    initializeFeedbackSearch();
    
    // Initialize feedback filters
    initializeFeedbackFilters();
}

// Initialize feedback charts
function initializeFeedbackCharts() {
    // Feedback trends chart
    const trendsChart = document.getElementById('feedbackTrendsChart');
    if (trendsChart && typeof Chart !== 'undefined') {
        const ctx = trendsChart.getContext('2d');
        new Chart(ctx, {
            type: 'line',
            data: {
                labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun'],
                datasets: [{
                    label: 'Positive Feedback',
                    data: [12, 19, 3, 5, 2, 3],
                    borderColor: '#198754',
                    backgroundColor: 'rgba(25, 135, 84, 0.1)',
                    tension: 0.4
                }, {
                    label: 'Negative Feedback',
                    data: [2, 3, 1, 2, 1, 1],
                    borderColor: '#dc3545',
                    backgroundColor: 'rgba(220, 53, 69, 0.1)',
                    tension: 0.4
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        position: 'top',
                    }
                },
                scales: {
                    y: {
                        beginAtZero: true
                    }
                }
            }
        });
    }
    
    // Feedback categories chart
    const categoriesChart = document.getElementById('feedbackCategoriesChart');
    if (categoriesChart && typeof Chart !== 'undefined') {
        const ctx = categoriesChart.getContext('2d');
        new Chart(ctx, {
            type: 'doughnut',
            data: {
                labels: ['Interview', 'System', 'Support', 'Feature'],
                datasets: [{
                    data: [40, 30, 20, 10],
                    backgroundColor: [
                        '#dc3545',
                        '#6c757d',
                        '#198754',
                        '#0dcaf0'
                    ]
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        position: 'bottom',
                    }
                }
            }
        });
    }
}

// Initialize feedback search
function initializeFeedbackSearch() {
    const searchInput = document.getElementById('searchFeedback');
    if (searchInput) {
        searchInput.addEventListener('input', function() {
            const searchTerm = this.value.toLowerCase();
            const feedbackRows = document.querySelectorAll('tbody tr');
            
            feedbackRows.forEach(row => {
                const text = row.textContent.toLowerCase();
                if (text.includes(searchTerm)) {
                    row.style.display = '';
                } else {
                    row.style.display = 'none';
                }
            });
        });
    }
}

// Initialize feedback filters
function initializeFeedbackFilters() {
    const filters = ['feedbackTypeFilter', 'feedbackRatingFilter', 'feedbackStatusFilter', 'feedbackDateFilter'];
    filters.forEach(filterId => {
        const filter = document.getElementById(filterId);
        if (filter) {
            filter.addEventListener('change', applyFeedbackFilters);
        }
    });
}

// Apply feedback filters
function applyFeedbackFilters() {
    const typeFilter = document.getElementById('feedbackTypeFilter').value;
    const ratingFilter = document.getElementById('feedbackRatingFilter').value;
    const statusFilter = document.getElementById('feedbackStatusFilter').value;
    const dateFilter = document.getElementById('feedbackDateFilter').value;
    
    const feedbackRows = document.querySelectorAll('tbody tr');
    
    feedbackRows.forEach(row => {
        let showRow = true;
        
        if (typeFilter) {
            const typeBadge = row.querySelector('.badge');
            const type = typeBadge.textContent.toLowerCase();
            if (typeFilter === 'interview' && !type.includes('interview')) showRow = false;
            if (typeFilter === 'system' && !type.includes('system')) showRow = false;
            if (typeFilter === 'support' && !type.includes('support')) showRow = false;
            if (typeFilter === 'feature' && !type.includes('feature')) showRow = false;
        }
        
        if (ratingFilter) {
            const rating = row.querySelector('.rating span').textContent;
            if (rating !== ratingFilter) showRow = false;
        }
        
        if (statusFilter) {
            const statusBadge = row.querySelectorAll('.badge')[1];
            const status = statusBadge.textContent.toLowerCase();
            if (statusFilter === 'pending' && status !== 'pending') showRow = false;
            if (statusFilter === 'reviewed' && status !== 'reviewed') showRow = false;
            if (statusFilter === 'resolved' && status !== 'resolved') showRow = false;
            if (statusFilter === 'closed' && status !== 'closed') showRow = false;
        }
        
        row.style.display = showRow ? '' : 'none';
    });
}

// Clear feedback filters
function clearFeedbackFilters() {
    const filters = ['searchFeedback', 'feedbackTypeFilter', 'feedbackRatingFilter', 'feedbackStatusFilter', 'feedbackDateFilter'];
    filters.forEach(id => {
        const element = document.getElementById(id);
        if (element) {
            element.value = '';
        }
    });
    applyFeedbackFilters();
    showAlert('info', 'Filters cleared');
}

// Admin Settings
function initializeAdminSettings() {
    // Initialize settings forms
    initializeSettingsForms();
    
    // Initialize settings tabs
    initializeSettingsTabs();
}

// Initialize settings forms
function initializeSettingsForms() {
    const forms = ['generalSettingsForm', 'userRegistrationForm', 'passwordPolicyForm', 'sessionManagementForm', 'smtpConfigForm', 'emailTemplatesForm', 'aiSettingsForm', 'linkedinIntegrationForm', 'videoIntegrationForm'];
    
    forms.forEach(formId => {
        const form = document.getElementById(formId);
        if (form) {
            form.addEventListener('submit', function(e) {
                e.preventDefault();
                handleSettingsFormSubmit(formId);
            });
        }
    });
}

// Handle settings form submission
function handleSettingsFormSubmit(formId) {
    const form = document.getElementById(formId);
    const formData = new FormData(form);
    
    // Simulate saving settings
    showAlert('success', 'Settings saved successfully');
    
    // Log form data (in real app, this would be sent to server)
    console.log(`Saving ${formId}:`, Object.fromEntries(formData));
}

// Initialize settings tabs
function initializeSettingsTabs() {
    const tabButtons = document.querySelectorAll('#settingsTabs button[data-bs-toggle="tab"]');
    tabButtons.forEach(button => {
        button.addEventListener('shown.bs.tab', function(e) {
            const targetTab = e.target.getAttribute('data-bs-target');
            console.log(`Switched to ${targetTab} tab`);
        });
    });
}

// Save all settings
function saveAllSettings() {
    const forms = document.querySelectorAll('form[id$="Form"]');
    let savedCount = 0;
    
    forms.forEach(form => {
        const formData = new FormData(form);
        console.log(`Saving ${form.id}:`, Object.fromEntries(formData));
        savedCount++;
    });
    
    showAlert('success', `All settings saved successfully (${savedCount} forms)`);
}

// Admin Navigation
function initializeAdminNavigation() {
    // Mobile menu toggle
    const navbarToggler = document.querySelector('.navbar-toggler');
    const navbarCollapse = document.querySelector('.navbar-collapse');
    
    if (navbarToggler && navbarCollapse) {
        navbarToggler.addEventListener('click', function() {
            navbarCollapse.classList.toggle('show');
        });
    }
    
    // Active navigation highlighting
    highlightActiveAdminNavigation();
}

// Highlight active admin navigation
function highlightActiveAdminNavigation() {
    const currentPage = window.location.pathname.split('/').pop();
    const navLinks = document.querySelectorAll('.admin-sidebar .nav-link');
    
    navLinks.forEach(link => {
        const href = link.getAttribute('href');
        if (href === currentPage) {
            link.classList.add('active');
        } else {
            link.classList.remove('active');
        }
    });
}

// Initialize admin charts
function initializeAdminCharts() {
    // Initialize any additional charts
    if (typeof Chart !== 'undefined') {
        // Chart.js is available, initialize charts
        console.log('Chart.js loaded, admin charts ready');
    }
}

// Utility functions
function showAlert(type, message) {
    // Remove existing alerts
    const existingAlerts = document.querySelectorAll('.alert');
    existingAlerts.forEach(alert => alert.remove());
    
    // Create new alert
    const alertDiv = document.createElement('div');
    alertDiv.className = `alert alert-${type} alert-dismissible fade show`;
    alertDiv.innerHTML = `
        ${message}
        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
    `;
    
    // Add to page
    const container = document.querySelector('.alert-container') || document.body;
    container.appendChild(alertDiv);
    
    // Auto-dismiss after 5 seconds
    setTimeout(() => {
        if (alertDiv.parentNode) {
            alertDiv.remove();
        }
    }, 5000);
}

// Export functions for global access
window.showAlert = showAlert;
window.clearUserFilters = clearUserFilters;
window.clearFeedbackFilters = clearFeedbackFilters;
window.addUser = addUser;
window.saveAllSettings = saveAllSettings;

// Error handling
window.addEventListener('error', function(e) {
    console.error('Admin JavaScript error:', e.error);
    showAlert('danger', 'An error occurred. Please refresh the page.');
});
