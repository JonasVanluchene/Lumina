import React from 'react';
import { Navigate, useLocation } from 'react-router-dom';
import { Container } from 'react-bootstrap';
import { useAuth } from '../features/auth/authContext';
import LoadingSpinner from './LoadingSpinner';

const ProtectedRoute = ({ children }) => {
  const { isAuthenticated, loading } = useAuth();
  const location = useLocation();

  // Show loading while checking authentication
  if (loading) {
    return (
      <Container fluid className="min-vh-100 d-flex align-items-center justify-content-center">
        <LoadingSpinner size="large" text="Checking authentication..." />
      </Container>
    );
  }

  // Redirect to login if not authenticated
  if (!isAuthenticated) {
    return <Navigate to="/login" state={{ from: location }} replace />;
  }

  return children;
};

export default ProtectedRoute;
