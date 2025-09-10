import React, { useEffect, useState } from "react";
import { fetchMissions } from "../api/missionApi";
import * as signalR from "@microsoft/signalr";

export default function MissionList() {
  const [missions, setMissions] = useState([]);

  useEffect(() => {
    // Load initial missions
    fetchMissions().then(setMissions);

    // Setup SignalR connection
    const connection = new signalR.HubConnectionBuilder()
      .withUrl("http://localhost:5253/hubs/missions")
      .withAutomaticReconnect()
      .build();

    connection.start().catch((err) => console.error("SignalR error:", err));

    // Listen for new missions
    connection.on("MissionCreated", (mission) => {
      setMissions((prev) => [mission, ...prev]);
    });

    return () => {
      connection.stop();
    };
  }, []);

  return (
    <div style={{ padding: "20px" }}>
      <h2>📋 Active Missions</h2>
      {missions.length === 0 && <p>No missions yet.</p>}
      <ul>
        {missions.map((m) => (
          <li key={m.id}>
            <strong>{m.decoded}</strong> (Button {m.button}) —{" "}
            {new Date(m.startedAt).toLocaleString()}
          </li>
        ))}
      </ul>
    </div>
  );
}
