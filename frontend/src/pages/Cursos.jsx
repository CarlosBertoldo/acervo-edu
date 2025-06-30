import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
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
  FileText,
  Building,
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
  AlertTriangle
} from 'lucide-react';
import { formatDate, truncateText } from '../utils';

// Dados mockados expandidos para demonstração
const mockCursos = [
  {
    id: 1,
    nomeCurso: 'Excel Avançado para Gestores',
    codigoCurso: 'EXC-ADV-001',
    descricaoAcademia: 'Academia de Tecnologia',
    status: 'Backlog',
    tipoAmbiente: 'Presencial',
    tipoAcesso: 'Restrito',
    dataInicioOperacao: '2024-02-15',
    origem: 'Manual',
    criadoPor: 'João Silva',
    comentariosInternos: 'Curso solicitado pela diretoria para capacitação de gestores',
    totalArquivos: 0,
    progresso: 0,
    createdAt: '2024-01-15T10:00:00Z',
    updatedAt: '2024-01-15T10:00:00Z'
  },
  {
    id: 2,
    nomeCurso: 'PowerBI Dashboard Executivo',
    codigoCurso: 'PBI-DASH-002',
    descricaoAcademia: 'Academia de Business Intelligence',
    status: 'Em Desenvolvimento',
    tipoAmbiente: 'Online',
    tipoAcesso: 'Público',
    dataInicioOperacao: '2024-01-20',
    origem: 'Senior',
    criadoPor: null,
    comentariosInternos: 'Integração com dados do ERP Senior em andamento',
    totalArquivos: 8,
    progresso: 65,
    createdAt: '2024-01-10T14:30:00Z',
    updatedAt: '2024-01-25T16:45:00Z'
  },
  {
    id: 3,
    nomeCurso: 'Photoshop Básico para Marketing',
    codigoCurso: 'PS-BAS-003',
    descricaoAcademia: 'Academia de Design',
    status: 'Backlog',
    tipoAmbiente: 'Híbrido',
    tipoAcesso: 'Restrito',
    dataInicioOperacao: '2024-03-01',
    origem: 'Manual',
    criadoPor: 'Maria Santos',
    comentariosInternos: 'Aguardando aprovação do orçamento para licenças',
    totalArquivos: 0,
    progresso: 0,
    createdAt: '2024-01-20T09:15:00Z',
    updatedAt: '2024-01-20T09:15:00Z'
  },
  {
    id: 4,
    nomeCurso: 'AutoCAD 2024 Intermediário',
    codigoCurso: 'CAD-INT-004',
    descricaoAcademia: 'Academia de Engenharia',
    status: 'Em Desenvolvimento',
    tipoAmbiente: 'Presencial',
    tipoAcesso: 'Restrito',
    dataInicioOperacao: '2024-02-01',
    origem: 'Manual',
    criadoPor: 'Carlos Oliveira',
    comentariosInternos: 'Módulos 1 e 2 já finalizados, trabalhando no módulo 3',
    totalArquivos: 12,
    progresso: 45,
    createdAt: '2024-01-05T11:20:00Z',
    updatedAt: '2024-01-28T14:10:00Z'
  },
  {
    id: 5,
    nomeCurso: 'Gestão de Projetos com MS Project',
    codigoCurso: 'GP-MSP-005',
    descricaoAcademia: 'Academia de Gestão',
    status: 'Veiculado',
    tipoAmbiente: 'Online',
    tipoAcesso: 'Público',
    dataInicioOperacao: '2024-01-15',
    origem: 'Senior',
    criadoPor: null,
    comentariosInternos: 'Curso finalizado e disponível na plataforma',
    totalArquivos: 15,
    progresso: 100,
    createdAt: '2023-12-10T08:00:00Z',
    updatedAt: '2024-01-15T17:30:00Z'
  },
  {
    id: 6,
    nomeCurso: 'Python para Análise de Dados',
    codigoCurso: 'PY-DATA-006',
    descricaoAcademia: 'Academia de Programação',
    status: 'Veiculado',
    tipoAmbiente: 'Online',
    tipoAcesso: 'Público',
    dataInicioOperacao: '2024-01-08',
    origem: 'Manual',
    criadoPor: 'Ana Costa',
    comentariosInternos: 'Curso muito bem avaliado pelos alunos',
    totalArquivos: 22,
    progresso: 100,
    createdAt: '2023-11-15T13:45:00Z',
    updatedAt: '2024-01-08T10:20:00Z'
  },
  {
    id: 7,
    nomeCurso: 'Inglês Técnico para TI',
    codigoCurso: 'ENG-TEC-007',
    descricaoAcademia: 'Academia de Idiomas',
    status: 'Backlog',
    tipoAmbiente: 'Híbrido',
    tipoAcesso: 'Restrito',
    dataInicioOperacao: '2024-04-01',
    origem: 'Manual',
    criadoPor: 'Pedro Lima',
    comentariosInternos: 'Aguardando contratação de professor especializado',
    totalArquivos: 0,
    progresso: 0,
    createdAt: '2024-01-22T15:30:00Z',
    updatedAt: '2024-01-22T15:30:00Z'
  },
  {
    id: 8,
    nomeCurso: 'Segurança da Informação Básica',
    codigoCurso: 'SEC-BAS-008',
    descricaoAcademia: 'Academia de Segurança',
    status: 'Em Desenvolvimento',
    tipoAmbiente: 'Online',
    tipoAcesso: 'Público',
    dataInicioOperacao: '2024-02-10',
    origem: 'Senior',
    criadoPor: null,
    comentariosInternos: 'Conteúdo sendo revisado pelo departamento jurídico',
    totalArquivos: 6,
    progresso: 30,
    createdAt: '2024-01-12T12:00:00Z',
    updatedAt: '2024-01-26T09:45:00Z'
  },
  {
    id: 9,
    nomeCurso: 'React.js Fundamentals',
    codigoCurso: 'REACT-FUND-009',
    descricaoAcademia: 'Academia de Desenvolvimento Web',
    status: 'Veiculado',
    tipoAmbiente: 'Online',
    tipoAcesso: 'Público',
    dataInicioOperacao: '2024-01-05',
    origem: 'Manual',
    criadoPor: 'Lucas Ferreira',
    comentariosInternos: 'Curso com alta demanda, considerar versão avançada',
    totalArquivos: 18,
    progresso: 100,
    createdAt: '2023-12-01T09:00:00Z',
    updatedAt: '2024-01-05T14:20:00Z'
  },
  {
    id: 10,
    nomeCurso: 'Marketing Digital Estratégico',
    codigoCurso: 'MKT-DIG-010',
    descricaoAcademia: 'Academia de Marketing',
    status: 'Em Desenvolvimento',
    tipoAmbiente: 'Híbrido',
    tipoAcesso: 'Restrito',
    dataInicioOperacao: '2024-02-20',
    origem: 'Manual',
    criadoPor: 'Fernanda Costa',
    comentariosInternos: 'Aguardando aprovação de cases de clientes reais',
    totalArquivos: 7,
    progresso: 25,
    createdAt: '2024-01-18T16:45:00Z',
    updatedAt: '2024-01-30T11:30:00Z'
  }
];

const STATUS_CONFIG = {
  'Backlog': { color: 'bg-gray-100 text-gray-800', label: 'Backlog' },
  'Em Desenvolvimento': { color: 'bg-blue-100 text-blue-800', label: 'Em Desenvolvimento' },
  'Veiculado': { color: 'bg-green-100 text-green-800', label: 'Veiculado' }
};

const ORIGEM_CONFIG = {
  'Manual': { color: 'bg-blue-100 text-blue-800', label: 'Manual' },
  'Senior': { color: 'bg-purple-100 text-purple-800', label: 'Senior' }
};

const TIPO_AMBIENTE_CONFIG = {
  'Presencial': { color: 'bg-orange-100 text-orange-800', label: 'Presencial' },
  'Online': { color: 'bg-green-100 text-green-800', label: 'Online' },
  'Híbrido': { color: 'bg-yellow-100 text-yellow-800', label: 'Híbrido' }
};

const TIPO_ACESSO_CONFIG = {
  'Público': { color: 'bg-green-100 text-green-800', label: 'Público' },
  'Restrito': { color: 'bg-red-100 text-red-800', label: 'Restrito' }
};

const Cursos = () => {
  const [cursos, setCursos] = useState(mockCursos);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [searchTerm, setSearchTerm] = useState('');
  const [refreshing, setRefreshing] = useState(false);
  
  // Estados para filtros
  const [showFilters, setShowFilters] = useState(false);
  const [selectedFilters, setSelectedFilters] = useState({
    status: '',
    origem: '',
    tipoAmbiente: '',
    tipoAcesso: ''
  });
  
  // Estados para paginação
  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  
  // Estados para seleção múltipla
  const [selectedCursos, setSelectedCursos] = useState([]);
  const [selectAll, setSelectAll] = useState(false);
  
  // Estados para modais
  const [cursoSelecionado, setCursoSelecionado] = useState(null);
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

  // Filtrar cursos
  const filteredCursos = cursos.filter(curso => {
    const matchesSearch = curso.nomeCurso.toLowerCase().includes(searchTerm.toLowerCase()) ||
                         curso.codigoCurso.toLowerCase().includes(searchTerm.toLowerCase()) ||
                         curso.descricaoAcademia.toLowerCase().includes(searchTerm.toLowerCase()) ||
                         (curso.criadoPor && curso.criadoPor.toLowerCase().includes(searchTerm.toLowerCase()));
    
    const matchesStatus = !selectedFilters.status || curso.status === selectedFilters.status;
    const matchesOrigem = !selectedFilters.origem || curso.origem === selectedFilters.origem;
    const matchesTipoAmbiente = !selectedFilters.tipoAmbiente || curso.tipoAmbiente === selectedFilters.tipoAmbiente;
    const matchesTipoAcesso = !selectedFilters.tipoAcesso || curso.tipoAcesso === selectedFilters.tipoAcesso;
    
    return matchesSearch && matchesStatus && matchesOrigem && matchesTipoAmbiente && matchesTipoAcesso;
  });

  // Paginação
  const totalPages = Math.ceil(filteredCursos.length / pageSize);
  const startIndex = (currentPage - 1) * pageSize;
  const endIndex = startIndex + pageSize;
  const currentCursos = filteredCursos.slice(startIndex, endIndex);

  // Funções de seleção
  const handleSelectCurso = (cursoId) => {
    setSelectedCursos(prev => 
      prev.includes(cursoId) 
        ? prev.filter(id => id !== cursoId)
        : [...prev, cursoId]
    );
  };

  const handleSelectAll = () => {
    if (selectAll) {
      setSelectedCursos([]);
    } else {
      setSelectedCursos(currentCursos.map(curso => curso.id));
    }
    setSelectAll(!selectAll);
  };

  // Funções de ação
  const abrirDetalhes = (curso) => {
    setCursoSelecionado(curso);
    setDetalhesModalOpen(true);
  };

  const abrirEdicao = (curso) => {
    setCursoSelecionado(curso);
    setEditModalOpen(true);
  };

  const abrirExclusao = (curso) => {
    setCursoSelecionado(curso);
    setDeleteModalOpen(true);
  };

  const confirmarExclusao = () => {
    setCursos(prev => prev.filter(c => c.id !== cursoSelecionado.id));
    setDeleteModalOpen(false);
    setCursoSelecionado(null);
  };

  const confirmarExclusaoLote = () => {
    setCursos(prev => prev.filter(c => !selectedCursos.includes(c.id)));
    setSelectedCursos([]);
    setBulkDeleteModalOpen(false);
    setSelectAll(false);
  };

  const duplicarCurso = (curso) => {
    const novoCurso = {
      ...curso,
      id: Math.max(...cursos.map(c => c.id)) + 1,
      nomeCurso: `${curso.nomeCurso} (Cópia)`,
      codigoCurso: `${curso.codigoCurso}-COPY`,
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString()
    };
    setCursos(prev => [novoCurso, ...prev]);
  };

  const getProgressColor = (progresso) => {
    if (progresso === 0) return 'bg-gray-300';
    if (progresso < 30) return 'bg-red-500';
    if (progresso < 70) return 'bg-yellow-500';
    return 'bg-green-500';
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="flex items-center space-x-2">
          <Loader2 className="h-6 w-6 animate-spin text-blue-600" />
          <span className="text-gray-600">Carregando cursos...</span>
        </div>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      {/* Cabeçalho */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">Cursos</h1>
          <p className="text-gray-600 mt-1">
            Gerencie todos os cursos do sistema
          </p>
        </div>
        
        <div className="flex items-center gap-3">
          {selectedCursos.length > 0 && (
            <Button 
              variant="destructive" 
              onClick={() => setBulkDeleteModalOpen(true)}
            >
              <Trash2 className="h-4 w-4 mr-2" />
              Excluir ({selectedCursos.length})
            </Button>
          )}
          <Button variant="outline" onClick={handleRefresh} disabled={refreshing}>
            <RefreshCw className={`h-4 w-4 mr-2 ${refreshing ? 'animate-spin' : ''}`} />
            Atualizar
          </Button>
          <Button onClick={() => setCreateModalOpen(true)}>
            <Plus className="h-4 w-4 mr-2" />
            Novo Curso
          </Button>
        </div>
      </div>

      {/* Barra de busca e filtros */}
      <div className="flex items-center gap-4">
        <div className="relative flex-1">
          <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 h-4 w-4" />
          <Input
            placeholder="Buscar por nome, código, academia ou criador..."
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
            <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
              <div>
                <label className="text-sm font-medium text-gray-700 mb-2 block">Status</label>
                <select 
                  className="w-full p-2 border border-gray-300 rounded-md"
                  value={selectedFilters.status}
                  onChange={(e) => setSelectedFilters(prev => ({ ...prev, status: e.target.value }))}
                >
                  <option value="">Todos</option>
                  <option value="Backlog">Backlog</option>
                  <option value="Em Desenvolvimento">Em Desenvolvimento</option>
                  <option value="Veiculado">Veiculado</option>
                </select>
              </div>
              <div>
                <label className="text-sm font-medium text-gray-700 mb-2 block">Origem</label>
                <select 
                  className="w-full p-2 border border-gray-300 rounded-md"
                  value={selectedFilters.origem}
                  onChange={(e) => setSelectedFilters(prev => ({ ...prev, origem: e.target.value }))}
                >
                  <option value="">Todas</option>
                  <option value="Manual">Manual</option>
                  <option value="Senior">Senior</option>
                </select>
              </div>
              <div>
                <label className="text-sm font-medium text-gray-700 mb-2 block">Tipo de Ambiente</label>
                <select 
                  className="w-full p-2 border border-gray-300 rounded-md"
                  value={selectedFilters.tipoAmbiente}
                  onChange={(e) => setSelectedFilters(prev => ({ ...prev, tipoAmbiente: e.target.value }))}
                >
                  <option value="">Todos</option>
                  <option value="Presencial">Presencial</option>
                  <option value="Online">Online</option>
                  <option value="Híbrido">Híbrido</option>
                </select>
              </div>
              <div>
                <label className="text-sm font-medium text-gray-700 mb-2 block">Tipo de Acesso</label>
                <select 
                  className="w-full p-2 border border-gray-300 rounded-md"
                  value={selectedFilters.tipoAcesso}
                  onChange={(e) => setSelectedFilters(prev => ({ ...prev, tipoAcesso: e.target.value }))}
                >
                  <option value="">Todos</option>
                  <option value="Público">Público</option>
                  <option value="Restrito">Restrito</option>
                </select>
              </div>
            </div>
            <div className="flex justify-end mt-4">
              <Button 
                variant="outline" 
                onClick={() => setSelectedFilters({ status: '', origem: '', tipoAmbiente: '', tipoAcesso: '' })}
              >
                <X className="h-4 w-4 mr-2" />
                Limpar Filtros
              </Button>
            </div>
          </CardContent>
        </Card>
      )}

      {/* Tabela de cursos */}
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
                <TableHead>Curso</TableHead>
                <TableHead>Academia</TableHead>
                <TableHead>Status</TableHead>
                <TableHead>Origem</TableHead>
                <TableHead>Ambiente</TableHead>
                <TableHead>Acesso</TableHead>
                <TableHead>Progresso</TableHead>
                <TableHead>Arquivos</TableHead>
                <TableHead>Data Início</TableHead>
                <TableHead>Criado por</TableHead>
                <TableHead className="w-12">Ações</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {currentCursos.length === 0 ? (
                <TableRow>
                  <TableCell colSpan={12} className="text-center py-8 text-gray-500">
                    <FileText className="h-12 w-12 mx-auto mb-4 text-gray-300" />
                    <p>Nenhum curso encontrado</p>
                  </TableCell>
                </TableRow>
              ) : (
                currentCursos.map((curso) => (
                  <TableRow key={curso.id} className="hover:bg-gray-50">
                    <TableCell>
                      <Checkbox 
                        checked={selectedCursos.includes(curso.id)}
                        onCheckedChange={() => handleSelectCurso(curso.id)}
                      />
                    </TableCell>
                    <TableCell>
                      <div>
                        <div className="font-medium text-gray-900">
                          {truncateText(curso.nomeCurso, 40)}
                        </div>
                        <div className="text-sm text-gray-500">
                          {curso.codigoCurso}
                        </div>
                      </div>
                    </TableCell>
                    <TableCell>
                      <div className="text-sm text-gray-900">
                        {truncateText(curso.descricaoAcademia, 30)}
                      </div>
                    </TableCell>
                    <TableCell>
                      <Badge className={STATUS_CONFIG[curso.status]?.color}>
                        {STATUS_CONFIG[curso.status]?.label}
                      </Badge>
                    </TableCell>
                    <TableCell>
                      <Badge variant="outline" className={ORIGEM_CONFIG[curso.origem]?.color}>
                        {ORIGEM_CONFIG[curso.origem]?.label}
                      </Badge>
                    </TableCell>
                    <TableCell>
                      <Badge variant="outline" className={TIPO_AMBIENTE_CONFIG[curso.tipoAmbiente]?.color}>
                        {TIPO_AMBIENTE_CONFIG[curso.tipoAmbiente]?.label}
                      </Badge>
                    </TableCell>
                    <TableCell>
                      <Badge variant="outline" className={TIPO_ACESSO_CONFIG[curso.tipoAcesso]?.color}>
                        {TIPO_ACESSO_CONFIG[curso.tipoAcesso]?.label}
                      </Badge>
                    </TableCell>
                    <TableCell>
                      <div className="flex items-center gap-2">
                        <div className="w-16 bg-gray-200 rounded-full h-2">
                          <div 
                            className={`h-2 rounded-full ${getProgressColor(curso.progresso)}`}
                            style={{ width: `${curso.progresso}%` }}
                          ></div>
                        </div>
                        <span className="text-xs text-gray-600">{curso.progresso}%</span>
                      </div>
                    </TableCell>
                    <TableCell>
                      <div className="flex items-center gap-1 text-sm text-gray-600">
                        <FileText className="h-3 w-3" />
                        <span>{curso.totalArquivos}</span>
                      </div>
                    </TableCell>
                    <TableCell>
                      <div className="text-sm text-gray-900">
                        {formatDate(curso.dataInicioOperacao)}
                      </div>
                    </TableCell>
                    <TableCell>
                      <div className="text-sm text-gray-900">
                        {curso.criadoPor || '-'}
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
                          <DropdownMenuItem onClick={() => abrirDetalhes(curso)}>
                            <Eye className="h-4 w-4 mr-2" />
                            Ver Detalhes
                          </DropdownMenuItem>
                          <DropdownMenuItem onClick={() => abrirEdicao(curso)}>
                            <Edit className="h-4 w-4 mr-2" />
                            Editar
                          </DropdownMenuItem>
                          <DropdownMenuItem onClick={() => duplicarCurso(curso)}>
                            <Copy className="h-4 w-4 mr-2" />
                            Duplicar
                          </DropdownMenuItem>
                          <DropdownMenuItem>
                            <Share className="h-4 w-4 mr-2" />
                            Compartilhar
                          </DropdownMenuItem>
                          <DropdownMenuSeparator />
                          <DropdownMenuItem 
                            className="text-red-600"
                            onClick={() => abrirExclusao(curso)}
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
            Mostrando {startIndex + 1} a {Math.min(endIndex, filteredCursos.length)} de {filteredCursos.length} cursos
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
            <DialogTitle>Detalhes do Curso</DialogTitle>
            <DialogDescription>
              Informações completas sobre o curso selecionado
            </DialogDescription>
          </DialogHeader>
          {cursoSelecionado && (
            <div className="space-y-6">
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="text-sm font-medium text-gray-700">Nome do Curso</label>
                  <p className="text-sm text-gray-900 bg-gray-50 p-2 rounded">
                    {cursoSelecionado.nomeCurso}
                  </p>
                </div>
                <div>
                  <label className="text-sm font-medium text-gray-700">Código</label>
                  <p className="text-sm text-gray-900 bg-gray-50 p-2 rounded">
                    {cursoSelecionado.codigoCurso}
                  </p>
                </div>
                <div>
                  <label className="text-sm font-medium text-gray-700">Academia</label>
                  <p className="text-sm text-gray-900 bg-gray-50 p-2 rounded">
                    {cursoSelecionado.descricaoAcademia}
                  </p>
                </div>
                <div>
                  <label className="text-sm font-medium text-gray-700">Status</label>
                  <div className="mt-1">
                    <Badge className={STATUS_CONFIG[cursoSelecionado.status]?.color}>
                      {STATUS_CONFIG[cursoSelecionado.status]?.label}
                    </Badge>
                  </div>
                </div>
                <div>
                  <label className="text-sm font-medium text-gray-700">Tipo de Ambiente</label>
                  <div className="mt-1">
                    <Badge variant="outline" className={TIPO_AMBIENTE_CONFIG[cursoSelecionado.tipoAmbiente]?.color}>
                      {TIPO_AMBIENTE_CONFIG[cursoSelecionado.tipoAmbiente]?.label}
                    </Badge>
                  </div>
                </div>
                <div>
                  <label className="text-sm font-medium text-gray-700">Tipo de Acesso</label>
                  <div className="mt-1">
                    <Badge variant="outline" className={TIPO_ACESSO_CONFIG[cursoSelecionado.tipoAcesso]?.color}>
                      {TIPO_ACESSO_CONFIG[cursoSelecionado.tipoAcesso]?.label}
                    </Badge>
                  </div>
                </div>
                <div>
                  <label className="text-sm font-medium text-gray-700">Data de Início</label>
                  <p className="text-sm text-gray-900 bg-gray-50 p-2 rounded">
                    {formatDate(cursoSelecionado.dataInicioOperacao)}
                  </p>
                </div>
                <div>
                  <label className="text-sm font-medium text-gray-700">Origem</label>
                  <div className="mt-1">
                    <Badge variant="outline" className={ORIGEM_CONFIG[cursoSelecionado.origem]?.color}>
                      {ORIGEM_CONFIG[cursoSelecionado.origem]?.label}
                    </Badge>
                  </div>
                </div>
                {cursoSelecionado.criadoPor && (
                  <div>
                    <label className="text-sm font-medium text-gray-700">Criado por</label>
                    <p className="text-sm text-gray-900 bg-gray-50 p-2 rounded">
                      {cursoSelecionado.criadoPor}
                    </p>
                  </div>
                )}
                <div>
                  <label className="text-sm font-medium text-gray-700">Total de Arquivos</label>
                  <p className="text-sm text-gray-900 bg-gray-50 p-2 rounded">
                    {cursoSelecionado.totalArquivos}
                  </p>
                </div>
                <div>
                  <label className="text-sm font-medium text-gray-700">Progresso</label>
                  <div className="mt-1">
                    <div className="flex items-center gap-2">
                      <div className="flex-1 bg-gray-200 rounded-full h-2">
                        <div 
                          className={`h-2 rounded-full ${getProgressColor(cursoSelecionado.progresso)}`}
                          style={{ width: `${cursoSelecionado.progresso}%` }}
                        ></div>
                      </div>
                      <span className="text-sm text-gray-600">{cursoSelecionado.progresso}%</span>
                    </div>
                  </div>
                </div>
                <div>
                  <label className="text-sm font-medium text-gray-700">Criado em</label>
                  <p className="text-sm text-gray-900 bg-gray-50 p-2 rounded">
                    {formatDate(cursoSelecionado.createdAt)}
                  </p>
                </div>
              </div>
              {cursoSelecionado.comentariosInternos && (
                <div>
                  <label className="text-sm font-medium text-gray-700">Comentários Internos</label>
                  <p className="text-sm text-gray-900 bg-gray-50 p-3 rounded-md mt-1">
                    {cursoSelecionado.comentariosInternos}
                  </p>
                </div>
              )}
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
              Tem certeza que deseja excluir o curso "{cursoSelecionado?.nomeCurso}"?
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
              Tem certeza que deseja excluir {selectedCursos.length} curso(s) selecionado(s)?
              Esta ação não pode ser desfeita.
            </DialogDescription>
          </DialogHeader>
          <DialogFooter>
            <Button variant="outline" onClick={() => setBulkDeleteModalOpen(false)}>
              Cancelar
            </Button>
            <Button variant="destructive" onClick={confirmarExclusaoLote}>
              <Trash2 className="h-4 w-4 mr-2" />
              Excluir {selectedCursos.length} curso(s)
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  );
};

export default Cursos;

