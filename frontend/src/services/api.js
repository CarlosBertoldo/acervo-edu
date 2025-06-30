// Serviço de API para comunicação com o backend
const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000/api';

class ApiService {
  constructor() {
    this.baseURL = API_BASE_URL;
  }

  // Método para fazer requisições HTTP
  async request(endpoint, options = {}) {
    const url = `${this.baseURL}${endpoint}`;
    const token = localStorage.getItem('token');

    const config = {
      headers: {
        'Content-Type': 'application/json',
        ...options.headers,
      },
      ...options,
    };

    // Adicionar token de autorização se disponível
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }

    try {
      const response = await fetch(url, config);
      
      // Se não autorizado, limpar token e redirecionar para login
      if (response.status === 401) {
        localStorage.removeItem('token');
        localStorage.removeItem('refreshToken');
        localStorage.removeItem('user');
        window.location.href = '/login';
        return null;
      }

      const data = await response.json();

      if (!response.ok) {
        throw new Error(data.message || 'Erro na requisição');
      }

      return data;
    } catch (error) {
      console.error('Erro na API:', error);
      throw error;
    }
  }

  // Métodos de autenticação
  async login(email, password) {
    return this.request('/auth/login', {
      method: 'POST',
      body: JSON.stringify({ email, password }),
    });
  }

  async refreshToken(refreshToken) {
    return this.request('/auth/refresh', {
      method: 'POST',
      body: JSON.stringify({ refreshToken }),
    });
  }

  async forgotPassword(email) {
    return this.request('/auth/forgot-password', {
      method: 'POST',
      body: JSON.stringify({ email }),
    });
  }

  async resetPassword(token, newPassword, confirmPassword) {
    return this.request('/auth/reset-password', {
      method: 'POST',
      body: JSON.stringify({ token, newPassword, confirmPassword }),
    });
  }

  async changePassword(currentPassword, newPassword, confirmPassword) {
    return this.request('/auth/change-password', {
      method: 'POST',
      body: JSON.stringify({ currentPassword, newPassword, confirmPassword }),
    });
  }

  async logout() {
    return this.request('/auth/logout', {
      method: 'POST',
    });
  }

  async getCurrentUser() {
    return this.request('/auth/me');
  }

  // Métodos de cursos
  async getCursos(filters = {}) {
    const queryParams = new URLSearchParams();
    
    Object.entries(filters).forEach(([key, value]) => {
      if (value !== null && value !== undefined && value !== '') {
        queryParams.append(key, value);
      }
    });

    const queryString = queryParams.toString();
    const endpoint = queryString ? `/cursos?${queryString}` : '/cursos';
    
    return this.request(endpoint);
  }

  async getCursosKanban() {
    return this.request('/cursos/kanban');
  }

  async getCurso(id) {
    return this.request(`/cursos/${id}`);
  }

  async createCurso(cursoData) {
    return this.request('/cursos', {
      method: 'POST',
      body: JSON.stringify(cursoData),
    });
  }

  async updateCurso(id, cursoData) {
    return this.request(`/cursos/${id}`, {
      method: 'PUT',
      body: JSON.stringify(cursoData),
    });
  }

  async updateStatusCurso(id, status, comentario = '') {
    return this.request(`/cursos/${id}/status`, {
      method: 'PATCH',
      body: JSON.stringify({ status, comentario }),
    });
  }

  async deleteCurso(id) {
    return this.request(`/cursos/${id}`, {
      method: 'DELETE',
    });
  }

  async checkCodigoCurso(codigo, excludeId = null) {
    const queryParams = new URLSearchParams();
    if (excludeId) {
      queryParams.append('excludeId', excludeId);
    }
    
    const queryString = queryParams.toString();
    const endpoint = queryString 
      ? `/cursos/check-codigo/${codigo}?${queryString}` 
      : `/cursos/check-codigo/${codigo}`;
    
    return this.request(endpoint);
  }

  // Métodos de dashboard
  async getDashboardStats() {
    return this.request('/cursos/dashboard/stats');
  }

  // Métodos de arquivos
  async getArquivos(cursoId, categoria = null) {
    const queryParams = new URLSearchParams();
    queryParams.append('cursoId', cursoId);
    
    if (categoria) {
      queryParams.append('categoria', categoria);
    }

    return this.request(`/arquivos?${queryParams.toString()}`);
  }

  async uploadArquivo(cursoId, formData) {
    const token = localStorage.getItem('token');
    
    const response = await fetch(`${this.baseURL}/arquivos/${cursoId}`, {
      method: 'POST',
      headers: {
        Authorization: `Bearer ${token}`,
      },
      body: formData,
    });

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || 'Erro no upload');
    }

    return response.json();
  }

  async deleteArquivo(id) {
    return this.request(`/arquivos/${id}`, {
      method: 'DELETE',
    });
  }

  async shareArquivo(id, shareData) {
    return this.request(`/arquivos/${id}/share`, {
      method: 'POST',
      body: JSON.stringify(shareData),
    });
  }

  async getDownloadUrl(id) {
    return this.request(`/arquivos/${id}/download`);
  }
}

export default new ApiService();

