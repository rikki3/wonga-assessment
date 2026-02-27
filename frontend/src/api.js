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
        throw new Error(msg || `HTTP ${res.status}`);
    }

    return data;
}