import React, { useState, useEffect } from 'react';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import { Alert, AlertDescription } from '@/components/ui/alert';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import { 
  RefreshCw, 
  Database, 
  CheckCircle, 
  XCircle, 
  Clock, 
  AlertTriangle,
  Play,
  Pause,
  Settings,
  FileText,
  TrendingUp,
  Calendar,
  Users,
  Loader2,
  Download,
  Eye
} from 'lucide-react';
import apiService from '../services/api';
import { formatDate, formatDuration } from '../utils';

const SeniorIntegration = () => {
  const [loading, setLoading] = useState(true);
  const [syncing, setSyncing] = useState(false);
  const [connectionStatus, setConnectionStatus] = useState(null);
  const [syncStats, setSyncStats] = useState(null);
  const [conflicts, setConflicts] = useState([]);
  const [syncLogs, setSyncLogs] = useState([]);
  const [seniorCursos, setSeniorCursos] = useState([]);
  const [error, setError] = useState('');
  const [activeTab, setActiveTab] = useState('dashboard');
  
  // Estados para modais
  const [showConflictModal, setShowConflictModal] = useState(false);
  const [selectedConflict, setSelectedConflict] = useState(null);
  const [showSeniorCursoModal, setShowSeniorCursoModal] = useState(false);
  const [selectedSeniorCurso, setSelectedSeniorCurso] = useState(null);

  useEffect(() => {
    loadInitialData();
  }, []);

  const loadInitialData = async () => {
    try {
      setLoading(true);
      setError('');
      
      await Promise.all([
        checkConnection(),
        loadSyncStats(),
        loadConflicts(),
        loadSyncLogs()
      ]);
    } catch (error) {
      console.error('Erro ao carregar dados:', error);
      setError('Erro ao carregar dados da integração Senior');
    } finally {
      setLoading(false);
    }
  };

  const checkConnection = async () => {
    try {
      const response = await apiService.request('/senior/test-connection');
      setConnectionStatus(response.data);
    } catch (error) {
      setConnectionStatus(false);
    }
  };

  const loadSyncStats = async () => {
    try {
      const response = await apiService.request('/senior/stats');
      setSyncStats(response.data);
    } catch (error) {
      console.error('Erro ao carregar estatísticas:', error);
    }
  };

  const loadConflicts = async () => {
    try {
      const response = await apiService.request('/senior/conflicts');
      setConflicts(response.data);
    } catch (error) {
      console.error('Erro ao carregar conflitos:', error);
    }
  };

  const loadSyncLogs = async () => {
    try {
      const response = await apiService.request('/senior/logs?pageSize=20');
      setSyncLogs(response.data);
    } catch (error) {
      console.error('Erro ao carregar logs:', error);
    }
  };

  const loadSeniorCursos = async () => {
    try {
      const response = await apiService.request('/senior/cursos?pageSize=50');
      setSeniorCursos(response.data);
    } catch (error) {
      console.error('Erro ao carregar cursos do Senior:', error);
    }
  };

  const handleSync = async () => {
    try {
      setSyncing(true);
      setError('');
      
      const response = await apiService.request('/senior/sync', { method: 'POST' });
      
      if (response.success) {
        await loadInitialData();
      } else {
        setError(response.message || 'Erro na sincronização');
      }
    } catch (error) {
      console.error('Erro na sincronização:', error);
      setError('Erro ao executar sincronização');
    } finally {
      setSyncing(false);
    }
  };

  const handleScheduleSync = async () => {
    try {
      const response = await apiService.request('/senior/schedule', { method: 'POST' });
      
      if (response.success) {
        await loadSyncStats();
      }
    } catch (error) {
      console.error('Erro ao agendar sincronização:', error);
      setError('Erro ao agendar sincronização automática');
    }
  };

  const handleCancelSync = async () => {
    try {
      const response = await apiService.request('/senior/schedule', { method: 'DELETE' });
      
      if (response.success) {
        await loadSyncStats();
      }
    } catch (error) {
      console.error('Erro ao cancelar sincronização:', error);
      setError('Erro ao cancelar sincronização automática');
    }
  };

  const handleResolveConflict = async (conflictId, resolution) => {
    try {
      const response = await apiService.request(`/senior/conflicts/${conflictId}/resolve`, {
        method: 'POST',
        body: JSON.stringify({ resolution })
      });
      
      if (response.success) {
        await loadConflicts();
        setShowConflictModal(false);
      }
    } catch (error) {
      console.error('Erro ao resolver conflito:', error);
      setError('Erro ao resolver conflito');
    }
  };

  const getStatusIcon = (status) => {
    if (status === null) return <Clock className="h-4 w-4 text-gray-400" />;
    return status ? <CheckCircle className="h-4 w-4 text-green-500" /> : <XCircle className="h-4 w-4 text-red-500" />;
  };

  const getStatusText = (status) => {
    if (status === null) return 'Verificando...';
    return status ? 'Conectado' : 'Desconectado';
  };

  const getLogTypeColor = (tipo) => {
    switch (tipo) {
      case 'Success': return 'bg-green-100 text-green-800';
      case 'Error': return 'bg-red-100 text-red-800';
      case 'Warning': return 'bg-yellow-100 text-yellow-800';
      case 'Conflict': return 'bg-orange-100 text-orange-800';
      default: return 'bg-blue-100 text-blue-800';
    }
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="flex items-center space-x-2">
          <Loader2 className="h-6 w-6 animate-spin text-blue-600" />
          <span className="text-gray-600">Carregando integração Senior...</span>
        </div>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      {/* Cabeçalho */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">Integração Senior</h1>
          <p className="text-gray-600 mt-1">
            Gerencie a sincronização de cursos com o sistema Senior
          </p>
        </div>
        
        <div className="flex items-center gap-3">
          <Button
            variant="outline"
            onClick={loadInitialData}
            disabled={loading}
          >
            <RefreshCw className={`h-4 w-4 mr-2 ${loading ? 'animate-spin' : ''}`} />
            Atualizar
          </Button>
          
          <Button
            onClick={handleSync}
            disabled={syncing || !connectionStatus}
          >
            {syncing ? (
              <>
                <Loader2 className="h-4 w-4 mr-2 animate-spin" />
                Sincronizando...
              </>
            ) : (
              <>
                <Database className="h-4 w-4 mr-2" />
                Sincronizar Agora
              </>
            )}
          </Button>
        </div>
      </div>

      {/* Alertas */}
      {error && (
        <Alert variant="destructive">
          <AlertTriangle className="h-4 w-4" />
          <AlertDescription>{error}</AlertDescription>
        </Alert>
      )}

      {/* Cards de Status */}
      <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
        <Card>
          <CardContent className="p-4">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm text-gray-600">Conexão Senior</p>
                <p className="text-lg font-semibold">{getStatusText(connectionStatus)}</p>
              </div>
              {getStatusIcon(connectionStatus)}
            </div>
          </CardContent>
        </Card>
        
        <Card>
          <CardContent className="p-4">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm text-gray-600">Última Sincronização</p>
                <p className="text-lg font-semibold">
                  {syncStats?.ultimaSincronizacao 
                    ? formatDate(syncStats.ultimaSincronizacao)
                    : 'Nunca'
                  }
                </p>
              </div>
              <Calendar className="h-8 w-8 text-blue-600" />
            </div>
          </CardContent>
        </Card>
        
        <Card>
          <CardContent className="p-4">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm text-gray-600">Cursos do Senior</p>
                <p className="text-lg font-semibold">{syncStats?.cursosOrigemSenior || 0}</p>
              </div>
              <Users className="h-8 w-8 text-green-600" />
            </div>
          </CardContent>
        </Card>
        
        <Card>
          <CardContent className="p-4">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm text-gray-600">Conflitos Ativos</p>
                <p className="text-lg font-semibold">{conflicts.length}</p>
              </div>
              <AlertTriangle className={`h-8 w-8 ${conflicts.length > 0 ? 'text-red-600' : 'text-gray-400'}`} />
            </div>
          </CardContent>
        </Card>
      </div>

      {/* Tabs */}
      <Tabs value={activeTab} onValueChange={setActiveTab}>
        <TabsList className="grid w-full grid-cols-4">
          <TabsTrigger value="dashboard">Dashboard</TabsTrigger>
          <TabsTrigger value="conflicts">Conflitos</TabsTrigger>
          <TabsTrigger value="senior-cursos">Cursos Senior</TabsTrigger>
          <TabsTrigger value="logs">Logs</TabsTrigger>
        </TabsList>

        <TabsContent value="dashboard" className="mt-6">
          <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
            {/* Configurações de Sincronização */}
            <Card>
              <CardHeader>
                <CardTitle className="flex items-center gap-2">
                  <Settings className="h-5 w-5" />
                  Configurações de Sincronização
                </CardTitle>
              </CardHeader>
              <CardContent className="space-y-4">
                <div className="flex items-center justify-between">
                  <div>
                    <p className="font-medium">Sincronização Automática</p>
                    <p className="text-sm text-gray-600">
                      {syncStats?.proximaSincronizacao 
                        ? `Próxima: ${formatDate(syncStats.proximaSincronizacao)}`
                        : 'Não agendada'
                      }
                    </p>
                  </div>
                  
                  {syncStats?.proximaSincronizacao ? (
                    <Button variant="outline" size="sm" onClick={handleCancelSync}>
                      <Pause className="h-4 w-4 mr-2" />
                      Cancelar
                    </Button>
                  ) : (
                    <Button size="sm" onClick={handleScheduleSync}>
                      <Play className="h-4 w-4 mr-2" />
                      Agendar
                    </Button>
                  )}
                </div>
                
                <div className="pt-4 border-t">
                  <p className="text-sm text-gray-600 mb-2">Frequência: Diária às 02:00</p>
                  <p className="text-sm text-gray-600">
                    Status: {syncStats?.sincronizacaoEmAndamento ? 'Em andamento' : 'Parada'}
                  </p>
                </div>
              </CardContent>
            </Card>

            {/* Estatísticas */}
            <Card>
              <CardHeader>
                <CardTitle className="flex items-center gap-2">
                  <TrendingUp className="h-5 w-5" />
                  Estatísticas
                </CardTitle>
              </CardHeader>
              <CardContent className="space-y-4">
                <div className="grid grid-cols-2 gap-4">
                  <div>
                    <p className="text-sm text-gray-600">Total de Cursos</p>
                    <p className="text-2xl font-bold">{syncStats?.totalCursosLocais || 0}</p>
                  </div>
                  <div>
                    <p className="text-sm text-gray-600">Do Senior</p>
                    <p className="text-2xl font-bold">{syncStats?.cursosOrigemSenior || 0}</p>
                  </div>
                </div>
                
                {syncStats?.duracaoUltimaSincronizacao && (
                  <div className="pt-4 border-t">
                    <p className="text-sm text-gray-600">Duração da última sincronização</p>
                    <p className="font-medium">{formatDuration(syncStats.duracaoUltimaSincronizacao)}</p>
                  </div>
                )}
              </CardContent>
            </Card>
          </div>
        </TabsContent>

        <TabsContent value="conflicts" className="mt-6">
          <Card>
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <AlertTriangle className="h-5 w-5" />
                Conflitos de Sincronização ({conflicts.length})
              </CardTitle>
            </CardHeader>
            <CardContent>
              {conflicts.length === 0 ? (
                <div className="text-center py-8 text-gray-500">
                  <CheckCircle className="h-12 w-12 mx-auto mb-4 text-green-400" />
                  <p>Nenhum conflito encontrado</p>
                  <p className="text-sm">Todos os dados estão sincronizados</p>
                </div>
              ) : (
                <div className="space-y-4">
                  {conflicts.map(conflict => (
                    <div key={conflict.id} className="border rounded-lg p-4">
                      <div className="flex items-center justify-between mb-2">
                        <div>
                          <h4 className="font-medium">{conflict.nomeCurso}</h4>
                          <p className="text-sm text-gray-600">Código: {conflict.codigoCurso}</p>
                        </div>
                        <Badge variant="destructive">
                          {conflict.tipo}
                        </Badge>
                      </div>
                      
                      <p className="text-sm text-gray-600 mb-3">
                        Detectado em: {formatDate(conflict.dataDeteccao)}
                      </p>
                      
                      <div className="flex gap-2">
                        <Button 
                          size="sm" 
                          variant="outline"
                          onClick={() => {
                            setSelectedConflict(conflict);
                            setShowConflictModal(true);
                          }}
                        >
                          <Eye className="h-4 w-4 mr-2" />
                          Ver Detalhes
                        </Button>
                        <Button 
                          size="sm"
                          onClick={() => handleResolveConflict(conflict.id, 'UsarSenior')}
                        >
                          Usar Senior
                        </Button>
                        <Button 
                          size="sm" 
                          variant="outline"
                          onClick={() => handleResolveConflict(conflict.id, 'ManterLocal')}
                        >
                          Manter Local
                        </Button>
                      </div>
                    </div>
                  ))}
                </div>
              )}
            </CardContent>
          </Card>
        </TabsContent>

        <TabsContent value="senior-cursos" className="mt-6">
          <Card>
            <CardHeader>
              <CardTitle className="flex items-center justify-between">
                <div className="flex items-center gap-2">
                  <Database className="h-5 w-5" />
                  Cursos no Senior
                </div>
                <Button 
                  variant="outline" 
                  size="sm"
                  onClick={loadSeniorCursos}
                >
                  <RefreshCw className="h-4 w-4 mr-2" />
                  Carregar
                </Button>
              </CardTitle>
            </CardHeader>
            <CardContent>
              {seniorCursos.length === 0 ? (
                <div className="text-center py-8 text-gray-500">
                  <Database className="h-12 w-12 mx-auto mb-4 text-gray-300" />
                  <p>Clique em "Carregar" para buscar cursos do Senior</p>
                </div>
              ) : (
                <div className="space-y-4 max-h-96 overflow-y-auto">
                  {seniorCursos.map(curso => (
                    <div key={curso.codigo} className="border rounded-lg p-4">
                      <div className="flex items-center justify-between">
                        <div>
                          <h4 className="font-medium">{curso.nome}</h4>
                          <p className="text-sm text-gray-600">Código: {curso.codigo}</p>
                          <p className="text-sm text-gray-600">Status: {curso.status}</p>
                        </div>
                        <Button 
                          size="sm" 
                          variant="outline"
                          onClick={() => {
                            setSelectedSeniorCurso(curso);
                            setShowSeniorCursoModal(true);
                          }}
                        >
                          <Eye className="h-4 w-4 mr-2" />
                          Ver Detalhes
                        </Button>
                      </div>
                    </div>
                  ))}
                </div>
              )}
            </CardContent>
          </Card>
        </TabsContent>

        <TabsContent value="logs" className="mt-6">
          <Card>
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <FileText className="h-5 w-5" />
                Logs de Sincronização
              </CardTitle>
            </CardHeader>
            <CardContent>
              <div className="space-y-3 max-h-96 overflow-y-auto">
                {syncLogs.map(log => (
                  <div key={log.id} className="flex items-start gap-3 p-3 border rounded-lg">
                    <Badge className={getLogTypeColor(log.tipo)}>
                      {log.tipo}
                    </Badge>
                    <div className="flex-1">
                      <p className="text-sm">{log.mensagem}</p>
                      {log.detalhes && (
                        <p className="text-xs text-gray-600 mt-1">{log.detalhes}</p>
                      )}
                      <p className="text-xs text-gray-500 mt-1">
                        {formatDate(log.dataHora)}
                      </p>
                    </div>
                  </div>
                ))}
              </div>
            </CardContent>
          </Card>
        </TabsContent>
      </Tabs>

      {/* Modal de Conflito */}
      <Dialog open={showConflictModal} onOpenChange={setShowConflictModal}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>Detalhes do Conflito</DialogTitle>
            <DialogDescription>
              Resolva o conflito escolhendo qual versão manter
            </DialogDescription>
          </DialogHeader>
          
          {selectedConflict && (
            <div className="space-y-4">
              <div>
                <h4 className="font-medium">{selectedConflict.nomeCurso}</h4>
                <p className="text-sm text-gray-600">Código: {selectedConflict.codigoCurso}</p>
              </div>
              
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <h5 className="font-medium text-sm">Versão Local</h5>
                  <p className="text-sm text-gray-600">{selectedConflict.valorLocal}</p>
                </div>
                <div>
                  <h5 className="font-medium text-sm">Versão Senior</h5>
                  <p className="text-sm text-gray-600">{selectedConflict.valorSenior}</p>
                </div>
              </div>
              
              <div className="flex gap-2 pt-4">
                <Button 
                  onClick={() => handleResolveConflict(selectedConflict.id, 'ManterLocal')}
                >
                  Manter Local
                </Button>
                <Button 
                  variant="outline"
                  onClick={() => handleResolveConflict(selectedConflict.id, 'UsarSenior')}
                >
                  Usar Senior
                </Button>
                <Button 
                  variant="outline"
                  onClick={() => handleResolveConflict(selectedConflict.id, 'Ignorar')}
                >
                  Ignorar
                </Button>
              </div>
            </div>
          )}
        </DialogContent>
      </Dialog>

      {/* Modal de Curso Senior */}
      <Dialog open={showSeniorCursoModal} onOpenChange={setShowSeniorCursoModal}>
        <DialogContent className="max-w-2xl">
          <DialogHeader>
            <DialogTitle>Detalhes do Curso Senior</DialogTitle>
          </DialogHeader>
          
          {selectedSeniorCurso && (
            <div className="space-y-4">
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="text-sm font-medium">Código</label>
                  <p className="text-sm text-gray-600">{selectedSeniorCurso.codigo}</p>
                </div>
                <div>
                  <label className="text-sm font-medium">Status</label>
                  <p className="text-sm text-gray-600">{selectedSeniorCurso.status}</p>
                </div>
              </div>
              
              <div>
                <label className="text-sm font-medium">Nome</label>
                <p className="text-sm text-gray-600">{selectedSeniorCurso.nome}</p>
              </div>
              
              <div>
                <label className="text-sm font-medium">Descrição da Academia</label>
                <p className="text-sm text-gray-600">{selectedSeniorCurso.descricaoAcademia}</p>
              </div>
              
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="text-sm font-medium">Tipo de Ambiente</label>
                  <p className="text-sm text-gray-600">{selectedSeniorCurso.tipoAmbiente}</p>
                </div>
                <div>
                  <label className="text-sm font-medium">Tipo de Acesso</label>
                  <p className="text-sm text-gray-600">{selectedSeniorCurso.tipoAcesso}</p>
                </div>
              </div>
              
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="text-sm font-medium">Data de Criação</label>
                  <p className="text-sm text-gray-600">{formatDate(selectedSeniorCurso.dataCriacao)}</p>
                </div>
                <div>
                  <label className="text-sm font-medium">Última Atualização</label>
                  <p className="text-sm text-gray-600">{formatDate(selectedSeniorCurso.dataAtualizacao)}</p>
                </div>
              </div>
            </div>
          )}
        </DialogContent>
      </Dialog>
    </div>
  );
};

export default SeniorIntegration;

