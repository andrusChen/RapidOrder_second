const API = process.env.REACT_APP_API || "http://localhost:5253";

export async function fetchUsers() {
  const res = await fetch(`${API}/api/users`);
  return res.json();
}

export async function createUser(user) {
  const res = await fetch(`${API}/api/users`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(user),
  });
  return res.json();
}

export async function updateUser(id, user) {
  const res = await fetch(`${API}/api/users/${id}`, {
    method: "PUT",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(user),
  });
  return res.json();
}

export async function deleteUser(id) {
  await fetch(`${API}/api/users/${id}`, { method: "DELETE" });
}


