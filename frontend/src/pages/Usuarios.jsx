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
  Plus, 
  Search, 
  Filter, 
  MoreVertical, 
  Calendar, 
  User, 
  Mail,
  Shield,
  Loader2,
  RefreshCw,
  Eye,
  Edit,
  Trash2,
  Copy,
  Share,
  Download,
  Upload,
  ChevronLeft,
  ChevronRight,
  ChevronsLeft,
  ChevronsRight,
  X,
  Check,
  AlertTriangle,
  UserCheck,
  UserX,
  Clock,
  Activity
} from 'lucide-react';
import { formatDate, truncateText } from '../utils';

// Dados mockados de usuários
const mockUsuarios = [
  {
    id: 1,
    nome: 'João Silva',
    email: 'joao.silva@ferreiracosta.com',
    tipoUsuario: 'Admin',
    status: 'Ativo',
    ultimoLogin: '2024-01-29T14:30:00Z',
    totalCursosCriados: 5,
    totalLoginsUltimos30Dias: 28,
    createdAt: '2023-06-15T10:00:00Z',
    updatedAt: '2024-01-29T14:30:00Z',
    avatar: null,
    telefone: '(81) 99999-1234',
    departamento: 'TI',
    cargo: 'Coordenador de Sistemas'
  },
  {
    id: 2,
    nome: 'Maria Santos',
    email: 'maria.santos@ferreiracosta.com',
    tipoUsuario: 'Editor',
    status: 'Ativo',
    ultimoLogin: '2024-01-29T09:15:00Z',
    totalCursosCriados: 12,
    totalLoginsUltimos30Dias: 25,
    createdAt: '2023-08-20T14:20:00Z',
    updatedAt: '2024-01-29T09:15:00Z',
    avatar: null,
    telefone: '(81) 98888-5678',
    departamento: 'Design',
    cargo: 'Designer Instrucional'
  },
  {
    id: 3,
    nome: 'Carlos Oliveira',
    email: 'carlos.oliveira@ferreiracosta.com',
    tipoUsuario: 'Editor',
    status: 'Ativo',
    ultimoLogin: '2024-01-28T16:45:00Z',
    totalCursosCriados: 8,
    totalLoginsUltimos30Dias: 22,
    createdAt: '2023-09-10T11:30:00Z',
    updatedAt: '2024-01-28T16:45:00Z',
    avatar: null,
    telefone: '(81) 97777-9012',
    departamento: 'Engenharia',
    cargo: 'Especialista Técnico'
  },
  {
    id: 4,
    nome: 'Ana Costa',
    email: 'ana.costa@ferreiracosta.com',
    tipoUsuario: 'Editor',
    status: 'Ativo',
    ultimoLogin: '2024-01-29T11:20:00Z',
    totalCursosCriados: 15,
    totalLoginsUltimos30Dias: 30,
    createdAt: '2023-07-05T09:45:00Z',
    updatedAt: '2024-01-29T11:20:00Z',
    avatar: null,
    telefone: '(81) 96666-3456',
    departamento: 'Programação',
    cargo: 'Desenvolvedora Senior'
  },
  {
    id: 5,
    nome: 'Pedro Lima',
    email: 'pedro.lima@ferreiracosta.com',
    tipoUsuario: 'Visualizador',
    status: 'Ativo',
    ultimoLogin: '2024-01-26T13:10:00Z',
    totalCursosCriados: 2,
    totalLoginsUltimos30Dias: 15,
    createdAt: '2023-11-12T15:00:00Z',
    updatedAt: '2024-01-26T13:10:00Z',
    avatar: null,
    telefone: '(81) 95555-7890',
    departamento: 'Idiomas',
    cargo: 'Professor de Inglês'
  },
  {
    id: 6,
    nome: 'Fernanda Costa',
    email: 'fernanda.costa@ferreiracosta.com',
    tipoUsuario: 'Editor',
    status: 'Ativo',
    ultimoLogin: '2024-01-29T08:30:00Z',
    totalCursosCriados: 7,
    totalLoginsUltimos30Dias: 26,
    createdAt: '2023-10-18T12:15:00Z',
    updatedAt: '2024-01-29T08:30:00Z',
    avatar: null,
    telefone: '(81) 94444-2468',
    departamento: 'Marketing',
    cargo: 'Analista de Marketing Digital'
  },
  {
    id: 7,
    nome: 'Lucas Ferreira',
    email: 'lucas.ferreira@ferreiracosta.com',
    tipoUsuario: 'Editor',
    status: 'Inativo',
    ultimoLogin: '2024-01-15T17:20:00Z',
    totalCursosCriados: 3,
    totalLoginsUltimos30Dias: 8,
    createdAt: '2023-12-01T10:30:00Z',
    updatedAt: '2024-01-15T17:20:00Z',
    avatar: null,
    telefone: '(81) 93333-1357',
    departamento: 'Desenvolvimento Web',
    cargo: 'Desenvolvedor Frontend'
  },
  {
    id: 8,
    nome: 'Juliana Alves',
    email: 'juliana.alves@ferreiracosta.com',
    tipoUsuario: 'Visualizador',
    status: 'Ativo',
    ultimoLogin: '2024-01-29T10:45:00Z',
    totalCursosCriados: 0,
    totalLoginsUltimos30Dias: 20,
    createdAt: '2024-01-10T14:00:00Z',
    updatedAt: '2024-01-29T10:45:00Z',
    avatar: null,
    telefone: '(81) 92222-9876',
    departamento: 'RH',
    cargo: 'Analista de Recursos Humanos'
  }
];

const TIPO_USUARIO_CONFIG = {
  'Admin': { color: 'bg-red-100 text-red-800', label: 'Administrador', icon: Shield },
  'Editor': { color: 'bg-blue-100 text-blue-800', label: 'Editor', icon: Edit },
  'Visualizador': { color: 'bg-green-100 text-green-800', label: 'Visualizador', icon: Eye }
};

const STATUS_CONFIG = {
  'Ativo': { color: 'bg-green-100 text-green-800', label: 'Ativo', icon: UserCheck },
  'Inativo': { color: 'bg-red-100 text-red-800', label: 'Inativo', icon: UserX }
};

const Usuarios = () => {
  const [usuarios, setUsuarios] = useState(mockUsuarios);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [searchTerm, setSearchTerm] = useState('');
  const [refreshing, setRefreshing] = useState(false);
  
  // Estados para filtros
  const [showFilters, setShowFilters] = useState(false);
  const [selectedFilters, setSelectedFilters] = useState({
    tipoUsuario: '',
    status: '',
    departamento: ''
  });
  
  // Estados para paginação
  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  
  // Estados para seleção múltipla
  const [selectedUsuarios, setSelectedUsuarios] = useState([]);
  const [selectAll, setSelectAll] = useState(false);
  
  // Estados para modais
  const [usuarioSelecionado, setUsuarioSelecionado] = useState(null);
  const [detalhesModalOpen, setDetalhesModalOpen] = useState(false);
  const [editModalOpen, setEditModalOpen] = useState(false);
  const [deleteModalOpen, setDeleteModalOpen] = useState(false);
  const [createModalOpen, setCreateModalOpen] = useState(false);
  const [bulkDeleteModalOpen, setBulkDeleteModalOpen] = useState(false);

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

  // Filtrar usuários
  const filteredUsuarios = usuarios.filter(usuario => {
    const matchesSearch = usuario.nome.toLowerCase().includes(searchTerm.toLowerCase()) ||
                         usuario.email.toLowerCase().includes(searchTerm.toLowerCase()) ||
                         usuario.departamento.toLowerCase().includes(searchTerm.toLowerCase()) ||
                         usuario.cargo.toLowerCase().includes(searchTerm.toLowerCase());
    
    const matchesTipoUsuario = !selectedFilters.tipoUsuario || usuario.tipoUsuario === selectedFilters.tipoUsuario;
    const matchesStatus = !selectedFilters.status || usuario.status === selectedFilters.status;
    const matchesDepartamento = !selectedFilters.departamento || usuario.departamento === selectedFilters.departamento;
    
    return matchesSearch && matchesTipoUsuario && matchesStatus && matchesDepartamento;
  });

  // Paginação
  const totalPages = Math.ceil(filteredUsuarios.length / pageSize);
  const startIndex = (currentPage - 1) * pageSize;
  const endIndex = startIndex + pageSize;
  const currentUsuarios = filteredUsuarios.slice(startIndex, endIndex);

  // Funções de seleção
  const handleSelectUsuario = (usuarioId) => {
    setSelectedUsuarios(prev => 
      prev.includes(usuarioId) 
        ? prev.filter(id => id !== usuarioId)
        : [...prev, usuarioId]
    );
  };

  const handleSelectAll = () => {
    if (selectAll) {
      setSelectedUsuarios([]);
    } else {
      setSelectedUsuarios(currentUsuarios.map(usuario => usuario.id));
    }
    setSelectAll(!selectAll);
  };

  // Funções de ação
  const abrirDetalhes = (usuario) => {
    setUsuarioSelecionado(usuario);
    setDetalhesModalOpen(true);
  };

  const abrirEdicao = (usuario) => {
    setUsuarioSelecionado(usuario);
    setEditModalOpen(true);
  };

  const abrirExclusao = (usuario) => {
    setUsuarioSelecionado(usuario);
    setDeleteModalOpen(true);
  };

  const confirmarExclusao = () => {
    setUsuarios(prev => prev.filter(u => u.id !== usuarioSelecionado.id));
    setDeleteModalOpen(false);
    setUsuarioSelecionado(null);
  };

  const confirmarExclusaoLote = () => {
    setUsuarios(prev => prev.filter(u => !selectedUsuarios.includes(u.id)));
    setSelectedUsuarios([]);
    setBulkDeleteModalOpen(false);
    setSelectAll(false);
  };

  const alternarStatus = (usuario) => {
    const novoStatus = usuario.status === 'Ativo' ? 'Inativo' : 'Ativo';
    setUsuarios(prev => prev.map(u => 
      u.id === usuario.id ? { ...u, status: novoStatus, updatedAt: new Date().toISOString() } : u
    ));
  };

  const duplicarUsuario = (usuario) => {
    const novoUsuario = {
      ...usuario,
      id: Math.max(...usuarios.map(u => u.id)) + 1,
      nome: `${usuario.nome} (Cópia)`,
      email: `copia.${usuario.email}`,
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString()
    };
    setUsuarios(prev => [novoUsuario, ...prev]);
  };

  const getInitials = (nome) => {
    return nome.split(' ').map(n => n[0]).join('').toUpperCase().slice(0, 2);
  };

  const getDepartamentos = () => {
    return [...new Set(usuarios.map(u => u.departamento))].sort();
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="flex items-center space-x-2">
          <Loader2 className="h-6 w-6 animate-spin text-blue-600" />
          <span className="text-gray-600">Carregando usuários...</span>
        </div>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      {/* Cabeçalho */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">Usuários</h1>
          <p className="text-gray-600 mt-1">
            Gerencie usuários e permissões do sistema
          </p>
        </div>
        
        <div className="flex items-center gap-3">
          {selectedUsuarios.length > 0 && (
            <Button 
              variant="destructive" 
              onClick={() => setBulkDeleteModalOpen(true)}
            >
              <Trash2 className="h-4 w-4 mr-2" />
              Excluir ({selectedUsuarios.length})
            </Button>
          )}
          <Button variant="outline" onClick={handleRefresh} disabled={refreshing}>
            <RefreshCw className={`h-4 w-4 mr-2 ${refreshing ? 'animate-spin' : ''}`} />
            Atualizar
          </Button>
          <Button onClick={() => setCreateModalOpen(true)}>
            <Plus className="h-4 w-4 mr-2" />
            Novo Usuário
          </Button>
        </div>
      </div>

      {/* Cards de estatísticas */}
      <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
        <Card>
          <CardContent className="p-4">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm font-medium text-gray-600">Total de Usuários</p>
                <p className="text-2xl font-bold text-gray-900">{usuarios.length}</p>
              </div>
              <User className="h-8 w-8 text-blue-600" />
            </div>
          </CardContent>
        </Card>
        
        <Card>
          <CardContent className="p-4">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm font-medium text-gray-600">Usuários Ativos</p>
                <p className="text-2xl font-bold text-green-600">
                  {usuarios.filter(u => u.status === 'Ativo').length}
                </p>
              </div>
              <UserCheck className="h-8 w-8 text-green-600" />
            </div>
          </CardContent>
        </Card>
        
        <Card>
          <CardContent className="p-4">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm font-medium text-gray-600">Administradores</p>
                <p className="text-2xl font-bold text-red-600">
                  {usuarios.filter(u => u.tipoUsuario === 'Admin').length}
                </p>
              </div>
              <Shield className="h-8 w-8 text-red-600" />
            </div>
          </CardContent>
        </Card>
        
        <Card>
          <CardContent className="p-4">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm font-medium text-gray-600">Novos (30 dias)</p>
                <p className="text-2xl font-bold text-purple-600">
                  {usuarios.filter(u => {
                    const created = new Date(u.createdAt);
                    const thirtyDaysAgo = new Date();
                    thirtyDaysAgo.setDate(thirtyDaysAgo.getDate() - 30);
                    return created > thirtyDaysAgo;
                  }).length}
                </p>
              </div>
              <Activity className="h-8 w-8 text-purple-600" />
            </div>
          </CardContent>
        </Card>
      </div>

      {/* Barra de busca e filtros */}
      <div className="flex items-center gap-4">
        <div className="relative flex-1">
          <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 h-4 w-4" />
          <Input
            placeholder="Buscar por nome, email, departamento ou cargo..."
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
        <Button variant="outline">
          <Download className="h-4 w-4 mr-2" />
          Exportar
        </Button>
      </div>

      {/* Filtros expandidos */}
      {showFilters && (
        <Card>
          <CardContent className="pt-6">
            <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
              <div>
                <label className="text-sm font-medium text-gray-700 mb-2 block">Tipo de Usuário</label>
                <select 
                  className="w-full p-2 border border-gray-300 rounded-md"
                  value={selectedFilters.tipoUsuario}
                  onChange={(e) => setSelectedFilters(prev => ({ ...prev, tipoUsuario: e.target.value }))}
                >
                  <option value="">Todos</option>
                  <option value="Admin">Administrador</option>
                  <option value="Editor">Editor</option>
                  <option value="Visualizador">Visualizador</option>
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
                  <option value="Ativo">Ativo</option>
                  <option value="Inativo">Inativo</option>
                </select>
              </div>
              <div>
                <label className="text-sm font-medium text-gray-700 mb-2 block">Departamento</label>
                <select 
                  className="w-full p-2 border border-gray-300 rounded-md"
                  value={selectedFilters.departamento}
                  onChange={(e) => setSelectedFilters(prev => ({ ...prev, departamento: e.target.value }))}
                >
                  <option value="">Todos</option>
                  {getDepartamentos().map(dept => (
                    <option key={dept} value={dept}>{dept}</option>
                  ))}
                </select>
              </div>
            </div>
            <div className="flex justify-end mt-4">
              <Button 
                variant="outline" 
                onClick={() => setSelectedFilters({ tipoUsuario: '', status: '', departamento: '' })}
              >
                <X className="h-4 w-4 mr-2" />
                Limpar Filtros
              </Button>
            </div>
          </CardContent>
        </Card>
      )}

      {/* Tabela de usuários */}
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
                <TableHead>Usuário</TableHead>
                <TableHead>Email</TableHead>
                <TableHead>Tipo</TableHead>
                <TableHead>Status</TableHead>
                <TableHead>Departamento</TableHead>
                <TableHead>Cursos Criados</TableHead>
                <TableHead>Último Login</TableHead>
                <TableHead>Logins (30d)</TableHead>
                <TableHead className="w-12">Ações</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {currentUsuarios.length === 0 ? (
                <TableRow>
                  <TableCell colSpan={10} className="text-center py-8 text-gray-500">
                    <User className="h-12 w-12 mx-auto mb-4 text-gray-300" />
                    <p>Nenhum usuário encontrado</p>
                  </TableCell>
                </TableRow>
              ) : (
                currentUsuarios.map((usuario) => (
                  <TableRow key={usuario.id} className="hover:bg-gray-50">
                    <TableCell>
                      <Checkbox 
                        checked={selectedUsuarios.includes(usuario.id)}
                        onCheckedChange={() => handleSelectUsuario(usuario.id)}
                      />
                    </TableCell>
                    <TableCell>
                      <div className="flex items-center gap-3">
                        <div className="w-8 h-8 bg-blue-100 rounded-full flex items-center justify-center">
                          <span className="text-xs font-medium text-blue-600">
                            {getInitials(usuario.nome)}
                          </span>
                        </div>
                        <div>
                          <div className="font-medium text-gray-900">
                            {usuario.nome}
                          </div>
                          <div className="text-sm text-gray-500">
                            {usuario.cargo}
                          </div>
                        </div>
                      </div>
                    </TableCell>
                    <TableCell>
                      <div className="text-sm text-gray-900">
                        {usuario.email}
                      </div>
                    </TableCell>
                    <TableCell>
                      <Badge className={TIPO_USUARIO_CONFIG[usuario.tipoUsuario]?.color}>
                        {TIPO_USUARIO_CONFIG[usuario.tipoUsuario]?.label}
                      </Badge>
                    </TableCell>
                    <TableCell>
                      <Badge className={STATUS_CONFIG[usuario.status]?.color}>
                        {STATUS_CONFIG[usuario.status]?.label}
                      </Badge>
                    </TableCell>
                    <TableCell>
                      <div className="text-sm text-gray-900">
                        {usuario.departamento}
                      </div>
                    </TableCell>
                    <TableCell>
                      <div className="text-sm text-gray-900">
                        {usuario.totalCursosCriados}
                      </div>
                    </TableCell>
                    <TableCell>
                      <div className="text-sm text-gray-900">
                        {formatDate(usuario.ultimoLogin)}
                      </div>
                    </TableCell>
                    <TableCell>
                      <div className="text-sm text-gray-900">
                        {usuario.totalLoginsUltimos30Dias}
                      </div>
                    </TableCell>
                    <TableCell>
                      <DropdownMenu>
                        <DropdownMenuTrigger asChild>
                          <Button variant="ghost" size="sm" className="h-8 w-8 p-0">
                            <MoreVertical className="h-4 w-4" />
                          </Button>
                        </DropdownMenuTrigger>
                        <DropdownMenuContent align="end">
                          <DropdownMenuItem onClick={() => abrirDetalhes(usuario)}>
                            <Eye className="h-4 w-4 mr-2" />
                            Ver Detalhes
                          </DropdownMenuItem>
                          <DropdownMenuItem onClick={() => abrirEdicao(usuario)}>
                            <Edit className="h-4 w-4 mr-2" />
                            Editar
                          </DropdownMenuItem>
                          <DropdownMenuItem onClick={() => alternarStatus(usuario)}>
                            {usuario.status === 'Ativo' ? (
                              <>
                                <UserX className="h-4 w-4 mr-2" />
                                Desativar
                              </>
                            ) : (
                              <>
                                <UserCheck className="h-4 w-4 mr-2" />
                                Ativar
                              </>
                            )}
                          </DropdownMenuItem>
                          <DropdownMenuItem onClick={() => duplicarUsuario(usuario)}>
                            <Copy className="h-4 w-4 mr-2" />
                            Duplicar
                          </DropdownMenuItem>
                          <DropdownMenuSeparator />
                          <DropdownMenuItem 
                            className="text-red-600"
                            onClick={() => abrirExclusao(usuario)}
                          >
                            <Trash2 className="h-4 w-4 mr-2" />
                            Excluir
                          </DropdownMenuItem>
                        </DropdownMenuContent>
                      </DropdownMenu>
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
            Mostrando {startIndex + 1} a {Math.min(endIndex, filteredUsuarios.length)} de {filteredUsuarios.length} usuários
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
            <DialogTitle>Detalhes do Usuário</DialogTitle>
            <DialogDescription>
              Informações completas sobre o usuário selecionado
            </DialogDescription>
          </DialogHeader>
          {usuarioSelecionado && (
            <div className="space-y-6">
              <div className="flex items-center gap-4">
                <div className="w-16 h-16 bg-blue-100 rounded-full flex items-center justify-center">
                  <span className="text-xl font-medium text-blue-600">
                    {getInitials(usuarioSelecionado.nome)}
                  </span>
                </div>
                <div>
                  <h3 className="text-xl font-semibold text-gray-900">
                    {usuarioSelecionado.nome}
                  </h3>
                  <p className="text-gray-600">{usuarioSelecionado.cargo}</p>
                  <div className="flex items-center gap-2 mt-1">
                    <Badge className={TIPO_USUARIO_CONFIG[usuarioSelecionado.tipoUsuario]?.color}>
                      {TIPO_USUARIO_CONFIG[usuarioSelecionado.tipoUsuario]?.label}
                    </Badge>
                    <Badge className={STATUS_CONFIG[usuarioSelecionado.status]?.color}>
                      {STATUS_CONFIG[usuarioSelecionado.status]?.label}
                    </Badge>
                  </div>
                </div>
              </div>
              
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="text-sm font-medium text-gray-700">Email</label>
                  <p className="text-sm text-gray-900 bg-gray-50 p-2 rounded">
                    {usuarioSelecionado.email}
                  </p>
                </div>
                <div>
                  <label className="text-sm font-medium text-gray-700">Telefone</label>
                  <p className="text-sm text-gray-900 bg-gray-50 p-2 rounded">
                    {usuarioSelecionado.telefone}
                  </p>
                </div>
                <div>
                  <label className="text-sm font-medium text-gray-700">Departamento</label>
                  <p className="text-sm text-gray-900 bg-gray-50 p-2 rounded">
                    {usuarioSelecionado.departamento}
                  </p>
                </div>
                <div>
                  <label className="text-sm font-medium text-gray-700">Cargo</label>
                  <p className="text-sm text-gray-900 bg-gray-50 p-2 rounded">
                    {usuarioSelecionado.cargo}
                  </p>
                </div>
                <div>
                  <label className="text-sm font-medium text-gray-700">Cursos Criados</label>
                  <p className="text-sm text-gray-900 bg-gray-50 p-2 rounded">
                    {usuarioSelecionado.totalCursosCriados}
                  </p>
                </div>
                <div>
                  <label className="text-sm font-medium text-gray-700">Logins (30 dias)</label>
                  <p className="text-sm text-gray-900 bg-gray-50 p-2 rounded">
                    {usuarioSelecionado.totalLoginsUltimos30Dias}
                  </p>
                </div>
                <div>
                  <label className="text-sm font-medium text-gray-700">Último Login</label>
                  <p className="text-sm text-gray-900 bg-gray-50 p-2 rounded">
                    {formatDate(usuarioSelecionado.ultimoLogin)}
                  </p>
                </div>
                <div>
                  <label className="text-sm font-medium text-gray-700">Criado em</label>
                  <p className="text-sm text-gray-900 bg-gray-50 p-2 rounded">
                    {formatDate(usuarioSelecionado.createdAt)}
                  </p>
                </div>
              </div>
            </div>
          )}
        </DialogContent>
      </Dialog>

      {/* Modal de Exclusão */}
      <Dialog open={deleteModalOpen} onOpenChange={setDeleteModalOpen}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle className="flex items-center gap-2 text-red-600">
              <AlertTriangle className="h-5 w-5" />
              Confirmar Exclusão
            </DialogTitle>
            <DialogDescription>
              Tem certeza que deseja excluir o usuário "{usuarioSelecionado?.nome}"?
              Esta ação não pode ser desfeita.
            </DialogDescription>
          </DialogHeader>
          <DialogFooter>
            <Button variant="outline" onClick={() => setDeleteModalOpen(false)}>
              Cancelar
            </Button>
            <Button variant="destructive" onClick={confirmarExclusao}>
              <Trash2 className="h-4 w-4 mr-2" />
              Excluir
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>

      {/* Modal de Exclusão em Lote */}
      <Dialog open={bulkDeleteModalOpen} onOpenChange={setBulkDeleteModalOpen}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle className="flex items-center gap-2 text-red-600">
              <AlertTriangle className="h-5 w-5" />
              Confirmar Exclusão em Lote
            </DialogTitle>
            <DialogDescription>
              Tem certeza que deseja excluir {selectedUsuarios.length} usuário(s) selecionado(s)?
              Esta ação não pode ser desfeita.
            </DialogDescription>
          </DialogHeader>
          <DialogFooter>
            <Button variant="outline" onClick={() => setBulkDeleteModalOpen(false)}>
              Cancelar
            </Button>
            <Button variant="destructive" onClick={confirmarExclusaoLote}>
              <Trash2 className="h-4 w-4 mr-2" />
              Excluir {selectedUsuarios.length} usuário(s)
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  );
};

export default Usuarios;

