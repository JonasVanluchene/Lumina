import React from 'react';
import { Container, Row, Col, Card, Button } from 'react-bootstrap';
import { Link } from 'react-router-dom';
import { useAuth } from '../features/auth/authContext';

export default function Home() {
    const { isAuthenticated } = useAuth();

    return (
        <Container className="py-5">
            <Row className="justify-content-center">
                <Col lg={8}>
                    <div className="text-center mb-5">
                        <h1 className="display-4 fw-bold text-primary mb-3">
                            Welcome to Lumina
                        </h1>
                        <p className="lead text-muted">
                            A modern React application with ASP.NET Web API backend
                        </p>
                    </div>
                </Col>
            </Row>

            <Row className="g-4">
                <Col md={4}>
                    <Card className="h-100 shadow-sm">
                        <Card.Body className="text-center">
                            <Card.Title className="text-primary">
                                🚀 Modern Tech Stack
                            </Card.Title>
                            <Card.Text>
                                Built with React, Bootstrap, and ASP.NET Core for optimal performance and user experience.
                            </Card.Text>
                        </Card.Body>
                    </Card>
                </Col>

                <Col md={4}>
                    <Card className="h-100 shadow-sm">
                        <Card.Body className="text-center">
                            <Card.Title className="text-primary">
                                🔐 Secure Authentication
                            </Card.Title>
                            <Card.Text>
                                JWT-based authentication with refresh tokens and protected routes for enhanced security.
                            </Card.Text>
                        </Card.Body>
                    </Card>
                </Col>

                <Col md={4}>
                    <Card className="h-100 shadow-sm">
                        <Card.Body className="text-center">
                            <Card.Title className="text-primary">
                                📱 Responsive Design
                            </Card.Title>
                            <Card.Text>
                                Mobile-first responsive design that works beautifully on all devices and screen sizes.
                            </Card.Text>
                        </Card.Body>
                    </Card>
                </Col>
            </Row>

            <Row className="justify-content-center mt-5">
                <Col md={6} className="text-center">
                    {!isAuthenticated ? (
                        <div>
                            <h3 className="mb-3">Get Started Today</h3>
                            <p className="text-muted mb-4">
                                Create an account or sign in to access your dashboard
                            </p>
                            <div className="d-grid gap-2 d-md-flex justify-content-md-center">
                                <Button 
                                    as={Link} 
                                    to="/register" 
                                    variant="primary" 
                                    size="lg"
                                    className="me-md-2"
                                >
                                    Get Started
                                </Button>
                                <Button 
                                    as={Link} 
                                    to="/login" 
                                    variant="outline-primary" 
                                    size="lg"
                                >
                                    Sign In
                                </Button>
                            </div>
                        </div>
                    ) : (
                        <div>
                            <h3 className="mb-3">Welcome Back!</h3>
                            <p className="text-muted mb-4">
                                Ready to continue where you left off?
                            </p>
                            <Button 
                                as={Link} 
                                to="/dashboard" 
                                variant="primary" 
                                size="lg"
                            >
                                Go to Dashboard
                            </Button>
                        </div>
                    )}
                </Col>
            </Row>
        </Container>
    );
}