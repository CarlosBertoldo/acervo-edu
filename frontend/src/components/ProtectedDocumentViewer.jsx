import React, { useEffect, useRef, useState } from 'react';
import { Alert, AlertDescription } from '@/components/ui/alert';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import { 
  Download, 
  ZoomIn, 
  ZoomOut, 
  RotateCw, 
  Shield, 
  Lock,
  Eye,
  EyeOff,
  FileText,
  ChevronLeft,
  ChevronRight
} from 'lucide-react';

const ProtectedDocumentViewer = ({ 
  src, 
  title, 
  allowDownload = false,
  allowPrint = false,
  watermarkText = '',
  restrictedDomain = null,
  onView = () => {},
  onDownload = () => {}
}) => {
  const iframeRef = useRef(null);
  const containerRef = useRef(null);
  
  const [zoom, setZoom] = useState(100);
  const [rotation, setRotation] = useState(0);
  const [currentPage, setCurrentPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    // Configurações de proteção
    const handleContextMenu = (e) => {
      e.preventDefault();
      return false;
    };

    const handleKeyDown = (e) => {
      // Bloquear teclas de atalho comuns
      const blockedKeys = [
        'F12', // DevTools
        'PrintScreen', // Print Screen
      ];

      const blockedCombinations = [
        { ctrl: true, key: 's' }, // Ctrl+S (Save)
        { ctrl: true, key: 'S' },
        { ctrl: true, key: 'p' }, // Ctrl+P (Print)
        { ctrl: true, key: 'P' },
        { ctrl: true, key: 'u' }, // Ctrl+U (View Source)
        { ctrl: true, key: 'U' },
        { ctrl: true, shift: true, key: 'I' }, // Ctrl+Shift+I (DevTools)
        { ctrl: true, shift: true, key: 'i' },
        { ctrl: true, shift: true, key: 'J' }, // Ctrl+Shift+J (Console)
        { ctrl: true, shift: true, key: 'j' },
        { ctrl: true, shift: true, key: 'C' }, // Ctrl+Shift+C (Inspect)
        { ctrl: true, shift: true, key: 'c' },
      ];

      if (blockedKeys.includes(e.key)) {
        e.preventDefault();
        return false;
      }

      for (const combo of blockedCombinations) {
        if (
          (combo.ctrl && e.ctrlKey) &&
          (combo.shift ? e.shiftKey : !e.shiftKey) &&
          (combo.alt ? e.altKey : !e.altKey) &&
          e.key === combo.key
        ) {
          e.preventDefault();
          return false;
        }
      }
    };

    const handleSelectStart = (e) => {
      e.preventDefault();
      return false;
    };

    const handleDragStart = (e) => {
      e.preventDefault();
      return false;
    };

    // Detectar tentativas de captura de tela
    const handleVisibilityChange = () => {
      if (document.hidden) {
        // Opcional: pausar ou ocultar conteúdo quando a aba não está visível
      }
    };

    // Adicionar event listeners
    document.addEventListener('contextmenu', handleContextMenu);
    document.addEventListener('keydown', handleKeyDown);
    document.addEventListener('selectstart', handleSelectStart);
    document.addEventListener('dragstart', handleDragStart);
    document.addEventListener('visibilitychange', handleVisibilityChange);

    // Cleanup
    return () => {
      document.removeEventListener('contextmenu', handleContextMenu);
      document.removeEventListener('keydown', handleKeyDown);
      document.removeEventListener('selectstart', handleSelectStart);
      document.removeEventListener('dragstart', handleDragStart);
      document.removeEventListener('visibilitychange', handleVisibilityChange);
    };
  }, []);

  // Verificar domínio restrito
  useEffect(() => {
    if (restrictedDomain) {
      const currentDomain = window.location.hostname;
      if (!currentDomain.includes(restrictedDomain)) {
        setError(`Este conteúdo só pode ser visualizado em: ${restrictedDomain}`);
        return;
      }
    }
  }, [restrictedDomain]);

  useEffect(() => {
    onView();
  }, [onView]);

  const handleZoomIn = () => {
    setZoom(prev => Math.min(prev + 25, 200));
  };

  const handleZoomOut = () => {
    setZoom(prev => Math.max(prev - 25, 50));
  };

  const handleRotate = () => {
    setRotation(prev => (prev + 90) % 360);
  };

  const handleDownload = () => {
    if (allowDownload) {
      onDownload();
      window.open(src, '_blank');
    }
  };

  const handlePrevPage = () => {
    setCurrentPage(prev => Math.max(prev - 1, 1));
  };

  const handleNextPage = () => {
    setCurrentPage(prev => Math.min(prev + 1, totalPages));
  };

  const handleIframeLoad = () => {
    setLoading(false);
    
    // Tentar detectar número de páginas (funciona apenas para alguns tipos de PDF)
    const iframe = iframeRef.current;
    if (iframe) {
      try {
        // Esta é uma tentativa básica - em produção seria necessário usar uma biblioteca específica
        setTotalPages(1); // Placeholder
      } catch (error) {
        console.log('Não foi possível detectar o número de páginas');
      }
    }
  };

  if (error) {
    return (
      <Card className="w-full">
        <CardContent className="p-6">
          <Alert variant="destructive">
            <Shield className="h-4 w-4" />
            <AlertDescription>{error}</AlertDescription>
          </Alert>
        </CardContent>
      </Card>
    );
  }

  return (
    <Card className="w-full overflow-hidden">
      <CardHeader className="pb-3">
        <div className="flex items-center justify-between">
          <CardTitle className="flex items-center gap-2">
            <FileText className="h-5 w-5" />
            {title || 'Documento'}
          </CardTitle>
          
          <div className="flex items-center gap-2">
            <Badge variant="secondary" className="bg-red-600 text-white">
              <Lock className="h-3 w-3 mr-1" />
              Protegido
            </Badge>
            {!allowDownload && (
              <Badge variant="secondary" className="bg-orange-600 text-white">
                <EyeOff className="h-3 w-3 mr-1" />
                Sem Download
              </Badge>
            )}
            {!allowPrint && (
              <Badge variant="secondary" className="bg-purple-600 text-white">
                <EyeOff className="h-3 w-3 mr-1" />
                Sem Impressão
              </Badge>
            )}
          </div>
        </div>
      </CardHeader>

      <CardContent className="p-0">
        {/* Barra de ferramentas */}
        <div className="flex items-center justify-between p-4 bg-gray-50 border-b">
          <div className="flex items-center gap-2">
            <Button variant="outline" size="sm" onClick={handleZoomOut}>
              <ZoomOut className="h-4 w-4" />
            </Button>
            
            <span className="text-sm font-medium min-w-16 text-center">
              {zoom}%
            </span>
            
            <Button variant="outline" size="sm" onClick={handleZoomIn}>
              <ZoomIn className="h-4 w-4" />
            </Button>
            
            <Button variant="outline" size="sm" onClick={handleRotate}>
              <RotateCw className="h-4 w-4" />
            </Button>
          </div>

          <div className="flex items-center gap-2">
            <Button variant="outline" size="sm" onClick={handlePrevPage} disabled={currentPage <= 1}>
              <ChevronLeft className="h-4 w-4" />
            </Button>
            
            <span className="text-sm">
              {currentPage} / {totalPages}
            </span>
            
            <Button variant="outline" size="sm" onClick={handleNextPage} disabled={currentPage >= totalPages}>
              <ChevronRight className="h-4 w-4" />
            </Button>
          </div>

          <div className="flex items-center gap-2">
            {allowDownload && (
              <Button variant="outline" size="sm" onClick={handleDownload}>
                <Download className="h-4 w-4 mr-2" />
                Download
              </Button>
            )}
          </div>
        </div>

        {/* Área de visualização */}
        <div 
          ref={containerRef}
          className="relative bg-gray-100 min-h-96"
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

          {/* Watermark */}
          {watermarkText && (
            <div className="absolute top-4 right-4 z-20 bg-black bg-opacity-50 text-white text-xs px-2 py-1 rounded pointer-events-none">
              {watermarkText}
            </div>
          )}

          {/* Loading */}
          {loading && (
            <div className="absolute inset-0 flex items-center justify-center bg-white bg-opacity-75 z-30">
              <div className="text-center">
                <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600 mx-auto mb-2"></div>
                <p className="text-sm text-gray-600">Carregando documento...</p>
              </div>
            </div>
          )}

          {/* Iframe para PDF */}
          <iframe
            ref={iframeRef}
            src={`${src}#toolbar=0&navpanes=0&scrollbar=0&page=${currentPage}&zoom=${zoom}&rotate=${rotation}`}
            className="w-full h-96 border-0"
            style={{
              transform: `scale(${zoom / 100}) rotate(${rotation}deg)`,
              transformOrigin: 'center center',
              userSelect: 'none',
              WebkitUserSelect: 'none',
              MozUserSelect: 'none',
              msUserSelect: 'none'
            }}
            onLoad={handleIframeLoad}
            onContextMenu={(e) => e.preventDefault()}
            sandbox="allow-same-origin allow-scripts"
            title={title || 'Documento protegido'}
          />

          {/* Overlay adicional para bloquear interações */}
          <div 
            className="absolute inset-0 z-5"
            style={{
              background: 'transparent',
              pointerEvents: 'none'
            }}
            onContextMenu={(e) => e.preventDefault()}
            onSelectStart={(e) => e.preventDefault()}
            onDragStart={(e) => e.preventDefault()}
          />
        </div>

        {/* Informações do documento */}
        <div className="p-4 bg-gray-50 border-t">
          <div className="flex items-center justify-between text-sm text-gray-600">
            <div className="flex items-center gap-4">
              <span>Zoom: {zoom}%</span>
              <span>Rotação: {rotation}°</span>
              <span>Página: {currentPage}/{totalPages}</span>
            </div>
            
            <div className="flex items-center gap-2">
              <Eye className="h-4 w-4" />
              <span>Visualização protegida</span>
            </div>
          </div>
        </div>
      </CardContent>
    </Card>
  );
};

export default ProtectedDocumentViewer;

