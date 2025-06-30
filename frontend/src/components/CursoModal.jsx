import React, { useState, useEffect } from 'react';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/textarea';
import { Badge } from '@/components/ui/badge';
import { 
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
  DialogFooter,
} from '@/components/ui/dialog';
import { 
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import { Switch } from '@/components/ui/switch';
import { Alert, AlertDescription } from '@/components/ui/alert';
import { 
  Save, 
  X, 
  AlertTriangle, 
  Info,
  Calendar,
  User,
  Building,
  Code,
  Globe,
  Lock,
  Loader2,
  CheckCircle
} from 'lucide-react';

const CursoModal = ({ 
  isOpen, 
  onClose, 
  curso = null, 
  onSave,
  mode = 'create' // 'create', 'edit', 'view'
}) => {
  const [formData, setFormData] = useState({
    codigo: '',
    nome: '',
    descricaoAcademia: '',
    status: 'Backlog',
    tipoAmbiente: 'Presencial',
    tipoAcesso: 'Publico',
    dataInicioOperacao: '',
    origem: 'Manual',
    criadoPor: '',
    comentariosInternos: '',
    progresso: 0,
    cargaHoraria: '',
    categoria: '',
    nivel: 'Iniciante',
    prioridade: 'Media',
    responsavel: '',
    departamento: '',
    tags: [],
    ativo: true
  });

  const [loading, setSaving] = useState(false);
  const [errors, setErrors] = useState({});
  const [newTag, setNewTag] = useState('');

  useEffect(() => {
    if (curso) {
      setFormData({
        codigo: curso.codigo || '',
        nome: curso.nome || '',
        descricaoAcademia: curso.descricaoAcademia || '',
        status: curso.status || 'Backlog',
        tipoAmbiente: curso.tipoAmbiente || 'Presencial',
        tipoAcesso: curso.tipoAcesso || 'Publico',
        dataInicioOperacao: curso.dataInicioOperacao || '',
        origem: curso.origem || 'Manual',
        criadoPor: curso.criadoPor || '',
        comentariosInternos: curso.comentariosInternos || '',
        progresso: curso.progresso || 0,
        cargaHoraria: curso.cargaHoraria || '',
        categoria: curso.categoria || '',
        nivel: curso.nivel || 'Iniciante',
        prioridade: curso.prioridade || 'Media',
        responsavel: curso.responsavel || '',
        departamento: curso.departamento || '',
        tags: curso.tags || [],
        ativo: curso.ativo !== false
      });
    } else {
      // Reset form for new course
      setFormData({
        codigo: '',
        nome: '',
        descricaoAcademia: '',
        status: 'Backlog',
        tipoAmbiente: 'Presencial',
        tipoAcesso: 'Publico',
        dataInicioOperacao: '',
        origem: 'Manual',
        criadoPor: 'Usuário Demo',
        comentariosInternos: '',
        progresso: 0,
        cargaHoraria: '',
        categoria: '',
        nivel: 'Iniciante',
        prioridade: 'Media',
        responsavel: '',
        departamento: '',
        tags: [],
        ativo: true
      });
    }
    setErrors({});
  }, [curso, isOpen]);

  const handleInputChange = (field, value) => {
    setFormData(prev => ({ ...prev, [field]: value }));
    // Clear error when user starts typing
    if (errors[field]) {
      setErrors(prev => ({ ...prev, [field]: null }));
    }
  };

  const addTag = () => {
    if (newTag.trim() && !formData.tags.includes(newTag.trim())) {
      setFormData(prev => ({
        ...prev,
        tags: [...prev.tags, newTag.trim()]
      }));
      setNewTag('');
    }
  };

  const removeTag = (tagToRemove) => {
    setFormData(prev => ({
      ...prev,
      tags: prev.tags.filter(tag => tag !== tagToRemove)
    }));
  };

  const validateForm = () => {
    const newErrors = {};

    if (!formData.codigo.trim()) {
      newErrors.codigo = 'Código é obrigatório';
    }
    if (!formData.nome.trim()) {
      newErrors.nome = 'Nome é obrigatório';
    }
    if (!formData.descricaoAcademia.trim()) {
      newErrors.descricaoAcademia = 'Descrição da Academia é obrigatória';
    }
    if (formData.origem === 'Manual' && !formData.criadoPor.trim()) {
      newErrors.criadoPor = 'Criado por é obrigatório para cursos manuais';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSave = async () => {
    if (!validateForm()) return;

    setSaving(true);
    
    try {
      // Simulate API call
      await new Promise(resolve => setTimeout(resolve, 1000));
      
      const cursoData = {
        ...formData,
        id: curso?.id || Date.now(),
        createdAt: curso?.createdAt || new Date().toISOString(),
        updatedAt: new Date().toISOString()
      };

      onSave(cursoData);
      onClose();
    } catch (error) {
      console.error('Erro ao salvar curso:', error);
    } finally {
      setSaving(false);
    }
  };

  const isReadOnly = mode === 'view';
  const isEditing = mode === 'edit';
  const isCreating = mode === 'create';

  const getStatusColor = (status) => {
    const colors = {
      'Backlog': 'bg-gray-100 text-gray-800',
      'Em Desenvolvimento': 'bg-blue-100 text-blue-800',
      'Veiculado': 'bg-green-100 text-green-800'
    };
    return colors[status] || 'bg-gray-100 text-gray-800';
  };

  const getPrioridadeColor = (prioridade) => {
    const colors = {
      'Baixa': 'bg-green-100 text-green-800',
      'Media': 'bg-yellow-100 text-yellow-800',
      'Alta': 'bg-red-100 text-red-800'
    };
    return colors[prioridade] || 'bg-gray-100 text-gray-800';
  };

  return (
    <Dialog open={isOpen} onOpenChange={onClose}>
      <DialogContent className="max-w-4xl max-h-[90vh] overflow-y-auto">
        <DialogHeader>
          <DialogTitle className="flex items-center gap-2">
            {isCreating && <><Save className="h-5 w-5" /> Novo Curso</>}
            {isEditing && <><Save className="h-5 w-5" /> Editar Curso</>}
            {isReadOnly && <><Info className="h-5 w-5" /> Detalhes do Curso</>}
          </DialogTitle>
          <DialogDescription>
            {isCreating && 'Preencha as informações para criar um novo curso'}
            {isEditing && 'Edite as informações do curso'}
            {isReadOnly && 'Visualize as informações detalhadas do curso'}
          </DialogDescription>
        </DialogHeader>

        <div className="space-y-6">
          {/* Informações Básicas */}
          <div className="space-y-4">
            <h3 className="text-lg font-semibold text-gray-900 border-b pb-2">
              Informações Básicas
            </h3>
            
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div>
                <label className="text-sm font-medium text-gray-700 mb-2 block">
                  Código do Curso *
                </label>
                <Input
                  value={formData.codigo}
                  onChange={(e) => handleInputChange('codigo', e.target.value)}
                  placeholder="Ex: FC-001"
                  disabled={isReadOnly || (isEditing && formData.origem === 'Senior')}
                  className={errors.codigo ? 'border-red-500' : ''}
                />
                {errors.codigo && (
                  <p className="text-sm text-red-600 mt-1">{errors.codigo}</p>
                )}
              </div>

              <div>
                <label className="text-sm font-medium text-gray-700 mb-2 block">
                  Nome do Curso *
                </label>
                <Input
                  value={formData.nome}
                  onChange={(e) => handleInputChange('nome', e.target.value)}
                  placeholder="Ex: Photoshop Básico para Marketing"
                  disabled={isReadOnly}
                  className={errors.nome ? 'border-red-500' : ''}
                />
                {errors.nome && (
                  <p className="text-sm text-red-600 mt-1">{errors.nome}</p>
                )}
              </div>

              <div className="md:col-span-2">
                <label className="text-sm font-medium text-gray-700 mb-2 block">
                  Descrição da Academia *
                </label>
                <Textarea
                  value={formData.descricaoAcademia}
                  onChange={(e) => handleInputChange('descricaoAcademia', e.target.value)}
                  placeholder="Descreva o objetivo e conteúdo do curso..."
                  rows={3}
                  disabled={isReadOnly}
                  className={errors.descricaoAcademia ? 'border-red-500' : ''}
                />
                {errors.descricaoAcademia && (
                  <p className="text-sm text-red-600 mt-1">{errors.descricaoAcademia}</p>
                )}
              </div>
            </div>
          </div>

          {/* Status e Configurações */}
          <div className="space-y-4">
            <h3 className="text-lg font-semibold text-gray-900 border-b pb-2">
              Status e Configurações
            </h3>
            
            <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
              <div>
                <label className="text-sm font-medium text-gray-700 mb-2 block">
                  Status
                </label>
                <Select
                  value={formData.status}
                  onValueChange={(value) => handleInputChange('status', value)}
                  disabled={isReadOnly}
                >
                  <SelectTrigger>
                    <SelectValue />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="Backlog">Backlog</SelectItem>
                    <SelectItem value="Em Desenvolvimento">Em Desenvolvimento</SelectItem>
                    <SelectItem value="Veiculado">Veiculado</SelectItem>
                  </SelectContent>
                </Select>
              </div>

              <div>
                <label className="text-sm font-medium text-gray-700 mb-2 block">
                  Tipo de Ambiente
                </label>
                <Select
                  value={formData.tipoAmbiente}
                  onValueChange={(value) => handleInputChange('tipoAmbiente', value)}
                  disabled={isReadOnly}
                >
                  <SelectTrigger>
                    <SelectValue />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="Presencial">Presencial</SelectItem>
                    <SelectItem value="Online">Online</SelectItem>
                    <SelectItem value="Hibrido">Híbrido</SelectItem>
                  </SelectContent>
                </Select>
              </div>

              <div>
                <label className="text-sm font-medium text-gray-700 mb-2 block">
                  Tipo de Acesso
                </label>
                <Select
                  value={formData.tipoAcesso}
                  onValueChange={(value) => handleInputChange('tipoAcesso', value)}
                  disabled={isReadOnly}
                >
                  <SelectTrigger>
                    <SelectValue />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="Publico">Público</SelectItem>
                    <SelectItem value="Restrito">Restrito</SelectItem>
                    <SelectItem value="Privado">Privado</SelectItem>
                  </SelectContent>
                </Select>
              </div>

              <div>
                <label className="text-sm font-medium text-gray-700 mb-2 block">
                  Nível
                </label>
                <Select
                  value={formData.nivel}
                  onValueChange={(value) => handleInputChange('nivel', value)}
                  disabled={isReadOnly}
                >
                  <SelectTrigger>
                    <SelectValue />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="Iniciante">Iniciante</SelectItem>
                    <SelectItem value="Intermediario">Intermediário</SelectItem>
                    <SelectItem value="Avancado">Avançado</SelectItem>
                  </SelectContent>
                </Select>
              </div>

              <div>
                <label className="text-sm font-medium text-gray-700 mb-2 block">
                  Prioridade
                </label>
                <Select
                  value={formData.prioridade}
                  onValueChange={(value) => handleInputChange('prioridade', value)}
                  disabled={isReadOnly}
                >
                  <SelectTrigger>
                    <SelectValue />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="Baixa">Baixa</SelectItem>
                    <SelectItem value="Media">Média</SelectItem>
                    <SelectItem value="Alta">Alta</SelectItem>
                  </SelectContent>
                </Select>
              </div>

              <div>
                <label className="text-sm font-medium text-gray-700 mb-2 block">
                  Carga Horária
                </label>
                <Input
                  value={formData.cargaHoraria}
                  onChange={(e) => handleInputChange('cargaHoraria', e.target.value)}
                  placeholder="Ex: 40h"
                  disabled={isReadOnly}
                />
              </div>
            </div>
          </div>

          {/* Origem e Responsabilidade */}
          <div className="space-y-4">
            <h3 className="text-lg font-semibold text-gray-900 border-b pb-2">
              Origem e Responsabilidade
            </h3>
            
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div>
                <label className="text-sm font-medium text-gray-700 mb-2 block">
                  Origem
                </label>
                <Select
                  value={formData.origem}
                  onValueChange={(value) => handleInputChange('origem', value)}
                  disabled={isReadOnly || isEditing}
                >
                  <SelectTrigger>
                    <SelectValue />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="Manual">Manual</SelectItem>
                    <SelectItem value="Senior">Senior</SelectItem>
                  </SelectContent>
                </Select>
              </div>

              {formData.origem === 'Manual' && (
                <div>
                  <label className="text-sm font-medium text-gray-700 mb-2 block">
                    Criado por *
                  </label>
                  <Input
                    value={formData.criadoPor}
                    onChange={(e) => handleInputChange('criadoPor', e.target.value)}
                    placeholder="Nome do criador"
                    disabled={isReadOnly}
                    className={errors.criadoPor ? 'border-red-500' : ''}
                  />
                  {errors.criadoPor && (
                    <p className="text-sm text-red-600 mt-1">{errors.criadoPor}</p>
                  )}
                </div>
              )}

              <div>
                <label className="text-sm font-medium text-gray-700 mb-2 block">
                  Responsável
                </label>
                <Input
                  value={formData.responsavel}
                  onChange={(e) => handleInputChange('responsavel', e.target.value)}
                  placeholder="Nome do responsável"
                  disabled={isReadOnly}
                />
              </div>

              <div>
                <label className="text-sm font-medium text-gray-700 mb-2 block">
                  Departamento
                </label>
                <Input
                  value={formData.departamento}
                  onChange={(e) => handleInputChange('departamento', e.target.value)}
                  placeholder="Ex: TI, Design, Marketing"
                  disabled={isReadOnly}
                />
              </div>

              <div>
                <label className="text-sm font-medium text-gray-700 mb-2 block">
                  Data de Início de Operação
                </label>
                <Input
                  type="date"
                  value={formData.dataInicioOperacao}
                  onChange={(e) => handleInputChange('dataInicioOperacao', e.target.value)}
                  disabled={isReadOnly}
                />
              </div>

              <div>
                <label className="text-sm font-medium text-gray-700 mb-2 block">
                  Categoria
                </label>
                <Input
                  value={formData.categoria}
                  onChange={(e) => handleInputChange('categoria', e.target.value)}
                  placeholder="Ex: Design, Programação, Marketing"
                  disabled={isReadOnly}
                />
              </div>
            </div>
          </div>

          {/* Tags */}
          <div className="space-y-4">
            <h3 className="text-lg font-semibold text-gray-900 border-b pb-2">
              Tags
            </h3>
            
            {!isReadOnly && (
              <div className="flex gap-2">
                <Input
                  value={newTag}
                  onChange={(e) => setNewTag(e.target.value)}
                  placeholder="Adicionar tag..."
                  onKeyPress={(e) => e.key === 'Enter' && addTag()}
                />
                <Button type="button" onClick={addTag} variant="outline">
                  Adicionar
                </Button>
              </div>
            )}
            
            <div className="flex flex-wrap gap-2">
              {formData.tags.map((tag, index) => (
                <Badge key={index} variant="secondary" className="flex items-center gap-1">
                  {tag}
                  {!isReadOnly && (
                    <X 
                      className="h-3 w-3 cursor-pointer" 
                      onClick={() => removeTag(tag)}
                    />
                  )}
                </Badge>
              ))}
            </div>
          </div>

          {/* Progresso e Comentários */}
          <div className="space-y-4">
            <h3 className="text-lg font-semibold text-gray-900 border-b pb-2">
              Progresso e Observações
            </h3>
            
            <div className="space-y-4">
              <div>
                <label className="text-sm font-medium text-gray-700 mb-2 block">
                  Progresso ({formData.progresso}%)
                </label>
                <input
                  type="range"
                  min="0"
                  max="100"
                  value={formData.progresso}
                  onChange={(e) => handleInputChange('progresso', parseInt(e.target.value))}
                  disabled={isReadOnly}
                  className="w-full"
                />
                <div className="w-full bg-gray-200 rounded-full h-2 mt-2">
                  <div 
                    className="bg-blue-600 h-2 rounded-full transition-all duration-300"
                    style={{ width: `${formData.progresso}%` }}
                  ></div>
                </div>
              </div>

              <div>
                <label className="text-sm font-medium text-gray-700 mb-2 block">
                  Comentários Internos
                </label>
                <Textarea
                  value={formData.comentariosInternos}
                  onChange={(e) => handleInputChange('comentariosInternos', e.target.value)}
                  placeholder="Observações internas sobre o curso..."
                  rows={3}
                  disabled={isReadOnly}
                />
              </div>

              <div className="flex items-center justify-between">
                <div>
                  <label className="text-sm font-medium text-gray-700">Curso Ativo</label>
                  <p className="text-xs text-gray-500">Curso visível e disponível no sistema</p>
                </div>
                <Switch
                  checked={formData.ativo}
                  onCheckedChange={(checked) => handleInputChange('ativo', checked)}
                  disabled={isReadOnly}
                />
              </div>
            </div>
          </div>

          {/* Resumo Visual (apenas para visualização) */}
          {isReadOnly && (
            <div className="space-y-4">
              <h3 className="text-lg font-semibold text-gray-900 border-b pb-2">
                Resumo
              </h3>
              
              <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
                <div className="text-center p-3 bg-gray-50 rounded-lg">
                  <Badge className={getStatusColor(formData.status)}>
                    {formData.status}
                  </Badge>
                  <p className="text-xs text-gray-500 mt-1">Status</p>
                </div>
                
                <div className="text-center p-3 bg-gray-50 rounded-lg">
                  <Badge className={getPrioridadeColor(formData.prioridade)}>
                    {formData.prioridade}
                  </Badge>
                  <p className="text-xs text-gray-500 mt-1">Prioridade</p>
                </div>
                
                <div className="text-center p-3 bg-gray-50 rounded-lg">
                  <p className="font-semibold text-blue-600">{formData.progresso}%</p>
                  <p className="text-xs text-gray-500 mt-1">Progresso</p>
                </div>
                
                <div className="text-center p-3 bg-gray-50 rounded-lg">
                  <Badge variant={formData.origem === 'Senior' ? 'default' : 'secondary'}>
                    {formData.origem}
                  </Badge>
                  <p className="text-xs text-gray-500 mt-1">Origem</p>
                </div>
              </div>
            </div>
          )}

          {/* Alertas */}
          {formData.origem === 'Senior' && (isEditing || isReadOnly) && (
            <Alert>
              <Info className="h-4 w-4" />
              <AlertDescription>
                Este curso foi importado do sistema Senior. Algumas informações não podem ser editadas.
              </AlertDescription>
            </Alert>
          )}
        </div>

        <DialogFooter className="flex gap-2">
          <Button variant="outline" onClick={onClose}>
            {isReadOnly ? 'Fechar' : 'Cancelar'}
          </Button>
          
          {!isReadOnly && (
            <Button onClick={handleSave} disabled={loading}>
              {loading ? (
                <>
                  <Loader2 className="h-4 w-4 mr-2 animate-spin" />
                  Salvando...
                </>
              ) : (
                <>
                  <Save className="h-4 w-4 mr-2" />
                  {isCreating ? 'Criar Curso' : 'Salvar Alterações'}
                </>
              )}
            </Button>
          )}
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
};

export default CursoModal;

