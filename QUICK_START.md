# HireZ Integration - Quick Start Guide

## 🚀 Getting Started (5 Minutes)

### Step 1: Start Backend
```bash
cd HireZ
dotnet run
```
✅ Backend runs on `http://localhost:5179`

### Step 2: Start Frontend (New Terminal)
```bash
cd HireZFrontend
python -m http.server 8000
```
✅ Frontend runs on `http://localhost:8000`

### Step 3: Open Browser
Navigate to: `http://localhost:8000`

### Step 4: Test Integration
1. **Register**: Go to Register page and create an account
2. **Login**: Login with your credentials
3. **Upload Resume**: Go to CV Upload and upload a PDF
4. **View Dashboard**: Check the dashboard for analytics

## ✅ What's Integrated

### Authentication ✅
- Login/Register with real API
- JWT token storage and management
- Protected routes
- Logout functionality

### Resume Management ✅
- Resume upload to backend
- File validation
- Progress tracking

### Dashboard ✅
- Real-time analytics data
- Charts with API data
- Metrics display

### Profile ✅
- Load user profile from API
- Display user information

## 📋 API Endpoints Ready

All endpoints are available and integrated:
- ✅ `/api/auth/login` - Login
- ✅ `/api/auth/register` - Register
- ✅ `/api/profile` - Get profile
- ✅ `/api/resume/upload` - Upload resume
- ✅ `/api/analytics/overview` - Dashboard overview
- ✅ `/api/analytics/trends` - Trends data

## 🔧 Configuration

### Change API URL
Edit `HireZFrontend/js/api-client.js`:
```javascript
const API_CONFIG = {
    baseURL: 'http://localhost:5179/api', // Change this
};
```

### Change CORS Origins
Edit `Program.cs` if using different frontend port:
```csharp
policy.WithOrigins("http://localhost:8000", "http://localhost:3000")
```

## 🐛 Troubleshooting

**CORS Errors?**
- Ensure backend is running
- Check CORS configuration in `Program.cs`

**401 Unauthorized?**
- Check if token exists in localStorage
- Try logging in again

**API Connection Failed?**
- Verify backend is running: `http://localhost:5179/swagger`
- Check API base URL in `api-client.js`

## 📚 Documentation

- **INTEGRATION_PLAN.md** - Detailed integration plan
- **INTEGRATION_GUIDE.md** - Complete setup guide
- **INTEGRATION_SUMMARY.md** - Summary of changes

## 🎯 Next Steps

1. Test all features
2. Complete remaining integrations (interview, jobs, feedback)
3. Add error handling improvements
4. Enhance UX with loading states

---

**Need Help?** Check the detailed guides in the documentation files!

