import React, { useState } from "react";
import { useNavigate, Link } from "react-router-dom";
import { apiPost } from "../api.js";
import { setAuth } from "../auth.js";

export default function Login() {
    const nav = useNavigate();
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [err, setErr] = useState("");

    async function submit(e){
        e.preventDefault();
        setErr("");
        try { // submit calls login endpoint
            const data = await apiPost("/auth/login", { email, password });
            setAuth(data.token, data.email);
            nav("/");
        } catch (e) {
            setErr(e.message);
        }
    }

    return (
        <div>
            <h2>Login</h2>
            <form onSubmit={submit} style={{ display: "grid", gap: 10 }}>
                <input placeholder="Email" value={email} onChange={e => setEmail(e.target.value)} />
                <input placeholder="Password" type="password" value={password} onChange={e => setPassword(e.target.value)} />
                <button type="submit">Login</button>
            </form>
            {err && <p style={{ color: "indianred" }}>{err}</p>}
            <p>No account? <Link to="/register">Register</Link></p>
        </div>
    )
}