import React, { useState } from "react";
import { useNavigate, Link } from "react-router-dom";
import { apiPost } from "../api.js";
import { setAuth } from "../auth.js";

export default function Register() {
    const nav = useNavigate();
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [firstName, setFirstName] = useState("");
    const [lastName, setLastName] = useState("");
    const [err, setErr] = useState("");

    async function submit(e) {
        e.preventDefault();
        setErr("");
        try { // submit calls registration endpoint
            const data = await apiPost("/auth/register", {email, password, firstName, lastName});
            setAuth(data.token, data.email);
            nav("/userinfo");
        } catch (e) {
            setErr(e.message);
        }
    }

    return (
        <div>
            <h2>Register</h2>
            <form onSubmit={submit} style={{ display: "grid", gap: 10 }}>
                <input placeholder="Email" value={email} onChange={e => setEmail(e.target.value)} />
                <input placeholder="First Name" value={firstName} onChange={e => setFirstName(e.target.value)} />
                <input placeholder="Last Name" value={lastName} onChange={e => setLastName(e.target.value)} />
                <input placeholder="Password" type="password" value={password} onChange={e => setPassword(e.target.value)} />
                <button type="submit">Create account</button>
            </form>
            {err && <p style={{ color: "indianred" }}>{err}</p>}
            <p>Already have an account? <Link to="/login">Login</Link></p>
        </div>
    );
}
