using Microsoft.OpenApi.Models;
using System.Reflection;

namespace AcervoEducacional.WebApi.Configuration
{
    /// <summary>
    /// Configuração do Swagger/OpenAPI para documentação da API
    /// </summary>
    public static class SwaggerConfiguration
    {
        /// <summary>
        /// Configura os serviços do Swagger
        /// </summary>
        public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                // Informações básicas da API
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1.0.0",
                    Title = "Acervo Educacional - Ferreira Costa",
                    Description = "API REST para o Sistema de Acervo Educacional da Ferreira Costa. " +
                                 "Esta API fornece endpoints para gestão de cursos, arquivos, usuários, " +
                                 "relatórios e autenticação com segurança JWT.",
                    Contact = new OpenApiContact
                    {
                        Name = "Equipe de Desenvolvimento - Ferreira Costa",
                        Email = "desenvolvimento@ferreiracosta.com",
                        Url = new Uri("https://www.ferreiracosta.com")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Propriedade da Ferreira Costa",
                        Url = new Uri("https://www.ferreiracosta.com/termos")
                    },
                    TermsOfService = new Uri("https://www.ferreiracosta.com/termos")
                });

                // Configuração de autenticação JWT
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Insira o token JWT no formato: Bearer {seu_token_aqui}"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });

                // Configurações de documentação
                options.EnableAnnotations();
                options.DescribeAllParametersInCamelCase();
                
                // Inclui comentários XML
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    options.IncludeXmlComments(xmlPath);
                }

                // Configurações de schema
                options.CustomSchemaIds(type => type.FullName?.Replace("+", "."));
                
                // Configurações de operação
                options.OperationFilter<SwaggerOperationFilter>();
                
                // Configurações de documento
                options.DocumentFilter<SwaggerDocumentFilter>();

                // Configurações de schema
                options.SchemaFilter<SwaggerSchemaFilter>();
            });

            return services;
        }

        /// <summary>
        /// Configura o middleware do Swagger
        /// </summary>
        public static IApplicationBuilder UseSwaggerConfiguration(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger(options =>
            {
                options.RouteTemplate = "api/docs/{documentName}/swagger.json";
            });

            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/api/docs/v1/swagger.json", "Acervo Educacional API v1");
                options.RoutePrefix = "api/docs";
                options.DocumentTitle = "Acervo Educacional - Ferreira Costa | API Documentation";
                
                // Configurações de interface
                options.DefaultModelsExpandDepth(-1); // Oculta modelos por padrão
                options.DefaultModelRendering(Swashbuckle.AspNetCore.SwaggerUI.ModelRendering.Model);
                options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
                options.EnableDeepLinking();
                options.EnableFilter();
                options.EnableValidator();
                options.ShowExtensions();
                options.ShowCommonExtensions();
                
                // CSS customizado com identidade Ferreira Costa
                options.InjectStylesheet("/swagger-ui/custom.css");
                
                // JavaScript customizado
                options.InjectJavaScript("/swagger-ui/custom.js");
                
                // Configurações de autenticação
                options.OAuthClientId("swagger-ui");
                options.OAuthAppName("Acervo Educacional API");
                options.OAuthUsePkce();
            });

            return app;
        }
    }

    /// <summary>
    /// Filtro para personalizar operações no Swagger
    /// </summary>
    public class SwaggerOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Adiciona tags baseadas no controller
            var controllerName = context.MethodInfo.DeclaringType?.Name.Replace("Controller", "");
            if (!string.IsNullOrEmpty(controllerName))
            {
                operation.Tags = new List<OpenApiTag>
                {
                    new OpenApiTag { Name = controllerName }
                };
            }

            // Adiciona exemplos de resposta
            if (operation.Responses.ContainsKey("200"))
            {
                operation.Responses["200"].Description = "Operação realizada com sucesso";
            }

            if (operation.Responses.ContainsKey("400"))
            {
                operation.Responses["400"].Description = "Dados inválidos ou parâmetros incorretos";
            }

            if (operation.Responses.ContainsKey("401"))
            {
                operation.Responses["401"].Description = "Token de acesso requerido ou inválido";
            }

            if (operation.Responses.ContainsKey("403"))
            {
                operation.Responses["403"].Description = "Acesso negado - permissões insuficientes";
            }

            if (operation.Responses.ContainsKey("404"))
            {
                operation.Responses["404"].Description = "Recurso não encontrado";
            }

            if (operation.Responses.ContainsKey("500"))
            {
                operation.Responses["500"].Description = "Erro interno do servidor";
            }

            // Remove parâmetros desnecessários
            var parametersToRemove = operation.Parameters
                .Where(p => p.Name.Equals("version", StringComparison.OrdinalIgnoreCase))
                .ToList();

            foreach (var parameter in parametersToRemove)
            {
                operation.Parameters.Remove(parameter);
            }
        }
    }

    /// <summary>
    /// Filtro para personalizar o documento Swagger
    /// </summary>
    public class SwaggerDocumentFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            // Organiza tags por ordem alfabética
            swaggerDoc.Tags = swaggerDoc.Tags?.OrderBy(t => t.Name).ToList();

            // Adiciona informações de servidor
            swaggerDoc.Servers = new List<OpenApiServer>
            {
                new OpenApiServer
                {
                    Url = "https://acervo.ferreiracosta.com",
                    Description = "Servidor de Produção"
                },
                new OpenApiServer
                {
                    Url = "https://acervo-dev.ferreiracosta.com",
                    Description = "Servidor de Desenvolvimento"
                },
                new OpenApiServer
                {
                    Url = "http://localhost:5000",
                    Description = "Servidor Local"
                }
            };

            // Remove paths desnecessários
            var pathsToRemove = swaggerDoc.Paths
                .Where(p => p.Key.Contains("/health") || p.Key.Contains("/metrics"))
                .Select(p => p.Key)
                .ToList();

            foreach (var path in pathsToRemove)
            {
                swaggerDoc.Paths.Remove(path);
            }
        }
    }

    /// <summary>
    /// Filtro para personalizar schemas no Swagger
    /// </summary>
    public class SwaggerSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            // Adiciona exemplos para tipos comuns
            if (context.Type == typeof(Guid) || context.Type == typeof(Guid?))
            {
                schema.Example = new Microsoft.OpenApi.Any.OpenApiString("123e4567-e89b-12d3-a456-426614174000");
            }

            if (context.Type == typeof(DateTime) || context.Type == typeof(DateTime?))
            {
                schema.Example = new Microsoft.OpenApi.Any.OpenApiString("2024-01-15T10:30:00Z");
            }

            // Remove propriedades sensíveis
            if (schema.Properties != null)
            {
                var sensitiveProperties = schema.Properties
                    .Where(p => p.Key.ToLower().Contains("password") || 
                               p.Key.ToLower().Contains("senha") ||
                               p.Key.ToLower().Contains("token"))
                    .Select(p => p.Key)
                    .ToList();

                foreach (var prop in sensitiveProperties)
                {
                    if (schema.Properties.ContainsKey(prop))
                    {
                        schema.Properties[prop].WriteOnly = true;
                        schema.Properties[prop].Example = new Microsoft.OpenApi.Any.OpenApiString("***");
                    }
                }
            }
        }
    }
}

