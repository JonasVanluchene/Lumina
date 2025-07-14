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
      const token = sessionStorage.getItem('token');
      const refreshToken = sessionStorage.getItem('refreshToken');
      
      if (token) {
        try {
          // Validate token with backend
          const result = await authApi.validateToken();
          if (result.success) {
            setUser(result.data);
          } else {
            // Token is invalid, clear storage
            clearAuthState();
          }
        } catch (error) {
          console.error('Token validation failed:', error);
          clearAuthState();
        }
      }
      
      setLoading(false);
    };

    initializeAuth();
  }, []);

  const clearAuthState = useCallback(() => {
    sessionStorage.removeItem('token');
    sessionStorage.removeItem('refreshToken');
    setUser(null);
    setError(null);
  }, []);

  const login = async (credentials) => {
    try {
      setLoading(true);
      setError(null);
      
      const result = await authApi.login(credentials);
      
      if (result.success) {
        const { token, refreshToken, user: userData } = result.data;
        
        // Store tokens
        sessionStorage.setItem('token', token);
        if (refreshToken) {
          sessionStorage.setItem('refreshToken', refreshToken);
        }
        
        // Set user state
        setUser(userData);
        
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
        // Auto-login after successful registration
        if (result.data.token) {
          const { token, refreshToken, user: userData } = result.data;
          
          sessionStorage.setItem('token', token);
          if (refreshToken) {
            sessionStorage.setItem('refreshToken', refreshToken);
          }
          
          setUser(userData);
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
      
      // Call logout API (even if it fails, we still clear local state)
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