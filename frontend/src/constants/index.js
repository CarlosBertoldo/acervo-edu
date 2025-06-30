// Status dos cursos para o Kanban
export const STATUS_CURSO = {
  BACKLOG: 'Backlog',
  EM_DESENVOLVIMENTO: 'EmDesenvolvimento', 
  VEICULADO: 'Veiculado'
};

export const STATUS_LABELS = {
  [STATUS_CURSO.BACKLOG]: 'Backlog',
  [STATUS_CURSO.EM_DESENVOLVIMENTO]: 'Em Desenvolvimento',
  [STATUS_CURSO.VEICULADO]: 'Veiculado'
};

export const STATUS_COLORS = {
  [STATUS_CURSO.BACKLOG]: {
    bg: 'bg-gray-100',
    border: 'border-gray-300',
    text: 'text-gray-700',
    badge: 'bg-gray-100 text-gray-800'
  },
  [STATUS_CURSO.EM_DESENVOLVIMENTO]: {
    bg: 'bg-blue-100',
    border: 'border-blue-300', 
    text: 'text-blue-700',
    badge: 'bg-blue-100 text-blue-800'
  },
  [STATUS_CURSO.VEICULADO]: {
    bg: 'bg-green-100',
    border: 'border-green-300',
    text: 'text-green-700',
    badge: 'bg-green-100 text-green-800'
  }
};

// Origem dos cursos
export const ORIGEM_CURSO = {
  MANUAL: 'Manual',
  SENIOR: 'Senior'
};

export const ORIGEM_LABELS = {
  [ORIGEM_CURSO.MANUAL]: 'Manual',
  [ORIGEM_CURSO.SENIOR]: 'Senior'
};

// Categorias de arquivos
export const CATEGORIA_ARQUIVO = {
  BRIEFING_DESENVOLVIMENTO: 'BriefingDesenvolvimento',
  BRIEFING_EXECUCAO: 'BriefingExecucao',
  PPT: 'PPT',
  CADERNO_EXERCICIO: 'CadernoExercicio',
  PLANO_AULA: 'PlanoAula',
  VIDEOS: 'Videos',
  PODCAST: 'Podcast',
  OUTROS: 'Outros'
};

export const CATEGORIA_LABELS = {
  [CATEGORIA_ARQUIVO.BRIEFING_DESENVOLVIMENTO]: 'Briefing de Desenvolvimento',
  [CATEGORIA_ARQUIVO.BRIEFING_EXECUCAO]: 'Briefing de Execução',
  [CATEGORIA_ARQUIVO.PPT]: 'PPT',
  [CATEGORIA_ARQUIVO.CADERNO_EXERCICIO]: 'Caderno de Exercício',
  [CATEGORIA_ARQUIVO.PLANO_AULA]: 'Plano de Aula',
  [CATEGORIA_ARQUIVO.VIDEOS]: 'Vídeos',
  [CATEGORIA_ARQUIVO.PODCAST]: 'Podcast',
  [CATEGORIA_ARQUIVO.OUTROS]: 'Outros Arquivos'
};

export const CATEGORIA_ICONS = {
  [CATEGORIA_ARQUIVO.BRIEFING_DESENVOLVIMENTO]: 'FileText',
  [CATEGORIA_ARQUIVO.BRIEFING_EXECUCAO]: 'FileCheck',
  [CATEGORIA_ARQUIVO.PPT]: 'Presentation',
  [CATEGORIA_ARQUIVO.CADERNO_EXERCICIO]: 'BookOpen',
  [CATEGORIA_ARQUIVO.PLANO_AULA]: 'GraduationCap',
  [CATEGORIA_ARQUIVO.VIDEOS]: 'Video',
  [CATEGORIA_ARQUIVO.PODCAST]: 'Mic',
  [CATEGORIA_ARQUIVO.OUTROS]: 'Folder'
};

// Tipos de arquivo permitidos
export const TIPOS_ARQUIVO_PERMITIDOS = {
  DOCUMENTOS: ['pdf', 'doc', 'docx', 'ppt', 'pptx', 'xls', 'xlsx'],
  VIDEOS: ['mp4', 'avi', 'mov', 'wmv', 'flv', 'webm'],
  AUDIOS: ['mp3', 'wav', 'aac', 'flac'],
  IMAGENS: ['jpg', 'jpeg', 'png', 'gif', 'bmp', 'svg', 'webp']
};

export const TODOS_TIPOS_PERMITIDOS = [
  ...TIPOS_ARQUIVO_PERMITIDOS.DOCUMENTOS,
  ...TIPOS_ARQUIVO_PERMITIDOS.VIDEOS,
  ...TIPOS_ARQUIVO_PERMITIDOS.AUDIOS,
  ...TIPOS_ARQUIVO_PERMITIDOS.IMAGENS
];

// Tamanho máximo de arquivo (500MB)
export const MAX_FILE_SIZE = 524288000;

// Configurações de paginação
export const PAGINATION = {
  DEFAULT_PAGE_SIZE: 20,
  PAGE_SIZE_OPTIONS: [10, 20, 50, 100]
};

// Configurações de toast/notificações
export const TOAST_DURATION = 5000;

// Configurações de tema
export const THEME = {
  PRIMARY: '#2563eb',
  SECONDARY: '#64748b',
  SUCCESS: '#16a34a',
  WARNING: '#d97706',
  ERROR: '#dc2626',
  INFO: '#0ea5e9'
};

// Rotas da aplicação
export const ROUTES = {
  HOME: '/',
  LOGIN: '/login',
  FORGOT_PASSWORD: '/forgot-password',
  RESET_PASSWORD: '/reset-password',
  DASHBOARD: '/dashboard',
  KANBAN: '/kanban',
  CURSOS: '/cursos',
  CURSO_DETALHES: '/cursos/:id',
  USUARIOS: '/usuarios',
  CONFIGURACOES: '/configuracoes',
  LOGS: '/logs'
};

// Configurações de validação
export const VALIDATION = {
  EMAIL_REGEX: /^[^\s@]+@[^\s@]+\.[^\s@]+$/,
  PASSWORD_MIN_LENGTH: 8,
  PASSWORD_REGEX: /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]/,
  CODIGO_CURSO_MAX_LENGTH: 50,
  NOME_CURSO_MAX_LENGTH: 500,
  DESCRICAO_MAX_LENGTH: 1000,
  COMENTARIOS_MAX_LENGTH: 2000
};

// Mensagens padrão
export const MESSAGES = {
  SUCCESS: {
    LOGIN: 'Login realizado com sucesso!',
    LOGOUT: 'Logout realizado com sucesso!',
    CURSO_CRIADO: 'Curso criado com sucesso!',
    CURSO_ATUALIZADO: 'Curso atualizado com sucesso!',
    CURSO_EXCLUIDO: 'Curso excluído com sucesso!',
    STATUS_ATUALIZADO: 'Status do curso atualizado com sucesso!',
    ARQUIVO_ENVIADO: 'Arquivo enviado com sucesso!',
    ARQUIVO_EXCLUIDO: 'Arquivo excluído com sucesso!',
    SENHA_ALTERADA: 'Senha alterada com sucesso!',
    EMAIL_RECUPERACAO_ENVIADO: 'Email de recuperação enviado com sucesso!'
  },
  ERROR: {
    GENERIC: 'Ocorreu um erro inesperado. Tente novamente.',
    NETWORK: 'Erro de conexão. Verifique sua internet.',
    UNAUTHORIZED: 'Você não tem permissão para esta ação.',
    NOT_FOUND: 'Recurso não encontrado.',
    VALIDATION: 'Dados inválidos. Verifique os campos.',
    FILE_TOO_LARGE: 'Arquivo muito grande. Tamanho máximo: 500MB.',
    FILE_TYPE_NOT_ALLOWED: 'Tipo de arquivo não permitido.',
    CODIGO_CURSO_EXISTS: 'Código do curso já existe.',
    CURSO_NOT_FOUND: 'Curso não encontrado.',
    ARQUIVO_NOT_FOUND: 'Arquivo não encontrado.'
  },
  CONFIRM: {
    DELETE_CURSO: 'Tem certeza que deseja excluir este curso?',
    DELETE_ARQUIVO: 'Tem certeza que deseja excluir este arquivo?',
    LOGOUT: 'Tem certeza que deseja sair?'
  }
};

