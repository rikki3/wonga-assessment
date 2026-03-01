import React, { useState } from "react";
import { useNavigate, Link } from "react-router-dom";
import { apiPost } from "../api.js";
import { isAuthed, setAuth } from "../auth.js";
import { CButton, CForm, CFormInput } from '@coreui/react'
import 'bootstrap/dist/css/bootstrap.min.css'
import '@coreui/coreui/dist/css/coreui.min.css'

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
        <div align="center">
            <h2 style={{ marginBottom: 16 }}>Login</h2>
            <div style={{ border: "1px solid lightgray", padding: 16, display: "inline-block", marginBottom: 16, borderRadius: 8 }}>
                <CForm onSubmit={submit} style={{ display: "grid", gap: 8, width: 600, textAlign: "left" }}>
                    <CFormInput
                        type="email"
                        id="emailInput"
                        label="Email address: "
                        onChange={e => setEmail(e.target.value)}
                        style={{ marginBottom: 16 }}
                    />
                    <CFormInput
                        type="password"
                        id="passwordInput"
                        label="Password: "
                        onChange={e => setPassword(e.target.value)}
                        style={{ marginBottom: 16 }}
                    />
                    <CButton type="submit" color="primary">Login</CButton>
                </CForm>
            </div>
            {err && <p style={{ color: "indianred" }}>{err}</p>}
            <p>No account? <Link to="/register">Register</Link></p>
        </div>
    )
}
