import React from 'react';
import { Container, Row, Col, Card, Button, Badge } from 'react-bootstrap';
import { useAuth } from '../../auth/authContext';

const Dashboard = () => {
  const { user, logout } = useAuth();

  return (
    <Container className="py-4">
      <Row>
        <Col>
          <div className="d-flex justify-content-between align-items-center mb-4">
            <h1 className="h2">Dashboard</h1>
            <Badge bg="success">Online</Badge>
          </div>
        </Col>
      </Row>

      <Row className="g-4">
        <Col lg={4}>
          <Card className="shadow-sm">
            <Card.Header className="bg-primary text-white">
              <h5 className="mb-0">👤 User Profile</h5>
            </Card.Header>
            <Card.Body>
              <div className="mb-3">
                <strong>Email:</strong>
                <div className="text-muted">{user?.email || 'Not available'}</div>
              </div>
              <div className="mb-3">
                <strong>Name:</strong>
                <div className="text-muted">{user?.name || 'Not available'}</div>
              </div>
              <div className="mb-3">
                <strong>Status:</strong>
                <div>
                  <Badge bg="success">Active</Badge>
                </div>
              </div>
              <Button variant="outline-primary" size="sm" className="w-100">
                Edit Profile
              </Button>
            </Card.Body>
          </Card>
        </Col>

        <Col lg={4}>
          <Card className="shadow-sm">
            <Card.Header className="bg-info text-white">
              <h5 className="mb-0">📊 Quick Stats</h5>
            </Card.Header>
            <Card.Body>
              <Row className="text-center">
                <Col>
                  <div className="h4 text-primary mb-1">0</div>
                  <div className="small text-muted">Projects</div>
                </Col>
                <Col>
                  <div className="h4 text-success mb-1">0</div>
                  <div className="small text-muted">Tasks</div>
                </Col>
                <Col>
                  <div className="h4 text-warning mb-1">0</div>
                  <div className="small text-muted">Messages</div>
                </Col>
              </Row>
            </Card.Body>
          </Card>
        </Col>

        <Col lg={4}>
          <Card className="shadow-sm">
            <Card.Header className="bg-warning text-dark">
              <h5 className="mb-0">⚡ Quick Actions</h5>
            </Card.Header>
            <Card.Body>
              <div className="d-grid gap-2">
                <Button variant="primary" size="sm">
                  Create New Project
                </Button>
                <Button variant="secondary" size="sm">
                  View Reports
                </Button>
                <Button variant="outline-info" size="sm">
                  Settings
                </Button>
                <Button 
                  variant="outline-danger" 
                  size="sm" 
                  onClick={logout}
                >
                  Logout
                </Button>
              </div>
            </Card.Body>
          </Card>
        </Col>
      </Row>

      <Row className="mt-4">
        <Col>
          <Card className="shadow-sm">
            <Card.Header>
              <h5 className="mb-0">📈 Recent Activity</h5>
            </Card.Header>
            <Card.Body>
              <div className="text-center py-5 text-muted">
                <p>No recent activity to display.</p>
                <Button variant="outline-primary">
                  Get Started
                </Button>
              </div>
            </Card.Body>
          </Card>
        </Col>
      </Row>
    </Container>
  );
};

export default Dashboard;