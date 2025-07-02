using System.Net;
using System.Text.Json;
using AcervoEducacional.Application.DTOs.Common;

namespace AcervoEducacional.WebApi.Middleware
{
    /// <summary>
    /// Middleware para tratamento global de erros e exceções
    /// </summary>
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;
        private readonly IWebHostEnvironment _environment;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger, IWebHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro não tratado na requisição {Method} {Path}", 
                    context.Request.Method, context.Request.Path);
                
                await HandleExceptionAsync(context, ex);
            }
        }

        /// <summary>
        /// Trata exceções e retorna resposta padronizada
        /// </summary>
        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            
            var response = new ApiResponse<object>
            {
                Success = false,
                Data = null
            };

            switch (exception)
            {
                case ArgumentException argEx:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.Message = "Parâmetros inválidos";
                    response.Error = argEx.Message;
                    break;

                case ArgumentNullException nullEx:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.Message = "Parâmetro obrigatório não informado";
                    response.Error = nullEx.ParamName ?? "Parâmetro não identificado";
                    break;

                case UnauthorizedAccessException:
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    response.Message = "Acesso não autorizado";
                    response.Error = "UNAUTHORIZED";
                    break;

                case KeyNotFoundException:
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    response.Message = "Recurso não encontrado";
                    response.Error = "NOT_FOUND";
                    break;

                case InvalidOperationException invOpEx:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.Message = "Operação inválida";
                    response.Error = invOpEx.Message;
                    break;

                case TimeoutException:
                    context.Response.StatusCode = (int)HttpStatusCode.RequestTimeout;
                    response.Message = "Tempo limite da requisição excedido";
                    response.Error = "TIMEOUT";
                    break;

                case NotSupportedException notSupEx:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.Message = "Operação não suportada";
                    response.Error = notSupEx.Message;
                    break;

                case FileNotFoundException:
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    response.Message = "Arquivo não encontrado";
                    response.Error = "FILE_NOT_FOUND";
                    break;

                case DirectoryNotFoundException:
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    response.Message = "Diretório não encontrado";
                    response.Error = "DIRECTORY_NOT_FOUND";
                    break;

                case HttpRequestException httpEx:
                    context.Response.StatusCode = (int)HttpStatusCode.BadGateway;
                    response.Message = "Erro na comunicação com serviço externo";
                    response.Error = httpEx.Message;
                    break;

                case TaskCanceledException:
                    context.Response.StatusCode = (int)HttpStatusCode.RequestTimeout;
                    response.Message = "Operação cancelada por timeout";
                    response.Error = "OPERATION_CANCELLED";
                    break;

                default:
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    response.Message = "Erro interno do servidor";
                    response.Error = "INTERNAL_SERVER_ERROR";
                    break;
            }

            // Em desenvolvimento, inclui detalhes da exceção
            if (_environment.IsDevelopment())
            {
                response.Error = exception.Message;
                response.Data = new
                {
                    StackTrace = exception.StackTrace,
                    InnerException = exception.InnerException?.Message,
                    Source = exception.Source
                };
            }

            // Log detalhado para monitoramento
            var logLevel = context.Response.StatusCode >= 500 ? LogLevel.Error : LogLevel.Warning;
            
            _logger.Log(logLevel, exception, 
                "Erro {StatusCode} na requisição {Method} {Path} - {Message}", 
                context.Response.StatusCode, 
                context.Request.Method, 
                context.Request.Path, 
                exception.Message);

            // Serializa e retorna a resposta
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = _environment.IsDevelopment()
            };

            var jsonResponse = JsonSerializer.Serialize(response, jsonOptions);
            await context.Response.WriteAsync(jsonResponse);
        }
    }

    /// <summary>
    /// Extensão para registrar o ErrorHandlingMiddleware
    /// </summary>
    public static class ErrorHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseErrorHandling(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ErrorHandlingMiddleware>();
        }
    }

    /// <summary>
    /// Exceções customizadas para o domínio da aplicação
    /// </summary>
    public class BusinessException : Exception
    {
        public string ErrorCode { get; }

        public BusinessException(string message, string errorCode = "BUSINESS_ERROR") : base(message)
        {
            ErrorCode = errorCode;
        }

        public BusinessException(string message, Exception innerException, string errorCode = "BUSINESS_ERROR") 
            : base(message, innerException)
        {
            ErrorCode = errorCode;
        }
    }

    public class ValidationException : Exception
    {
        public Dictionary<string, string[]> Errors { get; }

        public ValidationException(string message) : base(message)
        {
            Errors = new Dictionary<string, string[]>();
        }

        public ValidationException(Dictionary<string, string[]> errors) : base("Erro de validação")
        {
            Errors = errors;
        }
    }

    public class DuplicateException : BusinessException
    {
        public DuplicateException(string message) : base(message, "DUPLICATE_ERROR")
        {
        }
    }

    public class NotFoundException : BusinessException
    {
        public NotFoundException(string message) : base(message, "NOT_FOUND")
        {
        }
    }

    public class ForbiddenException : BusinessException
    {
        public ForbiddenException(string message) : base(message, "FORBIDDEN")
        {
        }
    }

    public class ConflictException : BusinessException
    {
        public ConflictException(string message) : base(message, "CONFLICT")
        {
        }
    }
}

