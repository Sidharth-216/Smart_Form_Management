import axios from 'axios'

const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL || 'http://localhost:5000/api',
  timeout: 15000,
})

api.interceptors.request.use((config) => {
  const token = localStorage.getItem('form-platform-token')
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

export const formsApi = {
  list: (params) => api.get('/forms', { params }),
  detail: (id) => api.get(`/forms/${id}`),
  search: (params) => api.get('/forms/search', { params }),
  facets: () => api.get('/forms/facets'),
  create: (payload) => api.post('/forms', payload),
  update: (id, payload) => api.put(`/forms/${id}`, payload),
  remove: (id) => api.delete(`/forms/${id}`),
  upload: (formData) => api.post('/forms/upload', formData),
}

export const authApi = {
  login: (payload) => api.post('/auth/login', payload),
  register: (payload) => api.post('/auth/register', payload),
}

export const aiApi = {
  extractText: (payload) => api.post('/ai/extract-text', payload),
  classify: (payload) => api.post('/ai/classify', payload),
  suggestTags: (payload) => api.post('/ai/suggest-tags', payload),
}

export const adminApi = {
  pendingUploads: () => api.get('/admin/uploads/pending'),
  approve: (id) => api.post(`/admin/uploads/${id}/approve`),
  reject: (id) => api.post(`/admin/uploads/${id}/reject`),
}

export default api
