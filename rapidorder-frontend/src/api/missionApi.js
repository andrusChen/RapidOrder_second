import axios from "axios";

const API_BASE = "http://localhost:5253"; // backend

export async function fetchMissions() {
  const res = await axios.get(`${API_BASE}/api/missions`);
  return res.data;
}

export async function acknowledgeMission(id) {
  const res = await axios.post(`${API_BASE}/api/missions/${id}/acknowledge`);
  return res.data;
}

export async function assignMission(id, userId) {
  const res = await axios.post(`${API_BASE}/api/missions/${id}/assign`, { userId });
  return res.data;
}

export async function finishMission(id) {
  const res = await axios.post(`${API_BASE}/api/missions/${id}/finish`);
  return res.data;
}

export async function cancelMission(id) {
  const res = await axios.post(`${API_BASE}/api/missions/${id}/cancel`);
  return res.data;
}