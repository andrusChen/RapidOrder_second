import React, { useEffect, useState } from "react";
import { fetchMissions } from "../api/missionApi";
import { getLearningMode, setLearningMode } from "../api/learningModeApi";
import * as signalR from "@microsoft/signalr";

export default function MissionList() {
  const [missions, setMissions] = useState([]);
  const [learningModeEnabled, setLearningModeEnabled] = useState(false);

  useEffect(() => {
    // Load initial missions
    fetchMissions().then(setMissions);

    // Load initial learning mode status
    getLearningMode().then(data => setLearningModeEnabled(data.enabled));

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

  const toggleLearningMode = async () => {
    const newStatus = !learningModeEnabled;
    await setLearningMode(newStatus);
    setLearningModeEnabled(newStatus);
  };

  return (
    <div style={{ padding: "20px" }}>
      <div>
        <button onClick={toggleLearningMode}>
          {learningModeEnabled ? "Disable" : "Enable"} Learning Mode
        </button>
        <p>Learning Mode is <strong>{learningModeEnabled ? "ON" : "OFF"}</strong></p>
      </div>
      <hr />
      <h2>📋 Active Missions</h2>
      {missions.length === 0 && <p>No missions yet.</p>}
      <ul>
        {missions.map((m) => (
          <li key={m.id}>
            <strong>{m.placeLabel}</strong>: {m.sourceDecoded} (Button {m.sourceButton}) —{" "}
            {new Date(m.startedAt).toLocaleString()}
          </li>
        ))}
      </ul>
    </div>
  );
}
