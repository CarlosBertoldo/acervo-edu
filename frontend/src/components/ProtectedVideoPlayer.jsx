import React, { useState, useRef, useEffect } from 'react';
import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import { Alert, AlertDescription } from '@/components/ui/alert';
import { 
  Play, 
  Pause, 
  Volume2, 
  VolumeX, 
  Maximize, 
  Minimize,
  SkipForward,
  SkipBack,
  Shield,
  Lock,
  Eye,
  EyeOff,
  AlertTriangle,
  Info
} from 'lucide-react';

const ProtectedVideoPlayer = ({ 
  src, 
  title,
  restrictions = {
    naoAvancar: false,
    bloqueioDownload: false,
    bloqueioContexto: false,
    bloqueioTeclas: false,
    bloqueioInspecionar: false,
    overlayProtecao: false
  },
  onProgress,
  onComplete
}) => {
  const videoRef = useRef(null);
  const containerRef = useRef(null);
  const [isPlaying, setIsPlaying] = useState(false);
  const [currentTime, setCurrentTime] = useState(0);
  const [duration, setDuration] = useState(0);
  const [volume, setVolume] = useState(1);
  const [isMuted, setIsMuted] = useState(false);
  const [isFullscreen, setIsFullscreen] = useState(false);
  const [showControls, setShowControls] = useState(true);
  const [lastValidTime, setLastValidTime] = useState(0);
  const [hasStarted, setHasStarted] = useState(false);
  const [violations, setViolations] = useState([]);

  useEffect(() => {
    const video = videoRef.current;
    if (!video) return;

    const handleLoadedMetadata = () => {
      setDuration(video.duration);
    };

    const handleTimeUpdate = () => {
      const current = video.currentTime;
      setCurrentTime(current);

      // Verificar se usuário tentou avançar (apenas se já começou a assistir)
      if (restrictions.naoAvancar && hasStarted && current > lastValidTime + 1) {
        video.currentTime = lastValidTime;
        addViolation('Tentativa de avançar o vídeo detectada');
        return;
      }

      // Atualizar último tempo válido se reprodução normal
      if (current >= lastValidTime) {
        setLastValidTime(current);
      }

      // Callback de progresso
      if (onProgress) {
        onProgress(current, duration);
      }
    };

    const handleEnded = () => {
      setIsPlaying(false);
      if (onComplete) {
        onComplete();
      }
    };

    const handlePlay = () => {
      setIsPlaying(true);
      setHasStarted(true);
    };

    const handlePause = () => {
      setIsPlaying(false);
    };

    video.addEventListener('loadedmetadata', handleLoadedMetadata);
    video.addEventListener('timeupdate', handleTimeUpdate);
    video.addEventListener('ended', handleEnded);
    video.addEventListener('play', handlePlay);
    video.addEventListener('pause', handlePause);

    return () => {
      video.removeEventListener('loadedmetadata', handleLoadedMetadata);
      video.removeEventListener('timeupdate', handleTimeUpdate);
      video.removeEventListener('ended', handleEnded);
      video.removeEventListener('play', handlePlay);
      video.removeEventListener('pause', handlePause);
    };
  }, [restrictions.naoAvancar, lastValidTime, hasStarted, onProgress, onComplete, duration]);

  // Bloquear teclas de atalho
  useEffect(() => {
    if (!restrictions.bloqueioTeclas) return;

    const handleKeyDown = (e) => {
      // Bloquear teclas comuns de vídeo
      const blockedKeys = [
        'Space', // Play/Pause
        'ArrowLeft', // Retroceder
        'ArrowRight', // Avançar
        'ArrowUp', // Volume +
        'ArrowDown', // Volume -
        'KeyF', // Fullscreen
        'KeyM', // Mute
        'Home', // Início
        'End', // Fim
        'PageUp', // Avançar muito
        'PageDown' // Retroceder muito
      ];

      if (blockedKeys.includes(e.code)) {
        e.preventDefault();
        addViolation(`Tecla ${e.code} bloqueada`);
      }

      // Bloquear F12, Ctrl+Shift+I, etc.
      if (restrictions.bloqueioInspecionar) {
        if (e.key === 'F12' || 
            (e.ctrlKey && e.shiftKey && e.key === 'I') ||
            (e.ctrlKey && e.shiftKey && e.key === 'C') ||
            (e.ctrlKey && e.key === 'u')) {
          e.preventDefault();
          addViolation('Tentativa de abrir ferramentas de desenvolvedor');
        }
      }
    };

    document.addEventListener('keydown', handleKeyDown);
    return () => document.removeEventListener('keydown', handleKeyDown);
  }, [restrictions.bloqueioTeclas, restrictions.bloqueioInspecionar]);

  // Bloquear menu de contexto
  useEffect(() => {
    if (!restrictions.bloqueioContexto) return;

    const handleContextMenu = (e) => {
      e.preventDefault();
      addViolation('Menu de contexto bloqueado');
    };

    const container = containerRef.current;
    if (container) {
      container.addEventListener('contextmenu', handleContextMenu);
      return () => container.removeEventListener('contextmenu', handleContextMenu);
    }
  }, [restrictions.bloqueioContexto]);

  // Bloquear seleção de texto
  useEffect(() => {
    if (!restrictions.overlayProtecao) return;

    const handleSelectStart = (e) => {
      e.preventDefault();
    };

    const container = containerRef.current;
    if (container) {
      container.addEventListener('selectstart', handleSelectStart);
      return () => container.removeEventListener('selectstart', handleSelectStart);
    }
  }, [restrictions.overlayProtecao]);

  const addViolation = (message) => {
    const violation = {
      id: Date.now(),
      message,
      timestamp: new Date().toLocaleTimeString()
    };
    setViolations(prev => [...prev.slice(-4), violation]); // Manter apenas últimas 5
  };

  const togglePlay = () => {
    const video = videoRef.current;
    if (!video) return;

    if (isPlaying) {
      video.pause();
    } else {
      video.play();
    }
  };

  const handleSeek = (e) => {
    if (restrictions.naoAvancar) {
      addViolation('Busca no vídeo não permitida');
      return;
    }

    const video = videoRef.current;
    if (!video) return;

    const rect = e.currentTarget.getBoundingClientRect();
    const percent = (e.clientX - rect.left) / rect.width;
    const newTime = percent * duration;
    
    video.currentTime = newTime;
    setLastValidTime(newTime);
  };

  const handleVolumeChange = (e) => {
    const newVolume = parseFloat(e.target.value);
    setVolume(newVolume);
    if (videoRef.current) {
      videoRef.current.volume = newVolume;
    }
    setIsMuted(newVolume === 0);
  };

  const toggleMute = () => {
    const video = videoRef.current;
    if (!video) return;

    if (isMuted) {
      video.volume = volume;
      setIsMuted(false);
    } else {
      video.volume = 0;
      setIsMuted(true);
    }
  };

  const toggleFullscreen = () => {
    const container = containerRef.current;
    if (!container) return;

    if (!isFullscreen) {
      if (container.requestFullscreen) {
        container.requestFullscreen();
      }
    } else {
      if (document.exitFullscreen) {
        document.exitFullscreen();
      }
    }
    setIsFullscreen(!isFullscreen);
  };

  const formatTime = (time) => {
    const minutes = Math.floor(time / 60);
    const seconds = Math.floor(time % 60);
    return `${minutes}:${seconds.toString().padStart(2, '0')}`;
  };

  const progressPercent = duration > 0 ? (currentTime / duration) * 100 : 0;

  return (
    <div 
      ref={containerRef}
      className="relative bg-black rounded-lg overflow-hidden group"
      style={{ userSelect: restrictions.overlayProtecao ? 'none' : 'auto' }}
    >
      {/* Overlay de proteção */}
      {restrictions.overlayProtecao && (
        <div className="absolute inset-0 z-10 pointer-events-none">
          <div className="absolute top-4 right-4">
            <Badge variant="outline" className="bg-black/50 text-white border-white/20">
              <Shield className="h-3 w-3 mr-1" />
              Protegido
            </Badge>
          </div>
          {/* Watermark */}
          <div className="absolute bottom-4 left-4 text-white/30 text-sm font-mono">
            Acervo Educacional FC
          </div>
        </div>
      )}

      {/* Vídeo */}
      <video
        ref={videoRef}
        src={src}
        className="w-full h-auto"
        onContextMenu={restrictions.bloqueioContexto ? (e) => e.preventDefault() : undefined}
        controlsList={restrictions.bloqueioDownload ? "nodownload" : ""}
        disablePictureInPicture={restrictions.bloqueioDownload}
        style={{
          pointerEvents: restrictions.naoAvancar ? 'none' : 'auto'
        }}
      />

      {/* Controles customizados */}
      <div 
        className={`absolute bottom-0 left-0 right-0 bg-gradient-to-t from-black/80 to-transparent p-4 transition-opacity duration-300 ${
          showControls ? 'opacity-100' : 'opacity-0'
        }`}
        onMouseEnter={() => setShowControls(true)}
        onMouseLeave={() => setShowControls(false)}
      >
        {/* Barra de progresso */}
        <div className="mb-4">
          <div 
            className="w-full h-2 bg-white/20 rounded-full cursor-pointer"
            onClick={handleSeek}
          >
            <div 
              className="h-full bg-blue-500 rounded-full transition-all duration-150"
              style={{ width: `${progressPercent}%` }}
            />
          </div>
        </div>

        {/* Controles */}
        <div className="flex items-center justify-between text-white">
          <div className="flex items-center gap-4">
            {/* Play/Pause */}
            <Button
              variant="ghost"
              size="sm"
              onClick={togglePlay}
              className="text-white hover:bg-white/20"
            >
              {isPlaying ? <Pause className="h-5 w-5" /> : <Play className="h-5 w-5" />}
            </Button>

            {/* Volume */}
            <div className="flex items-center gap-2">
              <Button
                variant="ghost"
                size="sm"
                onClick={toggleMute}
                className="text-white hover:bg-white/20"
              >
                {isMuted ? <VolumeX className="h-4 w-4" /> : <Volume2 className="h-4 w-4" />}
              </Button>
              <input
                type="range"
                min="0"
                max="1"
                step="0.1"
                value={isMuted ? 0 : volume}
                onChange={handleVolumeChange}
                className="w-20"
              />
            </div>

            {/* Tempo */}
            <span className="text-sm">
              {formatTime(currentTime)} / {formatTime(duration)}
            </span>
          </div>

          <div className="flex items-center gap-2">
            {/* Indicadores de restrição */}
            {restrictions.naoAvancar && (
              <Badge variant="outline" className="text-orange-300 border-orange-300">
                <Lock className="h-3 w-3 mr-1" />
                Sem avanço
              </Badge>
            )}
            
            {restrictions.bloqueioDownload && (
              <Badge variant="outline" className="text-red-300 border-red-300">
                <Shield className="h-3 w-3 mr-1" />
                Sem download
              </Badge>
            )}

            {/* Fullscreen */}
            <Button
              variant="ghost"
              size="sm"
              onClick={toggleFullscreen}
              className="text-white hover:bg-white/20"
            >
              {isFullscreen ? <Minimize className="h-4 w-4" /> : <Maximize className="h-4 w-4" />}
            </Button>
          </div>
        </div>
      </div>

      {/* Alertas de violação */}
      {violations.length > 0 && (
        <div className="absolute top-4 left-4 space-y-2 z-20">
          {violations.slice(-3).map(violation => (
            <Alert key={violation.id} className="bg-red-500/90 text-white border-red-400">
              <AlertTriangle className="h-4 w-4" />
              <AlertDescription className="text-sm">
                {violation.message}
              </AlertDescription>
            </Alert>
          ))}
        </div>
      )}

      {/* Informações do vídeo */}
      {title && (
        <div className="absolute top-4 left-4 z-10">
          <h3 className="text-white font-medium text-lg drop-shadow-lg">
            {title}
          </h3>
        </div>
      )}
    </div>
  );
};

export default ProtectedVideoPlayer;

