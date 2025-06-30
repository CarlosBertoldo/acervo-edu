import React, { useEffect, useRef, useState } from 'react';
import { Alert, AlertDescription } from '@/components/ui/alert';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import { 
  Play, 
  Pause, 
  Volume2, 
  VolumeX, 
  SkipBack,
  SkipForward,
  Repeat,
  Shuffle,
  Download,
  Shield, 
  Lock,
  Music,
  EyeOff
} from 'lucide-react';

const ProtectedAudioPlayer = ({ 
  src, 
  title, 
  artist = '',
  album = '',
  duration = 0,
  allowDownload = false,
  allowSeek = true,
  allowSpeedControl = true,
  watermarkText = '',
  restrictedDomain = null,
  onPlay = () => {},
  onPause = () => {},
  onTimeUpdate = () => {},
  onEnded = () => {},
  onDownload = () => {}
}) => {
  const audioRef = useRef(null);
  
  const [isPlaying, setIsPlaying] = useState(false);
  const [isMuted, setIsMuted] = useState(false);
  const [currentTime, setCurrentTime] = useState(0);
  const [audioDuration, setAudioDuration] = useState(duration);
  const [volume, setVolume] = useState(1);
  const [isLoading, setIsLoading] = useState(true);
  const [isRepeat, setIsRepeat] = useState(false);
  const [playbackRate, setPlaybackRate] = useState(1);
  const [error, setError] = useState('');

  useEffect(() => {
    const audio = audioRef.current;
    if (!audio) return;

    // Configurações de proteção
    audio.controlsList = 'nodownload noremoteplayback';
    audio.preload = 'metadata';

    // Event listeners para proteção
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

    // Event listeners do áudio
    const handleLoadedMetadata = () => {
      setAudioDuration(audio.duration);
      setIsLoading(false);
    };

    const handleTimeUpdate = () => {
      setCurrentTime(audio.currentTime);
      onTimeUpdate(audio.currentTime);
    };

    const handlePlay = () => {
      setIsPlaying(true);
      onPlay();
    };

    const handlePause = () => {
      setIsPlaying(false);
      onPause();
    };

    const handleEnded = () => {
      setIsPlaying(false);
      if (isRepeat) {
        audio.currentTime = 0;
        audio.play();
      } else {
        onEnded();
      }
    };

    const handleVolumeChange = () => {
      setVolume(audio.volume);
      setIsMuted(audio.muted);
    };

    const handleError = () => {
      setError('Erro ao carregar o áudio. Verifique se o arquivo está disponível.');
      setIsLoading(false);
    };

    // Adicionar event listeners
    document.addEventListener('contextmenu', handleContextMenu);
    document.addEventListener('keydown', handleKeyDown);
    document.addEventListener('selectstart', handleSelectStart);
    document.addEventListener('dragstart', handleDragStart);

    audio.addEventListener('loadedmetadata', handleLoadedMetadata);
    audio.addEventListener('timeupdate', handleTimeUpdate);
    audio.addEventListener('play', handlePlay);
    audio.addEventListener('pause', handlePause);
    audio.addEventListener('ended', handleEnded);
    audio.addEventListener('volumechange', handleVolumeChange);
    audio.addEventListener('error', handleError);

    // Cleanup
    return () => {
      document.removeEventListener('contextmenu', handleContextMenu);
      document.removeEventListener('keydown', handleKeyDown);
      document.removeEventListener('selectstart', handleSelectStart);
      document.removeEventListener('dragstart', handleDragStart);

      if (audio) {
        audio.removeEventListener('loadedmetadata', handleLoadedMetadata);
        audio.removeEventListener('timeupdate', handleTimeUpdate);
        audio.removeEventListener('play', handlePlay);
        audio.removeEventListener('pause', handlePause);
        audio.removeEventListener('ended', handleEnded);
        audio.removeEventListener('volumechange', handleVolumeChange);
        audio.removeEventListener('error', handleError);
      }
    };
  }, [isRepeat, onPlay, onPause, onTimeUpdate, onEnded]);

  // Verificar domínio restrito
  useEffect(() => {
    if (restrictedDomain) {
      const currentDomain = window.location.hostname;
      if (!currentDomain.includes(restrictedDomain)) {
        setError(`Este conteúdo só pode ser reproduzido em: ${restrictedDomain}`);
        return;
      }
    }
  }, [restrictedDomain]);

  const togglePlay = () => {
    const audio = audioRef.current;
    if (!audio) return;

    if (isPlaying) {
      audio.pause();
    } else {
      audio.play().catch(err => {
        console.error('Erro ao reproduzir áudio:', err);
        setError('Erro ao reproduzir o áudio. Tente novamente.');
      });
    }
  };

  const toggleMute = () => {
    const audio = audioRef.current;
    if (!audio) return;

    audio.muted = !audio.muted;
  };

  const handleSeek = (e) => {
    if (!allowSeek) return;
    
    const audio = audioRef.current;
    if (!audio) return;

    const rect = e.currentTarget.getBoundingClientRect();
    const pos = (e.clientX - rect.left) / rect.width;
    audio.currentTime = pos * audioDuration;
  };

  const handleVolumeChange = (e) => {
    const audio = audioRef.current;
    if (!audio) return;

    const newVolume = parseFloat(e.target.value);
    audio.volume = newVolume;
    setVolume(newVolume);
  };

  const handleSpeedChange = (speed) => {
    if (!allowSpeedControl) return;
    
    const audio = audioRef.current;
    if (!audio) return;

    audio.playbackRate = speed;
    setPlaybackRate(speed);
  };

  const skipBackward = () => {
    const audio = audioRef.current;
    if (!audio) return;

    audio.currentTime = Math.max(audio.currentTime - 10, 0);
  };

  const skipForward = () => {
    const audio = audioRef.current;
    if (!audio) return;

    audio.currentTime = Math.min(audio.currentTime + 10, audioDuration);
  };

  const toggleRepeat = () => {
    setIsRepeat(!isRepeat);
  };

  const handleDownload = () => {
    if (allowDownload) {
      onDownload();
      window.open(src, '_blank');
    }
  };

  const formatTime = (time) => {
    const minutes = Math.floor(time / 60);
    const seconds = Math.floor(time % 60);
    return `${minutes}:${seconds.toString().padStart(2, '0')}`;
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
            <Music className="h-5 w-5" />
            {title || 'Áudio'}
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
          </div>
        </div>
        
        {(artist || album) && (
          <div className="text-sm text-gray-600">
            {artist && <span>{artist}</span>}
            {artist && album && <span> • </span>}
            {album && <span>{album}</span>}
          </div>
        )}
      </CardHeader>

      <CardContent className="space-y-4">
        {/* Áudio element (oculto) */}
        <audio
          ref={audioRef}
          src={src}
          style={{ display: 'none' }}
          onContextMenu={(e) => e.preventDefault()}
          controlsList="nodownload noremoteplayback"
          preload="metadata"
        />

        {/* Watermark */}
        {watermarkText && (
          <div className="text-center">
            <Badge variant="outline" className="bg-gray-100">
              {watermarkText}
            </Badge>
          </div>
        )}

        {/* Barra de progresso */}
        <div className="space-y-2">
          <div 
            className="w-full h-2 bg-gray-200 rounded-full cursor-pointer"
            onClick={handleSeek}
          >
            <div 
              className="h-full bg-blue-500 rounded-full transition-all duration-100"
              style={{ width: `${(currentTime / audioDuration) * 100}%` }}
            />
          </div>
          
          <div className="flex justify-between text-xs text-gray-500">
            <span>{formatTime(currentTime)}</span>
            <span>{formatTime(audioDuration)}</span>
          </div>
        </div>

        {/* Controles principais */}
        <div className="flex items-center justify-center gap-4">
          <Button
            variant="outline"
            size="sm"
            onClick={skipBackward}
            disabled={!allowSeek}
          >
            <SkipBack className="h-4 w-4" />
          </Button>

          <Button
            size="lg"
            onClick={togglePlay}
            disabled={isLoading}
            className="rounded-full w-12 h-12"
          >
            {isLoading ? (
              <div className="animate-spin rounded-full h-5 w-5 border-b-2 border-white"></div>
            ) : isPlaying ? (
              <Pause className="h-6 w-6" />
            ) : (
              <Play className="h-6 w-6" />
            )}
          </Button>

          <Button
            variant="outline"
            size="sm"
            onClick={skipForward}
            disabled={!allowSeek}
          >
            <SkipForward className="h-4 w-4" />
          </Button>
        </div>

        {/* Controles secundários */}
        <div className="flex items-center justify-between">
          <div className="flex items-center gap-2">
            <Button
              variant="ghost"
              size="sm"
              onClick={toggleMute}
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
              className="w-20 h-1 bg-gray-200 rounded-lg appearance-none cursor-pointer"
            />
          </div>

          <div className="flex items-center gap-2">
            <Button
              variant={isRepeat ? "default" : "ghost"}
              size="sm"
              onClick={toggleRepeat}
            >
              <Repeat className="h-4 w-4" />
            </Button>

            {allowSpeedControl && (
              <select
                value={playbackRate}
                onChange={(e) => handleSpeedChange(parseFloat(e.target.value))}
                className="text-xs border rounded px-2 py-1"
              >
                <option value={0.5}>0.5x</option>
                <option value={0.75}>0.75x</option>
                <option value={1}>1x</option>
                <option value={1.25}>1.25x</option>
                <option value={1.5}>1.5x</option>
                <option value={2}>2x</option>
              </select>
            )}

            {allowDownload && (
              <Button
                variant="outline"
                size="sm"
                onClick={handleDownload}
              >
                <Download className="h-4 w-4" />
              </Button>
            )}
          </div>
        </div>

        {/* Informações adicionais */}
        <div className="text-center text-xs text-gray-500 pt-2 border-t">
          <div className="flex items-center justify-center gap-4">
            <span>Velocidade: {playbackRate}x</span>
            {isRepeat && <span>Repetição ativada</span>}
            <span>Reprodução protegida</span>
          </div>
        </div>
      </CardContent>
    </Card>
  );
};

export default ProtectedAudioPlayer;

