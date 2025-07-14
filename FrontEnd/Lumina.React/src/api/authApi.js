import httpClient from './httpClient';

export const login = async (credentials) => {
  const response = await httpClient.post('/auth/login', credentials);
  return response.data;
};

export const register = async (registrationData) => {
  const response = await httpClient.post('/auth/register', registrationData);
  return response.data;
};
