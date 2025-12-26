# HireZ Integration Testing Guide

This guide provides step-by-step instructions for testing all integrated features in the HireZ application.

## Prerequisites

1. **Backend Server Running**
   ```bash
   cd "HireZ-Desouky 2"
   dotnet run
   ```
   The API should be available at `http://localhost:5179` (or the port specified in `launchSettings.json`)

2. **Frontend Access**
   - Open `HireZFrontend/index.html` in a web browser, OR
   - Serve the frontend through the backend (if configured), OR
   - Use a local web server (e.g., `python -m http.server` in the HireZFrontend directory)

3. **Database Setup**
   - Ensure database migrations are applied: `dotnet ef database update`
   - Database should be seeded with initial data (if applicable)

---

## Test Scenarios

### 1. Authentication Integration ✅

#### Test 1.1: User Registration
1. Navigate to `register.html`
2. Fill in:
   - Email: `test@example.com`
   - Password: `Test123!`
   - Confirm Password: `Test123!`
3. Click "Register"
4. **Expected**: Success message, redirect to login page
5. **Verify**: Check browser console for API call to `/api/auth/register`

#### Test 1.2: User Login
1. Navigate to `login.html`
2. Enter credentials:
   - Email: `test@example.com`
   - Password: `Test123!`
3. Click "Sign In"
4. **Expected**: 
   - Success message
   - Redirect to `dashboard.html`
   - Token stored in `localStorage` (check DevTools → Application → Local Storage)
5. **Verify**: Check browser console for API call to `/api/auth/login`

#### Test 1.3: Logout
1. While logged in, click "Logout" in navigation
2. **Expected**: 
   - Redirect to login page
   - Token removed from localStorage
3. **Verify**: Check that `authToken` is removed from localStorage

---

### 2. Profile Integration ✅

#### Test 2.1: View Profile
1. Navigate to `profile.html` (must be logged in)
2. **Expected**: 
   - Profile data loads automatically
   - Email, role, and creation date displayed
3. **Verify**: Check browser console for API call to `/api/profile`
4. **Check Network Tab**: Should see GET request to `/api/profile` with Authorization header

---

### 3. Resume Upload Integration ✅

#### Test 3.1: Upload Resume
1. Navigate to `cv-upload.html` or `upload.html`
2. Click upload area or "Choose File"
3. Select a PDF, DOC, or DOCX file (test file provided or use any resume PDF)
4. **Expected**: 
   - Progress bar shows upload progress
   - Success message with resume ID
   - File uploaded successfully
5. **Verify**: 
   - Check browser console for API call to `/api/resume/upload`
   - Check Network Tab: POST request to `/api/resume/upload` with FormData
   - Response should contain `resumeId` and `fileName`

#### Test 3.2: View Resume List
1. Navigate to `dashboard.html`
2. **Expected**: 
   - List of uploaded resumes displayed
   - Each resume shows: filename, upload date, status
3. **Verify**: Check browser console for API call to `/api/resume` (GET)
4. **Check**: Resumes should appear in a list format

#### Test 3.3: View Resume Details
1. On dashboard, click "View" button on any resume
2. **Expected**: 
   - Alert or modal showing resume details:
     - Resume ID
     - File name
     - Status
     - Upload date
     - Extracted text preview (if available)
3. **Verify**: Check browser console for API call to `/api/resume/{id}`

#### Test 3.4: Reprocess Resume
1. On dashboard, find a resume with status "Processing" or "Failed"
2. Click "Reprocess" button
3. **Expected**: 
   - Success message: "Resume reprocessing queued"
   - Resume list refreshes after 2 seconds
4. **Verify**: Check browser console for API call to `/api/resume/{id}/reprocess` (POST)

---

### 4. ATS Matcher Integration ✅

#### Test 4.1: Create Job and Match Resume
1. Navigate to `ats-matcher.html`
2. **Prerequisites**: Must have at least one resume uploaded
3. Select a resume from the dropdown (if multiple resumes)
4. Fill in job details:
   - Job Title: `Senior Software Engineer`
   - Company: `Tech Corp`
   - Job Description: `We are looking for a Senior Software Engineer with experience in JavaScript, React, Node.js, and cloud technologies.`
   - Location: `New York, NY` (optional)
   - Job Type: `Full-time` (optional)
5. Click "Start Matching"
6. **Expected**: 
   - Loading state shows "Matching..."
   - Results card appears
   - Overall match score displayed (0-100%)
   - ATS score displayed
   - Matched keywords shown with green badges
   - Missing keywords shown with red badges
   - Recommendations displayed
7. **Verify**: 
   - Check browser console for:
     - POST `/api/jobs` (create job)
     - POST `/api/jobs/{jobId}/match/{resumeId}` (match resume)
   - Check Network Tab for both API calls
   - Response should contain `atsScore`, `matchedKeywords`, `missingKeywords`

#### Test 4.2: Resume Selection
1. On ATS Matcher page, check resume selector dropdown
2. **Expected**: 
   - Dropdown populated with user's resumes
   - Selected resume displayed in CV preview section
3. **Verify**: Check that resumes are loaded from `/api/resume` (GET)

---

### 5. Interview Preparation Integration ✅

#### Test 5.1: Generate Interview Questions
1. Navigate to `interview-prep.html`
2. **Prerequisites**: Must have at least one resume uploaded
3. Fill in form:
   - Select Resume: Choose a resume from dropdown
   - Job ID: Leave empty (optional) OR enter a job ID if you created one
   - Number of Questions: `8` (default)
   - Question Source: `AI Generated` or `Heuristic`
   - Job Title: `Frontend Developer` (for context)
4. Click "Generate Questions"
5. **Expected**: 
   - Loading state: "Generating..."
   - Questions section appears
   - Questions displayed with:
     - Question text
     - Category badge
     - Source badge (AI/Heuristic)
     - "Show Answer" and "Mark as Practiced" buttons
6. **Verify**: 
   - Check browser console for:
     - POST `/api/interview/sessions` (create session)
     - GET `/api/interview/sessions/{id}` (get session with questions)
   - Check Network Tab for API calls
   - Response should contain `sessionId` and `questions` array

#### Test 5.2: View Generated Questions
1. After questions are generated, scroll through the list
2. Click "Show Answer" on any question
3. **Expected**: Answer content toggles visibility
4. Click "Mark as Practiced"
5. **Expected**: 
   - Button changes to "Practiced"
   - Progress bar updates
   - Badge shows practiced count

#### Test 5.3: Regenerate Questions
1. After generating questions, click "Regenerate"
2. **Expected**: New set of questions generated
3. **Verify**: New API calls to create a new session

---

### 6. Feedback Integration ✅

#### Test 6.1: Submit Feedback
1. Navigate to `feedback.html`
2. Fill in feedback form:
   - Candidate: Select a resume from dropdown
   - Interview Type: `Technical Interview`
   - Overall Rating: Select stars (1-5)
   - Technical Skills: `Excellent`
   - Communication: `Good`
   - Problem Solving: `Average`
   - Cultural Fit: `Excellent`
   - Detailed Feedback: Enter some comments
   - Recommendation: Select `Hire`, `Maybe`, or `No Hire`
3. Click "Submit Feedback"
4. **Expected**: 
   - Loading state: "Submitting..."
   - Success message: "Feedback submitted successfully!"
   - Form resets
   - Feedback history table updates
5. **Verify**: 
   - Check browser console for:
     - POST `/api/resume/{resumeId}/feedback`
   - Check Network Tab for API call
   - Response should contain feedback `id`

#### Test 6.2: View Feedback History
1. On feedback page, select a resume from "Candidate" dropdown
2. **Expected**: 
   - Feedback history table updates
   - Shows all feedback entries for selected resume
   - Each entry shows: candidate name, interview type, rating, recommendation, date
3. **Verify**: 
   - Check browser console for:
     - GET `/api/resume/{resumeId}/feedback`
   - Check Network Tab for API call
   - Response should be an array of feedback objects

#### Test 6.3: View Feedback Details
1. In feedback history table, click "View Details" (eye icon) on any feedback entry
2. **Expected**: 
   - Alert or modal showing full feedback details:
     - Candidate name
     - Interview type
     - Rating
     - All skill assessments
     - Recommendation
     - Comments
3. **Verify**: 
   - Check browser console for:
     - GET `/api/resume/{resumeId}/feedback/{id}`

---

### 7. Analytics Integration ✅

#### Test 7.1: View Dashboard Analytics
1. Navigate to `dashboard.html`
2. **Expected**: 
   - Dashboard loads automatically
   - Analytics overview displayed:
     - Total resumes uploaded
     - Other metrics (if available)
   - Trends chart displayed (if Chart.js is loaded)
   - Chart shows resume uploads vs analyses over time
3. **Verify**: 
   - Check browser console for:
     - GET `/api/analytics/overview`
     - GET `/api/analytics/trends?days=30`
   - Check Network Tab for both API calls
   - Chart should update with real data

---

## Common Issues and Troubleshooting

### Issue: API calls return 401 Unauthorized
**Solution**: 
- Check if user is logged in
- Verify token in localStorage (`authToken`)
- Check if token is expired
- Try logging out and logging back in

### Issue: API calls return 404 Not Found
**Solution**:
- Verify backend server is running
- Check API endpoint URLs in `api-client.js`
- Verify route paths match backend controllers
- Check browser console for exact error

### Issue: CORS errors
**Solution**:
- Ensure backend CORS is configured in `Program.cs`
- Check if frontend and backend are on same origin
- Verify CORS policy allows your frontend origin

### Issue: Resume upload fails
**Solution**:
- Check file size (max 10MB)
- Verify file type (PDF, DOC, DOCX, TXT)
- Check backend file storage configuration
- Verify database connection

### Issue: Questions not generating
**Solution**:
- Ensure resume is uploaded and processed
- Check if resume has extracted text
- Verify interview service is configured
- Check backend logs for errors

### Issue: Feedback not submitting
**Solution**:
- Verify resume is selected
- Check all required fields are filled
- Verify backend FeedbackController has POST endpoint
- Check database for ResumeFeedback table

---

## API Endpoint Testing (Using Browser DevTools)

### Manual API Testing

You can test endpoints directly using browser console:

```javascript
// Test login
await API.auth.login('test@example.com', 'password');

// Test get profile
await API.profile.get();

// Test get resumes
await API.resume.getAll();

// Test create job
await API.jobs.create({
    title: 'Test Job',
    description: 'Test description',
    requirements: 'Test requirements'
});

// Test match resume
await API.jobs.matchResumeToJob(1, 1);

// Test create interview session
await API.interview.createSession({
    resumeId: 1,
    jobId: null,
    count: 8,
    preferredSource: 'ai'
});

// Test get feedback
await API.feedback.getForResume(1);

// Test analytics
await API.analytics.getOverview();
await API.analytics.getTrends(30);
```

---

## Verification Checklist

After completing all tests, verify:

- [ ] All API endpoints are called correctly
- [ ] Data flows from frontend to backend
- [ ] Responses are handled and displayed
- [ ] Error messages are user-friendly
- [ ] Loading states are shown during API calls
- [ ] Success messages appear after operations
- [ ] Navigation works correctly
- [ ] Authentication persists across page reloads
- [ ] Token is stored and sent with requests
- [ ] Unauthorized requests redirect to login

---

## Performance Testing

1. **Load Time**: Check how long pages take to load with API calls
2. **Concurrent Requests**: Test multiple API calls simultaneously
3. **Large Data**: Test with many resumes, feedback entries, etc.
4. **Network Conditions**: Test with slow 3G simulation in DevTools

---

## Browser Compatibility

Test in:
- [ ] Chrome/Edge (Chromium)
- [ ] Firefox
- [ ] Safari
- [ ] Mobile browsers (if applicable)

---

## Notes

- All API calls require authentication (except register/login)
- Token is automatically included in requests via `api-client.js`
- Errors are caught and displayed via `showAlert()` function
- Loading states are managed in each function
- Data is stored in browser localStorage for session persistence

---

## Next Steps

If all tests pass:
1. Test with real resume files
2. Test with multiple users
3. Test edge cases (empty data, large files, etc.)
4. Performance optimization
5. User acceptance testing

If tests fail:
1. Check browser console for errors
2. Check backend logs
3. Verify database state
4. Review integration code
5. Check network requests in DevTools

