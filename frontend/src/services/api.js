import axios from 'axios';

const BACKEND_URL = process.env.REACT_APP_BACKEND_URL;
const API = `${BACKEND_URL}/api`;

// Create axios instance with default config
const apiClient = axios.create({
  baseURL: API,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Players API
export const playersAPI = {
  // Get all players
  getAll: async () => {
    const response = await apiClient.get('/players/');
    return response.data;
  },

  // Get single player
  getById: async (id) => {
    const response = await apiClient.get(`/players/${id}/`);
    return response.data;
  },

  // Create new player
  create: async (playerData) => {
    const response = await apiClient.post('/players/', playerData);
    return response.data;
  },

  // Update player
  update: async (id, playerData) => {
    const response = await apiClient.put(`/players/${id}/`, playerData);
    return response.data;
  },

  // Delete player
  delete: async (id) => {
    const response = await apiClient.delete(`/players/${id}/`);
    return response.data;
  },

  // Import multiple players
  import: async (playersData) => {
    const response = await apiClient.post('/players/import/', playersData);
    return response.data;
  }
};

// Shuffle API
export const shuffleAPI = {
  // Shuffle all players
  shuffleAll: async () => {
    const response = await apiClient.post('/shuffle');
    return response.data;
  },

  // Shuffle specific players
  shuffleCustom: async (playerIds) => {
    const response = await apiClient.post('/shuffle/custom', playerIds);
    return response.data;
  }
};

// Health check
export const healthCheck = async () => {
  const response = await apiClient.get('/health/');
  return response.data;
};

export default apiClient;