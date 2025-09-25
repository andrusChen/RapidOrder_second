import React, { useEffect, useMemo, useState } from "react";
import { fetchPlaceGroups } from "../api/placeGroupsApi";
import { fetchPlaces, createPlace, updatePlace, deletePlace } from "../api/placesApi";
import { fetchCallButtons, updateCallButton } from "../api/callButtonsApi";
import { fetchUsers } from "../api/usersApi";

export default function TableManagement() {
  const [groups, setGroups] = useState([]);
  const [places, setPlaces] = useState([]);
  const [buttons, setButtons] = useState([]);
  const [users, setUsers] = useState([]);

  const [form, setForm] = useState({ id: null, number: "", description: "", placeGroupId: "" });
  const [assignForm, setAssignForm] = useState({ buttonId: "", placeId: "" });

  const unassignedButtons = useMemo(() => buttons.filter(b => !b.placeId), [buttons]);

  useEffect(() => {
    refreshAll();
  }, []);

  const refreshAll = async () => {
    const [g, p, b, u] = await Promise.all([
      fetchPlaceGroups(),
      fetchPlaces(),
      fetchCallButtons(),
      fetchUsers(),
    ]);
    setGroups(g);
    setPlaces(p);
    setButtons(b);
    setUsers(u);
  };

  const onEdit = (p) => {
    setForm({ id: p.id, number: String(p.number), description: p.description || "", placeGroupId: p.placeGroupId || "", assignedUserId: p.assignedUserId || "" });
  };

  const onDelete = async (p) => {
    if (!window.confirm(`Delete place #${p.number}?`)) return;
    await deletePlace(p.id);
    await refreshAll();
  };

  const resetForm = () => setForm({ id: null, number: "", description: "", placeGroupId: "", assignedUserId: "" });

  const onSubmit = async (e) => {
    e.preventDefault();
    const payload = {
      number: parseInt(form.number, 10),
      description: form.description,
      placeGroupId: form.placeGroupId ? parseInt(form.placeGroupId, 10) : null,
      assignedUserId: form.assignedUserId ? parseInt(form.assignedUserId, 10) : null,
    };
    if (form.id) {
      await updatePlace(form.id, { id: form.id, ...payload });
    } else {
      await createPlace(payload);
    }
    resetForm();
    await refreshAll();
  };

  const onAssign = async (e) => {
    e.preventDefault();
    if (!assignForm.buttonId || !assignForm.placeId) return;
    const btn = buttons.find(b => String(b.id) === String(assignForm.buttonId));
    if (!btn) return;
    await updateCallButton(btn.id, { ...btn, placeId: parseInt(assignForm.placeId, 10) });
    setAssignForm({ buttonId: "", placeId: "" });
    await refreshAll();
  };

  const onUnassign = async (btn) => {
    await updateCallButton(btn.id, { ...btn, placeId: null });
    await refreshAll();
  };

  return (
    <div style={{ padding: 20 }}>
      <h2>Table Management</h2>

      <div style={{ display: "flex", gap: 24, flexWrap: "wrap" }}>
        <section style={{ minWidth: 320 }}>
          <h3>{form.id ? `Edit Place #${form.number}` : "Create Place"}</h3>
          <form onSubmit={onSubmit}>
            <div style={{ marginBottom: 8 }}>
              <label>Number</label>
              <br />
              <input value={form.number} onChange={(e) => setForm({ ...form, number: e.target.value })} required type="number" min="1" />
            </div>
            <div style={{ marginBottom: 8 }}>
              <label>Description</label>
              <br />
              <input value={form.description} onChange={(e) => setForm({ ...form, description: e.target.value })} />
            </div>
            <div style={{ marginBottom: 8 }}>
              <label>Group</label>
              <br />
              <select value={form.placeGroupId} onChange={(e) => setForm({ ...form, placeGroupId: e.target.value })}>
                <option value="">(none)</option>
                {groups.map(g => (
                  <option key={g.id} value={g.id}>{g.name}</option>
                ))}
              </select>
            </div>
          <div style={{ marginBottom: 8 }}>
            <label>Assigned Staff</label>
            <br />
            <select value={form.assignedUserId || ""} onChange={(e) => setForm({ ...form, assignedUserId: e.target.value })}>
              <option value="">(none)</option>
              {users.map(u => (
                <option key={u.id} value={u.id}>{u.displayName}</option>
              ))}
            </select>
          </div>
            <div>
              <button type="submit">{form.id ? "Save" : "Create"}</button>
              {form.id && (
                <button type="button" onClick={resetForm} style={{ marginLeft: 8 }}>Cancel</button>
              )}
            </div>
          </form>
        </section>

        <section style={{ minWidth: 420 }}>
          <h3>Places</h3>
          <table border="1" cellPadding="8" style={{ borderCollapse: "collapse", width: "100%" }}>
            <thead>
              <tr>
                <th>#</th>
                <th>Description</th>
                <th>Group</th>
                <th>Buttons</th>
                <th>Assigned Staff</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {places.map(p => (
                <tr key={p.id}>
                  <td>#{p.number}</td>
                  <td>{p.description}</td>
                  <td>{p.placeGroup ? p.placeGroup.name : ""}</td>
                  <td>
                    {buttons.filter(b => b.placeId === p.id).map(b => (
                      <span key={b.id} style={{ display: "inline-flex", alignItems: "center", marginRight: 6 }}>
                        <code>{b.label || b.deviceCode}</code>
                        <button onClick={() => onUnassign(b)} style={{ marginLeft: 4 }}>×</button>
                      </span>
                    ))}
                  </td>
                  <td>{p.assignedUser ? p.assignedUser.displayName : ""}</td>
                  <td>
                    <button onClick={() => onEdit(p)}>Edit</button>
                    <button onClick={() => onDelete(p)} style={{ marginLeft: 6 }}>Delete</button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </section>
      </div>

      <hr />

      <section>
        <h3>Assign Button to Place</h3>
        <form onSubmit={onAssign}>
          <label>Button</label>
          <select value={assignForm.buttonId} onChange={(e) => setAssignForm({ ...assignForm, buttonId: e.target.value })} style={{ marginRight: 8 }}>
            <option value="">Select button…</option>
            {unassignedButtons.map(b => (
              <option key={b.id} value={b.id}>{b.label || b.deviceCode}</option>
            ))}
          </select>
          <label>Place</label>
          <select value={assignForm.placeId} onChange={(e) => setAssignForm({ ...assignForm, placeId: e.target.value })} style={{ marginRight: 8 }}>
            <option value="">Select place…</option>
            {places.map(p => (
              <option key={p.id} value={p.id}>#{p.number} {p.description}</option>
            ))}
          </select>
          <button type="submit" disabled={!assignForm.buttonId || !assignForm.placeId}>Assign</button>
        </form>
      </section>
    </div>
  );
}


