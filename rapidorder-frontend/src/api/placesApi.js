const API = process.env.REACT_APP_API || "http://localhost:5253";

export async function fetchPlaces() {
  const res = await fetch(`${API}/api/places`);
  return res.json();
}

export async function createPlace(place) {
  const res = await fetch(`${API}/api/places`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(place),
  });
  return res.json();
}

export async function updatePlace(id, place) {
  const res = await fetch(`${API}/api/places/${id}`, {
    method: "PUT",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(place),
  });
  return res.json();
}

export async function deletePlace(id) {
  await fetch(`${API}/api/places/${id}`, { method: "DELETE" });
}


