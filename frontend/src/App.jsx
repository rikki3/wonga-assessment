import React from "react";
import { Routes, Route, Navigate, Link, useNavigate } from "react-router-dom";
import Login from "./pages/Login.jsx";
import Register from "./pages/Register.jsx";
import Home from "./pages/Home.jsx";
import UserInfo from "./pages/UserInfo.jsx";
import { isAuthed, clearAuth } from "./auth.js";

function Protected({ children }) {
    return isAuthed() ? children : <Navigate to="/login" replace />;
}

function UpdateNavbarIfAuthed() {
    const nav = useNavigate();
    return isAuthed() ? (<>
        <Link key="home" to="/home">Home</Link>
        <Link key="user" to="/userinfo">User Info</Link>
        <Link key="logout" to="/login" onClick={() => {clearAuth(); nav("/"); }} >Logout</Link>
        </>
    ) : ([
        <Link key="home" to="/home">Home</Link>,
        <Link key="login" to="/login">Login</Link>
        ]);
}

export default function App() {
    
    return (
        <div style={{ fontFamily: "system-ui", maxWidth: 720, margin: "40px auto", padding: 16}}>
            <header style={{ display: "flex", justifyContent: "space-between", alignItems: "center" }}>
                <h1 style={{ margin: 0 }}>
                    Wonga Assessment
                </h1>
                <nav style={{ display: "flex", gap: 12, alignItems: "center" }} >
                    <UpdateNavbarIfAuthed />
                </nav>
            </header>

            <hr style={{ margin: "16px 0" }} />

            <Routes>
                <Route path="/" element={<Home />} />
                <Route path="/login" element={<Login />} />
                <Route path="/register" element={<Register />} />
                <Route path="/userinfo" element={<Protected><UserInfo/></Protected>} />
                <Route path="*" element={<Navigate to="/" replace />} />
            </Routes>
        </div>
    );
}