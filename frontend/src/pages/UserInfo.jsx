import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { apiGet } from "../api.js";
import { clearAuth } from "../auth.js";

export default function UserInfo() {
    const nav = useNavigate();
    const [user, setUser] = useState(null);
    const [loading, setLoading] = useState(true);
    const [err, setErr] = useState("");

    useEffect(() => {
        let active = true;

        async function loadUserInfo() {
            try {
                const data = await apiGet("/user/info");
                if (!active) return;
                setUser(data);
            } catch (e) {
                if (!active) return;

                if (e.status === 401 || e.status === 403) {
                    clearAuth();
                    nav("/login", { replace: true });
                    return;
                }

                setErr(e.message || "Failed to load user details.");
            } finally {
                if (active) {
                    setLoading(false);
                }
            }
        }

        loadUserInfo();

        return () => {
            active = false;
        };
    }, [nav]);

    if (loading) {
        return (
            <div>
                <h2>User Info (Protected)</h2>
                <p>Loading user info...</p>
            </div>
        );
    }

    if (err) {
        return (
            <div>
                <h2>User Info (Protected)</h2>
                <p style={{ color: "indianred" }}>{err}</p>
            </div>
        );
    }

    return (
        <div>
            <h2>User Info (Protected)</h2>
            <p>First name: <b>{user?.firstName}</b></p>
            <p>Last name: <b>{user?.lastName}</b></p>
            <p>Email: <b>{user?.email}</b></p>
        </div>
    )
}
