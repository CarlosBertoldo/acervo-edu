import React, { useState, useEffect } from 'react';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import { Alert, AlertDescription } from '@/components/ui/alert';
import { Switch } from '@/components/ui/switch';
import { Textarea } from '@/components/ui/textarea';
import { 
  Tabs,
  TabsContent,
  TabsList,
  TabsTrigger,
} from '@/components/ui/tabs';
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
  Settings, 
  Database, 
  Mail, 
  Cloud, 
  Shield, 
  Zap,
  Loader2,
  RefreshCw,
  Save,
  TestTube,
  CheckCircle,
  XCircle,
  AlertTriangle,
  Info,
  Key,
  Server,
  Globe,
  Lock,
  Unlock,
  Download,
  Upload,
  Calendar,
  Clock,
  Users,
  FileText,
  Smartphone,
  Bell,
  Eye,
  EyeOff,
  Copy,
  RotateCcw,
  Trash2,
  Plus,
  Edit,
  ExternalLink,
  HardDrive,
  Wifi,
  WifiOff
} from 'lucide-react';

// Dados mockados de configurações
const mockConfiguracoes = {
  sistema: {
    nomeAplicacao: 'Sistema Acervo Educacional',
    versao: '1.0.0',
    ambiente: 'Produção',
    manutencao: false,
    logLevel: 'Info',
    sessionTimeout: 30,
    maxFileSize: 100,
    allowedFileTypes: '.pdf,.docx,.pptx,.mp4,.mp3,.jpg,.png',
    backupAutomatico: true,
    backupHorario: '02:00',
    retencaoLogs: 90
  },
  senior: {
    ativo: true,
    servidor: 'https://senior.ferreiracosta.com.br',
    usuario: 'acervo_user',
    senha: '••••••••',
    database: 'SENIOR_DB',
    sincronizacaoAutomatica: true,
    intervaloSincronizacao: 60,
    ultimaSincronizacao: '2024-01-29T10:00:00Z',
    status: 'Conectado',
    cursosImportados: 156,
    usuariosImportados: 89
  },
  email: {
    ativo: true,
    servidor: 'smtp.gmail.com',
    porta: 587,
    ssl: true,
    usuario: 'noreply@ferreiracosta.com',
    senha: '••••••••',
    remetente: 'Sistema Acervo Educacional',
    templateRecuperacao: true,
    templateNotificacao: true,
    status: 'Conectado'
  },
  aws: {
    ativo: true,
    accessKey: 'AKIA••••••••••••••••',
    secretKey: '••••••••••••••••••••••••••••••••••••••••',
    region: 'us-east-1',
    bucket: 'acervo-educacional-files',
    cloudfront: 'https://d123456789.cloudfront.net',
    status: 'Conectado',
    espacoUsado: '2.5 GB',
    espacoTotal: '100 GB'
  },
  seguranca: {
    autenticacaoDupla: false,
    senhaComplexidade: true,
    senhaExpiracao: 90,
    tentativasLogin: 5,
    bloqueioTempo: 15,
    logAuditoria: true,
    criptografiaArquivos: true,
    httpsObrigatorio: true,
    corsOrigins: 'https://acervo.ferreiracosta.com'
  },
  notificacoes: {
    emailAtivo: true,
    smsAtivo: false,
    pushAtivo: true,
    notificarLogin: true,
    notificarUpload: false,
    notificarErros: true,
    notificarBackup: true,
    notificarSincronizacao: false
  }
};

const Configuracoes = () => {
  const [configuracoes, setConfiguracoes] = useState(mockConfiguracoes);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [testing, setTesting] = useState({});
  const [activeTab, setActiveTab] = useState('sistema');
  const [showPasswords, setShowPasswords] = useState({});
  const [backupModalOpen, setBackupModalOpen] = useState(false);
  const [restoreModalOpen, setRestoreModalOpen] = useState(false);

  useEffect(() => {
    // Simular carregamento
    setTimeout(() => {
      setLoading(false);
    }, 1000);
  }, []);

  const handleSave = async (secao) => {
    setSaving(true);
    // Simular salvamento
    setTimeout(() => {
      setSaving(false);
      // Mostrar notificação de sucesso
    }, 1000);
  };

  const handleTest = async (servico) => {
    setTesting(prev => ({ ...prev, [servico]: true }));
    // Simular teste de conexão
    setTimeout(() => {
      setTesting(prev => ({ ...prev, [servico]: false }));
      // Mostrar resultado do teste
    }, 2000);
  };

  const togglePassword = (campo) => {
    setShowPasswords(prev => ({ ...prev, [campo]: !prev[campo] }));
  };

  const updateConfig = (secao, campo, valor) => {
    setConfiguracoes(prev => ({
      ...prev,
      [secao]: {
        ...prev[secao],
        [campo]: valor
      }
    }));
  };

  const getStatusBadge = (status) => {
    const config = {
      'Conectado': { color: 'bg-green-100 text-green-800', icon: CheckCircle },
      'Desconectado': { color: 'bg-red-100 text-red-800', icon: XCircle },
      'Erro': { color: 'bg-red-100 text-red-800', icon: AlertTriangle },
      'Testando': { color: 'bg-yellow-100 text-yellow-800', icon: Loader2 }
    };
    
    const { color, icon: Icon } = config[status] || config['Desconectado'];
    
    return (
      <Badge className={color}>
        <Icon className="h-3 w-3 mr-1" />
        {status}
      </Badge>
    );
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="flex items-center space-x-2">
          <Loader2 className="h-6 w-6 animate-spin text-blue-600" />
          <span className="text-gray-600">Carregando configurações...</span>
        </div>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      {/* Cabeçalho */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">Configurações</h1>
          <p className="text-gray-600 mt-1">
            Gerencie configurações do sistema e integrações
          </p>
        </div>
        
        <div className="flex items-center gap-3">
          <Button variant="outline" onClick={() => setBackupModalOpen(true)}>
            <Download className="h-4 w-4 mr-2" />
            Backup
          </Button>
          <Button variant="outline" onClick={() => setRestoreModalOpen(true)}>
            <Upload className="h-4 w-4 mr-2" />
            Restaurar
          </Button>
        </div>
      </div>

      {/* Status das Integrações */}
      <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
        <Card>
          <CardContent className="p-4">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm font-medium text-gray-600">Sistema Senior</p>
                {getStatusBadge(configuracoes.senior.status)}
              </div>
              <Zap className="h-8 w-8 text-blue-600" />
            </div>
          </CardContent>
        </Card>
        
        <Card>
          <CardContent className="p-4">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm font-medium text-gray-600">Email SMTP</p>
                {getStatusBadge(configuracoes.email.status)}
              </div>
              <Mail className="h-8 w-8 text-green-600" />
            </div>
          </CardContent>
        </Card>
        
        <Card>
          <CardContent className="p-4">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm font-medium text-gray-600">AWS S3</p>
                {getStatusBadge(configuracoes.aws.status)}
              </div>
              <Cloud className="h-8 w-8 text-orange-600" />
            </div>
          </CardContent>
        </Card>
        
        <Card>
          <CardContent className="p-4">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm font-medium text-gray-600">Segurança</p>
                <Badge className="bg-blue-100 text-blue-800">
                  <Shield className="h-3 w-3 mr-1" />
                  Ativo
                </Badge>
              </div>
              <Shield className="h-8 w-8 text-purple-600" />
            </div>
          </CardContent>
        </Card>
      </div>

      {/* Tabs de Configurações */}
      <Tabs value={activeTab} onValueChange={setActiveTab}>
        <TabsList className="grid w-full grid-cols-6">
          <TabsTrigger value="sistema">Sistema</TabsTrigger>
          <TabsTrigger value="senior">Senior</TabsTrigger>
          <TabsTrigger value="email">Email</TabsTrigger>
          <TabsTrigger value="aws">AWS S3</TabsTrigger>
          <TabsTrigger value="seguranca">Segurança</TabsTrigger>
          <TabsTrigger value="notificacoes">Notificações</TabsTrigger>
        </TabsList>

        {/* Configurações do Sistema */}
        <TabsContent value="sistema">
          <Card>
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <Settings className="h-5 w-5" />
                Configurações do Sistema
              </CardTitle>
            </CardHeader>
            <CardContent className="space-y-6">
              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div>
                  <label className="text-sm font-medium text-gray-700 mb-2 block">
                    Nome da Aplicação
                  </label>
                  <Input
                    value={configuracoes.sistema.nomeAplicacao}
                    onChange={(e) => updateConfig('sistema', 'nomeAplicacao', e.target.value)}
                  />
                </div>
                <div>
                  <label className="text-sm font-medium text-gray-700 mb-2 block">
                    Versão
                  </label>
                  <Input
                    value={configuracoes.sistema.versao}
                    disabled
                    className="bg-gray-50"
                  />
                </div>
                <div>
                  <label className="text-sm font-medium text-gray-700 mb-2 block">
                    Ambiente
                  </label>
                  <select 
                    className="w-full p-2 border border-gray-300 rounded-md"
                    value={configuracoes.sistema.ambiente}
                    onChange={(e) => updateConfig('sistema', 'ambiente', e.target.value)}
                  >
                    <option value="Desenvolvimento">Desenvolvimento</option>
                    <option value="Homologação">Homologação</option>
                    <option value="Produção">Produção</option>
                  </select>
                </div>
                <div>
                  <label className="text-sm font-medium text-gray-700 mb-2 block">
                    Timeout de Sessão (minutos)
                  </label>
                  <Input
                    type="number"
                    value={configuracoes.sistema.sessionTimeout}
                    onChange={(e) => updateConfig('sistema', 'sessionTimeout', parseInt(e.target.value))}
                  />
                </div>
                <div>
                  <label className="text-sm font-medium text-gray-700 mb-2 block">
                    Tamanho Máximo de Arquivo (MB)
                  </label>
                  <Input
                    type="number"
                    value={configuracoes.sistema.maxFileSize}
                    onChange={(e) => updateConfig('sistema', 'maxFileSize', parseInt(e.target.value))}
                  />
                </div>
                <div>
                  <label className="text-sm font-medium text-gray-700 mb-2 block">
                    Retenção de Logs (dias)
                  </label>
                  <Input
                    type="number"
                    value={configuracoes.sistema.retencaoLogs}
                    onChange={(e) => updateConfig('sistema', 'retencaoLogs', parseInt(e.target.value))}
                  />
                </div>
              </div>
              
              <div>
                <label className="text-sm font-medium text-gray-700 mb-2 block">
                  Tipos de Arquivo Permitidos
                </label>
                <Input
                  value={configuracoes.sistema.allowedFileTypes}
                  onChange={(e) => updateConfig('sistema', 'allowedFileTypes', e.target.value)}
                  placeholder=".pdf,.docx,.pptx,.mp4,.mp3"
                />
              </div>

              <div className="space-y-4">
                <div className="flex items-center justify-between">
                  <div>
                    <label className="text-sm font-medium text-gray-700">Modo Manutenção</label>
                    <p className="text-xs text-gray-500">Bloqueia acesso ao sistema para manutenção</p>
                  </div>
                  <Switch
                    checked={configuracoes.sistema.manutencao}
                    onCheckedChange={(checked) => updateConfig('sistema', 'manutencao', checked)}
                  />
                </div>
                
                <div className="flex items-center justify-between">
                  <div>
                    <label className="text-sm font-medium text-gray-700">Backup Automático</label>
                    <p className="text-xs text-gray-500">Executa backup diário automaticamente</p>
                  </div>
                  <Switch
                    checked={configuracoes.sistema.backupAutomatico}
                    onCheckedChange={(checked) => updateConfig('sistema', 'backupAutomatico', checked)}
                  />
                </div>
              </div>

              <div className="flex justify-end">
                <Button onClick={() => handleSave('sistema')} disabled={saving}>
                  {saving ? <Loader2 className="h-4 w-4 mr-2 animate-spin" /> : <Save className="h-4 w-4 mr-2" />}
                  Salvar Configurações
                </Button>
              </div>
            </CardContent>
          </Card>
        </TabsContent>

        {/* Configurações Senior */}
        <TabsContent value="senior">
          <Card>
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <Zap className="h-5 w-5" />
                Integração Senior
              </CardTitle>
            </CardHeader>
            <CardContent className="space-y-6">
              <div className="flex items-center justify-between p-4 bg-blue-50 rounded-lg">
                <div>
                  <h3 className="font-medium text-blue-900">Status da Integração</h3>
                  <p className="text-sm text-blue-700">
                    Última sincronização: {new Date(configuracoes.senior.ultimaSincronizacao).toLocaleString('pt-BR')}
                  </p>
                </div>
                {getStatusBadge(configuracoes.senior.status)}
              </div>

              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div>
                  <label className="text-sm font-medium text-gray-700 mb-2 block">
                    Servidor
                  </label>
                  <Input
                    value={configuracoes.senior.servidor}
                    onChange={(e) => updateConfig('senior', 'servidor', e.target.value)}
                    placeholder="https://senior.empresa.com.br"
                  />
                </div>
                <div>
                  <label className="text-sm font-medium text-gray-700 mb-2 block">
                    Database
                  </label>
                  <Input
                    value={configuracoes.senior.database}
                    onChange={(e) => updateConfig('senior', 'database', e.target.value)}
                  />
                </div>
                <div>
                  <label className="text-sm font-medium text-gray-700 mb-2 block">
                    Usuário
                  </label>
                  <Input
                    value={configuracoes.senior.usuario}
                    onChange={(e) => updateConfig('senior', 'usuario', e.target.value)}
                  />
                </div>
                <div>
                  <label className="text-sm font-medium text-gray-700 mb-2 block">
                    Senha
                  </label>
                  <div className="relative">
                    <Input
                      type={showPasswords.senior ? 'text' : 'password'}
                      value={configuracoes.senior.senha}
                      onChange={(e) => updateConfig('senior', 'senha', e.target.value)}
                    />
                    <Button
                      type="button"
                      variant="ghost"
                      size="sm"
                      className="absolute right-0 top-0 h-full px-3"
                      onClick={() => togglePassword('senior')}
                    >
                      {showPasswords.senior ? <EyeOff className="h-4 w-4" /> : <Eye className="h-4 w-4" />}
                    </Button>
                  </div>
                </div>
                <div>
                  <label className="text-sm font-medium text-gray-700 mb-2 block">
                    Intervalo de Sincronização (minutos)
                  </label>
                  <Input
                    type="number"
                    value={configuracoes.senior.intervaloSincronizacao}
                    onChange={(e) => updateConfig('senior', 'intervaloSincronizacao', parseInt(e.target.value))}
                  />
                </div>
              </div>

              <div className="space-y-4">
                <div className="flex items-center justify-between">
                  <div>
                    <label className="text-sm font-medium text-gray-700">Integração Ativa</label>
                    <p className="text-xs text-gray-500">Habilita a integração com o sistema Senior</p>
                  </div>
                  <Switch
                    checked={configuracoes.senior.ativo}
                    onCheckedChange={(checked) => updateConfig('senior', 'ativo', checked)}
                  />
                </div>
                
                <div className="flex items-center justify-between">
                  <div>
                    <label className="text-sm font-medium text-gray-700">Sincronização Automática</label>
                    <p className="text-xs text-gray-500">Sincroniza dados automaticamente no intervalo definido</p>
                  </div>
                  <Switch
                    checked={configuracoes.senior.sincronizacaoAutomatica}
                    onCheckedChange={(checked) => updateConfig('senior', 'sincronizacaoAutomatica', checked)}
                  />
                </div>
              </div>

              <div className="grid grid-cols-2 gap-4 p-4 bg-gray-50 rounded-lg">
                <div>
                  <p className="text-sm font-medium text-gray-700">Cursos Importados</p>
                  <p className="text-2xl font-bold text-blue-600">{configuracoes.senior.cursosImportados}</p>
                </div>
                <div>
                  <p className="text-sm font-medium text-gray-700">Usuários Importados</p>
                  <p className="text-2xl font-bold text-green-600">{configuracoes.senior.usuariosImportados}</p>
                </div>
              </div>

              <div className="flex justify-between">
                <Button 
                  variant="outline" 
                  onClick={() => handleTest('senior')}
                  disabled={testing.senior}
                >
                  {testing.senior ? <Loader2 className="h-4 w-4 mr-2 animate-spin" /> : <TestTube className="h-4 w-4 mr-2" />}
                  Testar Conexão
                </Button>
                <Button onClick={() => handleSave('senior')} disabled={saving}>
                  {saving ? <Loader2 className="h-4 w-4 mr-2 animate-spin" /> : <Save className="h-4 w-4 mr-2" />}
                  Salvar Configurações
                </Button>
              </div>
            </CardContent>
          </Card>
        </TabsContent>

        {/* Configurações Email */}
        <TabsContent value="email">
          <Card>
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <Mail className="h-5 w-5" />
                Configurações de Email
              </CardTitle>
            </CardHeader>
            <CardContent className="space-y-6">
              <div className="flex items-center justify-between p-4 bg-green-50 rounded-lg">
                <div>
                  <h3 className="font-medium text-green-900">Status do Servidor SMTP</h3>
                  <p className="text-sm text-green-700">
                    Servidor: {configuracoes.email.servidor}:{configuracoes.email.porta}
                  </p>
                </div>
                {getStatusBadge(configuracoes.email.status)}
              </div>

              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div>
                  <label className="text-sm font-medium text-gray-700 mb-2 block">
                    Servidor SMTP
                  </label>
                  <Input
                    value={configuracoes.email.servidor}
                    onChange={(e) => updateConfig('email', 'servidor', e.target.value)}
                    placeholder="smtp.gmail.com"
                  />
                </div>
                <div>
                  <label className="text-sm font-medium text-gray-700 mb-2 block">
                    Porta
                  </label>
                  <Input
                    type="number"
                    value={configuracoes.email.porta}
                    onChange={(e) => updateConfig('email', 'porta', parseInt(e.target.value))}
                  />
                </div>
                <div>
                  <label className="text-sm font-medium text-gray-700 mb-2 block">
                    Usuário
                  </label>
                  <Input
                    value={configuracoes.email.usuario}
                    onChange={(e) => updateConfig('email', 'usuario', e.target.value)}
                  />
                </div>
                <div>
                  <label className="text-sm font-medium text-gray-700 mb-2 block">
                    Senha
                  </label>
                  <div className="relative">
                    <Input
                      type={showPasswords.email ? 'text' : 'password'}
                      value={configuracoes.email.senha}
                      onChange={(e) => updateConfig('email', 'senha', e.target.value)}
                    />
                    <Button
                      type="button"
                      variant="ghost"
                      size="sm"
                      className="absolute right-0 top-0 h-full px-3"
                      onClick={() => togglePassword('email')}
                    >
                      {showPasswords.email ? <EyeOff className="h-4 w-4" /> : <Eye className="h-4 w-4" />}
                    </Button>
                  </div>
                </div>
                <div className="col-span-2">
                  <label className="text-sm font-medium text-gray-700 mb-2 block">
                    Nome do Remetente
                  </label>
                  <Input
                    value={configuracoes.email.remetente}
                    onChange={(e) => updateConfig('email', 'remetente', e.target.value)}
                  />
                </div>
              </div>

              <div className="space-y-4">
                <div className="flex items-center justify-between">
                  <div>
                    <label className="text-sm font-medium text-gray-700">Email Ativo</label>
                    <p className="text-xs text-gray-500">Habilita o envio de emails pelo sistema</p>
                  </div>
                  <Switch
                    checked={configuracoes.email.ativo}
                    onCheckedChange={(checked) => updateConfig('email', 'ativo', checked)}
                  />
                </div>
                
                <div className="flex items-center justify-between">
                  <div>
                    <label className="text-sm font-medium text-gray-700">SSL/TLS</label>
                    <p className="text-xs text-gray-500">Usar conexão segura SSL/TLS</p>
                  </div>
                  <Switch
                    checked={configuracoes.email.ssl}
                    onCheckedChange={(checked) => updateConfig('email', 'ssl', checked)}
                  />
                </div>
                
                <div className="flex items-center justify-between">
                  <div>
                    <label className="text-sm font-medium text-gray-700">Template de Recuperação</label>
                    <p className="text-xs text-gray-500">Usar template personalizado para recuperação de senha</p>
                  </div>
                  <Switch
                    checked={configuracoes.email.templateRecuperacao}
                    onCheckedChange={(checked) => updateConfig('email', 'templateRecuperacao', checked)}
                  />
                </div>
              </div>

              <div className="flex justify-between">
                <Button 
                  variant="outline" 
                  onClick={() => handleTest('email')}
                  disabled={testing.email}
                >
                  {testing.email ? <Loader2 className="h-4 w-4 mr-2 animate-spin" /> : <TestTube className="h-4 w-4 mr-2" />}
                  Enviar Email de Teste
                </Button>
                <Button onClick={() => handleSave('email')} disabled={saving}>
                  {saving ? <Loader2 className="h-4 w-4 mr-2 animate-spin" /> : <Save className="h-4 w-4 mr-2" />}
                  Salvar Configurações
                </Button>
              </div>
            </CardContent>
          </Card>
        </TabsContent>

        {/* Configurações AWS */}
        <TabsContent value="aws">
          <Card>
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <Cloud className="h-5 w-5" />
                Configurações AWS S3
              </CardTitle>
            </CardHeader>
            <CardContent className="space-y-6">
              <div className="flex items-center justify-between p-4 bg-orange-50 rounded-lg">
                <div>
                  <h3 className="font-medium text-orange-900">Status do Armazenamento</h3>
                  <p className="text-sm text-orange-700">
                    Usado: {configuracoes.aws.espacoUsado} de {configuracoes.aws.espacoTotal}
                  </p>
                </div>
                {getStatusBadge(configuracoes.aws.status)}
              </div>

              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div>
                  <label className="text-sm font-medium text-gray-700 mb-2 block">
                    Access Key ID
                  </label>
                  <div className="relative">
                    <Input
                      type={showPasswords.awsAccess ? 'text' : 'password'}
                      value={configuracoes.aws.accessKey}
                      onChange={(e) => updateConfig('aws', 'accessKey', e.target.value)}
                    />
                    <Button
                      type="button"
                      variant="ghost"
                      size="sm"
                      className="absolute right-0 top-0 h-full px-3"
                      onClick={() => togglePassword('awsAccess')}
                    >
                      {showPasswords.awsAccess ? <EyeOff className="h-4 w-4" /> : <Eye className="h-4 w-4" />}
                    </Button>
                  </div>
                </div>
                <div>
                  <label className="text-sm font-medium text-gray-700 mb-2 block">
                    Secret Access Key
                  </label>
                  <div className="relative">
                    <Input
                      type={showPasswords.awsSecret ? 'text' : 'password'}
                      value={configuracoes.aws.secretKey}
                      onChange={(e) => updateConfig('aws', 'secretKey', e.target.value)}
                    />
                    <Button
                      type="button"
                      variant="ghost"
                      size="sm"
                      className="absolute right-0 top-0 h-full px-3"
                      onClick={() => togglePassword('awsSecret')}
                    >
                      {showPasswords.awsSecret ? <EyeOff className="h-4 w-4" /> : <Eye className="h-4 w-4" />}
                    </Button>
                  </div>
                </div>
                <div>
                  <label className="text-sm font-medium text-gray-700 mb-2 block">
                    Região
                  </label>
                  <select 
                    className="w-full p-2 border border-gray-300 rounded-md"
                    value={configuracoes.aws.region}
                    onChange={(e) => updateConfig('aws', 'region', e.target.value)}
                  >
                    <option value="us-east-1">US East (N. Virginia)</option>
                    <option value="us-west-2">US West (Oregon)</option>
                    <option value="sa-east-1">South America (São Paulo)</option>
                    <option value="eu-west-1">Europe (Ireland)</option>
                  </select>
                </div>
                <div>
                  <label className="text-sm font-medium text-gray-700 mb-2 block">
                    Bucket Name
                  </label>
                  <Input
                    value={configuracoes.aws.bucket}
                    onChange={(e) => updateConfig('aws', 'bucket', e.target.value)}
                  />
                </div>
                <div className="col-span-2">
                  <label className="text-sm font-medium text-gray-700 mb-2 block">
                    CloudFront URL
                  </label>
                  <Input
                    value={configuracoes.aws.cloudfront}
                    onChange={(e) => updateConfig('aws', 'cloudfront', e.target.value)}
                    placeholder="https://d123456789.cloudfront.net"
                  />
                </div>
              </div>

              <div className="space-y-4">
                <div className="flex items-center justify-between">
                  <div>
                    <label className="text-sm font-medium text-gray-700">AWS S3 Ativo</label>
                    <p className="text-xs text-gray-500">Habilita o armazenamento de arquivos no AWS S3</p>
                  </div>
                  <Switch
                    checked={configuracoes.aws.ativo}
                    onCheckedChange={(checked) => updateConfig('aws', 'ativo', checked)}
                  />
                </div>
              </div>

              <div className="flex justify-between">
                <Button 
                  variant="outline" 
                  onClick={() => handleTest('aws')}
                  disabled={testing.aws}
                >
                  {testing.aws ? <Loader2 className="h-4 w-4 mr-2 animate-spin" /> : <TestTube className="h-4 w-4 mr-2" />}
                  Testar Conexão
                </Button>
                <Button onClick={() => handleSave('aws')} disabled={saving}>
                  {saving ? <Loader2 className="h-4 w-4 mr-2 animate-spin" /> : <Save className="h-4 w-4 mr-2" />}
                  Salvar Configurações
                </Button>
              </div>
            </CardContent>
          </Card>
        </TabsContent>

        {/* Configurações de Segurança */}
        <TabsContent value="seguranca">
          <Card>
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <Shield className="h-5 w-5" />
                Configurações de Segurança
              </CardTitle>
            </CardHeader>
            <CardContent className="space-y-6">
              <Alert>
                <Shield className="h-4 w-4" />
                <AlertDescription>
                  Configurações de segurança afetam todos os usuários do sistema. Altere com cuidado.
                </AlertDescription>
              </Alert>

              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div>
                  <label className="text-sm font-medium text-gray-700 mb-2 block">
                    Expiração de Senha (dias)
                  </label>
                  <Input
                    type="number"
                    value={configuracoes.seguranca.senhaExpiracao}
                    onChange={(e) => updateConfig('seguranca', 'senhaExpiracao', parseInt(e.target.value))}
                  />
                </div>
                <div>
                  <label className="text-sm font-medium text-gray-700 mb-2 block">
                    Tentativas de Login
                  </label>
                  <Input
                    type="number"
                    value={configuracoes.seguranca.tentativasLogin}
                    onChange={(e) => updateConfig('seguranca', 'tentativasLogin', parseInt(e.target.value))}
                  />
                </div>
                <div>
                  <label className="text-sm font-medium text-gray-700 mb-2 block">
                    Tempo de Bloqueio (minutos)
                  </label>
                  <Input
                    type="number"
                    value={configuracoes.seguranca.bloqueioTempo}
                    onChange={(e) => updateConfig('seguranca', 'bloqueioTempo', parseInt(e.target.value))}
                  />
                </div>
                <div>
                  <label className="text-sm font-medium text-gray-700 mb-2 block">
                    Origens CORS Permitidas
                  </label>
                  <Input
                    value={configuracoes.seguranca.corsOrigins}
                    onChange={(e) => updateConfig('seguranca', 'corsOrigins', e.target.value)}
                  />
                </div>
              </div>

              <div className="space-y-4">
                <div className="flex items-center justify-between">
                  <div>
                    <label className="text-sm font-medium text-gray-700">Autenticação de Dois Fatores</label>
                    <p className="text-xs text-gray-500">Exige verificação adicional no login</p>
                  </div>
                  <Switch
                    checked={configuracoes.seguranca.autenticacaoDupla}
                    onCheckedChange={(checked) => updateConfig('seguranca', 'autenticacaoDupla', checked)}
                  />
                </div>
                
                <div className="flex items-center justify-between">
                  <div>
                    <label className="text-sm font-medium text-gray-700">Complexidade de Senha</label>
                    <p className="text-xs text-gray-500">Exige senhas com maiúsculas, números e símbolos</p>
                  </div>
                  <Switch
                    checked={configuracoes.seguranca.senhaComplexidade}
                    onCheckedChange={(checked) => updateConfig('seguranca', 'senhaComplexidade', checked)}
                  />
                </div>
                
                <div className="flex items-center justify-between">
                  <div>
                    <label className="text-sm font-medium text-gray-700">Log de Auditoria</label>
                    <p className="text-xs text-gray-500">Registra todas as ações dos usuários</p>
                  </div>
                  <Switch
                    checked={configuracoes.seguranca.logAuditoria}
                    onCheckedChange={(checked) => updateConfig('seguranca', 'logAuditoria', checked)}
                  />
                </div>
                
                <div className="flex items-center justify-between">
                  <div>
                    <label className="text-sm font-medium text-gray-700">Criptografia de Arquivos</label>
                    <p className="text-xs text-gray-500">Criptografa arquivos sensíveis no armazenamento</p>
                  </div>
                  <Switch
                    checked={configuracoes.seguranca.criptografiaArquivos}
                    onCheckedChange={(checked) => updateConfig('seguranca', 'criptografiaArquivos', checked)}
                  />
                </div>
                
                <div className="flex items-center justify-between">
                  <div>
                    <label className="text-sm font-medium text-gray-700">HTTPS Obrigatório</label>
                    <p className="text-xs text-gray-500">Força redirecionamento para HTTPS</p>
                  </div>
                  <Switch
                    checked={configuracoes.seguranca.httpsObrigatorio}
                    onCheckedChange={(checked) => updateConfig('seguranca', 'httpsObrigatorio', checked)}
                  />
                </div>
              </div>

              <div className="flex justify-end">
                <Button onClick={() => handleSave('seguranca')} disabled={saving}>
                  {saving ? <Loader2 className="h-4 w-4 mr-2 animate-spin" /> : <Save className="h-4 w-4 mr-2" />}
                  Salvar Configurações
                </Button>
              </div>
            </CardContent>
          </Card>
        </TabsContent>

        {/* Configurações de Notificações */}
        <TabsContent value="notificacoes">
          <Card>
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <Bell className="h-5 w-5" />
                Configurações de Notificações
              </CardTitle>
            </CardHeader>
            <CardContent className="space-y-6">
              <div className="space-y-4">
                <div className="flex items-center justify-between">
                  <div>
                    <label className="text-sm font-medium text-gray-700">Notificações por Email</label>
                    <p className="text-xs text-gray-500">Enviar notificações via email</p>
                  </div>
                  <Switch
                    checked={configuracoes.notificacoes.emailAtivo}
                    onCheckedChange={(checked) => updateConfig('notificacoes', 'emailAtivo', checked)}
                  />
                </div>
                
                <div className="flex items-center justify-between">
                  <div>
                    <label className="text-sm font-medium text-gray-700">Notificações SMS</label>
                    <p className="text-xs text-gray-500">Enviar notificações via SMS</p>
                  </div>
                  <Switch
                    checked={configuracoes.notificacoes.smsAtivo}
                    onCheckedChange={(checked) => updateConfig('notificacoes', 'smsAtivo', checked)}
                  />
                </div>
                
                <div className="flex items-center justify-between">
                  <div>
                    <label className="text-sm font-medium text-gray-700">Notificações Push</label>
                    <p className="text-xs text-gray-500">Enviar notificações push no navegador</p>
                  </div>
                  <Switch
                    checked={configuracoes.notificacoes.pushAtivo}
                    onCheckedChange={(checked) => updateConfig('notificacoes', 'pushAtivo', checked)}
                  />
                </div>
              </div>

              <hr />

              <div className="space-y-4">
                <h3 className="font-medium text-gray-900">Eventos para Notificação</h3>
                
                <div className="flex items-center justify-between">
                  <div>
                    <label className="text-sm font-medium text-gray-700">Login de Usuário</label>
                    <p className="text-xs text-gray-500">Notificar quando usuário faz login</p>
                  </div>
                  <Switch
                    checked={configuracoes.notificacoes.notificarLogin}
                    onCheckedChange={(checked) => updateConfig('notificacoes', 'notificarLogin', checked)}
                  />
                </div>
                
                <div className="flex items-center justify-between">
                  <div>
                    <label className="text-sm font-medium text-gray-700">Upload de Arquivo</label>
                    <p className="text-xs text-gray-500">Notificar quando arquivo é enviado</p>
                  </div>
                  <Switch
                    checked={configuracoes.notificacoes.notificarUpload}
                    onCheckedChange={(checked) => updateConfig('notificacoes', 'notificarUpload', checked)}
                  />
                </div>
                
                <div className="flex items-center justify-between">
                  <div>
                    <label className="text-sm font-medium text-gray-700">Erros do Sistema</label>
                    <p className="text-xs text-gray-500">Notificar quando ocorrem erros</p>
                  </div>
                  <Switch
                    checked={configuracoes.notificacoes.notificarErros}
                    onCheckedChange={(checked) => updateConfig('notificacoes', 'notificarErros', checked)}
                  />
                </div>
                
                <div className="flex items-center justify-between">
                  <div>
                    <label className="text-sm font-medium text-gray-700">Backup Concluído</label>
                    <p className="text-xs text-gray-500">Notificar quando backup é concluído</p>
                  </div>
                  <Switch
                    checked={configuracoes.notificacoes.notificarBackup}
                    onCheckedChange={(checked) => updateConfig('notificacoes', 'notificarBackup', checked)}
                  />
                </div>
                
                <div className="flex items-center justify-between">
                  <div>
                    <label className="text-sm font-medium text-gray-700">Sincronização Senior</label>
                    <p className="text-xs text-gray-500">Notificar sobre sincronização com Senior</p>
                  </div>
                  <Switch
                    checked={configuracoes.notificacoes.notificarSincronizacao}
                    onCheckedChange={(checked) => updateConfig('notificacoes', 'notificarSincronizacao', checked)}
                  />
                </div>
              </div>

              <div className="flex justify-end">
                <Button onClick={() => handleSave('notificacoes')} disabled={saving}>
                  {saving ? <Loader2 className="h-4 w-4 mr-2 animate-spin" /> : <Save className="h-4 w-4 mr-2" />}
                  Salvar Configurações
                </Button>
              </div>
            </CardContent>
          </Card>
        </TabsContent>
      </Tabs>

      {/* Modal de Backup */}
      <Dialog open={backupModalOpen} onOpenChange={setBackupModalOpen}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle className="flex items-center gap-2">
              <Download className="h-5 w-5" />
              Backup do Sistema
            </DialogTitle>
            <DialogDescription>
              Gere um backup completo do sistema incluindo dados e configurações
            </DialogDescription>
          </DialogHeader>
          <div className="space-y-4">
            <div className="space-y-2">
              <label className="text-sm font-medium">Incluir no Backup:</label>
              <div className="space-y-2">
                <div className="flex items-center space-x-2">
                  <input type="checkbox" id="backup-db" defaultChecked />
                  <label htmlFor="backup-db" className="text-sm">Banco de Dados</label>
                </div>
                <div className="flex items-center space-x-2">
                  <input type="checkbox" id="backup-files" defaultChecked />
                  <label htmlFor="backup-files" className="text-sm">Arquivos de Cursos</label>
                </div>
                <div className="flex items-center space-x-2">
                  <input type="checkbox" id="backup-config" defaultChecked />
                  <label htmlFor="backup-config" className="text-sm">Configurações</label>
                </div>
                <div className="flex items-center space-x-2">
                  <input type="checkbox" id="backup-logs" />
                  <label htmlFor="backup-logs" className="text-sm">Logs do Sistema</label>
                </div>
              </div>
            </div>
          </div>
          <DialogFooter>
            <Button variant="outline" onClick={() => setBackupModalOpen(false)}>
              Cancelar
            </Button>
            <Button>
              <Download className="h-4 w-4 mr-2" />
              Gerar Backup
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>

      {/* Modal de Restore */}
      <Dialog open={restoreModalOpen} onOpenChange={setRestoreModalOpen}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle className="flex items-center gap-2">
              <Upload className="h-5 w-5" />
              Restaurar Sistema
            </DialogTitle>
            <DialogDescription>
              Restaure o sistema a partir de um arquivo de backup
            </DialogDescription>
          </DialogHeader>
          <div className="space-y-4">
            <Alert className="border-yellow-200 bg-yellow-50">
              <AlertTriangle className="h-4 w-4 text-yellow-600" />
              <AlertDescription className="text-yellow-800">
                Esta operação irá sobrescrever os dados atuais. Certifique-se de ter um backup recente.
              </AlertDescription>
            </Alert>
            <div>
              <label className="text-sm font-medium">Arquivo de Backup:</label>
              <Input type="file" accept=".zip,.tar.gz" className="mt-2" />
            </div>
          </div>
          <DialogFooter>
            <Button variant="outline" onClick={() => setRestoreModalOpen(false)}>
              Cancelar
            </Button>
            <Button variant="destructive">
              <Upload className="h-4 w-4 mr-2" />
              Restaurar
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  );
};

export default Configuracoes;

