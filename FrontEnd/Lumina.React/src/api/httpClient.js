import axios from 'axios';

const httpClient = axios.create({
  baseURL: 'https://localhost:5001/api/',
  headers: {
    'Content-Type': 'application/json',
  },
});

// Automatically attach token
httpClient.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

export default httpClient;
