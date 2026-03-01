import React, { useState } from "react";
import { useNavigate, Link } from "react-router-dom";
import { apiPost } from "../api.js";
import { isAuthed, setAuth } from "../auth.js";
import { CButton, CForm, CFormInput } from "@coreui/react";
import "bootstrap/dist/css/bootstrap.min.css";
import "@coreui/coreui/dist/css/coreui.min.css";

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
        try {
            const data = await apiPost("/auth/register", {email, password, firstName, lastName});
            setAuth(data.token, data.email);
            nav("/userinfo");
        } catch (e) {
            setErr(e.message);
        }
    }

    return (
        <div align="center">
            <h2 style={{ marginBottom: 16 }}>Register</h2>
            <div style={{ border: "1px solid lightgray", padding: 16, display: "inline-block", marginBottom: 16, borderRadius: 8 }}>
                <CForm onSubmit={submit} style={{ display: "grid", gap: 8, width: 600, textAlign: "left" }}>
                    <CFormInput
                        type="email"
                        id="emailInput"
                        label="Email address: "
                        value={email}
                        onChange={e => setEmail(e.target.value)}
                        style={{ marginBottom: 16 }}
                    />
                    <CFormInput
                        type="text"
                        id="firstNameInput"
                        label="First Name: "
                        value={firstName}
                        onChange={e => setFirstName(e.target.value)}
                        style={{ marginBottom: 16 }}
                    />
                    <CFormInput
                        type="text"
                        id="lastNameInput"
                        label="Last Name: "
                        value={lastName}
                        onChange={e => setLastName(e.target.value)}
                        style={{ marginBottom: 16 }}
                    />
                    <CFormInput
                        type="password"
                        id="passwordInput"
                        label="Password: "
                        value={password}
                        onChange={e => setPassword(e.target.value)}
                        style={{ marginBottom: 16 }}
                    />
                    <CButton type="submit" color="primary">Create account</CButton>
                </CForm>
            </div>
            {err && <p style={{ color: "indianred" }}>{err}</p>}
            <p>Already have an account? <Link to="/login">Login</Link></p>
        </div>
    );
}
