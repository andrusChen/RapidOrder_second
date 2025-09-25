import React, { useEffect, useState } from "react";
import { fetchUsers, createUser, updateUser, deleteUser } from "../api/usersApi";

const roles = [
  { value: 0, label: "WAITER" },
  { value: 1, label: "MANAGER" },
  { value: 2, label: "ADMIN" },
];

export default function StaffManagement() {
  const [users, setUsers] = useState([]);
  const [form, setForm] = useState({ id: null, displayName: "", role: 0 });

  useEffect(() => {
    refresh();
  }, []);

  const refresh = async () => {
    const list = await fetchUsers();
    setUsers(list);
  };

  const onSubmit = async (e) => {
    e.preventDefault();
    const payload = { displayName: form.displayName, role: parseInt(form.role, 10) };
    if (form.id) {
      await updateUser(form.id, { id: form.id, ...payload });
    } else {
      await createUser(payload);
    }
    setForm({ id: null, displayName: "", role: 0 });
    await refresh();
  };

  const onEdit = (u) => setForm({ id: u.id, displayName: u.displayName, role: u.role });

  const onDelete = async (u) => {
    if (!window.confirm(`Delete ${u.displayName}?`)) return;
    await deleteUser(u.id);
    await refresh();
  };

  return (
    <div style={{ padding: 20 }}>
      <h2>Staff Management</h2>

      <section style={{ maxWidth: 420 }}>
        <h3>{form.id ? "Edit Staff" : "Add Staff"}</h3>
        <form onSubmit={onSubmit}>
          <div style={{ marginBottom: 8 }}>
            <label>Name</label>
            <br />
            <input value={form.displayName} onChange={(e) => setForm({ ...form, displayName: e.target.value })} required />
          </div>
          <div style={{ marginBottom: 8 }}>
            <label>Role</label>
            <br />
            <select value={form.role} onChange={(e) => setForm({ ...form, role: e.target.value })}>
              {roles.map(r => <option key={r.value} value={r.value}>{r.label}</option>)}
            </select>
          </div>
          <div>
            <button type="submit">{form.id ? "Save" : "Create"}</button>
            {form.id && (
              <button type="button" onClick={() => setForm({ id: null, displayName: "", role: 0 })} style={{ marginLeft: 8 }}>Cancel</button>
            )}
          </div>
        </form>
      </section>

      <hr />

      <section>
        <h3>Staff List</h3>
        <table border="1" cellPadding="8" style={{ borderCollapse: "collapse", width: "100%" }}>
          <thead>
            <tr>
              <th>Name</th>
              <th>Role</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            {users.map(u => (
              <tr key={u.id}>
                <td>{u.displayName}</td>
                <td>{roles.find(r => r.value === u.role)?.label || u.role}</td>
                <td>
                  <button onClick={() => onEdit(u)}>Edit</button>
                  <button onClick={() => onDelete(u)} style={{ marginLeft: 6 }}>Delete</button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </section>
    </div>
  );
}


