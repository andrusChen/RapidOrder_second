const API = process.env.REACT_APP_API || "http://localhost:5253";

export async function fetchActionMaps() {
  const res = await fetch(`${API}/api/actionmaps`);
  return res.json();
}

export async function upsertActionMap(payload) {
  const res = await fetch(`${API}/api/actionmaps/upsert`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(payload),
  });
  return res.json();
}

export async function deleteActionMap(id) {
  await fetch(`${API}/api/actionmaps/${id}`, { method: "DELETE" });
}


