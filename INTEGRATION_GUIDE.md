# HireZ Frontend-Backend Integration Guide

## Overview

This document provides instructions for integrating and running the HireZ application with the connected frontend and backend.

## Prerequisites

1. **.NET 8.0 SDK** - Required for running the backend API
2. **SQL Server** - Local SQL Server instance or SQL Server Express
3. **Web Browser** - Modern browser (Chrome, Firefox, Edge, Safari)
4. **Web Server** (optional) - For serving frontend files during development

## Setup Instructions

### 1. Backend Setup

1. **Update Connection String**
   - Edit `appsettings.json` or `appsettings.Development.json`
   - Update the `DefaultConnection` connection string to match your SQL Server instance:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=localhost;Database=HireZDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=true"
   }
   ```

2. **Run Database Migrations**
   - Migrations are automatically applied on startup (see `Program.cs`)
   - Database will be created automatically if it doesn't exist

3. **Start the Backend API**
   ```bash
   dotnet run
   ```
   - API will be available at: `http://localhost:5179` (HTTP) or `https://localhost:7059` (HTTPS)
   - Swagger UI will be available at: `http://localhost:5179/swagger` (development only)

### 2. Frontend Setup

1. **Configure API Base URL**
   - Edit `HireZFrontend/js/api-client.js`
   - Update `API_CONFIG.baseURL` if your backend is running on a different port:
   ```javascript
   const API_CONFIG = {
       baseURL: 'http://localhost:5179/api', // Change if needed
   };
   ```

2. **Serve Frontend Files**
   
   **Option A: Using Python HTTP Server**
   ```bash
   cd HireZFrontend
   python -m http.server 8000
   ```
   - Frontend will be available at: `http://localhost:8000`

   **Option B: Using Node.js http-server**
   ```bash
   cd HireZFrontend
   npx http-server -p 8000
   ```

   **Option C: Using PHP**
   ```bash
   cd HireZFrontend
   php -S localhost:8000
   ```

   **Option D: Using VS Code Live Server Extension**
   - Install "Live Server" extension in VS Code
   - Right-click on `index.html` and select "Open with Live Server"

3. **Update CORS Configuration (if using different port)**
   - If you serve the frontend on a port other than 8000 or 3000, update `Program.cs`:
   ```csharp
   policy.WithOrigins("http://localhost:8000", "http://localhost:3000", "http://YOUR_PORT")
   ```

## Running the Application

1. **Start Backend** (Terminal 1)
   ```bash
   cd HireZ
   dotnet run
   ```

2. **Start Frontend Server** (Terminal 2)
   ```bash
   cd HireZFrontend
   python -m http.server 8000
   ```

3. **Open Browser**
   - Navigate to: `http://localhost:8000`
   - Or open `index.html` directly (if CORS is configured properly)

## API Endpoints

### Authentication
- `POST /api/auth/register` - Register new user
- `POST /api/auth/login` - Login user (returns JWT token)

### Profile
- `GET /api/profile` - Get current user profile (requires auth)

### Resume
- `POST /api/resume/upload` - Upload resume file (requires auth)
- `GET /api/resume/{id}` - Get resume details (requires auth)
- `POST /api/resume/{id}/reprocess` - Reprocess resume (requires auth)

### Jobs
- `POST /api/jobs` - Create job posting (requires auth)
- `GET /api/jobs/{id}` - Get job details (requires auth)
- `POST /api/jobs/{jobId}/match/{resumeId}` - Match resume to job (requires auth)

### Interview
- `POST /api/interview/sessions` - Create interview session (requires auth)
- `GET /api/interview/sessions/{id}` - Get interview session (requires auth)

### Analytics
- `GET /api/analytics/overview` - Get dashboard overview (requires auth)
- `GET /api/analytics/trends?days=30` - Get trends data (requires auth)

### Feedback
- `GET /api/resume/{resumeId}/feedback` - Get feedback for resume (requires auth)
- `GET /api/resume/{resumeId}/feedback/{id}` - Get specific feedback (requires auth)

## Authentication Flow

1. User registers or logs in via `/api/auth/register` or `/api/auth/login`
2. Backend returns JWT token in response
3. Frontend stores token in `localStorage` as `authToken`
4. All subsequent API requests include token in `Authorization` header: `Bearer {token}`
5. On 401 responses, frontend redirects to login page

## Testing the Integration

### Test Authentication
1. Navigate to `register.html`
2. Register a new user
3. You should be redirected to `login.html`
4. Login with your credentials
5. You should be redirected to `dashboard.html`

### Test Resume Upload
1. Login to the application
2. Navigate to CV Upload page
3. Upload a PDF resume
4. Check for success message with resume ID

### Test Dashboard
1. Login to the application
2. Navigate to Dashboard
3. Verify that analytics data is loaded (if available)
4. Check browser console for any errors

## Troubleshooting

### CORS Errors
- **Symptom**: Browser console shows CORS errors
- **Solution**: 
  - Ensure backend is running
  - Check CORS configuration in `Program.cs`
  - Verify frontend origin matches CORS allowed origins
  - For development, you can temporarily allow all origins (not for production)

### 401 Unauthorized Errors
- **Symptom**: API returns 401 errors
- **Solution**:
  - Check if token is stored in localStorage
  - Verify token is included in Authorization header
  - Try logging in again to get a fresh token

### API Connection Errors
- **Symptom**: "Network error" or "Failed to fetch" errors
- **Solution**:
  - Verify backend is running (`http://localhost:5179/swagger`)
  - Check API base URL in `api-client.js`
  - Verify no firewall is blocking the connection
  - Check browser console for detailed error messages

### Database Errors
- **Symptom**: Backend fails to start or API returns 500 errors
- **Solution**:
  - Verify SQL Server is running
  - Check connection string in `appsettings.json`
  - Ensure database exists or can be created
  - Check backend logs for detailed error messages

## Development Tips

1. **Use Browser DevTools**
   - Open Network tab to monitor API requests
   - Check Console for JavaScript errors
   - Use Application tab to inspect localStorage

2. **Use Swagger UI**
   - Access at `http://localhost:5179/swagger`
   - Test API endpoints directly
   - Use "Authorize" button to test authenticated endpoints

3. **Debug API Client**
   - Check `api-client.js` for request/response logging
   - Verify token is being stored and sent correctly
   - Check error handling in catch blocks

4. **Check Backend Logs**
   - Backend logs will show incoming requests
   - Errors will be logged with stack traces
   - Check for validation errors or database issues

## Next Steps

1. Test all integrated features
2. Add additional error handling where needed
3. Implement remaining features (interview prep, ATS matcher, etc.)
4. Add loading states and better UX feedback
5. Implement token refresh if needed
6. Add request/response logging for debugging
7. Set up production configuration

