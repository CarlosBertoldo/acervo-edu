# Dockerfile para AcervoEducacional - Clean Architecture
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar arquivos de projeto
COPY ["src/AcervoEducacional.WebApi/AcervoEducacional.WebApi.csproj", "src/AcervoEducacional.WebApi/"]
COPY ["src/AcervoEducacional.Application/AcervoEducacional.Application.csproj", "src/AcervoEducacional.Application/"]
COPY ["src/AcervoEducacional.Infrastructure/AcervoEducacional.Infrastructure.csproj", "src/AcervoEducacional.Infrastructure/"]
COPY ["src/AcervoEducacional.Domain/AcervoEducacional.Domain.csproj", "src/AcervoEducacional.Domain/"]

# Restaurar dependências
RUN dotnet restore "src/AcervoEducacional.WebApi/AcervoEducacional.WebApi.csproj"

# Copiar código fonte
COPY . .

# Build da aplicação
WORKDIR "/src/src/AcervoEducacional.WebApi"
RUN dotnet build "AcervoEducacional.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AcervoEducacional.WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

# Criar usuário não-root
RUN addgroup --system --gid 1001 dotnet && \
    adduser --system --uid 1001 --ingroup dotnet --shell /bin/false dotnet

# Criar diretórios necessários
RUN mkdir -p /app/logs /app/uploads && \
    chown -R dotnet:dotnet /app

COPY --from=publish /app/publish .

# Mudar para usuário não-root
USER dotnet

ENTRYPOINT ["dotnet", "AcervoEducacional.WebApi.dll"]

