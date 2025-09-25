const API = process.env.REACT_APP_API || "http://localhost:5253";

export async function fetchCallButtons() {
  const res = await fetch(`${API}/api/callbuttons`);
  return res.json();
}

export async function createCallButton(cb) {
  const res = await fetch(`${API}/api/callbuttons`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(cb),
  });
  return res.json();
}

export async function updateCallButton(id, cb) {
  const res = await fetch(`${API}/api/callbuttons/${id}`, {
    method: "PUT",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(cb),
  });
  return res.json();
}

export async function assignCallButtonPlace(id, placeId) {
  const res = await fetch(`${API}/api/callbuttons/${id}/assign-place/${placeId}`, {
    method: "POST",
  });
  return res.json();
}

export async function deleteCallButton(id) {
  await fetch(`${API}/api/callbuttons/${id}`, { method: "DELETE" });
}


