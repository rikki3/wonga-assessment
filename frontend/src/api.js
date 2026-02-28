import { getToken } from "./auth.js";
const API_URL = import.meta.env.VITE_API_URL || "http://localhost:8080";

export async function apiPost(path, body) {
    const res = await fetch(`${API_URL}${path}`, {
        method: "POST",
        headers: {"Content-Type": "application/json" },
        body: JSON.stringify(body)
    });

    const text = await res.text();
    let data = null;
    try { data = text ? JSON.parse(text) : null; } catch { data = text; }

    if (!res.ok) {
        const msg = typeof data === "string" ? data : (data?.message || JSON.stringify(data));
        const err = new Error(msg || `HTTP ${res.status}`);
        err.status = res.status;
        throw err;
    }

    return data;
}

export async function apiGet(path) {
    const token = getToken();
    const headers = { "Content-Type": "application/json" };
    if (token) {
        headers.Authorization = `Bearer ${token}`;
    }

    const res = await fetch(`${API_URL}${path}`, {
        method: "GET",
        headers
    });

    const text = await res.text();
    let data = null;
    try { data = text ? JSON.parse(text) : null; } catch { data = text; }
    
    if (!res.ok) {
        const msg = typeof data === "string" ? data : (data?.message || JSON.stringify(data));
        const err = new Error(msg || `HTTP ${res.status}`);
        err.status = res.status;
        throw err;
    }

    return data;
}
