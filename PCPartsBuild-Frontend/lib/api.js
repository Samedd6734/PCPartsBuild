// Relative path for client-side to leverage Next.js rewrites
const CLIENT_API_URL = '/api';
// Full path for server-side if needed (fallback)
const SERVER_API_URL = 'http://localhost:5298/api';

const apiFetch = async (endpoint, options = {}) => {
  const isServer = typeof window === 'undefined';
  const API_URL = isServer ? SERVER_API_URL : CLIENT_API_URL;
  
  const token = !isServer ? localStorage.getItem('token') : null;
  const path = endpoint.startsWith('/') ? endpoint : `/${endpoint}`;

  const headers = {
    'Content-Type': 'application/json',
    ...(token && { 'Authorization': `Bearer ${token}` }),
    ...options.headers,
  };

  const response = await fetch(`${API_URL}${path}`, {
    ...options,
    headers,
    credentials: 'include',
  });

  if (response.status === 401 && !isServer) {
    localStorage.removeItem('token');
    localStorage.removeItem('loggedInUser');
  }

  return response;
};

export const api = {
  get: (url, options) => apiFetch(url, { ...options, method: 'GET' }),
  post: (url, body, options) => apiFetch(url, { ...options, method: 'POST', body: JSON.stringify(body) }),
  put: (url, body, options) => apiFetch(url, { ...options, method: 'PUT', body: JSON.stringify(body) }),
  patch: (url, body, options) => apiFetch(url, { ...options, method: 'PATCH', body: JSON.stringify(body) }),
  delete: (url, options) => apiFetch(url, { ...options, method: 'DELETE' }),
};
