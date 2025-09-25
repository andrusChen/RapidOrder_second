import React, { useEffect, useState } from "react";
import { fetchMissions, acknowledgeMission, assignMission, finishMission, cancelMission } from "../api/missionApi";
import { getLearningMode, setLearningMode } from "../api/learningModeApi";
import * as signalR from "@microsoft/signalr";
import { fetchUsers } from "../api/usersApi";

export default function MissionList() {
  const [missions, setMissions] = useState([]);
  const [learningModeEnabled, setLearningModeEnabled] = useState(false);
  const [users, setUsers] = useState([]);

  useEffect(() => {
    // Load initial missions and users
    fetchMissions().then(setMissions);
    fetchUsers().then(setUsers);

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
    connection.on("MissionUpdated", (mission) => {
      setMissions((prev) => prev.map(m => m.id === mission.id ? mission : m));
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

  const doAck = async (id) => { await acknowledgeMission(id); };
  const doAssign = async (id, userId) => { await assignMission(id, userId); };
  const doFinish = async (id) => { await finishMission(id); };
  const doCancel = async (id) => { await cancelMission(id); };

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
            <strong>{m.placeLabel}</strong>: {m.sourceDecoded} (Button {m.sourceButton}) — {new Date(m.startedAt).toLocaleString()} —
            <span style={{ marginLeft: 8 }}><em>{m.status}</em></span>
            <span style={{ marginLeft: 8 }}>
              <button onClick={() => doAck(m.id)}>Ack</button>
              <select onChange={(e) => doAssign(m.id, parseInt(e.target.value, 10))} defaultValue="">
                <option value="" disabled>{m.assignedUserName ? `Assigned: ${m.assignedUserName}` : "Assign…"}</option>
                {users.map(u => <option key={u.id} value={u.id}>{u.displayName}</option>)}
              </select>
              <button onClick={() => doFinish(m.id)} style={{ marginLeft: 4 }}>Finish</button>
              <button onClick={() => doCancel(m.id)} style={{ marginLeft: 4 }}>Cancel</button>
            </span>
          </li>
        ))}
      </ul>
    </div>
  );
}
