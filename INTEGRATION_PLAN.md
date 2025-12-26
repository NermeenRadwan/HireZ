# HireZ Frontend-Backend Integration Plan

## Project Overview

**HireZ** is an AI-powered Applicant Tracking System (ATS) that helps streamline the hiring process with:
- AI-powered candidate matching
- Smart resume parsing and analysis
- Interview management
- Analytics and feedback systems
- User authentication and profile management

## Architecture Summary

### Backend (ASP.NET Core 8.0)
- **Framework**: .NET 8.0 Web API
- **Database**: SQL Server with Entity Framework Core
- **Authentication**: JWT Bearer tokens
- **API Base URL**: `http://localhost:5179` (HTTP) or `https://localhost:7059` (HTTPS)
- **API Route Prefix**: `/api`

### Frontend
- **Technology**: HTML5, CSS3, Vanilla JavaScript
- **UI Framework**: Bootstrap 5.3.0
- **Location**: `HireZFrontend/` folder
- **Current State**: Static frontend with mock data, no API integration

## API Endpoints Mapping

### Authentication Endpoints
| Frontend Page | Backend Endpoint | Method | Auth Required |
|--------------|------------------|--------|---------------|
| `login.html` | `/api/auth/login` | POST | No |
| `register.html` | `/api/auth/register` | POST | No |

### Profile Endpoints
| Frontend Page | Backend Endpoint | Method | Auth Required |
|--------------|------------------|--------|---------------|
| `profile.html` | `/api/profile` | GET | Yes |

### Resume Endpoints
| Frontend Page | Backend Endpoint | Method | Auth Required |
|--------------|------------------|--------|---------------|
| `cv-upload.html` | `/api/resume/upload` | POST | Yes |
| Resume display | `/api/resume/{id}` | GET | Yes |
| Resume reprocess | `/api/resume/{id}/reprocess` | POST | Yes |

### Jobs/ATS Endpoints
| Frontend Page | Backend Endpoint | Method | Auth Required |
|--------------|------------------|--------|---------------|
| Jobs management | `/api/jobs` | POST | Yes |
| Job details | `/api/jobs/{id}` | GET | Yes |
| `ats-matcher.html` | `/api/jobs/{jobId}/match/{resumeId}` | POST | Yes |

### Interview Endpoints
| Frontend Page | Backend Endpoint | Method | Auth Required |
|--------------|------------------|--------|---------------|
| `interview-prep.html` | `/api/interview/sessions` | POST | Yes |
| Interview details | `/api/interview/sessions/{id}` | GET | Yes |

### Analytics Endpoints
| Frontend Page | Backend Endpoint | Method | Auth Required |
|--------------|------------------|--------|---------------|
| `dashboard.html` | `/api/analytics/overview` | GET | Yes |
| `dashboard.html` | `/api/analytics/trends?days=30` | GET | Yes |

### Feedback Endpoints
| Frontend Page | Backend Endpoint | Method | Auth Required |
|--------------|------------------|--------|---------------|
| `feedback.html` | `/api/resume/{resumeId}/feedback` | GET | Yes |
| Feedback details | `/api/resume/{resumeId}/feedback/{id}` | GET | Yes |

## Data Models & DTOs

### Authentication
- **LoginRequest**: `{ email: string, password: string }`
- **RegisterRequest**: `{ email: string, password: string }`
- **AuthResponse**: `{ token: string, expiresAt: DateTime, email: string }`

### Profile
- **ProfileResponse**: `{ id: int, email: string, role: string, createdAt: DateTime }`

### Resume
- **UploadResponse**: `{ resumeId: int, fileName: string, message?: string }`
- **ResumeDto**: Contains resume details, extracted text, feedback

### Jobs
- **CreateJobRequest**: Job creation data
- **JobDto**: Job details
- **AtsResultDto**: Matching results

### Interview
- **CreateInterviewSessionRequest**: `{ resumeId: int, jobId?: int, count: int, preferredSource?: string }`
- **InterviewSessionDto**: `{ id, resumeId, jobId, status, createdAt, questions: [] }`

### Analytics
- **AnalyticsOverviewDto**: Dashboard metrics
- **TrendPointDto[]**: Trend data points

## Implementation Strategy

### Phase 1: Infrastructure Setup
1. **Configure CORS** in backend (`Program.cs`)
   - Allow requests from frontend origin
   - Configure allowed methods and headers
   
2. **Create API Client Module** (`api-client.js`)
   - Centralized API communication
   - JWT token management (storage, retrieval, refresh)
   - Request/response interceptors
   - Error handling

3. **Authentication State Management**
   - Token storage (localStorage/sessionStorage)
   - Token validation
   - Auto-redirect on unauthorized
   - Logout functionality

### Phase 2: Authentication Integration
1. **Login Integration**
   - Connect form to `/api/auth/login`
   - Store JWT token on successful login
   - Redirect to dashboard

2. **Registration Integration**
   - Connect form to `/api/auth/register`
   - Handle validation errors
   - Redirect to login after success

3. **Protected Route Guards**
   - Check authentication on page load
   - Redirect to login if not authenticated

### Phase 3: Core Feature Integration
1. **Resume Upload**
   - File upload to `/api/resume/upload`
   - Handle FormData
   - Display upload progress
   - Show success/error messages

2. **Dashboard Analytics**
   - Fetch overview from `/api/analytics/overview`
   - Fetch trends from `/api/analytics/trends`
   - Update charts with real data

3. **Profile Management**
   - Fetch user profile on page load
   - Display user information
   - Handle profile updates (if endpoint exists)

### Phase 4: Advanced Features
1. **Interview Management**
   - Create interview sessions
   - Fetch interview questions
   - Display interview data

2. **Jobs/ATS Matcher**
   - Create jobs
   - Match resumes to jobs
   - Display ATS results

3. **Feedback System**
   - Fetch feedback for resumes
   - Display feedback details

### Phase 5: Error Handling & UX
1. **Global Error Handling**
   - Network errors
   - API errors (400, 401, 404, 500)
   - User-friendly error messages

2. **Loading States**
   - Show loading indicators during API calls
   - Disable forms during submission

3. **Success Feedback**
   - Toast notifications
   - Success messages
   - Confirmation dialogs

## Technical Considerations

### CORS Configuration
- Backend needs to allow requests from frontend origin
- In development: `http://localhost:8000` or similar
- In production: Configure for production domain

### JWT Token Handling
- Store token in localStorage or sessionStorage
- Include token in Authorization header: `Bearer {token}`
- Handle token expiration (401 responses)
- Refresh token if refresh endpoint exists

### File Upload
- Use FormData for file uploads
- Set appropriate Content-Type (multipart/form-data)
- Handle file size limits
- Validate file types on client side

### Error Responses
- Standardize error response format
- Handle validation errors (400)
- Handle authentication errors (401)
- Handle not found errors (404)
- Handle server errors (500)

## File Structure After Integration

```
HireZ/
├── Program.cs (with CORS configuration)
├── HireZFrontend/
│   ├── js/
│   │   ├── api-client.js (NEW - API client module)
│   │   ├── auth.js (NEW - Authentication utilities)
│   │   └── main.js (MODIFIED - Integrated with API)
│   ├── login.html (MODIFIED - Real API integration)
│   ├── register.html (MODIFIED - Real API integration)
│   ├── dashboard.html (MODIFIED - Real API integration)
│   ├── cv-upload.html (MODIFIED - Real API integration)
│   └── ... (other pages)
```

## Testing Strategy

1. **Authentication Flow**
   - Test login with valid/invalid credentials
   - Test registration
   - Test token persistence
   - Test logout

2. **API Integration**
   - Test each endpoint integration
   - Test error scenarios
   - Test loading states
   - Test data display

3. **End-to-End Flow**
   - User registration → Login → Upload Resume → View Dashboard
   - Create Job → Match Resume → View Results
   - Create Interview Session → View Questions

## Next Steps

1. Configure CORS in backend
2. Create API client module
3. Integrate authentication
4. Integrate core features incrementally
5. Add error handling and UX improvements
6. Test thoroughly
7. Document API usage

