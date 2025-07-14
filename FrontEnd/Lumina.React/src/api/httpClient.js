import axios from 'axios';
import { API_CONFIG, AUTH_CONFIG, ERROR_MESSAGES } from '../config/constants';

const httpClient = axios.create({
  baseURL: API_CONFIG.BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
  timeout: API_CONFIG.TIMEOUT,
  withCredentials: API_CONFIG.WITH_CREDENTIALS, // Enable credentials for CORS
});

// Helper function to get storage
const getStorage = () => {
  return AUTH_CONFIG.STORAGE_TYPE === 'localStorage' ? localStorage : sessionStorage;
};

// Request interceptor to add Authorization header
httpClient.interceptors.request.use(
  (config) => {
    const storage = getStorage();
    const token = storage.getItem(AUTH_CONFIG.TOKEN_KEY);
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Response interceptor for error handling and token refresh
httpClient.interceptors.response.use(
  (response) => {
    return response;
  },
  async (error) => {
    const originalRequest = error.config;
    const storage = getStorage();

    // Handle different types of errors
    if (!error.response) {
      // Network error
      error.message = ERROR_MESSAGES.NETWORK_ERROR;
      return Promise.reject(error);
    }

    const { status } = error.response;

    // Handle 401 errors (Unauthorized)
    if (status === 401 && !originalRequest._retry) {
      originalRequest._retry = true;

      try {
        const refreshToken = storage.getItem(AUTH_CONFIG.REFRESH_TOKEN_KEY);
        if (refreshToken) {
          const response = await axios.post(`${API_CONFIG.BASE_URL}/auth/refresh`, {
            refreshToken: refreshToken
          });

          const { token, refreshToken: newRefreshToken } = response.data;
          storage.setItem(AUTH_CONFIG.TOKEN_KEY, token);
          
          if (newRefreshToken) {
            storage.setItem(AUTH_CONFIG.REFRESH_TOKEN_KEY, newRefreshToken);
          }

          // Retry the original request with the new token
          originalRequest.headers.Authorization = `Bearer ${token}`;
          return httpClient(originalRequest);
        }
      } catch (refreshError) {
        // Refresh failed, clear tokens and redirect to login
        storage.removeItem(AUTH_CONFIG.TOKEN_KEY);
        storage.removeItem(AUTH_CONFIG.REFRESH_TOKEN_KEY);
        
        // Dispatch custom event for logout
        window.dispatchEvent(new CustomEvent('auth:logout'));
        
        return Promise.reject(refreshError);
      }
    }

    // Handle other HTTP errors
    switch (status) {
      case 403:
        error.message = ERROR_MESSAGES.FORBIDDEN;
        break;
      case 500:
      case 502:
      case 503:
      case 504:
        error.message = ERROR_MESSAGES.SERVER_ERROR;
        break;
      default:
        error.message = error.response?.data?.message || ERROR_MESSAGES.VALIDATION_ERROR;
    }

    return Promise.reject(error);
  }
);

export default httpClient;
