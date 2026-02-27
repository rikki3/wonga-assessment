import React from "react";
import { getEmail } from "../auth.js";

export default function Home() {
    return (
        <div>
            <h2>Home (Protected)</h2>
            <p>You are logged in as: <b>{getEmail()}</b></p>
        </div>
    )
}