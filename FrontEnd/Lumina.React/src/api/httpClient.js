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

// Request interceptor - no need to add Authorization header since we're using httpOnly cookies
httpClient.interceptors.request.use(
  (config) => {
    // Cookies are automatically sent with requests due to withCredentials: true
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Response interceptor for error handling
httpClient.interceptors.response.use(
  (response) => {
    return response;
  },
  async (error) => {
    // Handle different types of errors
    if (!error.response) {
      // Network error
      error.message = ERROR_MESSAGES.NETWORK_ERROR;
      return Promise.reject(error);
    }

    const { status } = error.response;

    // Handle 401 errors (Unauthorized) - cookie expired or invalid
    if (status === 401) {
      // Clear any client-side auth state and redirect to login
      window.dispatchEvent(new CustomEvent('auth:logout'));
      return Promise.reject(error);
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
