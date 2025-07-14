import React from 'react';
import { Spinner } from 'react-bootstrap';

const LoadingSpinner = ({ size = 'medium', text = 'Loading...', variant = 'primary' }) => {
  const sizeMap = {
    small: 'sm',
    medium: 'sm',
    large: 'lg'
  };

  return (
    <div className="d-flex flex-column align-items-center justify-content-center p-3">
      <Spinner 
        animation="border" 
        variant={variant}
        size={sizeMap[size]}
      />
      {text && (
        <p className="mt-2 mb-0 text-muted small">{text}</p>
      )}
    </div>
  );
};

export default LoadingSpinner;
