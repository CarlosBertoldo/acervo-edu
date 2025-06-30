import React, { useState, useEffect } from 'react';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/textarea';
import { Badge } from '@/components/ui/badge';
import { Switch } from '@/components/ui/switch';
import { 
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
  DialogFooter,
} from '@/components/ui/dialog';
import { 
  Tabs,
  TabsContent,
  TabsList,
  TabsTrigger,
} from '@/components/ui/tabs';
import { 
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import { Alert, AlertDescription } from '@/components/ui/alert';
import { 
  Share2, 
  Link, 
  Code, 
  Mail, 
  Copy, 
  Check,
  Calendar,
  Shield,
  Globe,
  Lock,
  Eye,
  EyeOff,
  Users,
  Clock,
  Download,
  ExternalLink,
  Settings,
  Info,
  AlertTriangle,
  CheckCircle,
  QrCode
} from 'lucide-react';

const ShareModal = ({ 
  isOpen, 
  onClose, 
  arquivo,
  onShare
}) => {
  const [activeTab, setActiveTab] = useState('link');
  const [shareConfig, setShareConfig] = useState({
    tipo: 'publico', // publico, restrito, privado
    expiracao: '', // data de expiração
    senha: '',
    dominiosPermitidos: '',
    limitarDownloads: false,
    maxDownloads: 10,
    limitarVisualizacoes: false,
    maxVisualizacoes: 100,
    notificarAcesso: false,
    emailNotificacao: '',
    permitirCompartilhamento: true,
    mostrarInformacoes: true,
    bloqueioContexto: false,
    bloqueioDownload: false,
    watermark: false
  });
  
  const [linkGerado, setLinkGerado] = useState('');
  const [codigoEmbed, setCodigoEmbed] = useState('');
  const [qrCode, setQrCode] = useState('');
  const [copied, setCopied] = useState('');
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (arquivo && isOpen) {
      gerarLink();
    }
  }, [arquivo, isOpen, shareConfig]);

  const gerarLink = () => {
    if (!arquivo) return;

    // Simular geração de link
    const baseUrl = 'https://acervo.ferreiracosta.com/share';
    const params = new URLSearchParams();
    
    if (shareConfig.senha) params.append('p', 'protected');
    if (shareConfig.expiracao) params.append('exp', shareConfig.expiracao);
    if (shareConfig.tipo === 'restrito') params.append('type', 'restricted');
    
    const hash = Math.random().toString(36).substring(2, 15);
    const link = `${baseUrl}/${hash}${params.toString() ? '?' + params.toString() : ''}`;
    
    setLinkGerado(link);

    // Gerar código embed
    const embedCode = `<iframe 
  src="${link}/embed" 
  width="800" 
  height="600" 
  frameborder="0" 
  allowfullscreen
  ${shareConfig.dominiosPermitidos ? `data-allowed-domains="${shareConfig.dominiosPermitidos}"` : ''}
></iframe>`;
    
    setCodigoEmbed(embedCode);

    // Simular QR Code
    setQrCode(`data:image/svg+xml;base64,${btoa(`<svg width="200" height="200" xmlns="http://www.w3.org/2000/svg"><rect width="200" height="200" fill="white"/><text x="100" y="100" text-anchor="middle" fill="black">QR Code</text></svg>`)}`);
  };

  const handleConfigChange = (key, value) => {
    setShareConfig(prev => ({ ...prev, [key]: value }));
  };

  const copyToClipboard = async (text, type) => {
    try {
      await navigator.clipboard.writeText(text);
      setCopied(type);
      setTimeout(() => setCopied(''), 2000);
    } catch (err) {
      console.error('Erro ao copiar:', err);
    }
  };

  const handleShare = async () => {
    setLoading(true);
    
    try {
      // Simular API call
      await new Promise(resolve => setTimeout(resolve, 1000));
      
      const shareData = {
        arquivo: arquivo.id,
        link: linkGerado,
        config: shareConfig,
        createdAt: new Date().toISOString()
      };

      if (onShare) {
        onShare(shareData);
      }

      onClose();
    } catch (error) {
      console.error('Erro ao compartilhar:', error);
    } finally {
      setLoading(false);
    }
  };

  const getTipoColor = (tipo) => {
    const colors = {
      'publico': 'bg-green-100 text-green-800',
      'restrito': 'bg-yellow-100 text-yellow-800',
      'privado': 'bg-red-100 text-red-800'
    };
    return colors[tipo] || 'bg-gray-100 text-gray-800';
  };

  const getTipoIcon = (tipo) => {
    const icons = {
      'publico': Globe,
      'restrito': Users,
      'privado': Lock
    };
    return icons[tipo] || Lock;
  };

  if (!arquivo) return null;

  return (
    <Dialog open={isOpen} onOpenChange={onClose}>
      <DialogContent className="max-w-4xl max-h-[90vh] overflow-y-auto">
        <DialogHeader>
          <DialogTitle className="flex items-center gap-2">
            <Share2 className="h-5 w-5" />
            Compartilhar Arquivo
          </DialogTitle>
          <DialogDescription>
            Configure as opções de compartilhamento para: <strong>{arquivo.nome}</strong>
          </DialogDescription>
        </DialogHeader>

        <Tabs value={activeTab} onValueChange={setActiveTab}>
          <TabsList className="grid w-full grid-cols-4">
            <TabsTrigger value="link">Link Público</TabsTrigger>
            <TabsTrigger value="embed">Código Embed</TabsTrigger>
            <TabsTrigger value="email">Enviar por Email</TabsTrigger>
            <TabsTrigger value="config">Configurações</TabsTrigger>
          </TabsList>

          {/* Configurações de Acesso */}
          <div className="my-4 p-4 bg-gray-50 rounded-lg">
            <div className="flex items-center justify-between mb-4">
              <h3 className="font-medium text-gray-900">Tipo de Acesso</h3>
              <Badge className={getTipoColor(shareConfig.tipo)}>
                {React.createElement(getTipoIcon(shareConfig.tipo), { className: "h-3 w-3 mr-1" })}
                {shareConfig.tipo.charAt(0).toUpperCase() + shareConfig.tipo.slice(1)}
              </Badge>
            </div>
            
            <div className="grid grid-cols-3 gap-2">
              <Button
                variant={shareConfig.tipo === 'publico' ? 'default' : 'outline'}
                size="sm"
                onClick={() => handleConfigChange('tipo', 'publico')}
                className="flex items-center gap-2"
              >
                <Globe className="h-4 w-4" />
                Público
              </Button>
              <Button
                variant={shareConfig.tipo === 'restrito' ? 'default' : 'outline'}
                size="sm"
                onClick={() => handleConfigChange('tipo', 'restrito')}
                className="flex items-center gap-2"
              >
                <Users className="h-4 w-4" />
                Restrito
              </Button>
              <Button
                variant={shareConfig.tipo === 'privado' ? 'default' : 'outline'}
                size="sm"
                onClick={() => handleConfigChange('tipo', 'privado')}
                className="flex items-center gap-2"
              >
                <Lock className="h-4 w-4" />
                Privado
              </Button>
            </div>
          </div>

          {/* Tab: Link Público */}
          <TabsContent value="link" className="space-y-4">
            <div>
              <label className="text-sm font-medium text-gray-700 mb-2 block">
                Link de Compartilhamento
              </label>
              <div className="flex gap-2">
                <Input
                  value={linkGerado}
                  readOnly
                  className="flex-1"
                />
                <Button
                  variant="outline"
                  onClick={() => copyToClipboard(linkGerado, 'link')}
                >
                  {copied === 'link' ? <Check className="h-4 w-4" /> : <Copy className="h-4 w-4" />}
                </Button>
              </div>
              <p className="text-xs text-gray-500 mt-1">
                Este link permite acesso direto ao arquivo conforme as configurações definidas
              </p>
            </div>

            {/* QR Code */}
            <div className="text-center">
              <h4 className="font-medium text-gray-900 mb-2">QR Code</h4>
              <div className="inline-block p-4 bg-white border rounded-lg">
                <img src={qrCode} alt="QR Code" className="w-32 h-32" />
              </div>
              <p className="text-xs text-gray-500 mt-2">
                Escaneie para acessar o arquivo
              </p>
            </div>

            {/* Estatísticas de Acesso */}
            <div className="grid grid-cols-2 gap-4 p-4 bg-blue-50 rounded-lg">
              <div className="text-center">
                <p className="text-2xl font-bold text-blue-600">{arquivo.visualizacoes || 0}</p>
                <p className="text-sm text-blue-700">Visualizações</p>
              </div>
              <div className="text-center">
                <p className="text-2xl font-bold text-green-600">{arquivo.downloads || 0}</p>
                <p className="text-sm text-green-700">Downloads</p>
              </div>
            </div>
          </TabsContent>

          {/* Tab: Código Embed */}
          <TabsContent value="embed" className="space-y-4">
            <div>
              <label className="text-sm font-medium text-gray-700 mb-2 block">
                Código HTML para Incorporação
              </label>
              <div className="relative">
                <Textarea
                  value={codigoEmbed}
                  readOnly
                  rows={6}
                  className="font-mono text-sm"
                />
                <Button
                  variant="outline"
                  size="sm"
                  className="absolute top-2 right-2"
                  onClick={() => copyToClipboard(codigoEmbed, 'embed')}
                >
                  {copied === 'embed' ? <Check className="h-4 w-4" /> : <Copy className="h-4 w-4" />}
                </Button>
              </div>
              <p className="text-xs text-gray-500 mt-1">
                Cole este código em qualquer página HTML para incorporar o arquivo
              </p>
            </div>

            {/* Configurações do Embed */}
            <div className="space-y-4 p-4 border rounded-lg">
              <h4 className="font-medium text-gray-900">Configurações do Embed</h4>
              
              <div>
                <label className="text-sm font-medium text-gray-700 mb-2 block">
                  Domínios Permitidos
                </label>
                <Input
                  value={shareConfig.dominiosPermitidos}
                  onChange={(e) => handleConfigChange('dominiosPermitidos', e.target.value)}
                  placeholder="exemplo.com, outrosite.com.br"
                />
                <p className="text-xs text-gray-500 mt-1">
                  Deixe vazio para permitir qualquer domínio
                </p>
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div className="flex items-center justify-between">
                  <div>
                    <label className="text-sm font-medium text-gray-700">Bloquear Download</label>
                    <p className="text-xs text-gray-500">Impede download do arquivo</p>
                  </div>
                  <Switch
                    checked={shareConfig.bloqueioDownload}
                    onCheckedChange={(checked) => handleConfigChange('bloqueioDownload', checked)}
                  />
                </div>

                <div className="flex items-center justify-between">
                  <div>
                    <label className="text-sm font-medium text-gray-700">Watermark</label>
                    <p className="text-xs text-gray-500">Adiciona marca d'água</p>
                  </div>
                  <Switch
                    checked={shareConfig.watermark}
                    onCheckedChange={(checked) => handleConfigChange('watermark', checked)}
                  />
                </div>
              </div>
            </div>
          </TabsContent>

          {/* Tab: Enviar por Email */}
          <TabsContent value="email" className="space-y-4">
            <div className="grid grid-cols-2 gap-4">
              <div>
                <label className="text-sm font-medium text-gray-700 mb-2 block">
                  Para (emails)
                </label>
                <Textarea
                  placeholder="email1@exemplo.com, email2@exemplo.com"
                  rows={3}
                />
              </div>
              <div>
                <label className="text-sm font-medium text-gray-700 mb-2 block">
                  Assunto
                </label>
                <Input
                  defaultValue={`Compartilhamento: ${arquivo.nome}`}
                />
              </div>
            </div>

            <div>
              <label className="text-sm font-medium text-gray-700 mb-2 block">
                Mensagem
              </label>
              <Textarea
                placeholder="Olá! Estou compartilhando este arquivo com você..."
                rows={4}
              />
            </div>

            <div className="flex items-center justify-between p-4 bg-gray-50 rounded-lg">
              <div>
                <label className="text-sm font-medium text-gray-700">Incluir Link de Download</label>
                <p className="text-xs text-gray-500">Adiciona link direto para download</p>
              </div>
              <Switch defaultChecked />
            </div>

            <Button className="w-full">
              <Mail className="h-4 w-4 mr-2" />
              Enviar por Email
            </Button>
          </TabsContent>

          {/* Tab: Configurações Avançadas */}
          <TabsContent value="config" className="space-y-6">
            {/* Expiração */}
            <div className="space-y-4">
              <h4 className="font-medium text-gray-900 border-b pb-2">Expiração</h4>
              
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="text-sm font-medium text-gray-700 mb-2 block">
                    Data de Expiração
                  </label>
                  <Input
                    type="datetime-local"
                    value={shareConfig.expiracao}
                    onChange={(e) => handleConfigChange('expiracao', e.target.value)}
                  />
                </div>
                <div>
                  <label className="text-sm font-medium text-gray-700 mb-2 block">
                    Senha de Acesso
                  </label>
                  <Input
                    type="password"
                    value={shareConfig.senha}
                    onChange={(e) => handleConfigChange('senha', e.target.value)}
                    placeholder="Deixe vazio para sem senha"
                  />
                </div>
              </div>
            </div>

            {/* Limites */}
            <div className="space-y-4">
              <h4 className="font-medium text-gray-900 border-b pb-2">Limites de Acesso</h4>
              
              <div className="grid grid-cols-2 gap-4">
                <div className="space-y-2">
                  <div className="flex items-center justify-between">
                    <label className="text-sm font-medium text-gray-700">Limitar Downloads</label>
                    <Switch
                      checked={shareConfig.limitarDownloads}
                      onCheckedChange={(checked) => handleConfigChange('limitarDownloads', checked)}
                    />
                  </div>
                  {shareConfig.limitarDownloads && (
                    <Input
                      type="number"
                      value={shareConfig.maxDownloads}
                      onChange={(e) => handleConfigChange('maxDownloads', parseInt(e.target.value))}
                      placeholder="Máximo de downloads"
                    />
                  )}
                </div>

                <div className="space-y-2">
                  <div className="flex items-center justify-between">
                    <label className="text-sm font-medium text-gray-700">Limitar Visualizações</label>
                    <Switch
                      checked={shareConfig.limitarVisualizacoes}
                      onCheckedChange={(checked) => handleConfigChange('limitarVisualizacoes', checked)}
                    />
                  </div>
                  {shareConfig.limitarVisualizacoes && (
                    <Input
                      type="number"
                      value={shareConfig.maxVisualizacoes}
                      onChange={(e) => handleConfigChange('maxVisualizacoes', parseInt(e.target.value))}
                      placeholder="Máximo de visualizações"
                    />
                  )}
                </div>
              </div>
            </div>

            {/* Notificações */}
            <div className="space-y-4">
              <h4 className="font-medium text-gray-900 border-b pb-2">Notificações</h4>
              
              <div className="space-y-3">
                <div className="flex items-center justify-between">
                  <div>
                    <label className="text-sm font-medium text-gray-700">Notificar Acessos</label>
                    <p className="text-xs text-gray-500">Receba email quando alguém acessar</p>
                  </div>
                  <Switch
                    checked={shareConfig.notificarAcesso}
                    onCheckedChange={(checked) => handleConfigChange('notificarAcesso', checked)}
                  />
                </div>

                {shareConfig.notificarAcesso && (
                  <Input
                    type="email"
                    value={shareConfig.emailNotificacao}
                    onChange={(e) => handleConfigChange('emailNotificacao', e.target.value)}
                    placeholder="Email para notificações"
                  />
                )}
              </div>
            </div>

            {/* Segurança */}
            <div className="space-y-4">
              <h4 className="font-medium text-gray-900 border-b pb-2">Segurança</h4>
              
              <div className="space-y-3">
                <div className="flex items-center justify-between">
                  <div>
                    <label className="text-sm font-medium text-gray-700">Bloquear Menu de Contexto</label>
                    <p className="text-xs text-gray-500">Impede clique direito no arquivo</p>
                  </div>
                  <Switch
                    checked={shareConfig.bloqueioContexto}
                    onCheckedChange={(checked) => handleConfigChange('bloqueioContexto', checked)}
                  />
                </div>

                <div className="flex items-center justify-between">
                  <div>
                    <label className="text-sm font-medium text-gray-700">Mostrar Informações</label>
                    <p className="text-xs text-gray-500">Exibe detalhes do arquivo</p>
                  </div>
                  <Switch
                    checked={shareConfig.mostrarInformacoes}
                    onCheckedChange={(checked) => handleConfigChange('mostrarInformacoes', checked)}
                  />
                </div>

                <div className="flex items-center justify-between">
                  <div>
                    <label className="text-sm font-medium text-gray-700">Permitir Recompartilhamento</label>
                    <p className="text-xs text-gray-500">Usuários podem compartilhar novamente</p>
                  </div>
                  <Switch
                    checked={shareConfig.permitirCompartilhamento}
                    onCheckedChange={(checked) => handleConfigChange('permitirCompartilhamento', checked)}
                  />
                </div>
              </div>
            </div>
          </TabsContent>
        </Tabs>

        {/* Alertas */}
        {shareConfig.tipo === 'publico' && (
          <Alert>
            <Info className="h-4 w-4" />
            <AlertDescription>
              Link público: qualquer pessoa com o link poderá acessar o arquivo.
            </AlertDescription>
          </Alert>
        )}

        {shareConfig.expiracao && (
          <Alert>
            <Clock className="h-4 w-4" />
            <AlertDescription>
              Este link expirará em {new Date(shareConfig.expiracao).toLocaleString('pt-BR')}.
            </AlertDescription>
          </Alert>
        )}

        <DialogFooter className="flex gap-2">
          <Button variant="outline" onClick={onClose}>
            Cancelar
          </Button>
          <Button onClick={handleShare} disabled={loading}>
            {loading ? (
              <>
                <div className="animate-spin rounded-full h-4 w-4 border-b-2 border-white mr-2"></div>
                Gerando...
              </>
            ) : (
              <>
                <Share2 className="h-4 w-4 mr-2" />
                Compartilhar
              </>
            )}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
};

export default ShareModal;

