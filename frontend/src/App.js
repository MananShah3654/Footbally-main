import { useEffect } from "react";
import "./App.css";
import { BrowserRouter, Routes, Route } from "react-router-dom";
import axios from "axios";
import Dashboard from "./components/Dashboard";

const BACKEND_URL = process.env.REACT_APP_BACKEND_URL;
const API = `${BACKEND_URL}/api`;

const Home = () => {
  const healthCheck = async () => {
    try {
      const response = await axios.get(`${API}/status/health`);
      console.log('API Health Check:', response.data);
    } catch (e) {
      console.error('API Health Check Failed:', e);
    }
  };

  useEffect(() => {
    healthCheck();
  }, []);

  return <Dashboard />;
};

function App() {
  return (
    <div className="App">
      <BrowserRouter>
        <Routes>
          <Route path="/" element={<Home />}>
            <Route index element={<Home />} />
          </Route>
        </Routes>
      </BrowserRouter>
    </div>
  );
}

export default App;
