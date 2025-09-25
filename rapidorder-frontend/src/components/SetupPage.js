import React, { useEffect, useState } from "react";
import { fetchPlaceGroups, createPlaceGroup, updatePlaceGroup, deletePlaceGroup } from "../api/placeGroupsApi";
import { fetchPlaces, createPlace, updatePlace, deletePlace } from "../api/placesApi";
import { fetchCallButtons, updateCallButton, assignCallButtonPlace } from "../api/callButtonsApi";
import { fetchActionMaps, upsertActionMap, deleteActionMap } from "../api/actionMapsApi";

export default function SetupPage() {
  const [groups, setGroups] = useState([]);
  const [places, setPlaces] = useState([]);
  const [buttons, setButtons] = useState([]);
  const [actionMaps, setActionMaps] = useState([]);

  useEffect(() => {
    refreshAll();
  }, []);

  const refreshAll = async () => {
    const [g, p, b, am] = await Promise.all([
      fetchPlaceGroups(),
      fetchPlaces(),
      fetchCallButtons(),
      fetchActionMaps(),
    ]);
    setGroups(g);
    setPlaces(p);
    setButtons(b);
    setActionMaps(am);
  };

  const handleCreateGroup = async () => {
    const name = prompt("Group name?");
    if (!name) return;
    await createPlaceGroup({ name });
    await refreshAll();
  };

  const handleRenameGroup = async (g) => {
    const name = prompt("New name?", g.name);
    if (!name) return;
    await updatePlaceGroup(g.id, { ...g, name });
    await refreshAll();
  };

  const handleDeleteGroup = async (g) => {
    if (!window.confirm(`Delete group '${g.name}'?`)) return;
    await deletePlaceGroup(g.id);
    await refreshAll();
  };

  const handleCreatePlace = async () => {
    const number = parseInt(prompt("Table number?"), 10);
    if (!number) return;
    const description = prompt("Description?") || "";
    const groupId = prompt("PlaceGroupId (optional)?");
    await createPlace({ number, description, placeGroupId: groupId ? parseInt(groupId, 10) : null });
    await refreshAll();
  };

  const handleEditPlace = async (p) => {
    const number = parseInt(prompt("Table number?", p.number), 10);
    const description = prompt("Description?", p.description) || "";
    const placeGroupIdRaw = prompt("PlaceGroupId (optional)?", p.placeGroupId || "");
    const placeGroupId = placeGroupIdRaw ? parseInt(placeGroupIdRaw, 10) : null;
    await updatePlace(p.id, { ...p, number, description, placeGroupId });
    await refreshAll();
  };

  const handleDeletePlace = async (p) => {
    if (!window.confirm(`Delete place #${p.number}?`)) return;
    await deletePlace(p.id);
    await refreshAll();
  };

  const handleUpdateButton = async (b) => {
    const label = prompt("Label?", b.label || "");
    const buttonId = prompt("ButtonId?", b.buttonId || "");
    await updateCallButton(b.id, { ...b, label, buttonId, placeId: b.placeId || null });
    await refreshAll();
  };

  const handleAssignButtonPlace = async (b) => {
    const placeId = parseInt(prompt("Assign to placeId?"), 10);
    if (!placeId) return;
    await assignCallButtonPlace(b.id, placeId);
    await refreshAll();
  };

  const handleUpsertActionMap = async () => {
    const deviceCode = prompt("DeviceCode?");
    if (!deviceCode) return;
    const buttonNumber = parseInt(prompt("Button number?"), 10);
    if (isNaN(buttonNumber)) return;
    const missionType = prompt("MissionType (ORDER, PAYMENT, PAYMENT_EC, SERVICE, ASSISTANCE)?", "ORDER");
    await upsertActionMap({ deviceCode, buttonNumber, missionType });
    await refreshAll();
  };

  const handleDeleteActionMap = async (am) => {
    await deleteActionMap(am.id);
    await refreshAll();
  };

  return (
    <div style={{ padding: 20 }}>
      <h2>Setup</h2>

      <section>
        <h3>Place Groups</h3>
        <button onClick={handleCreateGroup}>+ Add Group</button>
        <ul>
          {groups.map((g) => (
            <li key={g.id}>
              <strong>{g.name}</strong> (Places: {g.places?.length || 0})
              <button onClick={() => handleRenameGroup(g)} style={{ marginLeft: 8 }}>Rename</button>
              <button onClick={() => handleDeleteGroup(g)} style={{ marginLeft: 4 }}>Delete</button>
            </li>
          ))}
        </ul>
      </section>

      <hr />

      <section>
        <h3>Places (Tables)</h3>
        <button onClick={handleCreatePlace}>+ Add Place</button>
        <ul>
          {places.map((p) => (
            <li key={p.id}>
              <strong>#{p.number}</strong> {p.description} {p.placeGroup ? `(Group: ${p.placeGroup.name})` : ""}
              <button onClick={() => handleEditPlace(p)} style={{ marginLeft: 8 }}>Edit</button>
              <button onClick={() => handleDeletePlace(p)} style={{ marginLeft: 4 }}>Delete</button>
            </li>
          ))}
        </ul>
      </section>

      <hr />

      <section>
        <h3>Call Buttons</h3>
        <ul>
          {buttons.map((b) => (
            <li key={b.id}>
              <strong>{b.label || b.deviceCode}</strong>
              {b.place ? ` → Place #${b.place.number}` : " (Unassigned)"}
              <span style={{ marginLeft: 8 }}>DeviceCode: {b.deviceCode}</span>
              <button onClick={() => handleUpdateButton(b)} style={{ marginLeft: 8 }}>Edit</button>
              <button onClick={() => handleAssignButtonPlace(b)} style={{ marginLeft: 4 }}>Assign Place</button>
            </li>
          ))}
        </ul>
      </section>

      <hr />

      <section>
        <h3>Action Maps (Button → MissionType)</h3>
        <button onClick={handleUpsertActionMap}>+ Add / Update</button>
        <ul>
          {actionMaps.map((am) => (
            <li key={am.id}>
              <code>{am.deviceCode}</code> button {am.buttonNumber} → {am.missionType}
              <button onClick={() => handleDeleteActionMap(am)} style={{ marginLeft: 8 }}>Delete</button>
            </li>
          ))}
        </ul>
      </section>
    </div>
  );
}


