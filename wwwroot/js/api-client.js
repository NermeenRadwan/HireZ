// wwwroot/js/api-client.js
const API_BASE = "/api";

export class AuthError extends Error {
    constructor(message) { super(message); this.name = "AuthError"; }
}

function getStoredAuth() {
    try {
        const raw = localStorage.getItem("hirez_auth");
        if (!raw) return null;
        const obj = JSON.parse(raw);
        if (!obj?.token) { localStorage.removeItem("hirez_auth"); return null; }
        if (obj.expiresAt) {
            const expires = new Date(obj.expiresAt);
            if (isNaN(expires.getTime()) || expires.getTime() < Date.now()) {
                localStorage.removeItem("hirez_auth"); return null;
            }
        }
        return obj;
    } catch {
        localStorage.removeItem("hirez_auth");
        return null;
    }
}

function setStoredAuth(obj) {
    if (!obj) { localStorage.removeItem("hirez_auth"); return; }
    localStorage.setItem("hirez_auth", JSON.stringify(obj));
}

export function getToken() {
    const auth = getStoredAuth();
    return auth ? auth.token : null;
}

async function request(path, { method = "GET", body = null, headers = {} } = {}) {
    const token = getToken();
    const h = Object.assign({}, headers);
    if (body !== null && !(body instanceof FormData)) {
        h["Content-Type"] = "application/json";
        body = JSON.stringify(body);
    }
    if (token) h["Authorization"] = `Bearer ${token}`;

    const res = await fetch(API_BASE + path, { method, headers: h, body, credentials: "omit" });

    if (res.status === 401) {
        setStoredAuth(null);
        throw new AuthError("Authentication required. Please login again.");
    }

    if (res.status === 204 || res.status === 205) return null;

    const text = await res.text();
    const ct = res.headers.get("content-type") || "";
    const isJson = ct.indexOf("application/json") !== -1;

    if (!res.ok) {
        let msg = text || `Request failed: ${res.status}`;
        if (isJson && text) {
            try { const j = JSON.parse(text); msg = j.error || j.message || JSON.stringify(j); } catch { }
        }
        const err = new Error(msg); err.status = res.status; throw err;
    }

    if (!isJson) return text;
    return text ? JSON.parse(text) : null;
}

export async function login(email, password) {
    const res = await request("/auth/login", { method: "POST", body: { email, password } });
    if (!res || !res.token) throw new Error("Login failed (invalid response).");
    setStoredAuth({ token: res.token, expiresAt: res.expiresAt ?? null, email: res.email ?? null });
    return res;
}

export function logout() {
    setStoredAuth(null);
    window.location.href = "/login.html";
}

export async function getProfile() { return request("/profile"); }
export async function getOverview() { return request("/analytics/overview"); }
export async function getTrends(days = 30) { return request(`/analytics/trends?days=${encodeURIComponent(days)}`); }
