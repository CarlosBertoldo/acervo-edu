import React from 'react';
import ProtectedVideoPlayer from './ProtectedVideoPlayer';
import ProtectedAudioPlayer from './ProtectedAudioPlayer';
import ProtectedDocumentViewer from './ProtectedDocumentViewer';
import { Card, CardContent } from '@/components/ui/card';
import { Alert, AlertDescription } from '@/components/ui/alert';
import { FileText, AlertCircle } from 'lucide-react';

const ProtectedMediaViewer = ({ 
  arquivo,
  protectionSettings = {},
  onPlay = () => {},
  onPause = () => {},
  onTimeUpdate = () => {},
  onEnded = () => {},
  onView = () => {},
  onDownload = () => {}
}) => {
  if (!arquivo) {
    return (
      <Card className="w-full">
        <CardContent className="p-6">
          <Alert>
            <AlertCircle className="h-4 w-4" />
            <AlertDescription>Nenhum arquivo selecionado para visualização.</AlertDescription>
          </Alert>
        </CardContent>
      </Card>
    );
  }

  const {
    allowDownload = false,
    allowSeek = true,
    allowSpeedControl = true,
    allowPrint = false,
    watermarkText = '',
    restrictedDomain = null
  } = protectionSettings;

  // Detectar tipo de arquivo pela extensão
  const getFileType = (filename) => {
    if (!filename) return 'unknown';
    
    const extension = filename.toLowerCase().split('.').pop();
    
    const videoExtensions = ['mp4', 'avi', 'mov', 'wmv', 'flv', 'webm', 'mkv'];
    const audioExtensions = ['mp3', 'wav', 'aac', 'flac', 'ogg', 'm4a'];
    const documentExtensions = ['pdf', 'doc', 'docx', 'ppt', 'pptx', 'xls', 'xlsx'];
    const imageExtensions = ['jpg', 'jpeg', 'png', 'gif', 'bmp', 'svg', 'webp'];

    if (videoExtensions.includes(extension)) return 'video';
    if (audioExtensions.includes(extension)) return 'audio';
    if (documentExtensions.includes(extension)) return 'document';
    if (imageExtensions.includes(extension)) return 'image';
    
    return 'unknown';
  };

  const fileType = getFileType(arquivo.nomeArquivo);

  // Componente para imagens protegidas
  const ProtectedImageViewer = ({ src, title }) => (
    <Card className="w-full overflow-hidden">
      <CardContent className="p-0">
        <div 
          className="relative bg-gray-100"
          style={{
            userSelect: 'none',
            WebkitUserSelect: 'none',
            MozUserSelect: 'none',
            msUserSelect: 'none'
          }}
          onContextMenu={(e) => e.preventDefault()}
          onSelectStart={(e) => e.preventDefault()}
          onDragStart={(e) => e.preventDefault()}
        >
          {/* Watermark */}
          {watermarkText && (
            <div className="absolute top-4 right-4 z-20 bg-black bg-opacity-50 text-white text-xs px-2 py-1 rounded pointer-events-none">
              {watermarkText}
            </div>
          )}

          <img
            src={src}
            alt={title}
            className="w-full h-auto max-h-96 object-contain"
            style={{
              userSelect: 'none',
              WebkitUserSelect: 'none',
              MozUserSelect: 'none',
              msUserSelect: 'none',
              pointerEvents: 'none'
            }}
            onContextMenu={(e) => e.preventDefault()}
            onDragStart={(e) => e.preventDefault()}
          />

          {/* Overlay de proteção */}
          <div 
            className="absolute inset-0 z-10 pointer-events-none"
            style={{
              background: 'transparent',
              userSelect: 'none',
              WebkitUserSelect: 'none',
              MozUserSelect: 'none',
              msUserSelect: 'none'
            }}
          />
        </div>
      </CardContent>
    </Card>
  );

  // Componente para arquivos não suportados
  const UnsupportedFileViewer = ({ arquivo }) => (
    <Card className="w-full">
      <CardContent className="p-6 text-center">
        <FileText className="h-12 w-12 mx-auto mb-4 text-gray-400" />
        <h3 className="text-lg font-medium mb-2">{arquivo.nomeArquivo}</h3>
        <p className="text-gray-600 mb-4">
          Tipo de arquivo não suportado para visualização online.
        </p>
        <p className="text-sm text-gray-500 mb-4">
          Tamanho: {formatFileSize(arquivo.tamanho)}
        </p>
        {allowDownload && (
          <button
            onClick={onDownload}
            className="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700 transition-colors"
          >
            Baixar Arquivo
          </button>
        )}
      </CardContent>
    </Card>
  );

  const formatFileSize = (bytes) => {
    if (bytes === 0) return '0 Bytes';
    const k = 1024;
    const sizes = ['Bytes', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
  };

  // Renderizar o componente apropriado baseado no tipo de arquivo
  switch (fileType) {
    case 'video':
      return (
        <ProtectedVideoPlayer
          src={arquivo.urlAcesso}
          title={arquivo.nomeArquivo}
          allowDownload={allowDownload}
          allowSeek={allowSeek}
          allowSpeedControl={allowSpeedControl}
          watermarkText={watermarkText}
          restrictedDomain={restrictedDomain}
          onPlay={onPlay}
          onPause={onPause}
          onTimeUpdate={onTimeUpdate}
          onEnded={onEnded}
        />
      );

    case 'audio':
      return (
        <ProtectedAudioPlayer
          src={arquivo.urlAcesso}
          title={arquivo.nomeArquivo}
          allowDownload={allowDownload}
          allowSeek={allowSeek}
          allowSpeedControl={allowSpeedControl}
          watermarkText={watermarkText}
          restrictedDomain={restrictedDomain}
          onPlay={onPlay}
          onPause={onPause}
          onTimeUpdate={onTimeUpdate}
          onEnded={onEnded}
          onDownload={onDownload}
        />
      );

    case 'document':
      return (
        <ProtectedDocumentViewer
          src={arquivo.urlAcesso}
          title={arquivo.nomeArquivo}
          allowDownload={allowDownload}
          allowPrint={allowPrint}
          watermarkText={watermarkText}
          restrictedDomain={restrictedDomain}
          onView={onView}
          onDownload={onDownload}
        />
      );

    case 'image':
      return (
        <ProtectedImageViewer
          src={arquivo.urlAcesso}
          title={arquivo.nomeArquivo}
        />
      );

    default:
      return <UnsupportedFileViewer arquivo={arquivo} />;
  }
};

export default ProtectedMediaViewer;

