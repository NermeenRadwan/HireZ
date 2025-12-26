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
async function handleLogin(e) {
    e.preventDefault();
    
    const formData = new FormData(e.target);
    const email = formData.get('email');
    const password = formData.get('password');
    
    if (!email || !password) {
        showAlert('warning', 'Please enter both email and password.');
        return;
    }
    
    // Show loading state
    const submitBtn = e.target.querySelector('button[type="submit"]');
    const originalText = submitBtn.innerHTML;
    submitBtn.innerHTML = '<i class="fas fa-spinner fa-spin me-2"></i>Signing In...';
    submitBtn.disabled = true;
    
    try {
        // Call API
        await API.auth.login(email, password);
        showAlert('success', 'Login successful! Redirecting to dashboard...');
        setTimeout(() => {
            window.location.href = 'dashboard.html';
        }, 1000);
    } catch (error) {
        console.error('Login error:', error);
        showAlert('danger', error.message || 'Invalid email or password. Please try again.');
        submitBtn.innerHTML = originalText;
        submitBtn.disabled = false;
    }
}

// Handle register form submission
async function handleRegister(e) {
    e.preventDefault();
    
    const formData = new FormData(e.target);
    const email = formData.get('email');
    const password = formData.get('password');
    const confirmPassword = formData.get('confirmPassword');
    
    // Validate password match
    if (password !== confirmPassword) {
        showAlert('danger', 'Passwords do not match. Please try again.');
        return;
    }
    
    if (!email || !password) {
        showAlert('warning', 'Please fill in all required fields.');
        return;
    }
    
    // Show loading state
    const submitBtn = e.target.querySelector('button[type="submit"]');
    const originalText = submitBtn.innerHTML;
    submitBtn.innerHTML = '<i class="fas fa-spinner fa-spin me-2"></i>Creating Account...';
    submitBtn.disabled = true;
    
    try {
        // Call API
        await API.auth.register(email, password);
        showAlert('success', 'Account created successfully! Redirecting to login...');
        setTimeout(() => {
            window.location.href = 'login.html';
        }, 1500);
    } catch (error) {
        console.error('Registration error:', error);
        showAlert('danger', error.message || 'Registration failed. Please try again.');
        submitBtn.innerHTML = originalText;
        submitBtn.disabled = false;
    }
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
async function initializeDashboard() {
    // Load dashboard data from API
    try {
        await loadDashboardData();
    } catch (error) {
        console.error('Error loading dashboard data:', error);
        showAlert('warning', 'Failed to load dashboard data. Showing default view.');
    }
    
    // Initialize charts if Chart.js is available
    if (typeof Chart !== 'undefined') {
        initializeCharts();
    }
    
    // Initialize candidate interactions
    initializeCandidateInteractions();
}

// Load dashboard data from API
async function loadDashboardData() {
    try {
        // Load analytics overview
        const overview = await API.analytics.getOverview();
        
        // Update dashboard metrics if elements exist
        updateDashboardMetrics(overview);
        
        // Load trends data
        const trends = await API.analytics.getTrends(30);
        
        // Update charts with real data
        updateDashboardCharts(trends);

        // Load user resumes
        await loadResumeList();
        
    } catch (error) {
        console.error('Error fetching dashboard data:', error);
        throw error;
    }
}

// Load and display resume list
async function loadResumeList() {
    try {
        const resumes = await API.resume.getAll();
        displayResumeList(resumes);
    } catch (error) {
        console.error('Error loading resumes:', error);
    }
}

// Display resume list on dashboard
function displayResumeList(resumes) {
    // Look for resume list container in dashboard
    const resumeContainer = document.getElementById('resumeList') || 
                           document.querySelector('.resume-list') ||
                           document.querySelector('#candidateList');
    
    if (!resumeContainer) return;

    if (resumes.length === 0) {
        resumeContainer.innerHTML = '<p class="text-muted">No resumes uploaded yet. <a href="cv-upload.html">Upload your first resume</a></p>';
        return;
    }

    let html = '';
    resumes.forEach(resume => {
        html += `
            <div class="card mb-3 resume-item" data-resume-id="${resume.id}">
                <div class="card-body">
                    <div class="d-flex justify-content-between align-items-center">
                        <div>
                            <h6 class="mb-1">${resume.fileName || `Resume #${resume.id}`}</h6>
                            <small class="text-muted">Uploaded: ${new Date(resume.uploadedAt).toLocaleDateString()}</small>
                            <span class="badge bg-${resume.status === 'Completed' ? 'success' : resume.status === 'Processing' ? 'warning' : 'secondary'} ms-2">${resume.status || 'Unknown'}</span>
                        </div>
                        <div>
                            <button class="btn btn-sm btn-outline-primary" onclick="viewResume(${resume.id})">
                                <i class="fas fa-eye me-1"></i>View
                            </button>
                            ${resume.status !== 'Completed' ? `
                                <button class="btn btn-sm btn-outline-warning" onclick="reprocessResume(${resume.id})">
                                    <i class="fas fa-redo me-1"></i>Reprocess
                                </button>
                            ` : ''}
                        </div>
                    </div>
                </div>
            </div>
        `;
    });
    resumeContainer.innerHTML = html;
}

// View resume details
async function viewResume(resumeId) {
    try {
        const resume = await API.resume.get(resumeId);
        const details = `
Resume Details:
- ID: ${resume.id}
- File Name: ${resume.fileName}
- Status: ${resume.status}
- Uploaded: ${new Date(resume.uploadedAt).toLocaleString()}
${resume.extractedText ? `\nExtracted Text Preview:\n${resume.extractedText.substring(0, 200)}...` : ''}
        `;
        alert(details);
    } catch (error) {
        console.error('Error loading resume:', error);
        showAlert('danger', 'Failed to load resume details.');
    }
}

// Reprocess resume
async function reprocessResume(resumeId) {
    try {
        await API.resume.reprocess(resumeId);
        showAlert('success', 'Resume reprocessing queued. Please check back in a few moments.');
        setTimeout(() => loadResumeList(), 2000);
    } catch (error) {
        console.error('Error reprocessing resume:', error);
        showAlert('danger', 'Failed to reprocess resume.');
    }
}

// Update dashboard metrics
function updateDashboardMetrics(overview) {
    if (!overview) return;
    
    // Update stat cards - look for elements with specific classes or IDs
    // Total Candidates (resumes uploaded)
    const totalCandidatesElements = document.querySelectorAll('.stat-number');
    if (totalCandidatesElements.length > 0 && overview.resumesUploaded !== undefined) {
        totalCandidatesElements[0].textContent = overview.resumesUploaded || 0;
    }
    
    // You can add more metric updates here based on the overview structure
    // For example: overview.resumesAnalyzed, overview.interviewSessions, etc.
    
    console.log('Dashboard overview loaded:', overview);
}

// Update dashboard charts with real data
function updateDashboardCharts(trends) {
    if (!trends || !Array.isArray(trends) || trends.length === 0) {
        console.warn('No trends data available');
        return;
    }
    
    // Store trends data for chart initialization
    window.dashboardTrends = trends;
    
    // If chart is already initialized, update it
    if (window.pipelineChartInstance) {
        updateChartWithTrends(window.pipelineChartInstance, trends);
    }
}

// Update existing chart with trends data
function updateChartWithTrends(chart, trends) {
    const labels = trends.map(t => {
        const date = new Date(t.date);
        return date.toLocaleDateString('en-US', { month: 'short', day: 'numeric' });
    });
    
    const uploadedData = trends.map(t => t.uploadedCount || 0);
    const analyzedData = trends.map(t => t.analyzedCount || 0);
    
    chart.data.labels = labels;
    if (chart.data.datasets.length > 0) {
        chart.data.datasets[0].data = uploadedData;
        if (chart.data.datasets.length > 1) {
            chart.data.datasets[1].data = analyzedData;
        }
    }
    chart.update();
}

// Initialize dashboard charts
function initializeCharts() {
    const pipelineChart = document.getElementById('pipelineChart');
    if (pipelineChart && typeof Chart !== 'undefined') {
        const ctx = pipelineChart.getContext('2d');
        
        // Use trends data if available, otherwise use default data
        let labels = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun'];
        let uploadedData = [12, 19, 3, 5, 2, 3];
        let analyzedData = [8, 15, 2, 4, 1, 2];
        
        if (window.dashboardTrends && Array.isArray(window.dashboardTrends) && window.dashboardTrends.length > 0) {
            labels = window.dashboardTrends.map(t => {
                const date = new Date(t.date);
                return date.toLocaleDateString('en-US', { month: 'short', day: 'numeric' });
            });
            uploadedData = window.dashboardTrends.map(t => t.uploadedCount || 0);
            analyzedData = window.dashboardTrends.map(t => t.analyzedCount || 0);
        }
        
        window.pipelineChartInstance = new Chart(ctx, {
            type: 'line',
            data: {
                labels: labels,
                datasets: [{
                    label: 'Resumes Uploaded',
                    data: uploadedData,
                    borderColor: '#007bff',
                    backgroundColor: 'rgba(0, 123, 255, 0.1)',
                    tension: 0.4
                }, {
                    label: 'Resumes Analyzed',
                    data: analyzedData,
                    borderColor: '#28a745',
                    backgroundColor: 'rgba(40, 167, 69, 0.1)',
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
async function handleFiles(files) {
    if (files.length === 0) return;
    
    // Only handle first file for now (backend expects single file)
    const file = files[0];
    
    // Validate file type
    const allowedTypes = ['application/pdf', 'application/msword', 
                          'application/vnd.openxmlformats-officedocument.wordprocessingml.document',
                          'text/plain'];
    if (!allowedTypes.includes(file.type)) {
        showAlert('warning', 'Invalid file type. Please upload PDF, DOC, DOCX, or TXT files.');
        return;
    }
    
    // Validate file size (10MB max)
    const maxSize = 10 * 1024 * 1024; // 10MB
    if (file.size > maxSize) {
        showAlert('warning', 'File size exceeds 10MB limit.');
        return;
    }
    
    // Show upload progress
    const progressCard = document.getElementById('uploadProgress');
    if (progressCard) {
        progressCard.style.display = 'block';
    }
    
    try {
        // Show initial progress
        updateUploadProgress(0, 'Uploading file...');
        
        // Upload file to API
        const response = await API.resume.upload(file);
        
        // Update progress
        updateUploadProgress(100, 'Upload complete!');
        
        showAlert('success', `Resume uploaded successfully! (ID: ${response.resumeId})`);
        
        // Hide progress after delay
        setTimeout(() => {
            if (progressCard) {
                progressCard.style.display = 'none';
            }
            // Optionally redirect or refresh the page
            // window.location.reload();
        }, 2000);
    } catch (error) {
        console.error('Upload error:', error);
        showAlert('danger', error.message || 'Failed to upload resume. Please try again.');
        if (progressCard) {
            progressCard.style.display = 'none';
        }
    }
}

// Update upload progress
function updateUploadProgress(percent, message) {
    const progressBar = document.getElementById('progressBar');
    const progressText = document.getElementById('progressText');
    const progressPercent = document.getElementById('progressPercent');
    
    if (progressBar) {
        progressBar.style.width = percent + '%';
        progressBar.setAttribute('aria-valuenow', percent);
    }
    
    if (progressPercent) {
        progressPercent.textContent = Math.round(percent) + '%';
    }
    
    if (progressText) {
        progressText.textContent = message || 'Uploading...';
    }
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
async function initializeFeedback() {
    const feedbackForm = document.getElementById('feedbackForm');
    if (feedbackForm) {
        feedbackForm.addEventListener('submit', handleFeedback);
    }
    
    // Initialize rating system
    initializeRatingSystem();

    // Load user resumes for candidate selection
    try {
        const resumes = await API.resume.getAll();
        const candidateSelect = document.getElementById('candidateSelect');
        if (candidateSelect && resumes.length > 0) {
            candidateSelect.innerHTML = '<option value="">Select a resume</option>';
            resumes.forEach(resume => {
                const option = document.createElement('option');
                option.value = resume.id;
                option.textContent = resume.fileName || `Resume #${resume.id}`;
                candidateSelect.appendChild(option);
            });

            // Load feedback when resume is selected
            candidateSelect.addEventListener('change', async function() {
                const resumeId = parseInt(this.value);
                if (resumeId) {
                    await loadFeedbackForResume(resumeId);
                }
            });
        }
    } catch (error) {
        console.error('Error loading resumes for feedback:', error);
    }
}

// Handle feedback submission
async function handleFeedback(e) {
    e.preventDefault();
    
    const formData = new FormData(e.target);
    const candidateSelect = document.getElementById('candidateSelect');
    const resumeId = candidateSelect ? parseInt(candidateSelect.value) : null;
    const rating = formData.get('rating');
    const recommendation = formData.get('recommendation');
    
    if (!resumeId) {
        showAlert('warning', 'Please select a candidate/resume.');
        return;
    }
    
    if (!rating || !recommendation) {
        showAlert('warning', 'Please provide a rating and recommendation.');
        return;
    }
    
    // Show loading state
    const submitBtn = e.target.querySelector('button[type="submit"]');
    const originalText = submitBtn.innerHTML;
    submitBtn.innerHTML = '<i class="fas fa-spinner fa-spin me-2"></i>Submitting...';
    submitBtn.disabled = true;
    
    try {
        const feedbackData = {
            candidateName: candidateSelect.options[candidateSelect.selectedIndex].text,
            interviewType: formData.get('interviewType'),
            rating: parseInt(rating),
            technicalSkills: formData.get('technicalSkills'),
            communication: formData.get('communication'),
            problemSolving: formData.get('problemSolving'),
            culturalFit: formData.get('culturalFit'),
            comments: formData.get('feedbackComments'),
            recommendation: recommendation
        };

        await API.feedback.create(resumeId, feedbackData);
        showAlert('success', 'Feedback submitted successfully!');
        e.target.reset();
        
        // Reload feedback list
        await loadFeedbackForResume(resumeId);
        
        submitBtn.innerHTML = originalText;
        submitBtn.disabled = false;
    } catch (error) {
        console.error('Feedback submission error:', error);
        showAlert('danger', error.message || 'Failed to submit feedback. Please try again.');
        submitBtn.innerHTML = originalText;
        submitBtn.disabled = false;
    }
}

// Load feedback for a resume
async function loadFeedbackForResume(resumeId) {
    try {
        const feedbackList = await API.feedback.getForResume(resumeId);
        displayFeedbackList(feedbackList);
    } catch (error) {
        console.error('Error loading feedback:', error);
    }
}

// Display feedback list
function displayFeedbackList(feedbackList) {
    const feedbackHistory = document.querySelector('#feedbackHistory tbody');
    if (!feedbackHistory) return;

    feedbackHistory.innerHTML = '';
    
    if (feedbackList.length === 0) {
        feedbackHistory.innerHTML = '<tr><td colspan="7" class="text-center">No feedback yet</td></tr>';
        return;
    }

    feedbackList.forEach(feedback => {
        const row = document.createElement('tr');
        // Parse feedback JSON if available
        let feedbackData = {};
        try {
            if (feedback.content) {
                feedbackData = JSON.parse(feedback.content);
            }
        } catch (e) {
            // If not JSON, use content as is
        }

        row.innerHTML = `
            <td>
                <div class="d-flex align-items-center">
                    <div class="candidate-avatar me-3">
                        <i class="fas fa-user"></i>
                    </div>
                    <div>
                        <h6 class="mb-1">${feedbackData.candidateName || 'Candidate'}</h6>
                        <p class="text-muted mb-0">Resume #${feedback.resumeId}</p>
                    </div>
                </div>
            </td>
            <td>${feedbackData.interviewType || 'N/A'}</td>
            <td>
                <div class="rating">
                    ${generateStars(feedbackData.rating || 0)}
                    <span class="ms-1">${feedbackData.rating || 0}/5</span>
                </div>
            </td>
            <td><span class="badge bg-${getRecommendationBadgeColor(feedbackData.recommendation)}">${feedbackData.recommendation || 'N/A'}</span></td>
            <td>${new Date(feedback.createdAt).toLocaleDateString()}</td>
            <td>
                <div class="btn-group btn-group-sm">
                    <button class="btn btn-outline-primary" onclick="viewFeedbackDetails(${feedback.id}, ${feedback.resumeId})" title="View Details">
                        <i class="fas fa-eye"></i>
                    </button>
                </div>
            </td>
        `;
        feedbackHistory.appendChild(row);
    });
}

function generateStars(rating) {
    let stars = '';
    for (let i = 1; i <= 5; i++) {
        if (i <= rating) {
            stars += '<i class="fas fa-star text-warning"></i>';
        } else {
            stars += '<i class="far fa-star text-muted"></i>';
        }
    }
    return stars;
}

function getRecommendationBadgeColor(recommendation) {
    if (recommendation === 'hire') return 'success';
    if (recommendation === 'maybe') return 'warning';
    if (recommendation === 'no-hire') return 'danger';
    return 'secondary';
}

async function viewFeedbackDetails(feedbackId, resumeId) {
    try {
        const feedback = await API.feedback.getById(resumeId, feedbackId);
        // Display feedback details in a modal or alert
        let feedbackData = {};
        try {
            if (feedback.content) {
                feedbackData = JSON.parse(feedback.content);
            }
        } catch (e) {
            feedbackData = { comments: feedback.content };
        }

        const details = `
            Candidate: ${feedbackData.candidateName || 'N/A'}
            Interview Type: ${feedbackData.interviewType || 'N/A'}
            Rating: ${feedbackData.rating || 'N/A'}/5
            Technical Skills: ${feedbackData.technicalSkills || 'N/A'}
            Communication: ${feedbackData.communication || 'N/A'}
            Problem Solving: ${feedbackData.problemSolving || 'N/A'}
            Cultural Fit: ${feedbackData.culturalFit || 'N/A'}
            Recommendation: ${feedbackData.recommendation || 'N/A'}
            Comments: ${feedbackData.comments || 'No comments'}
        `;
        alert(details);
    } catch (error) {
        console.error('Error loading feedback details:', error);
        showAlert('danger', 'Failed to load feedback details.');
    }
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
async function initializeProfile() {
    // Load user profile data
    try {
        const profile = await API.profile.get();
        updateProfileDisplay(profile);
    } catch (error) {
        console.error('Error loading profile:', error);
        showAlert('warning', 'Failed to load profile data.');
    }
    
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

// Update profile display with data from API
function updateProfileDisplay(profile) {
    // Update email display if element exists
    const emailInput = document.getElementById('email');
    if (emailInput && profile.email) {
        emailInput.value = profile.email;
        emailInput.setAttribute('readonly', true); // Email typically shouldn't be editable
    }
    
    // Update role display if element exists
    const roleDisplay = document.getElementById('userRole');
    if (roleDisplay && profile.role) {
        roleDisplay.textContent = profile.role;
    }
    
    // Update created date if element exists
    const createdDate = document.getElementById('createdDate');
    if (createdDate && profile.createdAt) {
        const date = new Date(profile.createdAt);
        createdDate.textContent = date.toLocaleDateString();
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
    
    // Logout functionality
    setupLogoutHandlers();
}

// Setup logout handlers
function setupLogoutHandlers() {
    const logoutLinks = document.querySelectorAll('a[href*="login.html"], .logout-link');
    logoutLinks.forEach(link => {
        link.addEventListener('click', function(e) {
            // Only handle if it's actually a logout action
            if (this.textContent.toLowerCase().includes('logout') || this.classList.contains('logout-link')) {
                e.preventDefault();
                handleLogout();
            }
        });
    });
}

// Handle logout
function handleLogout() {
    API.auth.logout();
    showAlert('success', 'Logged out successfully!');
    setTimeout(() => {
        window.location.href = 'login.html';
    }, 500);
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
window.viewResume = viewResume;
window.reprocessResume = reprocessResume;
window.viewFeedbackDetails = viewFeedbackDetails;
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