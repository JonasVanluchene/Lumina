import { useState } from 'react';
import {Button, Form, Container} from 'react-bootstrap'
import { useAuth } from '../authContext';


export default function RegisterForm() {
  const {register, login} = useAuth();
  const [formData, setFormData] = useState({
    firstName: '',
    lastName: '',
    email: '',
    password: '',
    username: '',
  });

const [error, setError] = useState('');

const handleChange = (e) => {
  const { name, value } = e.target;
  setFormData((prevData) => ({
    ...prevData,
    [name]: value,
  }));
};

const handleSubmit = async (e) => {  
  e.preventDefault();
  console.log('Form submitted:', formData);
  setError('');
    try {
      const user = await register(formData);
      login(user);
    } catch (error) {
      setError(error.response?.data?.message || 'Registration failed');
    }
};


return (
  <Container>
    <Form onSubmit={handleSubmit}>
        <Form.Group className="mb-3" controlId="formEmail">
          <Form.Label>Email address</Form.Label>
          <Form.Control name='email' type="email" placeholder="Enter email" value={formData.email} onChange={handleChange}/>
          <Form.Text className="text-muted">
            We'll never share your email with anyone else.
          </Form.Text>
        </Form.Group>
          <Form.Group className="mb-3" controlId="formUsername">
            <Form.Label>Username (optional)</Form.Label>
            <Form.Control name='username' type="text" placeholder="Enter optional username" value={formData.username} onChange={handleChange} />
          </Form.Group>
        <Form.Group className="mb-3" controlId="formFirstName">
          <Form.Label>First Name</Form.Label>
          <Form.Control name='firstName' type="text" placeholder="First Name" value={formData.firstName} onChange={handleChange}/>
          </Form.Group>
          <Form.Group className="mb-3" controlId="formLastName">
            <Form.Label>Last Name</Form.Label>
            <Form.Control name='lastName' type="text" placeholder="Last Name"value={formData.lastName} onChange={handleChange}/>
          </Form.Group> 
        <Form.Group className="mb-3" controlId="formPassword">
          <Form.Label>Password</Form.Label>
          <Form.Control name='password' type="password" placeholder="Password" value={formData.password} onChange={handleChange}/>
        </Form.Group>
        <Form.Group className="mb-3" controlId="formNewsLetter">
          <Form.Check type="checkbox" label="Subscribe to newsletter" />
        </Form.Group>
        <Button variant="primary" type="submit">
          Submit
        </Button>
      </Form>
      {error && <div className="text-danger mb-3">{error}</div>}
  </Container>
)



};
















// Registering with steps V
// import { RegisterStepOne, RegisterStepTwo, RegisterStepThree } from '../components';

//Registring with 1 page V

// import React, { useState } from 'react';
// import { Container, Row, Col, Card, Form, Button, Alert } from 'react-bootstrap';
// import { Link } from 'react-router-dom';
// import { useAuth } from '../authContext';
// import LoadingSpinner from '../../../components/LoadingSpinner';

// const RegisterPage = () => {
//   const [formData, setFormData] = useState({
//     firstName: '',
//     lastName: '',
//     email: '',
//     password: '',
//     confirmPassword: ''
//   });
//   const [localError, setLocalError] = useState('');

//   const { register, loading, error, clearError } = useAuth();

//   const handleChange = (e) => {
//     const { name, value } = e.target;
//     setFormData(prev => ({
//       ...prev,
//       [name]: value
//     }));

//     // Clear errors when user starts typing
//     if (localError) setLocalError('');
//     if (error) clearError();
//   };

//   const handleSubmit = async (e) => {
//     e.preventDefault();

//     // Basic validation
//     if (!formData.firstName || !formData.lastName || !formData.email || !formData.password) {
//       setLocalError('Please fill in all fields');
//       return;
//     }

//     if (!formData.email.includes('@')) {
//       setLocalError('Please enter a valid email address');
//       return;
//     }

//     if (formData.password.length < 6) {
//       setLocalError('Password must be at least 6 characters long');
//       return;
//     }

//     if (formData.password !== formData.confirmPassword) {
//       setLocalError('Passwords do not match');
//       return;
//     }

//     const result = await register({
//       firstName: formData.firstName,
//       lastName: formData.lastName,
//       email: formData.email,
//       password: formData.password
//     });

//     if (!result.success) {
//       setLocalError(result.message);
//     }
//   };

//   const displayError = localError || error;

//   return (
//     <Container fluid className="min-vh-100 d-flex align-items-center justify-content-center bg-light">
//       <Row className="w-100 justify-content-center">
//         <Col xs={12} sm={10} md={8} lg={6} xl={5}>
//           <Card className="shadow">
//             <Card.Body className="p-4">
//               <div className="text-center mb-4">
//                 <h2 className="h3 mb-3">Create Your Account</h2>
//                 <p className="text-muted">
//                   Already have an account?{' '}
//                   <Link to="/login" className="text-decoration-none">
//                     Sign in here
//                   </Link>
//                 </p>
//               </div>

//               {displayError && (
//                 <Alert
//                   variant="danger"
//                   dismissible
//                   onClose={() => {
//                     setLocalError('');
//                     clearError();
//                   }}
//                   className="mb-3"
//                 >
//                   {displayError}
//                 </Alert>
//               )}

//               <Form onSubmit={handleSubmit}>
//                 <Row>
//                   <Col md={6}>
//                     <Form.Group className="mb-3">
//                       <Form.Label>First Name</Form.Label>
//                       <Form.Control
//                         type="text"
//                         name="firstName"
//                         placeholder="Enter your first name"
//                         value={formData.firstName}
//                         onChange={handleChange}
//                         disabled={loading}
//                         required
//                       />
//                     </Form.Group>
//                   </Col>
//                   <Col md={6}>
//                     <Form.Group className="mb-3">
//                       <Form.Label>Last Name</Form.Label>
//                       <Form.Control
//                         type="text"
//                         name="lastName"
//                         placeholder="Enter your last name"
//                         value={formData.lastName}
//                         onChange={handleChange}
//                         disabled={loading}
//                         required
//                       />
//                     </Form.Group>
//                   </Col>
//                 </Row>

//                 <Form.Group className="mb-3">
//                   <Form.Label>Email address</Form.Label>
//                   <Form.Control
//                     type="email"
//                     name="email"
//                     placeholder="Enter your email"
//                     value={formData.email}
//                     onChange={handleChange}
//                     disabled={loading}
//                     required
//                   />
//                 </Form.Group>

//                 <Form.Group className="mb-3">
//                   <Form.Label>Password</Form.Label>
//                   <Form.Control
//                     type="password"
//                     name="password"
//                     placeholder="Enter your password"
//                     value={formData.password}
//                     onChange={handleChange}
//                     disabled={loading}
//                     required
//                   />
//                   <Form.Text className="text-muted">
//                     Password must be at least 6 characters long.
//                   </Form.Text>
//                 </Form.Group>

//                 <Form.Group className="mb-3">
//                   <Form.Label>Confirm Password</Form.Label>
//                   <Form.Control
//                     type="password"
//                     name="confirmPassword"
//                     placeholder="Confirm your password"
//                     value={formData.confirmPassword}
//                     onChange={handleChange}
//                     disabled={loading}
//                     required
//                   />
//                 </Form.Group>

//                 <Form.Check
//                   type="checkbox"
//                   id="terms"
//                   label="I agree to the Terms of Service and Privacy Policy"
//                   className="mb-3"
//                   required
//                 />

//                 <Button
//                   variant="primary"
//                   type="submit"
//                   className="w-100"
//                   disabled={loading}
//                 >
//                   {loading ? (
//                     <>
//                       <LoadingSpinner size="small" text="" />
//                       <span className="ms-2">Creating account...</span>
//                     </>
//                   ) : (
//                     'Create Account'
//                   )}
//                 </Button>
//               </Form>
//             </Card.Body>
//           </Card>
//         </Col>
//       </Row>
//     </Container>
//   );
// };

// export default RegisterPage;
