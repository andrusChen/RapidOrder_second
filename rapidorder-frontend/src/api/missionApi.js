import axios from "axios";

const API_BASE = "http://localhost:5253"; // backend

export async function fetchMissions() {
  const res = await axios.get(`${API_BASE}/api/missions`);
  return res.data;
}
