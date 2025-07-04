namespace AcervoEducacional.Domain.Enums;

public enum StatusCurso
{
    Backlog = 1,
    EmDesenvolvimento = 2,
    Veiculado = 3,
    Rascunho = 4,
    Planejamento = 5,
    Revisao = 6,
    Aprovado = 7,
    Publicado = 8,
    Ativo = 9,
    Pausado = 10,
    Concluido = 11,
    Arquivado = 12
}

public enum TipoAmbiente
{
    Presencial = 1,
    Online = 2,
    Hibrido = 3
}

public enum TipoAcesso
{
    Publico = 1,
    Restrito = 2,
    Privado = 3
}

public enum OrigemCurso
{
    Manual = 1,
    Senior = 2
}

public enum TipoUsuario
{
    Admin = 1,
    Usuario = 2,
    Administrador = 3
}

public enum StatusUsuario
{
    Ativo = 1,
    Inativo = 2,
    Bloqueado = 3
}

public enum CategoriaArquivo
{
    BriefingDesenvolvimento = 1,
    BriefingExecucao = 2,
    PPT = 3,
    CadernoExercicio = 4,
    PlanoAula = 5,
    Videos = 6,
    Podcast = 7,
    Outros = 8,
    Imagem = 9,
    Video = 10,
    Audio = 11,
    Documento = 12,
    Apresentacao = 13,
    Planilha = 14,
    Arquivo = 15
}

public enum TipoCompartilhamento
{
    Publico = 1,
    Restrito = 2,
    ComExpiracao = 3
}

public enum TipoAtividade
{
    Login = 1,
    Logout = 2,
    CriacaoCurso = 3,
    EdicaoCurso = 4,
    ExclusaoCurso = 5,
    MovimentacaoCurso = 6,
    UploadArquivo = 7,
    DownloadArquivo = 8,
    CompartilhamentoArquivo = 9,
    ExclusaoArquivo = 10,
    SincronizacaoSenior = 11,
    ExportacaoRelatorio = 12,
    LoginFalhou = 13,
    RefreshToken = 14,
    SolicitacaoRecuperacaoSenha = 15,
    AlteracaoSenha = 16,
    CriacaoUsuario = 17,
    EdicaoUsuario = 18,
    ExclusaoUsuario = 19,
    AlteracaoStatusCurso = 20,
    EventoSeguranca = 21
}

public enum StatusSincronizacao
{
    Sucesso = 1,
    Erro = 2,
    Parcial = 3,
    EmAndamento = 4
}

public enum TipoConflito
{
    DadosAlterados = 1,
    CursoExcluido = 2,
    CampoObrigatorio = 3,
    FormatoInvalido = 4
}

public enum ResolucaoConflito
{
    ManterLocal = 1,
    UsarSenior = 2,
    Ignorar = 3,
    Manual = 4
}

public enum TipoConfiguracao
{
    Texto = 1,
    Numero = 2,
    Boolean = 3,
    Senha = 4,
    Json = 5
}

