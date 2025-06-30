import React, { useState, useEffect } from 'react';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import { Alert, AlertDescription } from '@/components/ui/alert';
import { Checkbox } from '@/components/ui/checkbox';
import { 
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/components/ui/table';
import { 
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
  DialogFooter,
} from '@/components/ui/dialog';
import { 
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
  DropdownMenuSeparator,
} from '@/components/ui/dropdown-menu';
import { 
  Search, 
  Filter, 
  MoreVertical, 
  Calendar, 
  User, 
  Activity,
  Shield,
  Loader2,
  RefreshCw,
  Eye,
  Download,
  Upload,
  ChevronLeft,
  ChevronRight,
  ChevronsLeft,
  ChevronsRight,
  X,
  FileText,
  BarChart3,
  TrendingUp,
  Clock,
  AlertTriangle,
  CheckCircle,
  XCircle,
  Info,
  Edit,
  Trash2,
  Plus,
  Share,
  Copy,
  Settings,
  Database,
  LogIn,
  LogOut,
  UserPlus,
  UserMinus,
  FileUp,
  FileDown,
  Zap
} from 'lucide-react';
import { formatDate, truncateText } from '../utils';
import { LineChart, Line, AreaChart, Area, BarChart, Bar, PieChart, Pie, Cell, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer } from 'recharts';

// Dados mockados de logs
const mockLogs = [
  {
    id: 1,
    usuario: 'João Silva',
    usuarioId: 1,
    acao: 'LOGIN',
    descricao: 'Usuário fez login no sistema',
    recurso: 'Sistema',
    recursoId: null,
    enderecoIp: '192.168.1.100',
    userAgent: 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36',
    status: 'Sucesso',
    timestamp: '2024-01-29T14:30:00Z',
    detalhes: { sessionId: 'sess_123456', loginMethod: 'email' }
  },
  {
    id: 2,
    usuario: 'Maria Santos',
    usuarioId: 2,
    acao: 'CRIAR_CURSO',
    descricao: 'Criou o curso "Photoshop Básico para Marketing"',
    recurso: 'Curso',
    recursoId: 3,
    enderecoIp: '192.168.1.101',
    userAgent: 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36',
    status: 'Sucesso',
    timestamp: '2024-01-29T13:45:00Z',
    detalhes: { cursoNome: 'Photoshop Básico para Marketing', academia: 'Academia de Design' }
  },
  {
    id: 3,
    usuario: 'Carlos Oliveira',
    usuarioId: 3,
    acao: 'UPLOAD_ARQUIVO',
    descricao: 'Fez upload do arquivo "manual_autocad.pdf"',
    recurso: 'Arquivo',
    recursoId: 15,
    enderecoIp: '192.168.1.102',
    userAgent: 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36',
    status: 'Sucesso',
    timestamp: '2024-01-29T12:20:00Z',
    detalhes: { nomeArquivo: 'manual_autocad.pdf', tamanho: '2.5MB', categoria: 'Plano de Aula' }
  },
  {
    id: 4,
    usuario: 'Ana Costa',
    usuarioId: 4,
    acao: 'EDITAR_CURSO',
    descricao: 'Editou o curso "Python para Análise de Dados"',
    recurso: 'Curso',
    recursoId: 6,
    enderecoIp: '192.168.1.103',
    userAgent: 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36',
    status: 'Sucesso',
    timestamp: '2024-01-29T11:15:00Z',
    detalhes: { alteracoes: ['status', 'progresso'], statusAnterior: 'Em Desenvolvimento', statusNovo: 'Veiculado' }
  },
  {
    id: 5,
    usuario: 'Sistema',
    usuarioId: null,
    acao: 'SYNC_SENIOR',
    descricao: 'Sincronização automática com sistema Senior',
    recurso: 'Integração',
    recursoId: null,
    enderecoIp: '10.0.0.1',
    userAgent: 'AcervoEducacional/1.0 (Automated)',
    status: 'Sucesso',
    timestamp: '2024-01-29T10:00:00Z',
    detalhes: { cursosAtualizados: 5, novosUsuarios: 0, erros: 0 }
  },
  {
    id: 6,
    usuario: 'Pedro Lima',
    usuarioId: 5,
    acao: 'VISUALIZAR_ARQUIVO',
    descricao: 'Visualizou o arquivo "video_aula_01.mp4"',
    recurso: 'Arquivo',
    recursoId: 22,
    enderecoIp: '192.168.1.104',
    userAgent: 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36',
    status: 'Sucesso',
    timestamp: '2024-01-29T09:30:00Z',
    detalhes: { nomeArquivo: 'video_aula_01.mp4', duracao: '15:30', categoria: 'Vídeos' }
  },
  {
    id: 7,
    usuario: 'Fernanda Costa',
    usuarioId: 6,
    acao: 'COMPARTILHAR_ARQUIVO',
    descricao: 'Compartilhou arquivo com link público',
    recurso: 'Arquivo',
    recursoId: 18,
    enderecoIp: '192.168.1.105',
    userAgent: 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36',
    status: 'Sucesso',
    timestamp: '2024-01-29T08:45:00Z',
    detalhes: { tipoCompartilhamento: 'público', expiração: '2024-02-29', downloads: 0 }
  },
  {
    id: 8,
    usuario: 'Lucas Ferreira',
    usuarioId: 7,
    acao: 'LOGIN_FALHA',
    descricao: 'Tentativa de login falhada - senha incorreta',
    recurso: 'Sistema',
    recursoId: null,
    enderecoIp: '192.168.1.106',
    userAgent: 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36',
    status: 'Erro',
    timestamp: '2024-01-29T07:20:00Z',
    detalhes: { tentativa: 3, bloqueado: false, motivo: 'senha_incorreta' }
  },
  {
    id: 9,
    usuario: 'João Silva',
    usuarioId: 1,
    acao: 'EXCLUIR_ARQUIVO',
    descricao: 'Excluiu o arquivo "rascunho_apresentacao.pptx"',
    recurso: 'Arquivo',
    recursoId: 25,
    enderecoIp: '192.168.1.100',
    userAgent: 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36',
    status: 'Sucesso',
    timestamp: '2024-01-28T16:10:00Z',
    detalhes: { nomeArquivo: 'rascunho_apresentacao.pptx', tamanho: '1.2MB', motivo: 'arquivo_obsoleto' }
  },
  {
    id: 10,
    usuario: 'Juliana Alves',
    usuarioId: 8,
    acao: 'EXPORTAR_RELATORIO',
    descricao: 'Exportou relatório de atividades em PDF',
    recurso: 'Relatório',
    recursoId: null,
    enderecoIp: '192.168.1.107',
    userAgent: 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36',
    status: 'Sucesso',
    timestamp: '2024-01-28T15:30:00Z',
    detalhes: { tipoRelatorio: 'atividades', periodo: '30_dias', formato: 'PDF' }
  }
];

// Dados para gráficos
const dadosAtividadePorDia = [
  { data: '24/01', atividades: 45, logins: 12, uploads: 8 },
  { data: '25/01', atividades: 52, logins: 15, uploads: 12 },
  { data: '26/01', atividades: 38, logins: 10, uploads: 6 },
  { data: '27/01', atividades: 61, logins: 18, uploads: 15 },
  { data: '28/01', atividades: 49, logins: 14, uploads: 10 },
  { data: '29/01', atividades: 67, logins: 20, uploads: 18 }
];

const dadosAcoesPorTipo = [
  { nome: 'Login', valor: 156, cor: '#3B82F6' },
  { nome: 'Upload', valor: 89, cor: '#10B981' },
  { nome: 'Edição', valor: 67, cor: '#F59E0B' },
  { nome: 'Visualização', valor: 234, cor: '#8B5CF6' },
  { nome: 'Compartilhamento', valor: 45, cor: '#EF4444' },
  { nome: 'Exclusão', valor: 23, cor: '#6B7280' }
];

const dadosStatusPorHora = [
  { hora: '00h', sucesso: 2, erro: 0, warning: 1 },
  { hora: '06h', sucesso: 8, erro: 1, warning: 2 },
  { hora: '08h', sucesso: 25, erro: 2, warning: 3 },
  { hora: '10h', sucesso: 45, erro: 3, warning: 5 },
  { hora: '12h', sucesso: 38, erro: 1, warning: 4 },
  { hora: '14h', sucesso: 52, erro: 4, warning: 6 },
  { hora: '16h', sucesso: 41, erro: 2, warning: 3 },
  { hora: '18h', sucesso: 28, erro: 1, warning: 2 },
  { hora: '20h', sucesso: 15, erro: 0, warning: 1 },
  { hora: '22h', sucesso: 8, erro: 0, warning: 0 }
];

const ACAO_CONFIG = {
  'LOGIN': { color: 'bg-blue-100 text-blue-800', label: 'Login', icon: LogIn },
  'LOGOUT': { color: 'bg-gray-100 text-gray-800', label: 'Logout', icon: LogOut },
  'LOGIN_FALHA': { color: 'bg-red-100 text-red-800', label: 'Login Falha', icon: XCircle },
  'CRIAR_CURSO': { color: 'bg-green-100 text-green-800', label: 'Criar Curso', icon: Plus },
  'EDITAR_CURSO': { color: 'bg-yellow-100 text-yellow-800', label: 'Editar Curso', icon: Edit },
  'EXCLUIR_CURSO': { color: 'bg-red-100 text-red-800', label: 'Excluir Curso', icon: Trash2 },
  'UPLOAD_ARQUIVO': { color: 'bg-purple-100 text-purple-800', label: 'Upload', icon: FileUp },
  'DOWNLOAD_ARQUIVO': { color: 'bg-indigo-100 text-indigo-800', label: 'Download', icon: FileDown },
  'VISUALIZAR_ARQUIVO': { color: 'bg-cyan-100 text-cyan-800', label: 'Visualizar', icon: Eye },
  'COMPARTILHAR_ARQUIVO': { color: 'bg-orange-100 text-orange-800', label: 'Compartilhar', icon: Share },
  'EXCLUIR_ARQUIVO': { color: 'bg-red-100 text-red-800', label: 'Excluir Arquivo', icon: Trash2 },
  'CRIAR_USUARIO': { color: 'bg-green-100 text-green-800', label: 'Criar Usuário', icon: UserPlus },
  'EDITAR_USUARIO': { color: 'bg-yellow-100 text-yellow-800', label: 'Editar Usuário', icon: Edit },
  'EXCLUIR_USUARIO': { color: 'bg-red-100 text-red-800', label: 'Excluir Usuário', icon: UserMinus },
  'SYNC_SENIOR': { color: 'bg-teal-100 text-teal-800', label: 'Sync Senior', icon: Zap },
  'EXPORTAR_RELATORIO': { color: 'bg-pink-100 text-pink-800', label: 'Exportar', icon: Download },
  'CONFIGURACAO': { color: 'bg-slate-100 text-slate-800', label: 'Configuração', icon: Settings }
};

const STATUS_CONFIG = {
  'Sucesso': { color: 'bg-green-100 text-green-800', label: 'Sucesso', icon: CheckCircle },
  'Erro': { color: 'bg-red-100 text-red-800', label: 'Erro', icon: XCircle },
  'Warning': { color: 'bg-yellow-100 text-yellow-800', label: 'Aviso', icon: AlertTriangle },
  'Info': { color: 'bg-blue-100 text-blue-800', label: 'Info', icon: Info }
};

const Logs = () => {
  const [logs, setLogs] = useState(mockLogs);
  const [loading, setLoading] = useState(true);
  const [refreshing, setRefreshing] = useState(false);
  const [searchTerm, setSearchTerm] = useState('');
  
  // Estados para filtros
  const [showFilters, setShowFilters] = useState(false);
  const [selectedFilters, setSelectedFilters] = useState({
    acao: '',
    status: '',
    usuario: '',
    dataInicio: '',
    dataFim: ''
  });
  
  // Estados para paginação
  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  
  // Estados para seleção múltipla
  const [selectedLogs, setSelectedLogs] = useState([]);
  const [selectAll, setSelectAll] = useState(false);
  
  // Estados para modais
  const [logSelecionado, setLogSelecionado] = useState(null);
  const [detalhesModalOpen, setDetalhesModalOpen] = useState(false);
  const [relatorioModalOpen, setRelatorioModalOpen] = useState(false);

  useEffect(() => {
    // Simular carregamento
    setTimeout(() => {
      setLoading(false);
    }, 1000);
  }, []);

  const handleRefresh = () => {
    setRefreshing(true);
    setTimeout(() => {
      setRefreshing(false);
    }, 1000);
  };

  // Filtrar logs
  const filteredLogs = logs.filter(log => {
    const matchesSearch = log.usuario.toLowerCase().includes(searchTerm.toLowerCase()) ||
                         log.descricao.toLowerCase().includes(searchTerm.toLowerCase()) ||
                         log.acao.toLowerCase().includes(searchTerm.toLowerCase()) ||
                         log.enderecoIp.includes(searchTerm);
    
    const matchesAcao = !selectedFilters.acao || log.acao === selectedFilters.acao;
    const matchesStatus = !selectedFilters.status || log.status === selectedFilters.status;
    const matchesUsuario = !selectedFilters.usuario || log.usuario.toLowerCase().includes(selectedFilters.usuario.toLowerCase());
    
    let matchesData = true;
    if (selectedFilters.dataInicio || selectedFilters.dataFim) {
      const logDate = new Date(log.timestamp);
      if (selectedFilters.dataInicio) {
        matchesData = matchesData && logDate >= new Date(selectedFilters.dataInicio);
      }
      if (selectedFilters.dataFim) {
        matchesData = matchesData && logDate <= new Date(selectedFilters.dataFim);
      }
    }
    
    return matchesSearch && matchesAcao && matchesStatus && matchesUsuario && matchesData;
  });

  // Paginação
  const totalPages = Math.ceil(filteredLogs.length / pageSize);
  const startIndex = (currentPage - 1) * pageSize;
  const endIndex = startIndex + pageSize;
  const currentLogs = filteredLogs.slice(startIndex, endIndex);

  // Funções de seleção
  const handleSelectLog = (logId) => {
    setSelectedLogs(prev => 
      prev.includes(logId) 
        ? prev.filter(id => id !== logId)
        : [...prev, logId]
    );
  };

  const handleSelectAll = () => {
    if (selectAll) {
      setSelectedLogs([]);
    } else {
      setSelectedLogs(currentLogs.map(log => log.id));
    }
    setSelectAll(!selectAll);
  };

  // Funções de ação
  const abrirDetalhes = (log) => {
    setLogSelecionado(log);
    setDetalhesModalOpen(true);
  };

  const exportarLogs = (formato) => {
    // Simular exportação
    console.log(`Exportando ${selectedLogs.length || filteredLogs.length} logs em formato ${formato}`);
  };

  const getAcoes = () => {
    return [...new Set(logs.map(l => l.acao))].sort();
  };

  const getUsuarios = () => {
    return [...new Set(logs.map(l => l.usuario))].sort();
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="flex items-center space-x-2">
          <Loader2 className="h-6 w-6 animate-spin text-blue-600" />
          <span className="text-gray-600">Carregando logs...</span>
        </div>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      {/* Cabeçalho */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">Logs e Relatórios</h1>
          <p className="text-gray-600 mt-1">
            Monitore atividades e gere relatórios do sistema
          </p>
        </div>
        
        <div className="flex items-center gap-3">
          {selectedLogs.length > 0 && (
            <Button variant="outline">
              <Download className="h-4 w-4 mr-2" />
              Exportar ({selectedLogs.length})
            </Button>
          )}
          <Button variant="outline" onClick={() => setRelatorioModalOpen(true)}>
            <BarChart3 className="h-4 w-4 mr-2" />
            Relatórios
          </Button>
          <Button variant="outline" onClick={handleRefresh} disabled={refreshing}>
            <RefreshCw className={`h-4 w-4 mr-2 ${refreshing ? 'animate-spin' : ''}`} />
            Atualizar
          </Button>
        </div>
      </div>

      {/* Cards de estatísticas */}
      <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
        <Card>
          <CardContent className="p-4">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm font-medium text-gray-600">Total de Logs</p>
                <p className="text-2xl font-bold text-gray-900">{logs.length}</p>
              </div>
              <Activity className="h-8 w-8 text-blue-600" />
            </div>
          </CardContent>
        </Card>
        
        <Card>
          <CardContent className="p-4">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm font-medium text-gray-600">Ações Hoje</p>
                <p className="text-2xl font-bold text-green-600">
                  {logs.filter(l => {
                    const today = new Date();
                    const logDate = new Date(l.timestamp);
                    return logDate.toDateString() === today.toDateString();
                  }).length}
                </p>
              </div>
              <TrendingUp className="h-8 w-8 text-green-600" />
            </div>
          </CardContent>
        </Card>
        
        <Card>
          <CardContent className="p-4">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm font-medium text-gray-600">Erros (24h)</p>
                <p className="text-2xl font-bold text-red-600">
                  {logs.filter(l => l.status === 'Erro').length}
                </p>
              </div>
              <XCircle className="h-8 w-8 text-red-600" />
            </div>
          </CardContent>
        </Card>
        
        <Card>
          <CardContent className="p-4">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm font-medium text-gray-600">Usuários Ativos</p>
                <p className="text-2xl font-bold text-purple-600">
                  {new Set(logs.filter(l => l.usuarioId).map(l => l.usuarioId)).size}
                </p>
              </div>
              <User className="h-8 w-8 text-purple-600" />
            </div>
          </CardContent>
        </Card>
      </div>

      {/* Gráficos */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <Card>
          <CardHeader>
            <CardTitle>Atividades por Dia</CardTitle>
          </CardHeader>
          <CardContent>
            <ResponsiveContainer width="100%" height={300}>
              <AreaChart data={dadosAtividadePorDia}>
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="data" />
                <YAxis />
                <Tooltip />
                <Legend />
                <Area type="monotone" dataKey="atividades" stackId="1" stroke="#3B82F6" fill="#3B82F6" fillOpacity={0.6} />
                <Area type="monotone" dataKey="logins" stackId="1" stroke="#10B981" fill="#10B981" fillOpacity={0.6} />
                <Area type="monotone" dataKey="uploads" stackId="1" stroke="#F59E0B" fill="#F59E0B" fillOpacity={0.6} />
              </AreaChart>
            </ResponsiveContainer>
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>Ações por Tipo</CardTitle>
          </CardHeader>
          <CardContent>
            <ResponsiveContainer width="100%" height={300}>
              <PieChart>
                <Pie
                  data={dadosAcoesPorTipo}
                  cx="50%"
                  cy="50%"
                  labelLine={false}
                  label={({ nome, valor }) => `${nome}: ${valor}`}
                  outerRadius={80}
                  fill="#8884d8"
                  dataKey="valor"
                >
                  {dadosAcoesPorTipo.map((entry, index) => (
                    <Cell key={`cell-${index}`} fill={entry.cor} />
                  ))}
                </Pie>
                <Tooltip />
              </PieChart>
            </ResponsiveContainer>
          </CardContent>
        </Card>
      </div>

      {/* Barra de busca e filtros */}
      <div className="flex items-center gap-4">
        <div className="relative flex-1">
          <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 h-4 w-4" />
          <Input
            placeholder="Buscar por usuário, ação, descrição ou IP..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="pl-10"
          />
        </div>
        <Button 
          variant="outline" 
          onClick={() => setShowFilters(!showFilters)}
          className={showFilters ? 'bg-blue-50 border-blue-300' : ''}
        >
          <Filter className="h-4 w-4 mr-2" />
          Filtros
        </Button>
        <Button variant="outline" onClick={() => exportarLogs('excel')}>
          <Download className="h-4 w-4 mr-2" />
          Exportar
        </Button>
      </div>

      {/* Filtros expandidos */}
      {showFilters && (
        <Card>
          <CardContent className="pt-6">
            <div className="grid grid-cols-1 md:grid-cols-5 gap-4">
              <div>
                <label className="text-sm font-medium text-gray-700 mb-2 block">Ação</label>
                <select 
                  className="w-full p-2 border border-gray-300 rounded-md"
                  value={selectedFilters.acao}
                  onChange={(e) => setSelectedFilters(prev => ({ ...prev, acao: e.target.value }))}
                >
                  <option value="">Todas</option>
                  {getAcoes().map(acao => (
                    <option key={acao} value={acao}>{ACAO_CONFIG[acao]?.label || acao}</option>
                  ))}
                </select>
              </div>
              <div>
                <label className="text-sm font-medium text-gray-700 mb-2 block">Status</label>
                <select 
                  className="w-full p-2 border border-gray-300 rounded-md"
                  value={selectedFilters.status}
                  onChange={(e) => setSelectedFilters(prev => ({ ...prev, status: e.target.value }))}
                >
                  <option value="">Todos</option>
                  <option value="Sucesso">Sucesso</option>
                  <option value="Erro">Erro</option>
                  <option value="Warning">Aviso</option>
                </select>
              </div>
              <div>
                <label className="text-sm font-medium text-gray-700 mb-2 block">Usuário</label>
                <Input
                  placeholder="Nome do usuário"
                  value={selectedFilters.usuario}
                  onChange={(e) => setSelectedFilters(prev => ({ ...prev, usuario: e.target.value }))}
                />
              </div>
              <div>
                <label className="text-sm font-medium text-gray-700 mb-2 block">Data Início</label>
                <Input
                  type="date"
                  value={selectedFilters.dataInicio}
                  onChange={(e) => setSelectedFilters(prev => ({ ...prev, dataInicio: e.target.value }))}
                />
              </div>
              <div>
                <label className="text-sm font-medium text-gray-700 mb-2 block">Data Fim</label>
                <Input
                  type="date"
                  value={selectedFilters.dataFim}
                  onChange={(e) => setSelectedFilters(prev => ({ ...prev, dataFim: e.target.value }))}
                />
              </div>
            </div>
            <div className="flex justify-end mt-4">
              <Button 
                variant="outline" 
                onClick={() => setSelectedFilters({ acao: '', status: '', usuario: '', dataInicio: '', dataFim: '' })}
              >
                <X className="h-4 w-4 mr-2" />
                Limpar Filtros
              </Button>
            </div>
          </CardContent>
        </Card>
      )}

      {/* Tabela de logs */}
      <Card>
        <CardContent className="p-0">
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead className="w-12">
                  <Checkbox 
                    checked={selectAll}
                    onCheckedChange={handleSelectAll}
                  />
                </TableHead>
                <TableHead>Data/Hora</TableHead>
                <TableHead>Usuário</TableHead>
                <TableHead>Ação</TableHead>
                <TableHead>Descrição</TableHead>
                <TableHead>Status</TableHead>
                <TableHead>IP</TableHead>
                <TableHead className="w-12">Ações</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {currentLogs.length === 0 ? (
                <TableRow>
                  <TableCell colSpan={8} className="text-center py-8 text-gray-500">
                    <Activity className="h-12 w-12 mx-auto mb-4 text-gray-300" />
                    <p>Nenhum log encontrado</p>
                  </TableCell>
                </TableRow>
              ) : (
                currentLogs.map((log) => (
                  <TableRow key={log.id} className="hover:bg-gray-50">
                    <TableCell>
                      <Checkbox 
                        checked={selectedLogs.includes(log.id)}
                        onCheckedChange={() => handleSelectLog(log.id)}
                      />
                    </TableCell>
                    <TableCell>
                      <div className="text-sm text-gray-900">
                        {formatDate(log.timestamp)}
                      </div>
                    </TableCell>
                    <TableCell>
                      <div className="flex items-center gap-2">
                        <div className="w-6 h-6 bg-blue-100 rounded-full flex items-center justify-center">
                          <User className="h-3 w-3 text-blue-600" />
                        </div>
                        <span className="text-sm text-gray-900">{log.usuario}</span>
                      </div>
                    </TableCell>
                    <TableCell>
                      <Badge className={ACAO_CONFIG[log.acao]?.color}>
                        {ACAO_CONFIG[log.acao]?.label || log.acao}
                      </Badge>
                    </TableCell>
                    <TableCell>
                      <div className="text-sm text-gray-900 max-w-xs">
                        {truncateText(log.descricao, 50)}
                      </div>
                    </TableCell>
                    <TableCell>
                      <Badge className={STATUS_CONFIG[log.status]?.color}>
                        {STATUS_CONFIG[log.status]?.label}
                      </Badge>
                    </TableCell>
                    <TableCell>
                      <div className="text-sm text-gray-600 font-mono">
                        {log.enderecoIp}
                      </div>
                    </TableCell>
                    <TableCell>
                      <Button 
                        variant="ghost" 
                        size="sm" 
                        onClick={() => abrirDetalhes(log)}
                        className="h-8 w-8 p-0"
                      >
                        <Eye className="h-4 w-4" />
                      </Button>
                    </TableCell>
                  </TableRow>
                ))
              )}
            </TableBody>
          </Table>
        </CardContent>
      </Card>

      {/* Paginação */}
      <div className="flex items-center justify-between">
        <div className="flex items-center gap-2">
          <span className="text-sm text-gray-700">
            Mostrando {startIndex + 1} a {Math.min(endIndex, filteredLogs.length)} de {filteredLogs.length} logs
          </span>
          <select 
            className="ml-4 p-1 border border-gray-300 rounded text-sm"
            value={pageSize}
            onChange={(e) => {
              setPageSize(Number(e.target.value));
              setCurrentPage(1);
            }}
          >
            <option value={10}>10 por página</option>
            <option value={20}>20 por página</option>
            <option value={50}>50 por página</option>
            <option value={100}>100 por página</option>
          </select>
        </div>
        
        <div className="flex items-center gap-2">
          <Button 
            variant="outline" 
            size="sm"
            onClick={() => setCurrentPage(1)}
            disabled={currentPage === 1}
          >
            <ChevronsLeft className="h-4 w-4" />
          </Button>
          <Button 
            variant="outline" 
            size="sm"
            onClick={() => setCurrentPage(prev => Math.max(1, prev - 1))}
            disabled={currentPage === 1}
          >
            <ChevronLeft className="h-4 w-4" />
          </Button>
          
          <span className="text-sm text-gray-700 px-4">
            Página {currentPage} de {totalPages}
          </span>
          
          <Button 
            variant="outline" 
            size="sm"
            onClick={() => setCurrentPage(prev => Math.min(totalPages, prev + 1))}
            disabled={currentPage === totalPages}
          >
            <ChevronRight className="h-4 w-4" />
          </Button>
          <Button 
            variant="outline" 
            size="sm"
            onClick={() => setCurrentPage(totalPages)}
            disabled={currentPage === totalPages}
          >
            <ChevronsRight className="h-4 w-4" />
          </Button>
        </div>
      </div>

      {/* Modal de Detalhes */}
      <Dialog open={detalhesModalOpen} onOpenChange={setDetalhesModalOpen}>
        <DialogContent className="max-w-4xl max-h-[80vh] overflow-y-auto">
          <DialogHeader>
            <DialogTitle>Detalhes do Log</DialogTitle>
            <DialogDescription>
              Informações completas sobre a atividade registrada
            </DialogDescription>
          </DialogHeader>
          {logSelecionado && (
            <div className="space-y-6">
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="text-sm font-medium text-gray-700">ID do Log</label>
                  <p className="text-sm text-gray-900 bg-gray-50 p-2 rounded">
                    #{logSelecionado.id}
                  </p>
                </div>
                <div>
                  <label className="text-sm font-medium text-gray-700">Data/Hora</label>
                  <p className="text-sm text-gray-900 bg-gray-50 p-2 rounded">
                    {formatDate(logSelecionado.timestamp)}
                  </p>
                </div>
                <div>
                  <label className="text-sm font-medium text-gray-700">Usuário</label>
                  <p className="text-sm text-gray-900 bg-gray-50 p-2 rounded">
                    {logSelecionado.usuario}
                  </p>
                </div>
                <div>
                  <label className="text-sm font-medium text-gray-700">Ação</label>
                  <div className="bg-gray-50 p-2 rounded">
                    <Badge className={ACAO_CONFIG[logSelecionado.acao]?.color}>
                      {ACAO_CONFIG[logSelecionado.acao]?.label || logSelecionado.acao}
                    </Badge>
                  </div>
                </div>
                <div className="col-span-2">
                  <label className="text-sm font-medium text-gray-700">Descrição</label>
                  <p className="text-sm text-gray-900 bg-gray-50 p-2 rounded">
                    {logSelecionado.descricao}
                  </p>
                </div>
                <div>
                  <label className="text-sm font-medium text-gray-700">Status</label>
                  <div className="bg-gray-50 p-2 rounded">
                    <Badge className={STATUS_CONFIG[logSelecionado.status]?.color}>
                      {STATUS_CONFIG[logSelecionado.status]?.label}
                    </Badge>
                  </div>
                </div>
                <div>
                  <label className="text-sm font-medium text-gray-700">Endereço IP</label>
                  <p className="text-sm text-gray-900 bg-gray-50 p-2 rounded font-mono">
                    {logSelecionado.enderecoIp}
                  </p>
                </div>
                <div className="col-span-2">
                  <label className="text-sm font-medium text-gray-700">User Agent</label>
                  <p className="text-sm text-gray-900 bg-gray-50 p-2 rounded break-all">
                    {logSelecionado.userAgent}
                  </p>
                </div>
                {logSelecionado.detalhes && (
                  <div className="col-span-2">
                    <label className="text-sm font-medium text-gray-700">Detalhes Adicionais</label>
                    <pre className="text-sm text-gray-900 bg-gray-50 p-2 rounded overflow-auto">
                      {JSON.stringify(logSelecionado.detalhes, null, 2)}
                    </pre>
                  </div>
                )}
              </div>
            </div>
          )}
        </DialogContent>
      </Dialog>

      {/* Modal de Relatórios */}
      <Dialog open={relatorioModalOpen} onOpenChange={setRelatorioModalOpen}>
        <DialogContent className="max-w-6xl max-h-[80vh] overflow-y-auto">
          <DialogHeader>
            <DialogTitle>Relatórios e Análises</DialogTitle>
            <DialogDescription>
              Visualize gráficos detalhados e exporte relatórios
            </DialogDescription>
          </DialogHeader>
          <div className="space-y-6">
            <Card>
              <CardHeader>
                <CardTitle>Status por Hora</CardTitle>
              </CardHeader>
              <CardContent>
                <ResponsiveContainer width="100%" height={300}>
                  <BarChart data={dadosStatusPorHora}>
                    <CartesianGrid strokeDasharray="3 3" />
                    <XAxis dataKey="hora" />
                    <YAxis />
                    <Tooltip />
                    <Legend />
                    <Bar dataKey="sucesso" stackId="a" fill="#10B981" name="Sucesso" />
                    <Bar dataKey="erro" stackId="a" fill="#EF4444" name="Erro" />
                    <Bar dataKey="warning" stackId="a" fill="#F59E0B" name="Aviso" />
                  </BarChart>
                </ResponsiveContainer>
              </CardContent>
            </Card>
            
            <div className="flex gap-4">
              <Button onClick={() => exportarLogs('pdf')}>
                <FileText className="h-4 w-4 mr-2" />
                Exportar PDF
              </Button>
              <Button onClick={() => exportarLogs('excel')}>
                <Download className="h-4 w-4 mr-2" />
                Exportar Excel
              </Button>
              <Button onClick={() => exportarLogs('csv')}>
                <Database className="h-4 w-4 mr-2" />
                Exportar CSV
              </Button>
            </div>
          </div>
        </DialogContent>
      </Dialog>
    </div>
  );
};

export default Logs;

