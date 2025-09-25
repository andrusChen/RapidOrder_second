import React from "react";
import MissionList from "./components/MissionList";
import SetupPage from "./components/SetupPage";
import TableManagement from "./components/TableManagement";
import StaffManagement from "./components/StaffManagement";
import { BrowserRouter as Router, Routes, Route, Link } from "react-router-dom";

function App() {
  return (
    <Router>
      <div style={{ padding: 16 }}>
        <h1>🚀 RapidOrder Dashboard</h1>
        <nav style={{ marginBottom: 16 }}>
          <Link to="/">Missions</Link>
          <span style={{ margin: "0 8px" }}>|</span>
          <Link to="/setup">Setup</Link>
          <span style={{ margin: "0 8px" }}>|</span>
          <Link to="/tables">Tables</Link>
          <span style={{ margin: "0 8px" }}>|</span>
          <Link to="/staff">Staff</Link>
        </nav>
        <Routes>
          <Route path="/" element={<MissionList />} />
          <Route path="/setup" element={<SetupPage />} />
          <Route path="/tables" element={<TableManagement />} />
          <Route path="/staff" element={<StaffManagement />} />
        </Routes>
      </div>
    </Router>
  );
}

export default App;
