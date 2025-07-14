import httpClient from './httpClient';

export const authApi = {
  login: async (credentials) => {
    try {
      const response = await httpClient.post('/auth/login', credentials);
      return {
        success: true,
        data: response.data,
        message: 'Login successful'
      };
    } catch (error) {
      return {
        success: false,
        data: null,
        message: error.response?.data?.message || 'Login failed'
      };
    }
  },

  register: async (registrationData) => {
    try {
      const response = await httpClient.post('/auth/register', registrationData);
      return {
        success: true,
        data: response.data,
        message: 'Registration successful'
      };
    } catch (error) {
      return {
        success: false,
        data: null,
        message: error.response?.data?.message || 'Registration failed'
      };
    }
  },

  logout: async () => {
    try {
      await httpClient.post('/auth/logout');
      return {
        success: true,
        message: 'Logout successful'
      };
    } catch (error) {
      // Even if logout fails on server, we should clear local state
      return {
        success: true,
        message: 'Logout completed'
      };
    }
  },

  refreshToken: async () => {
    try {
      const response = await httpClient.post('/auth/refresh');
      return {
        success: true,
        data: response.data
      };
    } catch (error) {
      return {
        success: false,
        message: 'Token refresh failed'
      };
    }
  },

  validateToken: async () => {
    try {
      const response = await httpClient.get('/auth/validate');
      return {
        success: true,
        data: response.data
      };
    } catch (error) {
      return {
        success: false,
        message: 'Token validation failed'
      };
    }
  }
};
