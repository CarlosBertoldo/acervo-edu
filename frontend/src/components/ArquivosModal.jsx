import React, { useState, useEffect } from 'react';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Badge } from '@/components/ui/badge';
import { 
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog';
import { 
  Tabs,
  TabsContent,
  TabsList,
  TabsTrigger,
} from '@/components/ui/tabs';
import { 
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import { Alert, AlertDescription } from '@/components/ui/alert';
import { 
  FileText, 
  Video, 
  Music, 
  Image, 
  Download, 
  Share2, 
  Eye, 
  MoreVertical,
  Upload,
  Search,
  Filter,
  Trash2,
  Edit,
  Copy,
  ExternalLink,
  Shield,
  Clock,
  User,
  Calendar,
  FileIcon,
  Play,
  Pause,
  Volume2,
  Maximize,
  X,
  CheckCircle,
  AlertTriangle,
  Info,
  Loader2,
  Plus,
  FolderOpen
} from 'lucide-react';

// Dados mockados de arquivos
const mockArquivos = {
  'Briefing de Desenvolvimento': [
    {
      id: 1,
      nome: 'briefing_photoshop_basico.pdf',
      tipo: 'pdf',
      tamanho: '2.5 MB',
      dataUpload: '2024-01-25T10:00:00Z',
      uploadPor: 'João Silva',
      categoria: 'Briefing de Desenvolvimento',
      url: '/files/briefing_photoshop_basico.pdf',
      compartilhado: true,
      linkPublico: 'https://acervo.fc.com/share/abc123',
      downloads: 15,
      visualizacoes: 45,
      protegido: true
    },
    {
      id: 2,
      nome: 'requisitos_tecnicos.docx',
      tipo: 'docx',
      tamanho: '1.2 MB',
      dataUpload: '2024-01-24T14:30:00Z',
      uploadPor: 'Maria Santos',
      categoria: 'Briefing de Desenvolvimento',
      url: '/files/requisitos_tecnicos.docx',
      compartilhado: false,
      downloads: 8,
      visualizacoes: 22,
      protegido: false
    }
  ],
  'Briefing de Execução': [
    {
      id: 3,
      nome: 'cronograma_execucao.pdf',
      tipo: 'pdf',
      tamanho: '1.8 MB',
      dataUpload: '2024-01-26T09:15:00Z',
      uploadPor: 'Carlos Oliveira',
      categoria: 'Briefing de Execução',
      url: '/files/cronograma_execucao.pdf',
      compartilhado: true,
      linkPublico: 'https://acervo.fc.com/share/def456',
      downloads: 12,
      visualizacoes: 38,
      protegido: true
    }
  ],
  'PPT': [
    {
      id: 4,
      nome: 'apresentacao_modulo_01.pptx',
      tipo: 'pptx',
      tamanho: '15.3 MB',
      dataUpload: '2024-01-27T11:20:00Z',
      uploadPor: 'Ana Costa',
      categoria: 'PPT',
      url: '/files/apresentacao_modulo_01.pptx',
      compartilhado: true,
      linkPublico: 'https://acervo.fc.com/share/ghi789',
      downloads: 25,
      visualizacoes: 67,
      protegido: true
    },
    {
      id: 5,
      nome: 'slides_ferramentas_basicas.pptx',
      tipo: 'pptx',
      tamanho: '12.7 MB',
      dataUpload: '2024-01-28T16:45:00Z',
      uploadPor: 'Pedro Lima',
      categoria: 'PPT',
      url: '/files/slides_ferramentas_basicas.pptx',
      compartilhado: false,
      downloads: 18,
      visualizacoes: 41,
      protegido: false
    }
  ],
  'Vídeos': [
    {
      id: 6,
      nome: 'aula_01_introducao.mp4',
      tipo: 'mp4',
      tamanho: '125.6 MB',
      duracao: '15:30',
      dataUpload: '2024-01-29T08:00:00Z',
      uploadPor: 'Fernanda Costa',
      categoria: 'Vídeos',
      url: '/files/aula_01_introducao.mp4',
      thumbnail: '/thumbnails/aula_01.jpg',
      compartilhado: true,
      linkPublico: 'https://acervo.fc.com/share/jkl012',
      downloads: 45,
      visualizacoes: 156,
      protegido: true,
      restricoes: {
        naoAvancar: true,
        bloqueioDownload: true,
        bloqueioContexto: true
      }
    },
    {
      id: 7,
      nome: 'aula_02_ferramentas.mp4',
      tipo: 'mp4',
      tamanho: '98.2 MB',
      duracao: '12:45',
      dataUpload: '2024-01-29T10:30:00Z',
      uploadPor: 'Lucas Ferreira',
      categoria: 'Vídeos',
      url: '/files/aula_02_ferramentas.mp4',
      thumbnail: '/thumbnails/aula_02.jpg',
      compartilhado: false,
      downloads: 32,
      visualizacoes: 89,
      protegido: true,
      restricoes: {
        naoAvancar: true,
        bloqueioDownload: true,
        bloqueioContexto: true
      }
    }
  ],
  'Podcast': [
    {
      id: 8,
      nome: 'entrevista_especialista.mp3',
      tipo: 'mp3',
      tamanho: '45.2 MB',
      duracao: '32:15',
      dataUpload: '2024-01-28T13:20:00Z',
      uploadPor: 'Juliana Alves',
      categoria: 'Podcast',
      url: '/files/entrevista_especialista.mp3',
      compartilhado: true,
      linkPublico: 'https://acervo.fc.com/share/mno345',
      downloads: 28,
      visualizacoes: 73,
      protegido: false
    }
  ],
  'Outros Arquivos': [
    {
      id: 9,
      nome: 'recursos_extras.zip',
      tipo: 'zip',
      tamanho: '67.8 MB',
      dataUpload: '2024-01-27T15:10:00Z',
      uploadPor: 'Roberto Silva',
      categoria: 'Outros Arquivos',
      url: '/files/recursos_extras.zip',
      compartilhado: false,
      downloads: 19,
      visualizacoes: 34,
      protegido: false
    }
  ]
};

const ArquivosModal = ({ 
  isOpen, 
  onClose, 
  curso 
}) => {
  const [arquivos, setArquivos] = useState(mockArquivos);
  const [activeTab, setActiveTab] = useState('Briefing de Desenvolvimento');
  const [searchTerm, setSearchTerm] = useState('');
  const [uploading, setUploading] = useState(false);
  const [selectedFiles, setSelectedFiles] = useState([]);

  const categorias = [
    'Briefing de Desenvolvimento',
    'Briefing de Execução', 
    'PPT',
    'Caderno de Exercício',
    'Plano de Aula',
    'Vídeos',
    'Podcast',
    'Outros Arquivos'
  ];

  const getFileIcon = (tipo) => {
    const icons = {
      'pdf': FileText,
      'docx': FileText,
      'pptx': FileText,
      'mp4': Video,
      'mp3': Music,
      'jpg': Image,
      'png': Image,
      'zip': FolderOpen
    };
    return icons[tipo] || FileIcon;
  };

  const getFileTypeColor = (tipo) => {
    const colors = {
      'pdf': 'bg-red-100 text-red-800',
      'docx': 'bg-blue-100 text-blue-800',
      'pptx': 'bg-orange-100 text-orange-800',
      'mp4': 'bg-purple-100 text-purple-800',
      'mp3': 'bg-green-100 text-green-800',
      'jpg': 'bg-pink-100 text-pink-800',
      'png': 'bg-pink-100 text-pink-800',
      'zip': 'bg-gray-100 text-gray-800'
    };
    return colors[tipo] || 'bg-gray-100 text-gray-800';
  };

  const formatDate = (dateString) => {
    return new Date(dateString).toLocaleDateString('pt-BR', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  };

  const handleFileUpload = async (files, categoria) => {
    setUploading(true);
    
    try {
      // Simulate file upload
      await new Promise(resolve => setTimeout(resolve, 2000));
      
      const newFiles = Array.from(files).map((file, index) => ({
        id: Date.now() + index,
        nome: file.name,
        tipo: file.name.split('.').pop().toLowerCase(),
        tamanho: `${(file.size / 1024 / 1024).toFixed(1)} MB`,
        dataUpload: new Date().toISOString(),
        uploadPor: 'Usuário Demo',
        categoria: categoria,
        url: URL.createObjectURL(file),
        compartilhado: false,
        downloads: 0,
        visualizacoes: 0,
        protegido: false
      }));

      setArquivos(prev => ({
        ...prev,
        [categoria]: [...(prev[categoria] || []), ...newFiles]
      }));
    } catch (error) {
      console.error('Erro no upload:', error);
    } finally {
      setUploading(false);
    }
  };

  const handleShare = (arquivo) => {
    // Simulate sharing
    console.log('Compartilhando arquivo:', arquivo.nome);
  };

  const handleDownload = (arquivo) => {
    // Simulate download
    console.log('Baixando arquivo:', arquivo.nome);
  };

  const handleDelete = (arquivoId, categoria) => {
    setArquivos(prev => ({
      ...prev,
      [categoria]: prev[categoria].filter(arquivo => arquivo.id !== arquivoId)
    }));
  };

  const filteredFiles = (categoria) => {
    const files = arquivos[categoria] || [];
    if (!searchTerm) return files;
    
    return files.filter(arquivo => 
      arquivo.nome.toLowerCase().includes(searchTerm.toLowerCase()) ||
      arquivo.uploadPor.toLowerCase().includes(searchTerm.toLowerCase())
    );
  };

  const getTotalFiles = () => {
    return Object.values(arquivos).reduce((total, files) => total + files.length, 0);
  };

  const getTotalSize = () => {
    let totalMB = 0;
    Object.values(arquivos).forEach(files => {
      files.forEach(arquivo => {
        const size = parseFloat(arquivo.tamanho.replace(' MB', ''));
        totalMB += size;
      });
    });
    return totalMB > 1024 ? `${(totalMB / 1024).toFixed(1)} GB` : `${totalMB.toFixed(1)} MB`;
  };

  return (
    <Dialog open={isOpen} onOpenChange={onClose}>
      <DialogContent className="max-w-6xl max-h-[90vh] overflow-hidden">
        <DialogHeader>
          <DialogTitle className="flex items-center gap-2">
            <FolderOpen className="h-5 w-5" />
            Arquivos do Curso: {curso?.nome}
          </DialogTitle>
          <DialogDescription>
            Gerencie todos os arquivos educacionais vinculados ao curso
          </DialogDescription>
        </DialogHeader>

        {/* Estatísticas */}
        <div className="grid grid-cols-3 gap-4 mb-4">
          <div className="bg-blue-50 p-3 rounded-lg text-center">
            <p className="text-2xl font-bold text-blue-600">{getTotalFiles()}</p>
            <p className="text-sm text-blue-700">Total de Arquivos</p>
          </div>
          <div className="bg-green-50 p-3 rounded-lg text-center">
            <p className="text-2xl font-bold text-green-600">{getTotalSize()}</p>
            <p className="text-sm text-green-700">Espaço Utilizado</p>
          </div>
          <div className="bg-purple-50 p-3 rounded-lg text-center">
            <p className="text-2xl font-bold text-purple-600">
              {Object.values(arquivos).reduce((total, files) => 
                total + files.reduce((sum, arquivo) => sum + arquivo.downloads, 0), 0
              )}
            </p>
            <p className="text-sm text-purple-700">Total Downloads</p>
          </div>
        </div>

        {/* Busca e Filtros */}
        <div className="flex gap-4 mb-4">
          <div className="flex-1 relative">
            <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 h-4 w-4" />
            <Input
              placeholder="Buscar arquivos..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="pl-10"
            />
          </div>
          <Button variant="outline">
            <Filter className="h-4 w-4 mr-2" />
            Filtros
          </Button>
        </div>

        {/* Tabs por Categoria */}
        <Tabs value={activeTab} onValueChange={setActiveTab} className="flex-1 overflow-hidden">
          <TabsList className="grid grid-cols-4 lg:grid-cols-8 w-full">
            {categorias.map(categoria => (
              <TabsTrigger 
                key={categoria} 
                value={categoria}
                className="text-xs"
              >
                {categoria.split(' ')[0]}
                {arquivos[categoria]?.length > 0 && (
                  <Badge variant="secondary" className="ml-1 text-xs">
                    {arquivos[categoria].length}
                  </Badge>
                )}
              </TabsTrigger>
            ))}
          </TabsList>

          {categorias.map(categoria => (
            <TabsContent 
              key={categoria} 
              value={categoria} 
              className="flex-1 overflow-y-auto"
            >
              <div className="space-y-4">
                {/* Upload Area */}
                <div className="border-2 border-dashed border-gray-300 rounded-lg p-6 text-center hover:border-blue-400 transition-colors">
                  <input
                    type="file"
                    multiple
                    onChange={(e) => handleFileUpload(e.target.files, categoria)}
                    className="hidden"
                    id={`upload-${categoria}`}
                    disabled={uploading}
                  />
                  <label 
                    htmlFor={`upload-${categoria}`}
                    className="cursor-pointer flex flex-col items-center gap-2"
                  >
                    {uploading ? (
                      <Loader2 className="h-8 w-8 text-blue-500 animate-spin" />
                    ) : (
                      <Upload className="h-8 w-8 text-gray-400" />
                    )}
                    <p className="text-sm text-gray-600">
                      {uploading ? 'Enviando arquivos...' : `Clique para enviar arquivos para ${categoria}`}
                    </p>
                    <p className="text-xs text-gray-500">
                      Suporta PDF, DOCX, PPTX, MP4, MP3, JPG, PNG, ZIP
                    </p>
                  </label>
                </div>

                {/* Lista de Arquivos */}
                <div className="space-y-2">
                  {filteredFiles(categoria).length === 0 ? (
                    <div className="text-center py-8 text-gray-500">
                      <FileIcon className="h-12 w-12 mx-auto mb-2 text-gray-300" />
                      <p>Nenhum arquivo encontrado nesta categoria</p>
                    </div>
                  ) : (
                    filteredFiles(categoria).map(arquivo => {
                      const FileIconComponent = getFileIcon(arquivo.tipo);
                      
                      return (
                        <div 
                          key={arquivo.id}
                          className="flex items-center gap-4 p-4 border rounded-lg hover:bg-gray-50 transition-colors"
                        >
                          {/* Ícone e Info Básica */}
                          <div className="flex items-center gap-3 flex-1 min-w-0">
                            <div className="flex-shrink-0">
                              <FileIconComponent className="h-8 w-8 text-gray-600" />
                            </div>
                            
                            <div className="flex-1 min-w-0">
                              <div className="flex items-center gap-2 mb-1">
                                <p className="font-medium text-gray-900 truncate">
                                  {arquivo.nome}
                                </p>
                                <Badge className={getFileTypeColor(arquivo.tipo)}>
                                  {arquivo.tipo.toUpperCase()}
                                </Badge>
                                {arquivo.protegido && (
                                  <Badge variant="outline" className="text-orange-600 border-orange-600">
                                    <Shield className="h-3 w-3 mr-1" />
                                    Protegido
                                  </Badge>
                                )}
                                {arquivo.compartilhado && (
                                  <Badge variant="outline" className="text-green-600 border-green-600">
                                    <Share2 className="h-3 w-3 mr-1" />
                                    Compartilhado
                                  </Badge>
                                )}
                              </div>
                              
                              <div className="flex items-center gap-4 text-xs text-gray-500">
                                <span className="flex items-center gap-1">
                                  <User className="h-3 w-3" />
                                  {arquivo.uploadPor}
                                </span>
                                <span className="flex items-center gap-1">
                                  <Calendar className="h-3 w-3" />
                                  {formatDate(arquivo.dataUpload)}
                                </span>
                                <span>{arquivo.tamanho}</span>
                                {arquivo.duracao && (
                                  <span className="flex items-center gap-1">
                                    <Clock className="h-3 w-3" />
                                    {arquivo.duracao}
                                  </span>
                                )}
                              </div>
                            </div>
                          </div>

                          {/* Estatísticas */}
                          <div className="flex items-center gap-4 text-xs text-gray-500">
                            <div className="text-center">
                              <p className="font-medium text-gray-900">{arquivo.visualizacoes}</p>
                              <p>Visualizações</p>
                            </div>
                            <div className="text-center">
                              <p className="font-medium text-gray-900">{arquivo.downloads}</p>
                              <p>Downloads</p>
                            </div>
                          </div>

                          {/* Ações */}
                          <div className="flex items-center gap-2">
                            <Button
                              variant="outline"
                              size="sm"
                              onClick={() => handleDownload(arquivo)}
                            >
                              <Eye className="h-4 w-4" />
                            </Button>
                            
                            <DropdownMenu>
                              <DropdownMenuTrigger asChild>
                                <Button variant="outline" size="sm">
                                  <MoreVertical className="h-4 w-4" />
                                </Button>
                              </DropdownMenuTrigger>
                              <DropdownMenuContent align="end">
                                <DropdownMenuItem onClick={() => handleDownload(arquivo)}>
                                  <Download className="h-4 w-4 mr-2" />
                                  Baixar
                                </DropdownMenuItem>
                                <DropdownMenuItem onClick={() => handleShare(arquivo)}>
                                  <Share2 className="h-4 w-4 mr-2" />
                                  Compartilhar
                                </DropdownMenuItem>
                                <DropdownMenuItem>
                                  <Copy className="h-4 w-4 mr-2" />
                                  Copiar Link
                                </DropdownMenuItem>
                                <DropdownMenuItem>
                                  <ExternalLink className="h-4 w-4 mr-2" />
                                  Código Embed
                                </DropdownMenuItem>
                                <DropdownMenuSeparator />
                                <DropdownMenuItem>
                                  <Edit className="h-4 w-4 mr-2" />
                                  Editar
                                </DropdownMenuItem>
                                <DropdownMenuItem 
                                  className="text-red-600"
                                  onClick={() => handleDelete(arquivo.id, categoria)}
                                >
                                  <Trash2 className="h-4 w-4 mr-2" />
                                  Excluir
                                </DropdownMenuItem>
                              </DropdownMenuContent>
                            </DropdownMenu>
                          </div>
                        </div>
                      );
                    })
                  )}
                </div>
              </div>
            </TabsContent>
          ))}
        </Tabs>

        {/* Footer com informações */}
        <div className="border-t pt-4 mt-4">
          <div className="flex justify-between items-center text-sm text-gray-500">
            <p>
              Total: {getTotalFiles()} arquivos ({getTotalSize()})
            </p>
            <Button variant="outline" onClick={onClose}>
              <X className="h-4 w-4 mr-2" />
              Fechar
            </Button>
          </div>
        </div>
      </DialogContent>
    </Dialog>
  );
};

export default ArquivosModal;

