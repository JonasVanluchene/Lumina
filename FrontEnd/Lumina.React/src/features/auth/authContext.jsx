import React, { createContext, useContext, useState, useEffect, useCallback } from 'react';
import { useNavigate } from 'react-router-dom';
import { authApi } from '../../api/authApi';

const AuthContext = createContext();

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  
  const navigate = useNavigate();

  // Initialize auth state on app load
  useEffect(() => {
    const initializeAuth = async () => {
      try {
        // Try to validate the httpOnly cookie and get user data
        // This will call a protected endpoint that requires the JWT cookie
        const result = await authApi.validateToken();
        if (result.success) {
          setUser(result.data);
        } else {
          // No valid session or cookie expired
          setUser(null);
        }
      } catch (error) {
        console.error('Auth initialization failed:', error);
        // If there's any error (network, 401, etc.), treat as not authenticated
        setUser(null);
      }
      
      setLoading(false);
    };

    initializeAuth();
  }, []);

  // Listen for logout events from httpClient
  useEffect(() => {
    const handleLogout = () => {
      clearAuthState();
      navigate('/login');
    };

    window.addEventListener('auth:logout', handleLogout);
    return () => window.removeEventListener('auth:logout', handleLogout);
  }, [navigate]);

  const clearAuthState = useCallback(() => {
    // No need to clear tokens since they're httpOnly cookies
    // The server will handle cookie cleanup on logout
    setUser(null);
    setError(null);
  }, []);

  const login = async (credentials) => {
    try {
      setLoading(true);
      setError(null);
      
      const result = await authApi.login(credentials);
      
      if (result.success) {
        // HttpOnly cookie is set by the server automatically
        // Set user data from response
        setUser(result.data);
        
        // Navigate to dashboard
        navigate('/dashboard');
        
        return { success: true };
      } else {
        setError(result.message);
        return { success: false, message: result.message };
      }
    } catch (error) {
      const errorMessage = error.response?.data?.message || 'Login failed';
      setError(errorMessage);
      return { success: false, message: errorMessage };
    } finally {
      setLoading(false);
    }
  };

  const register = async (registrationData) => {
    try {
      setLoading(true);
      setError(null);
      
      const result = await authApi.register(registrationData);
      
      if (result.success) {
        // HttpOnly cookie is set by the server automatically if auto-login is enabled
        // Check if user data is returned (auto-login)
        if (result.data && result.data.id) {
          setUser(result.data);
          navigate('/dashboard');
        } else {
          // If no auto-login, redirect to login page
          navigate('/login', { 
            state: { message: 'Registration successful! Please log in.' }
          });
        }
        
        return { success: true };
      } else {
        setError(result.message);
        return { success: false, message: result.message };
      }
    } catch (error) {
      const errorMessage = error.response?.data?.message || 'Registration failed';
      setError(errorMessage);
      return { success: false, message: errorMessage };
    } finally {
      setLoading(false);
    }
  };

  const logout = async () => {
    try {
      setLoading(true);
      
      // Call logout API to clear httpOnly cookie on server
      await authApi.logout();
      
      // Clear local state
      clearAuthState();
      
      // Navigate to login
      navigate('/login');
      
    } catch (error) {
      console.error('Logout error:', error);
      // Still clear local state even if API call fails
      clearAuthState();
      navigate('/login');
    } finally {
      setLoading(false);
    }
  };

  const clearError = useCallback(() => {
    setError(null);
  }, []);

  const value = {
    user,
    loading,
    error,
    login,
    register,
    logout,
    clearError,
    isAuthenticated: !!user,
  };

  return (
    <AuthContext.Provider value={value}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  
  return context;
};