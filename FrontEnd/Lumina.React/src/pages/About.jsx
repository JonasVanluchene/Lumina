import React from 'react';
import { Container, Row, Col, Card } from 'react-bootstrap';

export default function About() {
    return (
        <Container className="py-5">
            <Row className="justify-content-center">
                <Col lg={8}>
                    <div className="text-center mb-5">
                        <h1 className="display-4 fw-bold text-primary mb-3">
                            About Lumina
                        </h1>
                        <p className="lead text-muted">
                            Learn more about our modern web application
                        </p>
                    </div>
                </Col>
            </Row>

            <Row className="g-4">
                <Col lg={6}>
                    <Card className="h-100 shadow-sm">
                        <Card.Body>
                            <Card.Title className="text-primary mb-3">
                                🎯 Our Mission
                            </Card.Title>
                            <Card.Text>
                                Lumina is designed to showcase modern web development practices 
                                using React.js frontend with ASP.NET Core Web API backend. 
                                Our goal is to provide a robust, scalable, and secure foundation 
                                for building enterprise-level applications.
                            </Card.Text>
                        </Card.Body>
                    </Card>
                </Col>

                <Col lg={6}>
                    <Card className="h-100 shadow-sm">
                        <Card.Body>
                            <Card.Title className="text-primary mb-3">
                                🛠️ Technology Stack
                            </Card.Title>
                            <Card.Text>
                                <strong>Frontend:</strong> React.js, React Router, Bootstrap<br />
                                <strong>Backend:</strong> ASP.NET Core Web API<br />
                                <strong>Authentication:</strong> JWT with refresh tokens<br />
                                <strong>HTTP Client:</strong> Axios with interceptors<br />
                                <strong>Styling:</strong> Bootstrap 5
                            </Card.Text>
                        </Card.Body>
                    </Card>
                </Col>

                <Col lg={6}>
                    <Card className="h-100 shadow-sm">
                        <Card.Body>
                            <Card.Title className="text-primary mb-3">
                                🔧 Key Features
                            </Card.Title>
                            <Card.Text>
                                • Secure JWT-based authentication<br />
                                • Automatic token refresh<br />
                                • Protected routes<br />
                                • Responsive Bootstrap UI<br />
                                • Error handling and loading states<br />
                                • Professional code architecture
                            </Card.Text>
                        </Card.Body>
                    </Card>
                </Col>

                <Col lg={6}>
                    <Card className="h-100 shadow-sm">
                        <Card.Body>
                            <Card.Title className="text-primary mb-3">
                                🚀 Best Practices
                            </Card.Title>
                            <Card.Text>
                                This application implements industry best practices including 
                                proper error handling, loading states, responsive design, 
                                secure authentication, and clean code architecture that 
                                follows React and .NET conventions.
                            </Card.Text>
                        </Card.Body>
                    </Card>
                </Col>
            </Row>
        </Container>
    );
}