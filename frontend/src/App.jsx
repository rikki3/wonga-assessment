import React from "react";
import { Routes, Route, Navigate, Link, useNavigate } from "react-router-dom";
import Login from "./pages/Login.jsx";
import Register from "./pages/Register.jsx";
import Home from "./pages/Home.jsx";
import { isAuthed, clearAuth } from "./auth.js";

function Protected({ children }) {
    return isAuthed() ? children : <Navigate to="/login" replace />;
}

export default function App() {
    const nav = useNavigate();

    return (
        <div style={{ fontFamily: "system-ui", maxWidth: 720, margin: "40px auto", padding: 16}}>
            <header style={{ display: "flex", justifyContent: "space-between", alignItems: "center" }}>
                <h1 style={{ margin: 0 }}>Wonga Assessment</h1>
                <nav style={{ display: "flex", gap: 12, alignItems: "center" }} >
                    <Link to="/">Home</Link>
                    <Link to="/login">Login</Link>
                    <Link to="/register">Register</Link>
                    <button
                        onClick={() => {clearAuth(); nav("/login"); }}
                        style={{ padding: "6px 10px" }}
                    >
                        Logout
                    </button>
                </nav>
            </header>

            <hr style={{ margin: "16px 0" }} />

            <Routes>
                <Route path="/" element={<Protected><Home/></Protected>} />
                <Route path="/login" element={<Login />} />
                <Route path="/register" element={<Register />} />
                <Route path="*" element={<Navigate to="/" replace />} />
            </Routes>
        </div>
    );
}