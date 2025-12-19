// HireZ - Main JavaScript File
// Modern hiring platform functionality

document.addEventListener('DOMContentLoaded', function() {
    // Initialize all components
    initializeComponents();
});

// Initialize all components
function initializeComponents() {
    initializeAuth();
    initializeDashboard();
    initializeUpload();
    initializeInterview();
    initializeFeedback();
    initializeProfile();
    initializeNavigation();
    initializeAnimations();
}

// Authentication functionality
function initializeAuth() {
    // Login form handling
    const loginForm = document.getElementById('loginForm');
    if (loginForm) {
        loginForm.addEventListener('submit', handleLogin);
    }

    // Register form handling
    const registerForm = document.getElementById('registerForm');
    if (registerForm) {
        registerForm.addEventListener('submit', handleRegister);
    }

    // Password toggle functionality
    const togglePassword = document.getElementById('togglePassword');
    const toggleConfirmPassword = document.getElementById('toggleConfirmPassword');
    
    if (togglePassword) {
        togglePassword.addEventListener('click', function() {
            togglePasswordVisibility('password', this);
        });
    }
    
    if (toggleConfirmPassword) {
        toggleConfirmPassword.addEventListener('click', function() {
            togglePasswordVisibility('confirmPassword', this);
        });
    }
}

// Handle login form submission
function handleLogin(e) {
    e.preventDefault();
    
    const formData = new FormData(e.target);
    const email = formData.get('email');
    const password = formData.get('password');
    
    // Show loading state
    const submitBtn = e.target.querySelector('button[type="submit"]');
    const originalText = submitBtn.innerHTML;
    submitBtn.innerHTML = '<i class="fas fa-spinner fa-spin me-2"></i>Signing In...';
    submitBtn.disabled = true;
    
    // Simulate API call
    setTimeout(() => {
        if (email && password) {
            showAlert('success', 'Login successful! Redirecting to dashboard...');
            setTimeout(() => {
                window.location.href = 'dashboard.html';
            }, 1500);
        } else {
            showAlert('danger', 'Invalid email or password. Please try again.');
            submitBtn.innerHTML = originalText;
            submitBtn.disabled = false;
        }
    }, 2000);
}

// Handle register form submission
function handleRegister(e) {
    e.preventDefault();
    
    const formData = new FormData(e.target);
    const password = formData.get('password');
    const confirmPassword = formData.get('confirmPassword');
    
    // Validate password match
    if (password !== confirmPassword) {
        showAlert('danger', 'Passwords do not match. Please try again.');
        return;
    }
    
    // Show loading state
    const submitBtn = e.target.querySelector('button[type="submit"]');
    const originalText = submitBtn.innerHTML;
    submitBtn.innerHTML = '<i class="fas fa-spinner fa-spin me-2"></i>Creating Account...';
    submitBtn.disabled = true;
    
    // Simulate API call
    setTimeout(() => {
        showAlert('success', 'Account created successfully! Redirecting to login...');
        setTimeout(() => {
            window.location.href = 'login.html';
        }, 1500);
    }, 2000);
}

// Toggle password visibility
function togglePasswordVisibility(inputId, button) {
    const input = document.getElementById(inputId);
    const icon = button.querySelector('i');
    
    if (input.type === 'password') {
        input.type = 'text';
        icon.classList.remove('fa-eye');
        icon.classList.add('fa-eye-slash');
    } else {
        input.type = 'password';
        icon.classList.remove('fa-eye-slash');
        icon.classList.add('fa-eye');
    }
}

// Dashboard functionality
function initializeDashboard() {
    // Initialize charts if Chart.js is available
    if (typeof Chart !== 'undefined') {
        initializeCharts();
    }
    
    // Initialize candidate interactions
    initializeCandidateInteractions();
}

// Initialize dashboard charts
function initializeCharts() {
    const pipelineChart = document.getElementById('pipelineChart');
    if (pipelineChart) {
        const ctx = pipelineChart.getContext('2d');
        new Chart(ctx, {
            type: 'line',
            data: {
                labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun'],
                datasets: [{
                    label: 'Applications',
                    data: [12, 19, 3, 5, 2, 3],
                    borderColor: '#007bff',
                    backgroundColor: 'rgba(0, 123, 255, 0.1)',
                    tension: 0.4
                }, {
                    label: 'Interviews',
                    data: [8, 15, 2, 4, 1, 2],
                    borderColor: '#28a745',
                    backgroundColor: 'rgba(40, 167, 69, 0.1)',
                    tension: 0.4
                }, {
                    label: 'Hires',
                    data: [3, 8, 1, 2, 1, 1],
                    borderColor: '#ffc107',
                    backgroundColor: 'rgba(255, 193, 7, 0.1)',
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

// Initialize candidate interactions
function initializeCandidateInteractions() {
    // Add click handlers for candidate cards
    const candidateCards = document.querySelectorAll('.candidate-card');
    candidateCards.forEach(card => {
        card.addEventListener('click', function() {
            // Add visual feedback
            this.style.transform = 'scale(0.98)';
            setTimeout(() => {
                this.style.transform = '';
            }, 150);
        });
    });
}

// Upload functionality
function initializeUpload() {
    const uploadArea = document.getElementById('uploadArea');
    const fileInput = document.getElementById('fileInput');
    
    if (uploadArea && fileInput) {
        // Drag and drop functionality
        uploadArea.addEventListener('dragover', handleDragOver);
        uploadArea.addEventListener('dragleave', handleDragLeave);
        uploadArea.addEventListener('drop', handleDrop);
        uploadArea.addEventListener('click', () => fileInput.click());
        
        // File input change
        fileInput.addEventListener('change', handleFileSelect);
    }
    
    // Initialize upload methods
    initializeUploadMethods();
}

// Handle drag over
function handleDragOver(e) {
    e.preventDefault();
    e.currentTarget.classList.add('dragover');
}

// Handle drag leave
function handleDragLeave(e) {
    e.preventDefault();
    e.currentTarget.classList.remove('dragover');
}

// Handle drop
function handleDrop(e) {
    e.preventDefault();
    e.currentTarget.classList.remove('dragover');
    
    const files = e.dataTransfer.files;
    handleFiles(files);
}

// Handle file selection
function handleFileSelect(e) {
    const files = e.target.files;
    handleFiles(files);
}

// Handle files
function handleFiles(files) {
    if (files.length === 0) return;
    
    // Show upload progress
    const progressCard = document.getElementById('uploadProgress');
    if (progressCard) {
        progressCard.style.display = 'block';
        simulateUploadProgress();
    }
    
    // Process files
    Array.from(files).forEach(file => {
        console.log('Processing file:', file.name);
        // Here you would typically upload the file to your server
    });
}

// Simulate upload progress
function simulateUploadProgress() {
    const progressBar = document.getElementById('progressBar');
    const progressText = document.getElementById('progressText');
    const progressPercent = document.getElementById('progressPercent');
    
    let progress = 0;
    const interval = setInterval(() => {
        progress += Math.random() * 15;
        if (progress >= 100) {
            progress = 100;
            clearInterval(interval);
            progressText.textContent = 'Upload complete!';
            setTimeout(() => {
                document.getElementById('uploadProgress').style.display = 'none';
                showAlert('success', 'Files uploaded successfully!');
            }, 1000);
        }
        
        progressBar.style.width = progress + '%';
        progressPercent.textContent = Math.round(progress) + '%';
    }, 200);
}

// Initialize upload methods
function initializeUploadMethods() {
    // Single upload
    window.openSingleUpload = function() {
        document.getElementById('fileInput').click();
    };
    
    // Bulk upload
    window.openBulkUpload = function() {
        const input = document.createElement('input');
        input.type = 'file';
        input.multiple = true;
        input.accept = '.pdf,.doc,.docx,.txt';
        input.onchange = function(e) {
            handleFiles(e.target.files);
        };
        input.click();
    };
    
    // LinkedIn import
    window.openLinkedInImport = function() {
        showAlert('info', 'LinkedIn import feature coming soon!');
    };
}

// Interview functionality
function initializeInterview() {
    // Initialize interview timeline
    initializeInterviewTimeline();
    
    // Initialize interview actions
    initializeInterviewActions();
}

// Initialize interview timeline
function initializeInterviewTimeline() {
    const timelineItems = document.querySelectorAll('.timeline-item');
    timelineItems.forEach((item, index) => {
        // Add staggered animation
        setTimeout(() => {
            item.classList.add('fade-in');
        }, index * 100);
    });
}

// Initialize interview actions
function initializeInterviewActions() {
    // Video call buttons
    const videoButtons = document.querySelectorAll('.btn-success');
    videoButtons.forEach(button => {
        if (button.querySelector('.fa-video')) {
            button.addEventListener('click', function() {
                showAlert('info', 'Starting video call...');
                // Here you would integrate with your video calling service
            });
        }
    });
}

// Feedback functionality
function initializeFeedback() {
    const feedbackForm = document.getElementById('feedbackForm');
    if (feedbackForm) {
        feedbackForm.addEventListener('submit', handleFeedback);
    }
    
    // Initialize rating system
    initializeRatingSystem();
}

// Handle feedback submission
function handleFeedback(e) {
    e.preventDefault();
    
    const formData = new FormData(e.target);
    const rating = formData.get('rating');
    const recommendation = formData.get('recommendation');
    
    if (!rating || !recommendation) {
        showAlert('warning', 'Please provide a rating and recommendation.');
        return;
    }
    
    // Show loading state
    const submitBtn = e.target.querySelector('button[type="submit"]');
    const originalText = submitBtn.innerHTML;
    submitBtn.innerHTML = '<i class="fas fa-spinner fa-spin me-2"></i>Submitting...';
    submitBtn.disabled = true;
    
    // Simulate API call
    setTimeout(() => {
        showAlert('success', 'Feedback submitted successfully!');
        e.target.reset();
        submitBtn.innerHTML = originalText;
        submitBtn.disabled = false;
    }, 1500);
}

// Initialize rating system
function initializeRatingSystem() {
    const ratingInputs = document.querySelectorAll('.rating-input input[type="radio"]');
    ratingInputs.forEach(input => {
        input.addEventListener('change', function() {
            const rating = this.value;
            const labels = this.parentElement.querySelectorAll('label');
            
            labels.forEach((label, index) => {
                if (index < rating) {
                    label.style.color = '#ffc107';
                } else {
                    label.style.color = '#dee2e6';
                }
            });
        });
    });
}

// Profile functionality
function initializeProfile() {
    const profileForm = document.getElementById('profileForm');
    const companyForm = document.getElementById('companyForm');
    const securityForm = document.getElementById('securityForm');
    
    if (profileForm) {
        profileForm.addEventListener('submit', handleProfileUpdate);
    }
    
    if (companyForm) {
        companyForm.addEventListener('submit', handleCompanyUpdate);
    }
    
    if (securityForm) {
        securityForm.addEventListener('submit', handleSecurityUpdate);
    }
}

// Handle profile update
function handleProfileUpdate(e) {
    e.preventDefault();
    showAlert('success', 'Profile updated successfully!');
}

// Handle company update
function handleCompanyUpdate(e) {
    e.preventDefault();
    showAlert('success', 'Company information updated successfully!');
}

// Handle security update
function handleSecurityUpdate(e) {
    e.preventDefault();
    
    const currentPassword = document.getElementById('currentPassword').value;
    const newPassword = document.getElementById('newPassword').value;
    const confirmPassword = document.getElementById('confirmPassword').value;
    
    if (!currentPassword || !newPassword || !confirmPassword) {
        showAlert('warning', 'Please fill in all password fields.');
        return;
    }
    
    if (newPassword !== confirmPassword) {
        showAlert('danger', 'New passwords do not match.');
        return;
    }
    
    showAlert('success', 'Password updated successfully!');
    e.target.reset();
}

// Navigation functionality
function initializeNavigation() {
    // Mobile menu toggle
    const navbarToggler = document.querySelector('.navbar-toggler');
    const navbarCollapse = document.querySelector('.navbar-collapse');
    
    if (navbarToggler && navbarCollapse) {
        navbarToggler.addEventListener('click', function() {
            navbarCollapse.classList.toggle('show');
        });
    }
    
    // Active navigation highlighting
    highlightActiveNavigation();
}

// Highlight active navigation
function highlightActiveNavigation() {
    const currentPage = window.location.pathname.split('/').pop();
    const navLinks = document.querySelectorAll('.nav-link');
    
    navLinks.forEach(link => {
        const href = link.getAttribute('href');
        if (href === currentPage || (currentPage === '' && href === 'index.html')) {
            link.classList.add('active');
        } else {
            link.classList.remove('active');
        }
    });
}

// Animation functionality
function initializeAnimations() {
    // Intersection Observer for scroll animations
    const observerOptions = {
        threshold: 0.1,
        rootMargin: '0px 0px -50px 0px'
    };
    
    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.classList.add('fade-in');
            }
        });
    }, observerOptions);
    
    // Observe elements for animation
    const animatedElements = document.querySelectorAll('.feature-card, .stat-card, .value-card, .team-card');
    animatedElements.forEach(el => observer.observe(el));
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

// Clear filters function
function clearFilters() {
    const filters = ['searchCandidates', 'statusFilter', 'jobFilter'];
    filters.forEach(id => {
        const element = document.getElementById(id);
        if (element) {
            element.value = '';
        }
    });
    showAlert('info', 'Filters cleared');
}

// Search functionality
function initializeSearch() {
    const searchInput = document.getElementById('searchCandidates');
    if (searchInput) {
        searchInput.addEventListener('input', function() {
            const searchTerm = this.value.toLowerCase();
            const candidateRows = document.querySelectorAll('tbody tr');
            
            candidateRows.forEach(row => {
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

// Initialize search when DOM is ready
document.addEventListener('DOMContentLoaded', function() {
    initializeSearch();
});

// Export functions for global access
window.showAlert = showAlert;
window.clearFilters = clearFilters;
window.openSingleUpload = function() {
    document.getElementById('fileInput').click();
};
window.openBulkUpload = function() {
    const input = document.createElement('input');
    input.type = 'file';
    input.multiple = true;
    input.accept = '.pdf,.doc,.docx,.txt';
    input.onchange = function(e) {
        handleFiles(e.target.files);
    };
    input.click();
};
window.openLinkedInImport = function() {
    showAlert('info', 'LinkedIn import feature coming soon!');
};

// Error handling
window.addEventListener('error', function(e) {
    console.error('JavaScript error:', e.error);
    showAlert('danger', 'An error occurred. Please refresh the page.');
});

// Service Worker registration (for PWA functionality)
if ('serviceWorker' in navigator) {
    window.addEventListener('load', function() {
        navigator.serviceWorker.register('/sw.js')
            .then(function(registration) {
                console.log('ServiceWorker registration successful');
            })
            .catch(function(err) {
                console.log('ServiceWorker registration failed');
            });
    });
}