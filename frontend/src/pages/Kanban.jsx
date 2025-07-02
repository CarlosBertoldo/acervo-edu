import React, { useState, useEffect } from 'react';
import { DragDropContext, Droppable, Draggable } from 'react-beautiful-dnd';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import { Alert, AlertDescription } from '@/components/ui/alert';
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
  Folder,
  Eye,
  Edit,
  Trash2,
  Copy,
  Share,
  Settings,
  Clock,
  Users,
  Tag
} from 'lucide-react';
import { 
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
  DropdownMenuSeparator,
} from '@/components/ui/dropdown-menu';
import { 
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from '@/components/ui/dialog';
import ArquivosModal from '../components/ArquivosModal';
import { formatDate, truncateText } from '../utils';

// Dados mockados para demonstração
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
  }
];

const STATUS_CONFIG = {
  'Backlog': {
    label: 'Backlog',
    color: 'bg-ferreira-red-100 border-ferreira-red-300',
    headerColor: 'bg-ferreira-red-50',
    badgeColor: 'bg-ferreira-red-200 text-ferreira-red-700',
    icon: Clock
  },
  'Em Desenvolvimento': {
    label: 'Em Desenvolvimento',
    color: 'bg-ferreira-orange-100 border-ferreira-orange-300',
    headerColor: 'bg-ferreira-orange-50',
    badgeColor: 'bg-ferreira-orange-200 text-ferreira-orange-700',
    icon: Settings
  },
  'Veiculado': {
    label: 'Veiculado',
    color: 'bg-ferreira-green-100 border-ferreira-green-300',
    headerColor: 'bg-ferreira-green-50',
    badgeColor: 'bg-ferreira-green-200 text-ferreira-green-700',
    icon: Users
  }
};

const Kanban = () => {
  const [cursos, setCursos] = useState(mockCursos);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [searchTerm, setSearchTerm] = useState('');
  const [refreshing, setRefreshing] = useState(false);
  const [showFilters, setShowFilters] = useState(false);
  const [selectedFilters, setSelectedFilters] = useState({
    origem: '',
    tipoAmbiente: '',
    tipoAcesso: ''
  });
  
  // Estados para modais
  const [arquivosModalOpen, setArquivosModalOpen] = useState(false);
  const [cursoSelecionado, setCursoSelecionado] = useState(null);
  const [detalhesModalOpen, setDetalhesModalOpen] = useState(false);

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
                         curso.descricaoAcademia.toLowerCase().includes(searchTerm.toLowerCase());
    
    const matchesOrigem = !selectedFilters.origem || curso.origem === selectedFilters.origem;
    const matchesTipoAmbiente = !selectedFilters.tipoAmbiente || curso.tipoAmbiente === selectedFilters.tipoAmbiente;
    const matchesTipoAcesso = !selectedFilters.tipoAcesso || curso.tipoAcesso === selectedFilters.tipoAcesso;
    
    return matchesSearch && matchesOrigem && matchesTipoAmbiente && matchesTipoAcesso;
  });

  // Agrupar cursos por status
  const cursosPorStatus = {
    'Backlog': filteredCursos.filter(c => c.status === 'Backlog'),
    'Em Desenvolvimento': filteredCursos.filter(c => c.status === 'Em Desenvolvimento'),
    'Veiculado': filteredCursos.filter(c => c.status === 'Veiculado')
  };

  // Função para drag and drop
  const onDragEnd = (result) => {
    const { destination, source, draggableId } = result;

    if (!destination) return;

    if (destination.droppableId === source.droppableId && destination.index === source.index) {
      return;
    }

    const cursoId = parseInt(draggableId);
    const novoStatus = destination.droppableId;

    // Atualizar estado local
    setCursos(prev => prev.map(curso => 
      curso.id === cursoId ? { ...curso, status: novoStatus } : curso
    ));

    // Aqui você faria a chamada para a API
    console.log(`Movendo curso ${cursoId} para ${novoStatus}`);
  };

  const abrirArquivos = (curso) => {
    setCursoSelecionado(curso);
    setArquivosModalOpen(true);
  };

  const abrirDetalhes = (curso) => {
    setCursoSelecionado(curso);
    setDetalhesModalOpen(true);
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
          <span className="text-gray-600">Carregando kanban...</span>
        </div>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      {/* Cabeçalho */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">Kanban de Cursos</h1>
          <p className="text-gray-600 mt-1">
            Gerencie o fluxo de desenvolvimento dos cursos
          </p>
        </div>
        
        <div className="flex items-center gap-3">
          <Button variant="outline" onClick={handleRefresh} disabled={refreshing}>
            <RefreshCw className={`h-4 w-4 mr-2 ${refreshing ? 'animate-spin' : ''}`} />
            Atualizar
          </Button>
          <Button>
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
            placeholder="Buscar cursos..."
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
      </div>

      {/* Filtros expandidos */}
      {showFilters && (
        <Card>
          <CardContent className="pt-6">
            <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
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
          </CardContent>
        </Card>
      )}

      {/* Kanban Board */}
      <DragDropContext onDragEnd={onDragEnd}>
        <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
          {Object.entries(STATUS_CONFIG).map(([status, config]) => {
            const cursosStatus = cursosPorStatus[status] || [];
            const IconComponent = config.icon;
            
            return (
              <div key={status} className={`rounded-lg border-2 ${config.color} min-h-[600px]`}>
                {/* Header da coluna */}
                <div className={`p-4 ${config.headerColor} rounded-t-lg border-b`}>
                  <div className="flex items-center justify-between">
                    <div className="flex items-center gap-2">
                      <IconComponent className="h-5 w-5 text-gray-600" />
                      <h3 className="font-semibold text-gray-900">{config.label}</h3>
                      <Badge variant="secondary" className={config.badgeColor}>
                        {cursosStatus.length}
                      </Badge>
                    </div>
                  </div>
                </div>

                {/* Lista de cursos */}
                <Droppable droppableId={status}>
                  {(provided, snapshot) => (
                    <div
                      ref={provided.innerRef}
                      {...provided.droppableProps}
                      className={`p-4 space-y-3 min-h-[500px] ${
                        snapshot.isDraggingOver ? 'bg-blue-50' : ''
                      }`}
                    >
                      {cursosStatus.length === 0 ? (
                        <div className="text-center py-8 text-gray-500">
                          <Folder className="h-12 w-12 mx-auto mb-4 text-gray-300" />
                          <p>Nenhum curso nesta coluna</p>
                        </div>
                      ) : (
                        cursosStatus.map((curso, index) => (
                          <Draggable key={curso.id} draggableId={curso.id.toString()} index={index}>
                            {(provided, snapshot) => (
                              <Card
                                ref={provided.innerRef}
                                {...provided.draggableProps}
                                {...provided.dragHandleProps}
                                className={`cursor-move hover:shadow-md transition-shadow ${
                                  snapshot.isDragging ? 'shadow-lg rotate-2' : ''
                                }`}
                              >
                                <CardContent className="p-4">
                                  {/* Header do card */}
                                  <div className="flex items-start justify-between mb-3">
                                    <div className="flex-1">
                                      <h4 className="font-semibold text-gray-900 text-sm leading-tight">
                                        {curso.nomeCurso}
                                      </h4>
                                      <p className="text-xs text-gray-500 mt-1">
                                        {curso.codigoCurso}
                                      </p>
                                    </div>
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
                                        <DropdownMenuItem onClick={() => abrirArquivos(curso)}>
                                          <FileText className="h-4 w-4 mr-2" />
                                          Arquivos ({curso.totalArquivos})
                                        </DropdownMenuItem>
                                        <DropdownMenuSeparator />
                                        <DropdownMenuItem>
                                          <Edit className="h-4 w-4 mr-2" />
                                          Editar
                                        </DropdownMenuItem>
                                        <DropdownMenuItem>
                                          <Copy className="h-4 w-4 mr-2" />
                                          Duplicar
                                        </DropdownMenuItem>
                                        <DropdownMenuItem>
                                          <Share className="h-4 w-4 mr-2" />
                                          Compartilhar
                                        </DropdownMenuItem>
                                        <DropdownMenuSeparator />
                                        <DropdownMenuItem className="text-red-600">
                                          <Trash2 className="h-4 w-4 mr-2" />
                                          Excluir
                                        </DropdownMenuItem>
                                      </DropdownMenuContent>
                                    </DropdownMenu>
                                  </div>

                                  {/* Informações do curso */}
                                  <div className="space-y-2">
                                    <div className="flex items-center gap-2 text-xs text-gray-600">
                                      <Building className="h-3 w-3" />
                                      <span>{curso.descricaoAcademia}</span>
                                    </div>
                                    
                                    <div className="flex items-center gap-2 text-xs text-gray-600">
                                      <Calendar className="h-3 w-3" />
                                      <span>{formatDate(curso.dataInicioOperacao)}</span>
                                    </div>

                                    {curso.criadoPor && (
                                      <div className="flex items-center gap-2 text-xs text-gray-600">
                                        <User className="h-3 w-3" />
                                        <span>{curso.criadoPor}</span>
                                      </div>
                                    )}
                                  </div>

                                  {/* Badges */}
                                  <div className="flex flex-wrap gap-1 mt-3">
                                    <Badge variant="outline" className="text-xs">
                                      {curso.origem}
                                    </Badge>
                                    <Badge variant="outline" className="text-xs">
                                      {curso.tipoAmbiente}
                                    </Badge>
                                    <Badge variant="outline" className="text-xs">
                                      {curso.tipoAcesso}
                                    </Badge>
                                  </div>

                                  {/* Barra de progresso */}
                                  {curso.progresso > 0 && (
                                    <div className="mt-3">
                                      <div className="flex items-center justify-between text-xs text-gray-600 mb-1">
                                        <span>Progresso</span>
                                        <span>{curso.progresso}%</span>
                                      </div>
                                      <div className="w-full bg-gray-200 rounded-full h-2">
                                        <div 
                                          className={`h-2 rounded-full ${getProgressColor(curso.progresso)}`}
                                          style={{ width: `${curso.progresso}%` }}
                                        ></div>
                                      </div>
                                    </div>
                                  )}

                                  {/* Arquivos */}
                                  <div className="flex items-center justify-between mt-3 pt-3 border-t">
                                    <div className="flex items-center gap-1 text-xs text-gray-600">
                                      <FileText className="h-3 w-3" />
                                      <span>{curso.totalArquivos} arquivos</span>
                                    </div>
                                    <Button 
                                      variant="ghost" 
                                      size="sm" 
                                      className="h-6 px-2 text-xs"
                                      onClick={() => abrirArquivos(curso)}
                                    >
                                      Ver Arquivos
                                    </Button>
                                  </div>
                                </CardContent>
                              </Card>
                            )}
                          </Draggable>
                        ))
                      )}
                      {provided.placeholder}
                    </div>
                  )}
                </Droppable>
              </div>
            );
          })}
        </div>
      </DragDropContext>

      {/* Estatísticas do rodapé */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
        <Card>
          <CardContent className="p-4 text-center">
            <div className="text-2xl font-bold text-ferreira-red-600">{cursosPorStatus['Backlog'].length}</div>
            <div className="text-sm text-gray-600">Total de Cursos</div>
          </CardContent>
        </Card>
        <Card>
          <CardContent className="p-4 text-center">
            <div className="text-2xl font-bold text-ferreira-orange-600">{cursosPorStatus['Em Desenvolvimento'].length}</div>
            <div className="text-sm text-gray-600">Em Desenvolvimento</div>
          </CardContent>
        </Card>
        <Card>
          <CardContent className="p-4 text-center">
            <div className="text-2xl font-bold text-ferreira-green-600">{cursosPorStatus['Veiculado'].length}</div>
            <div className="text-sm text-gray-600">Veiculados</div>
          </CardContent>
        </Card>
      </div>

      {/* Modal de Arquivos */}
      {arquivosModalOpen && cursoSelecionado && (
        <ArquivosModal
          curso={cursoSelecionado}
          isOpen={arquivosModalOpen}
          onClose={() => {
            setArquivosModalOpen(false);
            setCursoSelecionado(null);
          }}
        />
      )}

      {/* Modal de Detalhes */}
      <Dialog open={detalhesModalOpen} onOpenChange={setDetalhesModalOpen}>
        <DialogContent className="max-w-2xl">
          <DialogHeader>
            <DialogTitle>Detalhes do Curso</DialogTitle>
            <DialogDescription>
              Informações completas sobre o curso selecionado
            </DialogDescription>
          </DialogHeader>
          {cursoSelecionado && (
            <div className="space-y-4">
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="text-sm font-medium text-gray-700">Nome do Curso</label>
                  <p className="text-sm text-gray-900">{cursoSelecionado.nomeCurso}</p>
                </div>
                <div>
                  <label className="text-sm font-medium text-gray-700">Código</label>
                  <p className="text-sm text-gray-900">{cursoSelecionado.codigoCurso}</p>
                </div>
                <div>
                  <label className="text-sm font-medium text-gray-700">Academia</label>
                  <p className="text-sm text-gray-900">{cursoSelecionado.descricaoAcademia}</p>
                </div>
                <div>
                  <label className="text-sm font-medium text-gray-700">Status</label>
                  <Badge className={STATUS_CONFIG[cursoSelecionado.status].badgeColor}>
                    {cursoSelecionado.status}
                  </Badge>
                </div>
                <div>
                  <label className="text-sm font-medium text-gray-700">Tipo de Ambiente</label>
                  <p className="text-sm text-gray-900">{cursoSelecionado.tipoAmbiente}</p>
                </div>
                <div>
                  <label className="text-sm font-medium text-gray-700">Tipo de Acesso</label>
                  <p className="text-sm text-gray-900">{cursoSelecionado.tipoAcesso}</p>
                </div>
                <div>
                  <label className="text-sm font-medium text-gray-700">Data de Início</label>
                  <p className="text-sm text-gray-900">{formatDate(cursoSelecionado.dataInicioOperacao)}</p>
                </div>
                <div>
                  <label className="text-sm font-medium text-gray-700">Origem</label>
                  <p className="text-sm text-gray-900">{cursoSelecionado.origem}</p>
                </div>
                {cursoSelecionado.criadoPor && (
                  <div>
                    <label className="text-sm font-medium text-gray-700">Criado por</label>
                    <p className="text-sm text-gray-900">{cursoSelecionado.criadoPor}</p>
                  </div>
                )}
                <div>
                  <label className="text-sm font-medium text-gray-700">Total de Arquivos</label>
                  <p className="text-sm text-gray-900">{cursoSelecionado.totalArquivos}</p>
                </div>
              </div>
              {cursoSelecionado.comentariosInternos && (
                <div>
                  <label className="text-sm font-medium text-gray-700">Comentários Internos</label>
                  <p className="text-sm text-gray-900 bg-gray-50 p-3 rounded-md">
                    {cursoSelecionado.comentariosInternos}
                  </p>
                </div>
              )}
            </div>
          )}
        </DialogContent>
      </Dialog>
    </div>
  );
};

export default Kanban;

