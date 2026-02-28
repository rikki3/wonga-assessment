import React from "react";
import { getEmail } from "../auth.js";

export default function Home() {
    return (
        <div>
            <h2>Home</h2>
            <p>Welcome{getEmail() ? `, ${getEmail()}` : ""}!</p>
        </div>
    )
}