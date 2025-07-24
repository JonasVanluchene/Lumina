import httpClient from './httpClient';

export const authApi = {
  login: async (credentials) => {
    try {
      const response = await httpClient.post('/auth/login', credentials);
      return {
        success: true,
        data: response.data, // User data from response
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
        data: response.data, // User data from response
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
      // The httpOnly cookie will be cleared by the server or expire
      return {
        success: true,
        message: 'Logout completed'
      };
    }
  },

  // Check authentication status by calling a protected endpoint
  validateToken: async () => {
    try {
      // Use the getCurrentUser method to validate - if it succeeds, user is authenticated
      const response = await httpClient.get('/user/me');
      return {
        success: true,
        data: response.data // Should return user data
      };
    } catch (error) {
      return {
        success: false,
        message: 'Authentication failed'
      };
    }
  },

  getCurrentUser: async () => {
    try {
      const response = await httpClient.get('/user/me'); // or whatever endpoint returns current user
      return {
        success: true,
        data: response.data
      };
    } catch (error) {
      return {
        success: false,
        message: 'Failed to get user data'
      };
    }
  }
};
