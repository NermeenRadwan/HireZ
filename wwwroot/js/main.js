// HireZ - Main JavaScript File
// Modern hiring platform functionality

document.addEventListener('DOMContentLoaded', function () {
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
    if (togglePassword) {
        togglePassword.addEventListener('click', function () {
            togglePasswordVisibility('password', this);
        });
    }

    const toggleConfirmPassword = document.getElementById('toggleConfirmPassword');
    if (toggleConfirmPassword) {
        toggleConfirmPassword.addEventListener('click', function () {
            togglePasswordVisibility('confirmPassword', this);
        });
    }
}

// Handle login form submission
async function handleLogin(e) {
    e.preventDefault();

    const formData = new FormData(e.target);
    const email = formData.get('email');
    const password = formData.get('password');

    // Basic validation
    if (!email || !password) {
        showAlert('danger', 'Please enter both email and password.');
        return;
    }

    // Show loading state
    const submitBtn = e.target.querySelector('button[type="submit"]');
    if (submitBtn) {
        const originalText = submitBtn.innerHTML;
        submitBtn.innerHTML = '<i class="fas fa-spinner fa-spin me-2"></i>Signing In...';
        submitBtn.disabled = true;

        try {
            console.log(`Attempting login for: ${email}`);  // For debugging, as per DEBUG_LOGIN.md

            const response = await fetch('/api/auth/login', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({ email, password }),
            });

            console.log(`API.auth.login response: ${response.status}`);  // Debugging

            if (!response.ok) {
                const errorData = await response.json();
                throw new Error(errorData.message || 'Login failed');
            }

            const data = await response.json();
            if (!data.token) {
                throw new Error('No token received from server');
            }

            // Store JWT token
            localStorage.setItem('jwtToken', data.token);

            showAlert('success', 'Login successful! Redirecting to dashboard...');
            setTimeout(() => {
                window.location.href = 'dashboard.html';
            }, 1500);
        } catch (error) {
            console.error('Login error:', error);
            showAlert('danger', error.message || 'An error occurred. Please try again.');
        } finally {
            submitBtn.innerHTML = originalText;
            submitBtn.disabled = false;
        }
    }
}

// Handle register form submission
async function handleRegister(e) {
    e.preventDefault();

    const formData = new FormData(e.target);
    const email = formData.get('email');
    const password = formData.get('password');
    const confirmPassword = formData.get('confirmPassword');

    // Validate
    if (!email || !password || !confirmPassword) {
        showAlert('danger', 'Please fill in all fields.');
        return;
    }

    if (password !== confirmPassword) {
        showAlert('danger', 'Passwords do not match.');
        return;
    }

    // Loading state
    const submitBtn = e.target.querySelector('button[type="submit"]');
    if (submitBtn) {
        const originalText = submitBtn.innerHTML;
        submitBtn.innerHTML = '<i class="fas fa-spinner fa-spin me-2"></i>Creating Account...';
        submitBtn.disabled = true;

        try {
            const response = await fetch('/api/auth/register', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({ email, password }),  // Add more fields if your form/DTO requires them (e.g., name)
            });

            if (!response.ok) {
                const errorData = await response.json();
                throw new Error(errorData.message || 'Registration failed');
            }

            showAlert('success', 'Account created successfully! Redirecting to login...');
            setTimeout(() => {
                window.location.href = 'login.html';
            }, 1500);
        } catch (error) {
            showAlert('danger', error.message || 'An error occurred. Please try again.');
        } finally {
            submitBtn.innerHTML = originalText;
            submitBtn.disabled = false;
        }
    }
}

// Toggle password visibility
function togglePasswordVisibility(inputId, button) {
    const input = document.getElementById(inputId);
    if (input) {
        const icon = button.querySelector('i');
        if (icon) {
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
    }
}

// Dashboard functionality
async function initializeDashboard() {
    // Initialize charts if Chart.js is available
    if (typeof Chart !== 'undefined') {
        await loadDashboardData();
    }

    // Initialize candidate interactions
    initializeCandidateInteractions();
}

// Load real dashboard data from API and initialize charts
async function loadDashboardData() {
    try {
        const applicationsData = await apiFetch('/api/analytics/applications', { method: 'GET' });
        const interviewsData = await apiFetch('/api/analytics/interviews', { method: 'GET' });
        const hiresData = await apiFetch('/api/analytics/hires', { method: 'GET' });

        // Assuming API returns { labels: [...], data: [...] } for each
        initializeCharts(
            applicationsData.labels || ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun'],
            applicationsData.data || [0, 0, 0, 0, 0, 0],
            interviewsData.data || [0, 0, 0, 0, 0, 0],
            hiresData.data || [0, 0, 0, 0, 0, 0]
        );
    } catch (error) {
        console.error('Error loading dashboard data:', error);
        // Fallback to dummy data
        initializeCharts(
            ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun'],
            [12, 19, 3, 5, 2, 3],
            [8, 15, 2, 4, 1, 2],
            [3, 8, 1, 2, 1, 1]
        );
    }
}

// Initialize dashboard charts with provided data
function initializeCharts(labels, applications, interviews, hires) {
    const pipelineChart = document.getElementById('pipelineChart');
    if (pipelineChart) {
        const ctx = pipelineChart.getContext('2d');
        if (ctx) {
            new Chart(ctx, {
                type: 'line',
                data: {
                    labels: labels,
                    datasets: [{
                        label: 'Applications',
                        data: applications,
                        borderColor: '#007bff',
                        backgroundColor: 'rgba(0, 123, 255, 0.1)',
                        tension: 0.4
                    }, {
                        label: 'Interviews',
                        data: interviews,
                        borderColor: '#28a745',
                        backgroundColor: 'rgba(40, 167, 69, 0.1)',
                        tension: 0.4
                    }, {
                        label: 'Hires',
                        data: hires,
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
                            position: 'top'
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
}

// Initialize candidate interactions
function initializeCandidateInteractions() {
    // Add click handlers for candidate cards
    const candidateCards = document.querySelectorAll('.candidate-card');
    candidateCards.forEach(card => {
        card.addEventListener('click', function () {
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

// Handle files with real API upload
async function handleFiles(files) {
    if (files.length === 0) return;

    // Show upload progress
    const progressCard = document.getElementById('uploadProgress');
    if (progressCard) {
        progressCard.style.display = 'block';
    }

    const progressBar = document.getElementById('progressBar');
    const progressText = document.getElementById('progressText');
    const progressPercent = document.getElementById('progressPercent');

    try {
        for (const file of files) {
            const formData = new FormData();
            formData.append('file', file);

            // Simulate progress while uploading
            let progress = 0;
            const interval = setInterval(() => {
                progress += Math.random() * 15;
                if (progress > 80) progress = 80;  // Cap until real upload finishes
                if (progressBar) progressBar.style.width = progress + '%';
                if (progressPercent) progressPercent.textContent = Math.round(progress) + '%';
            }, 200);

            const response = await apiFetch('/api/resume/upload', {
                method: 'POST',
                body: formData,
                headers: {}  // No Content-Type for FormData
            });

            clearInterval(interval);
            progress = 100;
            if (progressBar) progressBar.style.width = '100%';
            if (progressPercent) progressPercent.textContent = '100%';
            if (progressText) progressText.textContent = 'Upload complete!';

            // Display parsed resume data
            displayParsedResume(response);  // Assuming response is parsed data like { name: '...', skills: [...], etc. }

            showAlert('success', `Resume ${file.name} uploaded and parsed successfully!`);
        }
    } catch (error) {
        console.error('Upload error:', error);
        showAlert('danger', 'Error uploading resume. Please try again.');
    } finally {
        setTimeout(() => {
            if (progressCard) progressCard.style.display = 'none';
        }, 2000);
    }
}

// Display parsed resume data (add a container in HTML if needed, e.g., <div id="parsedResume"></div>)
function displayParsedResume(data) {
    const container = document.getElementById('parsedResume') || document.body;  // Fallback to body if no specific div
    if (container) {
        const resultDiv = document.createElement('div');
        resultDiv.className = 'parsed-resume';
        resultDiv.innerHTML = `
            <h3>Parsed Resume Data</h3>
            <p><strong>Name:</strong> ${data.name || 'N/A'}</p>
            <p><strong>Email:</strong> ${data.email || 'N/A'}</p>
            <p><strong>Skills:</strong> ${data.skills ? data.skills.join(', ') : 'N/A'}</p>
            <p><strong>Experience:</strong> ${data.experience || 'N/A'}</p>
            <!-- Add more fields based on your ParseResumeResponse DTO -->
        `;
        container.appendChild(resultDiv);
    }
}

// Initialize upload methods
function initializeUploadMethods() {
    // Single upload
    window.openSingleUpload = function () {
        const fileInput = document.getElementById('fileInput');
        if (fileInput) fileInput.click();
    };

    // Bulk upload
    window.openBulkUpload = function () {
        const input = document.createElement('input');
        input.type = 'file';
        input.multiple = true;
        input.accept = '.pdf,.doc,.docx,.txt';
        input.onchange = function (e) {
            handleFiles(e.target.files);
        };
        input.click();
    };

    // LinkedIn import
    window.openLinkedInImport = function () {
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
            button.addEventListener('click', function () {
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
async function handleFeedback(e) {
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
    if (submitBtn) {
        const originalText = submitBtn.innerHTML;
        submitBtn.innerHTML = '<i class="fas fa-spinner fa-spin me-2"></i>Submitting...';
        submitBtn.disabled = true;

        try {
            await apiFetch('/api/feedback/submit', {
                method: 'POST',
                body: JSON.stringify({ rating, recommendation }),
            });
            showAlert('success', 'Feedback submitted successfully!');
            e.target.reset();
        } catch (error) {
            showAlert('danger', 'Error submitting feedback.');
        } finally {
            submitBtn.innerHTML = originalText;
            submitBtn.disabled = false;
        }
    }
}

// Initialize rating system
function initializeRatingSystem() {
    const ratingInputs = document.querySelectorAll('.rating-input input[type="radio"]');
    ratingInputs.forEach(input => {
        input.addEventListener('change', function () {
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
async function initializeProfile() {
    const profileForm = document.getElementById('profileForm');
    const companyForm = document.getElementById('companyForm');
    const securityForm = document.getElementById('securityForm');

    if (profileForm || companyForm || securityForm) {
        await loadProfileData();
    }

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

// Load real profile data from API
async function loadProfileData() {
    try {
        const profileData = await apiFetch('/api/user/profile', { method: 'GET' });

        // Populate profile form (adjust IDs based on your HTML)
        const firstNameInput = document.getElementById('firstName');
        if (firstNameInput) firstNameInput.value = profileData.firstName || '';

        const lastNameInput = document.getElementById('lastName');
        if (lastNameInput) lastNameInput.value = profileData.lastName || '';

        const emailInput = document.getElementById('email');
        if (emailInput) emailInput.value = profileData.email || '';
        // Add more fields as per UserProfileResponse DTO

        // Populate company form if applicable
        if (profileData.company) {
            const companyNameInput = document.getElementById('companyName');
            if (companyNameInput) companyNameInput.value = profileData.company.name || '';
            // etc.
        }
    } catch (error) {
        console.error('Error loading profile:', error);
        showAlert('danger', 'Error loading profile data.');
    }
}

// Handle profile update
async function handleProfileUpdate(e) {
    e.preventDefault();

    const formData = new FormData(e.target);
    const data = {
        firstName: formData.get('firstName'),
        lastName: formData.get('lastName'),
        // Add more fields
    };

    try {
        await apiFetch('/api/user/update-profile', {
            method: 'PUT',
            body: JSON.stringify(data),
        });
        showAlert('success', 'Profile updated successfully!');
    } catch (error) {
        showAlert('danger', 'Error updating profile.');
    }
}

// Handle company update
async function handleCompanyUpdate(e) {
    e.preventDefault();

    const formData = new FormData(e.target);
    const data = {
        name: formData.get('companyName'),
        // Add more fields
    };

    try {
        await apiFetch('/api/user/update-company', {
            method: 'PUT',
            body: JSON.stringify(data),
        });
        showAlert('success', 'Company information updated successfully!');
    } catch (error) {
        showAlert('danger', 'Error updating company info.');
    }
}

// Handle security update
async function handleSecurityUpdate(e) {
    e.preventDefault();

    const formData = new FormData(e.target);
    const currentPassword = formData.get('currentPassword');
    const newPassword = formData.get('newPassword');
    const confirmPassword = formData.get('confirmPassword');

    if (!currentPassword || !newPassword || !confirmPassword) {
        showAlert('warning', 'Please fill in all password fields.');
        return;
    }

    if (newPassword !== confirmPassword) {
        showAlert('danger', 'New passwords do not match.');
        return;
    }

    try {
        await apiFetch('/api/user/update-password', {
            method: 'PUT',
            body: JSON.stringify({ currentPassword, newPassword }),
        });
        showAlert('success', 'Password updated successfully!');
        e.target.reset();
    } catch (error) {
        showAlert('danger', 'Error updating password.');
    }
}

// Navigation functionality
function initializeNavigation() {
    // Mobile menu toggle
    const navbarToggler = document.querySelector('.navbar-toggler');
    const navbarCollapse = document.querySelector('.navbar-collapse');

    if (navbarToggler && navbarCollapse) {
        navbarToggler.addEventListener('click', function () {
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
        if (link.getAttribute('href') === currentPage) {
            link.classList.add('active');
        }
    });
}

// Utility function for authorized API calls
async function apiFetch(url, options = {}) {
    const token = localStorage.getItem('jwtToken');
    if (!token) {
        showAlert('danger', 'Please log in first.');
        window.location.href = 'login.html';
        return;
    }

    const headers = new Headers(options.headers || {});
    headers.append('Authorization', `Bearer ${token}`);

    if (!(options.body instanceof FormData)) {
        headers.append('Content-Type', 'application/json');
    }

    const fetchOptions = {
        ...options,
        headers: headers,
    };

    try {
        const response = await fetch(url, fetchOptions);
        if (!response.ok) {
            if (response.status === 401) {
                localStorage.removeItem('jwtToken');
                showAlert('danger', 'Session expired. Please log in again.');
                window.location.href = 'login.html';
            }
            const errorData = await response.json();
            throw new Error(errorData.message || `API error: ${response.status}`);
        }
        return await response.json();
    } catch (error) {
        console.error('API fetch error:', error);
        showAlert('danger', error.message || 'An error occurred. Please try again.');
        throw error;
    }
}

// Global alert function
function showAlert(type, message) {
    const alertContainer = document.querySelector('.alert-container') || document.body;
    const alert = document.createElement('div');
    alert.className = `alert alert-${type} alert-dismissible fade show`;
    alert.innerHTML = `
        ${message}
        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
    `;
    alertContainer.prepend(alert);
    setTimeout(() => alert.remove(), 5000);
}

// Add this if initializeAnimations is missing
function initializeAnimations() {
    // Add any animation initializations here if needed
}