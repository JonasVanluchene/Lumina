import React, { useState, useEffect } from 'react';
import { Link, useLocation } from 'react-router-dom';
import { Container, Row, Col, Card, Form, Button, Alert } from 'react-bootstrap';
import { useAuth } from '../authContext';
import LoadingSpinner from '../../../components/LoadingSpinner';

const LoginForm = () => {
  const [formData, setFormData] = useState({
    email: '',
    password: ''
  });
  const [localError, setLocalError] = useState('');
  
  const { login, loading, error, clearError } = useAuth();
  const location = useLocation();
  
  // Get any message from navigation state (e.g., from registration)
  const message = location.state?.message;

  useEffect(() => {
    // Clear errors when component mounts or when navigating
    clearError();
    setLocalError('');
  }, [clearError]);

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));
    
    // Clear errors when user starts typing
    if (localError) setLocalError('');
    if (error) clearError();
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    
    // Basic validation
    if (!formData.email || !formData.password) {
      setLocalError('Please fill in all fields');
      return;
    }

    if (!formData.email.includes('@')) {
      setLocalError('Please enter a valid email address');
      return;
    }

    const result = await login(formData);
    
    if (!result.success) {
      setLocalError(result.message);
    }
  };

  const displayError = localError || error;

  return (
    <Container fluid className="min-vh-100 d-flex align-items-center justify-content-center bg-light">
      <Row className="w-100 justify-content-center">
        <Col xs={12} sm={8} md={6} lg={4}>
          <Card className="shadow">
            <Card.Body className="p-4">
              <div className="text-center mb-4">
                <h2 className="h3 mb-3">Sign in to your account</h2>
                <p className="text-muted">
                  Or{' '}
                  <Link to="/register" className="text-decoration-none">
                    create a new account
                  </Link>
                </p>
              </div>
              
              {message && (
                <Alert variant="success" className="mb-3">
                  {message}
                </Alert>
              )}
              
              {displayError && (
                <Alert 
                  variant="danger" 
                  dismissible 
                  onClose={() => {
                    setLocalError('');
                    clearError();
                  }}
                  className="mb-3"
                >
                  {displayError}
                </Alert>
              )}
              
              <Form onSubmit={handleSubmit}>
                <Form.Group className="mb-3">
                  <Form.Label>Email address</Form.Label>
                  <Form.Control
                    type="email"
                    name="email"
                    placeholder="Enter your email"
                    value={formData.email}
                    onChange={handleChange}
                    disabled={loading}
                    required
                  />
                </Form.Group>

                <Form.Group className="mb-3">
                  <Form.Label>Password</Form.Label>
                  <Form.Control
                    type="password"
                    name="password"
                    placeholder="Enter your password"
                    value={formData.password}
                    onChange={handleChange}
                    disabled={loading}
                    required
                  />
                </Form.Group>

                <div className="d-flex justify-content-between align-items-center mb-3">
                  <Form.Check 
                    type="checkbox"
                    id="remember-me"
                    label="Remember me"
                  />
                  <Link to="#" className="text-decoration-none small">
                    Forgot password?
                  </Link>
                </div>

                <Button 
                  variant="primary" 
                  type="submit" 
                  className="w-100"
                  disabled={loading}
                >
                  {loading ? (
                    <>
                      <LoadingSpinner size="small" text="" />
                      <span className="ms-2">Signing in...</span>
                    </>
                  ) : (
                    'Sign in'
                  )}
                </Button>
              </Form>
            </Card.Body>
          </Card>
        </Col>
      </Row>
    </Container>
  );
};

export default LoginForm;