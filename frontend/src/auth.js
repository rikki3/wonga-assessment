const TOKEN_KEY = "wonga_token"
const EMAIL_KEY = "wonga_email"

export function setAuth(token, email) {
    localStorage.setItem(TOKEN_KEY, token);
    localStorage.setItem(EMAIL_KEY, email);
}

export function clearAuth() {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(EMAIL_KEY);
}

export function getToken() {
    return localStorage.getItem(TOKEN_KEY);
}

export function getEmail() {
    return localStorage.getItem(EMAIL_KEY);
}

export function isAuthed() {
    return !!getToken();
}