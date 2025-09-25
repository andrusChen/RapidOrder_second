const API = process.env.REACT_APP_API || "http://localhost:5253";

export async function fetchPlaceGroups() {
  const res = await fetch(`${API}/api/placegroups`);
  return res.json();
}

export async function createPlaceGroup(group) {
  const res = await fetch(`${API}/api/placegroups`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(group),
  });
  return res.json();
}

export async function updatePlaceGroup(id, group) {
  const res = await fetch(`${API}/api/placegroups/${id}`, {
    method: "PUT",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(group),
  });
  return res.json();
}

export async function deletePlaceGroup(id) {
  await fetch(`${API}/api/placegroups/${id}`, { method: "DELETE" });
}


