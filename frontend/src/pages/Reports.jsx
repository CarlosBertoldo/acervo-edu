import React, { useState, useEffect } from 'react';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { Badge } from '@/components/ui/badge';
import { Separator } from '@/components/ui/separator';
import { 
  FileText, 
  Download, 
  Filter, 
  Calendar,
  Users,
  BookOpen,
  Files,
  RefreshCw,
  BarChart3,
  TrendingUp,
  Activity,
  AlertCircle
} from 'lucide-react';
import { api } from '../services/api';
import { formatDate, formatFileSize } from '../utils';

export default function Reports() {
  const [loading, setLoading] = useState(false);
  const [reportData, setReportData] = useState(null);
  const [filterOptions, setFilterOptions] = useState(null);
  const [availableReports, setAvailableReports] = useState([]);
  const [selectedReport, setSelectedReport] = useState('cursos');
  const [filters, setFilters] = useState({});
  const [exportFormat, setExportFormat] = useState('excel');

  useEffect(() => {
    loadInitialData();
  }, []);

  const loadInitialData = async () => {
    try {
      setLoading(true);
      
      const [filterOptionsRes, availableReportsRes] = await Promise.all([
        api.get('/reports/filter-options'),
        api.get('/reports/available')
      ]);

      setFilterOptions(filterOptionsRes.data.data);
      setAvailableReports(availableReportsRes.data.data);
    } catch (error) {
      console.error('Erro ao carregar dados iniciais:', error);
    } finally {
      setLoading(false);
    }
  };

  const generateReport = async () => {
    try {
      setLoading(true);
      
      const response = await api.post(`/reports/${selectedReport}`, filters);
      setReportData(response.data.data);
    } catch (error) {
      console.error('Erro ao gerar relatório:', error);
    } finally {
      setLoading(false);
    }
  };

  const exportReport = async (format) => {
    try {
      setLoading(true);
      
      const response = await api.post(`/reports/export/${format}/${selectedReport}`, filters, {
        responseType: 'blob'
      });

      // Criar download do arquivo
      const url = window.URL.createObjectURL(new Blob([response.data]));
      const link = document.createElement('a');
      link.href = url;
      
      const extension = format === 'excel' ? 'xlsx' : format;
      link.setAttribute('download', `relatorio_${selectedReport}_${new Date().toISOString().split('T')[0]}.${extension}`);
      
      document.body.appendChild(link);
      link.click();
      link.remove();
      window.URL.revokeObjectURL(url);
    } catch (error) {
      console.error('Erro ao exportar relatório:', error);
    } finally {
      setLoading(false);
    }
  };

  const updateFilter = (key, value) => {
    setFilters(prev => ({
      ...prev,
      [key]: value
    }));
  };

  const clearFilters = () => {
    setFilters({});
    setReportData(null);
  };

  const currentReportConfig = availableReports.find(r => r.type === selectedReport);

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold tracking-tight">Relatórios</h1>
          <p className="text-muted-foreground">
            Gere e exporte relatórios detalhados do sistema
          </p>
        </div>
        <div className="flex items-center space-x-2">
          <Button 
            variant="outline" 
            onClick={clearFilters}
            disabled={loading}
          >
            <Filter className="mr-2 h-4 w-4" />
            Limpar Filtros
          </Button>
          <Button 
            onClick={generateReport}
            disabled={loading || !currentReportConfig}
          >
            <BarChart3 className="mr-2 h-4 w-4" />
            {loading ? 'Gerando...' : 'Gerar Relatório'}
          </Button>
        </div>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-4 gap-6">
        {/* Sidebar - Configurações */}
        <div className="lg:col-span-1 space-y-4">
          <Card>
            <CardHeader>
              <CardTitle className="text-lg">Configurações</CardTitle>
              <CardDescription>
                Selecione o tipo de relatório e configure os filtros
              </CardDescription>
            </CardHeader>
            <CardContent className="space-y-4">
              {/* Tipo de Relatório */}
              <div className="space-y-2">
                <Label>Tipo de Relatório</Label>
                <Select value={selectedReport} onValueChange={setSelectedReport}>
                  <SelectTrigger>
                    <SelectValue />
                  </SelectTrigger>
                  <SelectContent>
                    {availableReports.map(report => (
                      <SelectItem key={report.type} value={report.type}>
                        <div className="flex items-center space-x-2">
                          {report.type === 'cursos' && <BookOpen className="h-4 w-4" />}
                          {report.type === 'atividades' && <Activity className="h-4 w-4" />}
                          {report.type === 'arquivos' && <Files className="h-4 w-4" />}
                          {report.type === 'senior' && <RefreshCw className="h-4 w-4" />}
                          <span>{report.name}</span>
                        </div>
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>

              {currentReportConfig && (
                <div className="space-y-2">
                  <Label>Descrição</Label>
                  <p className="text-sm text-muted-foreground">
                    {currentReportConfig.description}
                  </p>
                </div>
              )}

              <Separator />

              {/* Filtros Dinâmicos */}
              <div className="space-y-4">
                <Label className="text-base font-medium">Filtros</Label>
                
                {selectedReport === 'cursos' && (
                  <CursoFilters 
                    filters={filters} 
                    updateFilter={updateFilter}
                    filterOptions={filterOptions}
                  />
                )}

                {selectedReport === 'atividades' && (
                  <AtividadeFilters 
                    filters={filters} 
                    updateFilter={updateFilter}
                    filterOptions={filterOptions}
                  />
                )}

                {selectedReport === 'arquivos' && (
                  <ArquivoFilters 
                    filters={filters} 
                    updateFilter={updateFilter}
                    filterOptions={filterOptions}
                  />
                )}

                {selectedReport === 'senior' && (
                  <SeniorFilters 
                    filters={filters} 
                    updateFilter={updateFilter}
                    filterOptions={filterOptions}
                  />
                )}
              </div>

              <Separator />

              {/* Exportação */}
              <div className="space-y-2">
                <Label>Formato de Exportação</Label>
                <Select value={exportFormat} onValueChange={setExportFormat}>
                  <SelectTrigger>
                    <SelectValue />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="excel">Excel (.xlsx)</SelectItem>
                    <SelectItem value="pdf">PDF (.pdf)</SelectItem>
                    <SelectItem value="csv">CSV (.csv)</SelectItem>
                  </SelectContent>
                </Select>
                
                <Button 
                  variant="outline" 
                  className="w-full mt-2"
                  onClick={() => exportReport(exportFormat)}
                  disabled={loading || !reportData}
                >
                  <Download className="mr-2 h-4 w-4" />
                  Exportar {exportFormat.toUpperCase()}
                </Button>
              </div>
            </CardContent>
          </Card>
        </div>

        {/* Conteúdo Principal */}
        <div className="lg:col-span-3">
          {reportData ? (
            <ReportResults reportData={reportData} />
          ) : (
            <Card>
              <CardContent className="flex flex-col items-center justify-center py-12">
                <FileText className="h-12 w-12 text-muted-foreground mb-4" />
                <h3 className="text-lg font-medium mb-2">Nenhum relatório gerado</h3>
                <p className="text-muted-foreground text-center mb-4">
                  Configure os filtros e clique em "Gerar Relatório" para visualizar os dados
                </p>
                <Button onClick={generateReport} disabled={loading}>
                  <BarChart3 className="mr-2 h-4 w-4" />
                  Gerar Primeiro Relatório
                </Button>
              </CardContent>
            </Card>
          )}
        </div>
      </div>
    </div>
  );
}

// Componentes de Filtros Específicos
function CursoFilters({ filters, updateFilter, filterOptions }) {
  return (
    <div className="space-y-3">
      <div>
        <Label htmlFor="nome">Nome do Curso</Label>
        <Input
          id="nome"
          placeholder="Filtrar por nome..."
          value={filters.nome || ''}
          onChange={(e) => updateFilter('nome', e.target.value)}
        />
      </div>

      <div>
        <Label htmlFor="codigoCurso">Código do Curso</Label>
        <Input
          id="codigoCurso"
          placeholder="Filtrar por código..."
          value={filters.codigoCurso || ''}
          onChange={(e) => updateFilter('codigoCurso', e.target.value)}
        />
      </div>

      <div>
        <Label>Status</Label>
        <Select value={filters.status || ''} onValueChange={(value) => updateFilter('status', value)}>
          <SelectTrigger>
            <SelectValue placeholder="Todos os status" />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="">Todos</SelectItem>
            {filterOptions?.statusCurso?.map(status => (
              <SelectItem key={status.value} value={status.value}>
                {status.label}
              </SelectItem>
            ))}
          </SelectContent>
        </Select>
      </div>

      <div>
        <Label>Origem</Label>
        <Select value={filters.origem || ''} onValueChange={(value) => updateFilter('origem', value)}>
          <SelectTrigger>
            <SelectValue placeholder="Todas as origens" />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="">Todas</SelectItem>
            {filterOptions?.origemCurso?.map(origem => (
              <SelectItem key={origem.value} value={origem.value}>
                {origem.label}
              </SelectItem>
            ))}
          </SelectContent>
        </Select>
      </div>

      <div className="grid grid-cols-2 gap-2">
        <div>
          <Label htmlFor="dataInicioOperacaoInicio">Data Início (De)</Label>
          <Input
            id="dataInicioOperacaoInicio"
            type="date"
            value={filters.dataInicioOperacaoInicio || ''}
            onChange={(e) => updateFilter('dataInicioOperacaoInicio', e.target.value)}
          />
        </div>
        <div>
          <Label htmlFor="dataInicioOperacaoFim">Data Início (Até)</Label>
          <Input
            id="dataInicioOperacaoFim"
            type="date"
            value={filters.dataInicioOperacaoFim || ''}
            onChange={(e) => updateFilter('dataInicioOperacaoFim', e.target.value)}
          />
        </div>
      </div>
    </div>
  );
}

function AtividadeFilters({ filters, updateFilter, filterOptions }) {
  return (
    <div className="space-y-3">
      <div>
        <Label>Usuário</Label>
        <Select value={filters.usuarioId || ''} onValueChange={(value) => updateFilter('usuarioId', value)}>
          <SelectTrigger>
            <SelectValue placeholder="Todos os usuários" />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="">Todos</SelectItem>
            {filterOptions?.usuarios?.map(usuario => (
              <SelectItem key={usuario.id} value={usuario.id}>
                {usuario.nome}
              </SelectItem>
            ))}
          </SelectContent>
        </Select>
      </div>

      <div>
        <Label htmlFor="acao">Ação</Label>
        <Input
          id="acao"
          placeholder="Filtrar por ação..."
          value={filters.acao || ''}
          onChange={(e) => updateFilter('acao', e.target.value)}
        />
      </div>

      <div className="grid grid-cols-2 gap-2">
        <div>
          <Label htmlFor="dataInicio">Data (De)</Label>
          <Input
            id="dataInicio"
            type="date"
            value={filters.dataInicio || ''}
            onChange={(e) => updateFilter('dataInicio', e.target.value)}
          />
        </div>
        <div>
          <Label htmlFor="dataFim">Data (Até)</Label>
          <Input
            id="dataFim"
            type="date"
            value={filters.dataFim || ''}
            onChange={(e) => updateFilter('dataFim', e.target.value)}
          />
        </div>
      </div>

      <div>
        <Label htmlFor="ipAddress">Endereço IP</Label>
        <Input
          id="ipAddress"
          placeholder="Filtrar por IP..."
          value={filters.ipAddress || ''}
          onChange={(e) => updateFilter('ipAddress', e.target.value)}
        />
      </div>
    </div>
  );
}

function ArquivoFilters({ filters, updateFilter, filterOptions }) {
  return (
    <div className="space-y-3">
      <div>
        <Label htmlFor="cursoNome">Nome do Curso</Label>
        <Input
          id="cursoNome"
          placeholder="Filtrar por curso..."
          value={filters.cursoNome || ''}
          onChange={(e) => updateFilter('cursoNome', e.target.value)}
        />
      </div>

      <div>
        <Label>Categoria</Label>
        <Select value={filters.categoria || ''} onValueChange={(value) => updateFilter('categoria', value)}>
          <SelectTrigger>
            <SelectValue placeholder="Todas as categorias" />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="">Todas</SelectItem>
            {filterOptions?.categoriaArquivo?.map(categoria => (
              <SelectItem key={categoria.value} value={categoria.value}>
                {categoria.label}
              </SelectItem>
            ))}
          </SelectContent>
        </Select>
      </div>

      <div className="grid grid-cols-2 gap-2">
        <div>
          <Label htmlFor="dataUploadInicio">Upload (De)</Label>
          <Input
            id="dataUploadInicio"
            type="date"
            value={filters.dataUploadInicio || ''}
            onChange={(e) => updateFilter('dataUploadInicio', e.target.value)}
          />
        </div>
        <div>
          <Label htmlFor="dataUploadFim">Upload (Até)</Label>
          <Input
            id="dataUploadFim"
            type="date"
            value={filters.dataUploadFim || ''}
            onChange={(e) => updateFilter('dataUploadFim', e.target.value)}
          />
        </div>
      </div>
    </div>
  );
}

function SeniorFilters({ filters, updateFilter, filterOptions }) {
  return (
    <div className="space-y-3">
      <div className="grid grid-cols-2 gap-2">
        <div>
          <Label htmlFor="dataSincronizacaoInicio">Sincronização (De)</Label>
          <Input
            id="dataSincronizacaoInicio"
            type="date"
            value={filters.dataSincronizacaoInicio || ''}
            onChange={(e) => updateFilter('dataSincronizacaoInicio', e.target.value)}
          />
        </div>
        <div>
          <Label htmlFor="dataSincronizacaoFim">Sincronização (Até)</Label>
          <Input
            id="dataSincronizacaoFim"
            type="date"
            value={filters.dataSincronizacaoFim || ''}
            onChange={(e) => updateFilter('dataSincronizacaoFim', e.target.value)}
          />
        </div>
      </div>

      <div>
        <Label htmlFor="codigoCurso">Código do Curso</Label>
        <Input
          id="codigoCurso"
          placeholder="Filtrar por código..."
          value={filters.codigoCurso || ''}
          onChange={(e) => updateFilter('codigoCurso', e.target.value)}
        />
      </div>

      <div className="flex items-center space-x-4">
        <label className="flex items-center space-x-2">
          <input
            type="checkbox"
            checked={filters.somenteErros || false}
            onChange={(e) => updateFilter('somenteErros', e.target.checked)}
          />
          <span className="text-sm">Somente erros</span>
        </label>

        <label className="flex items-center space-x-2">
          <input
            type="checkbox"
            checked={filters.somenteConflitos || false}
            onChange={(e) => updateFilter('somenteConflitos', e.target.checked)}
          />
          <span className="text-sm">Somente conflitos</span>
        </label>
      </div>
    </div>
  );
}

// Componente de Resultados
function ReportResults({ reportData }) {
  return (
    <div className="space-y-6">
      {/* Header do Relatório */}
      <Card>
        <CardHeader>
          <div className="flex items-center justify-between">
            <div>
              <CardTitle className="text-xl">{reportData.title}</CardTitle>
              <CardDescription>{reportData.description}</CardDescription>
            </div>
            <Badge variant="secondary">
              {reportData.totalRecords} registros
            </Badge>
          </div>
          <div className="flex items-center space-x-4 text-sm text-muted-foreground">
            <span>Gerado em: {formatDate(reportData.generatedAt)}</span>
            {reportData.generatedBy && <span>Por: {reportData.generatedBy}</span>}
          </div>
        </CardHeader>
      </Card>

      {/* Resumo */}
      {reportData.summary && Object.keys(reportData.summary).length > 0 && (
        <Card>
          <CardHeader>
            <CardTitle className="text-lg">Resumo</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
              {Object.entries(reportData.summary).map(([key, value]) => (
                <div key={key} className="text-center">
                  <div className="text-2xl font-bold text-primary">
                    {typeof value === 'number' ? value.toLocaleString() : value}
                  </div>
                  <div className="text-sm text-muted-foreground">
                    {formatSummaryKey(key)}
                  </div>
                </div>
              ))}
            </div>
          </CardContent>
        </Card>
      )}

      {/* Dados */}
      <Card>
        <CardHeader>
          <CardTitle className="text-lg">Dados</CardTitle>
        </CardHeader>
        <CardContent>
          <div className="overflow-x-auto">
            <table className="w-full border-collapse">
              <thead>
                <tr className="border-b">
                  {reportData.columns.map((column, index) => (
                    <th key={index} className="text-left p-2 font-medium">
                      {column}
                    </th>
                  ))}
                </tr>
              </thead>
              <tbody>
                {reportData.data.slice(0, 50).map((row, rowIndex) => (
                  <tr key={rowIndex} className="border-b hover:bg-muted/50">
                    {Object.values(row).slice(0, reportData.columns.length).map((cell, cellIndex) => (
                      <td key={cellIndex} className="p-2">
                        {formatCellValue(cell)}
                      </td>
                    ))}
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
          
          {reportData.data.length > 50 && (
            <div className="mt-4 text-center text-sm text-muted-foreground">
              Mostrando 50 de {reportData.totalRecords} registros. 
              Use a exportação para ver todos os dados.
            </div>
          )}
        </CardContent>
      </Card>
    </div>
  );
}

// Funções auxiliares
function formatSummaryKey(key) {
  const keyMap = {
    'TotalCursos': 'Total de Cursos',
    'CursosBacklog': 'Backlog',
    'CursosEmDesenvolvimento': 'Em Desenvolvimento',
    'CursosVeiculados': 'Veiculados',
    'CursosOrigemSenior': 'Origem Senior',
    'CursosOrigemManual': 'Origem Manual',
    'TotalArquivos': 'Total de Arquivos',
    'TamanhoTotalMB': 'Tamanho Total (MB)',
    'ArquivosPublicos': 'Arquivos Públicos',
    'ArquivosPrivados': 'Arquivos Privados',
    'TotalLogs': 'Total de Logs',
    'UsuariosUnicos': 'Usuários Únicos',
    'LogsErro': 'Logs de Erro',
    'LogsSucesso': 'Logs de Sucesso'
  };
  
  return keyMap[key] || key;
}

function formatCellValue(value) {
  if (value === null || value === undefined) return '-';
  if (typeof value === 'boolean') return value ? 'Sim' : 'Não';
  if (typeof value === 'string' && value.includes('T')) {
    // Provavelmente uma data ISO
    try {
      return formatDate(value);
    } catch {
      return value;
    }
  }
  return value.toString();
}

