import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { 
  BookOpen, 
  Users, 
  FileText, 
  TrendingUp,
  Calendar,
  Activity,
  Loader2,
  ArrowRight,
  Plus,
  Download,
  Upload,
  Eye,
  BarChart3,
  PieChart,
  RefreshCw
} from 'lucide-react';

// Dados mockados para demonstração
const mockStats = {
  totalCursos: 156,
  totalArquivos: 1247,
  totalUsuarios: 89,
  cursosAtivos: 23
};

const mockChartData = [
  { name: 'Backlog', value: 45, color: '#6B7280' },
  { name: 'Em Desenvolvimento', value: 23, color: '#3B82F6' },
  { name: 'Veiculado', value: 88, color: '#10B981' }
];

const mockActivities = [
  {
    id: 1,
    usuario: 'João Silva',
    acao: 'Upload de arquivo',
    curso: 'Curso de React Avançado',
    timestamp: '2024-06-29T10:30:00Z'
  },
  {
    id: 2,
    usuario: 'Maria Santos',
    acao: 'Criou novo curso',
    curso: 'Fundamentos de JavaScript',
    timestamp: '2024-06-29T09:15:00Z'
  },
  {
    id: 3,
    usuario: 'Pedro Costa',
    acao: 'Moveu curso para Veiculado',
    curso: 'CSS Grid e Flexbox',
    timestamp: '2024-06-29T08:45:00Z'
  }
];

const mockCursos = [
  {
    id: 1,
    nome: 'React Avançado',
    progresso: 85,
    status: 'Em Desenvolvimento',
    ultimaAtualizacao: '2024-06-29T10:30:00Z'
  },
  {
    id: 2,
    nome: 'Node.js Fundamentals',
    progresso: 60,
    status: 'Em Desenvolvimento',
    ultimaAtualizacao: '2024-06-29T09:15:00Z'
  },
  {
    id: 3,
    nome: 'TypeScript Essentials',
    progresso: 40,
    status: 'Em Desenvolvimento',
    ultimaAtualizacao: '2024-06-29T08:45:00Z'
  }
];

const Dashboard = () => {
  const [loading, setLoading] = useState(true);
  const [refreshing, setRefreshing] = useState(false);

  useEffect(() => {
    // Simular carregamento
    const timer = setTimeout(() => {
      setLoading(false);
    }, 1000);

    return () => clearTimeout(timer);
  }, []);

  const handleRefresh = async () => {
    setRefreshing(true);
    // Simular refresh
    setTimeout(() => {
      setRefreshing(false);
    }, 1000);
  };

  const formatDate = (dateString) => {
    return new Date(dateString).toLocaleDateString('pt-BR');
  };

  const formatDateTime = (dateString) => {
    return new Date(dateString).toLocaleString('pt-BR');
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center min-h-96">
        <div className="flex items-center space-x-2">
          <Loader2 className="h-6 w-6 animate-spin text-blue-600" />
          <span className="text-gray-600">Carregando dashboard...</span>
        </div>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex justify-between items-center">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">Dashboard</h1>
          <p className="text-gray-600 mt-1">Visão geral do sistema educacional</p>
        </div>
        <button
          onClick={handleRefresh}
          disabled={refreshing}
          className="flex items-center space-x-2 bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 disabled:opacity-50"
        >
          <RefreshCw className={`h-4 w-4 ${refreshing ? 'animate-spin' : ''}`} />
          <span>Atualizar</span>
        </button>
      </div>

      {/* Cards de Estatísticas */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        <div className="bg-gradient-to-r from-blue-500 to-blue-600 rounded-lg p-6 text-white">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-blue-100">Total de Cursos</p>
              <p className="text-3xl font-bold">{mockStats.totalCursos}</p>
            </div>
            <BookOpen className="h-12 w-12 text-blue-200" />
          </div>
        </div>

        <div className="bg-gradient-to-r from-green-500 to-green-600 rounded-lg p-6 text-white">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-green-100">Total de Arquivos</p>
              <p className="text-3xl font-bold">{mockStats.totalArquivos}</p>
            </div>
            <FileText className="h-12 w-12 text-green-200" />
          </div>
        </div>

        <div className="bg-gradient-to-r from-purple-500 to-purple-600 rounded-lg p-6 text-white">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-purple-100">Total de Usuários</p>
              <p className="text-3xl font-bold">{mockStats.totalUsuarios}</p>
            </div>
            <Users className="h-12 w-12 text-purple-200" />
          </div>
        </div>

        <div className="bg-gradient-to-r from-orange-500 to-orange-600 rounded-lg p-6 text-white">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-orange-100">Cursos Ativos</p>
              <p className="text-3xl font-bold">{mockStats.cursosAtivos}</p>
            </div>
            <TrendingUp className="h-12 w-12 text-orange-200" />
          </div>
        </div>
      </div>

      {/* Gráficos e Atividades */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {/* Distribuição de Cursos */}
        <div className="bg-white rounded-lg shadow p-6">
          <h3 className="text-lg font-semibold text-gray-900 mb-4">Distribuição de Cursos</h3>
          <div className="space-y-3">
            {mockChartData.map((item, index) => (
              <div key={index} className="flex items-center justify-between">
                <div className="flex items-center space-x-3">
                  <div 
                    className="w-4 h-4 rounded-full" 
                    style={{ backgroundColor: item.color }}
                  ></div>
                  <span className="text-gray-700">{item.name}</span>
                </div>
                <span className="font-semibold text-gray-900">{item.value}</span>
              </div>
            ))}
          </div>
        </div>

        {/* Atividades Recentes */}
        <div className="bg-white rounded-lg shadow p-6">
          <h3 className="text-lg font-semibold text-gray-900 mb-4">Atividades Recentes</h3>
          <div className="space-y-4">
            {mockActivities.map((activity) => (
              <div key={activity.id} className="flex items-start space-x-3">
                <div className="flex-shrink-0">
                  <Activity className="h-5 w-5 text-blue-600 mt-0.5" />
                </div>
                <div className="flex-1 min-w-0">
                  <p className="text-sm text-gray-900">
                    <span className="font-medium">{activity.usuario}</span> {activity.acao}
                  </p>
                  <p className="text-sm text-gray-600">{activity.curso}</p>
                  <p className="text-xs text-gray-500">{formatDateTime(activity.timestamp)}</p>
                </div>
              </div>
            ))}
          </div>
        </div>
      </div>

      {/* Cursos em Progresso */}
      <div className="bg-white rounded-lg shadow p-6">
        <div className="flex justify-between items-center mb-4">
          <h3 className="text-lg font-semibold text-gray-900">Cursos em Desenvolvimento</h3>
          <Link 
            to="/kanban" 
            className="text-blue-600 hover:text-blue-700 flex items-center space-x-1"
          >
            <span>Ver todos</span>
            <ArrowRight className="h-4 w-4" />
          </Link>
        </div>
        <div className="space-y-4">
          {mockCursos.map((curso) => (
            <div key={curso.id} className="border rounded-lg p-4">
              <div className="flex justify-between items-start mb-2">
                <h4 className="font-medium text-gray-900">{curso.nome}</h4>
                <span className="text-sm text-gray-500">{formatDate(curso.ultimaAtualizacao)}</span>
              </div>
              <div className="flex items-center space-x-4">
                <div className="flex-1">
                  <div className="flex justify-between text-sm text-gray-600 mb-1">
                    <span>Progresso</span>
                    <span>{curso.progresso}%</span>
                  </div>
                  <div className="w-full bg-gray-200 rounded-full h-2">
                    <div 
                      className="bg-blue-600 h-2 rounded-full transition-all duration-300" 
                      style={{ width: `${curso.progresso}%` }}
                    ></div>
                  </div>
                </div>
                <span className="text-sm bg-blue-100 text-blue-800 px-2 py-1 rounded">
                  {curso.status}
                </span>
              </div>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
};

export default Dashboard;

