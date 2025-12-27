# Login Debugging Guide

## Test Credentials

If the database has been seeded, you can use:
- **Email**: `admin@example.com`
- **Password**: `Admin@12345`

Or register a new account via the register page.

## Debugging Steps

1. **Open Browser Console** (F12) when testing login
2. **Check Network Tab** for API requests to `/api/auth/login`
3. **Look for console logs** that show:
   - "Attempting login for: [email]"
   - "API.auth.login called with email: [email]"
   - "API.auth.login response: [response]"
   - Any error messages

## Common Issues

1. **"Network error"**: Backend server not running
2. **"Invalid credentials"**: Wrong email/password
3. **"No token received"**: Backend didn't return token (check backend logs)
4. **CORS errors**: Frontend origin not allowed (shouldn't happen since using same origin)

## Backend Logs

Check the backend console/terminal for:
- "Login attempt for email: [email]"
- "Login successful" or "Login failed" messages
- Any exception stack traces

