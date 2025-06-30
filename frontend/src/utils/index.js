import { VALIDATION, TODOS_TIPOS_PERMITIDOS, MAX_FILE_SIZE } from '../constants';

// Formatação de data
export const formatDate = (date, options = {}) => {
  if (!date) return '';
  
  const defaultOptions = {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
    ...options
  };
  
  return new Date(date).toLocaleDateString('pt-BR', defaultOptions);
};

export const formatDateTime = (date) => {
  if (!date) return '';
  
  return new Date(date).toLocaleString('pt-BR', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit'
  });
};

// Formatação de tamanho de arquivo
export const formatFileSize = (bytes) => {
  if (!bytes || bytes === 0) return '0 B';
  
  const sizes = ['B', 'KB', 'MB', 'GB', 'TB'];
  const i = Math.floor(Math.log(bytes) / Math.log(1024));
  
  return `${(bytes / Math.pow(1024, i)).toFixed(2)} ${sizes[i]}`;
};

// Validações
export const validateEmail = (email) => {
  return VALIDATION.EMAIL_REGEX.test(email);
};

export const validatePassword = (password) => {
  if (password.length < VALIDATION.PASSWORD_MIN_LENGTH) {
    return { valid: false, message: `Senha deve ter pelo menos ${VALIDATION.PASSWORD_MIN_LENGTH} caracteres` };
  }
  
  if (!VALIDATION.PASSWORD_REGEX.test(password)) {
    return { 
      valid: false, 
      message: 'Senha deve conter pelo menos: 1 letra minúscula, 1 maiúscula, 1 número e 1 caractere especial' 
    };
  }
  
  return { valid: true };
};

export const validateCodigoCurso = (codigo) => {
  if (!codigo || codigo.trim().length === 0) {
    return { valid: false, message: 'Código do curso é obrigatório' };
  }
  
  if (codigo.length > VALIDATION.CODIGO_CURSO_MAX_LENGTH) {
    return { valid: false, message: `Código deve ter no máximo ${VALIDATION.CODIGO_CURSO_MAX_LENGTH} caracteres` };
  }
  
  return { valid: true };
};

export const validateNomeCurso = (nome) => {
  if (!nome || nome.trim().length === 0) {
    return { valid: false, message: 'Nome do curso é obrigatório' };
  }
  
  if (nome.length > VALIDATION.NOME_CURSO_MAX_LENGTH) {
    return { valid: false, message: `Nome deve ter no máximo ${VALIDATION.NOME_CURSO_MAX_LENGTH} caracteres` };
  }
  
  return { valid: true };
};

export const validateFile = (file) => {
  if (!file) {
    return { valid: false, message: 'Arquivo é obrigatório' };
  }
  
  // Verificar tamanho
  if (file.size > MAX_FILE_SIZE) {
    return { valid: false, message: 'Arquivo muito grande. Tamanho máximo: 500MB' };
  }
  
  // Verificar tipo
  const extension = file.name.split('.').pop().toLowerCase();
  if (!TODOS_TIPOS_PERMITIDOS.includes(extension)) {
    return { valid: false, message: 'Tipo de arquivo não permitido' };
  }
  
  return { valid: true };
};

// Utilitários de string
export const truncateText = (text, maxLength = 100) => {
  if (!text) return '';
  if (text.length <= maxLength) return text;
  return text.substring(0, maxLength) + '...';
};

export const capitalizeFirst = (text) => {
  if (!text) return '';
  return text.charAt(0).toUpperCase() + text.slice(1).toLowerCase();
};

export const slugify = (text) => {
  return text
    .toLowerCase()
    .normalize('NFD')
    .replace(/[\u0300-\u036f]/g, '') // Remove acentos
    .replace(/[^a-z0-9 -]/g, '') // Remove caracteres especiais
    .replace(/\s+/g, '-') // Substitui espaços por hífens
    .replace(/-+/g, '-') // Remove hífens duplicados
    .trim('-'); // Remove hífens do início e fim
};

// Utilitários de array
export const groupBy = (array, key) => {
  return array.reduce((groups, item) => {
    const group = item[key];
    groups[group] = groups[group] || [];
    groups[group].push(item);
    return groups;
  }, {});
};

export const sortBy = (array, key, direction = 'asc') => {
  return [...array].sort((a, b) => {
    const aVal = a[key];
    const bVal = b[key];
    
    if (aVal < bVal) return direction === 'asc' ? -1 : 1;
    if (aVal > bVal) return direction === 'asc' ? 1 : -1;
    return 0;
  });
};

// Utilitários de URL
export const buildQueryString = (params) => {
  const searchParams = new URLSearchParams();
  
  Object.entries(params).forEach(([key, value]) => {
    if (value !== null && value !== undefined && value !== '') {
      searchParams.append(key, value);
    }
  });
  
  return searchParams.toString();
};

export const parseQueryString = (queryString) => {
  const params = new URLSearchParams(queryString);
  const result = {};
  
  for (const [key, value] of params) {
    result[key] = value;
  }
  
  return result;
};

// Utilitários de localStorage
export const storage = {
  get: (key, defaultValue = null) => {
    try {
      const item = localStorage.getItem(key);
      return item ? JSON.parse(item) : defaultValue;
    } catch (error) {
      console.error('Erro ao ler localStorage:', error);
      return defaultValue;
    }
  },
  
  set: (key, value) => {
    try {
      localStorage.setItem(key, JSON.stringify(value));
    } catch (error) {
      console.error('Erro ao salvar no localStorage:', error);
    }
  },
  
  remove: (key) => {
    try {
      localStorage.removeItem(key);
    } catch (error) {
      console.error('Erro ao remover do localStorage:', error);
    }
  },
  
  clear: () => {
    try {
      localStorage.clear();
    } catch (error) {
      console.error('Erro ao limpar localStorage:', error);
    }
  }
};

// Utilitários de debounce
export const debounce = (func, wait) => {
  let timeout;
  return function executedFunction(...args) {
    const later = () => {
      clearTimeout(timeout);
      func(...args);
    };
    clearTimeout(timeout);
    timeout = setTimeout(later, wait);
  };
};

// Utilitários de cores
export const getStatusColor = (status) => {
  const colors = {
    Backlog: 'gray',
    EmDesenvolvimento: 'blue',
    Veiculado: 'green'
  };
  return colors[status] || 'gray';
};

export const getOrigemColor = (origem) => {
  const colors = {
    Manual: 'blue',
    Senior: 'purple'
  };
  return colors[origem] || 'gray';
};

// Utilitários de download
export const downloadFile = (url, filename) => {
  const link = document.createElement('a');
  link.href = url;
  link.download = filename;
  document.body.appendChild(link);
  link.click();
  document.body.removeChild(link);
};

export const downloadBlob = (blob, filename) => {
  const url = window.URL.createObjectURL(blob);
  downloadFile(url, filename);
  window.URL.revokeObjectURL(url);
};

// Utilitários de clipboard
export const copyToClipboard = async (text) => {
  try {
    await navigator.clipboard.writeText(text);
    return true;
  } catch (error) {
    console.error('Erro ao copiar para clipboard:', error);
    return false;
  }
};

// Utilitários de tema
export const getTheme = () => {
  return localStorage.getItem('theme') || 'light';
};

export const setTheme = (theme) => {
  localStorage.setItem('theme', theme);
  document.documentElement.classList.toggle('dark', theme === 'dark');
};

export const toggleTheme = () => {
  const currentTheme = getTheme();
  const newTheme = currentTheme === 'light' ? 'dark' : 'light';
  setTheme(newTheme);
  return newTheme;
};

