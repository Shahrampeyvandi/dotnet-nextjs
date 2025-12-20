const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5028/api';

async function fetchAPI<T>(endpoint: string, options?: RequestInit): Promise<T> {
  const response = await fetch(`${API_BASE_URL}${endpoint}`, {
    ...options,
    credentials: 'include', // برای Session cookies
    headers: {
      'Content-Type': 'application/json',
      ...options?.headers,
    },
  });

  if (!response.ok) {
    const error = await response.text();
    throw new Error(error || `HTTP error! status: ${response.status}`);
  }

  return response.json();
}

export const api = {
  get: <T>(endpoint: string) => fetchAPI<T>(endpoint, { method: 'GET' }),
  post: <T>(endpoint: string, data?: any) =>
    fetchAPI<T>(endpoint, {
      method: 'POST',
      body: JSON.stringify(data),
    }),
  put: <T>(endpoint: string, data?: any) =>
    fetchAPI<T>(endpoint, {
      method: 'PUT',
      body: JSON.stringify(data),
    }),
  delete: <T>(endpoint: string) => fetchAPI<T>(endpoint, { method: 'DELETE' }),
};

