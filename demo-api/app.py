#!/usr/bin/env python3
"""
API Demo - Sistema Acervo Educacional Ferreira Costa
Demonstração funcional da API com endpoints principais
"""

from flask import Flask, jsonify, request
from flask_cors import CORS
from datetime import datetime
import json

app = Flask(__name__)
CORS(app)  # Permitir CORS para o frontend

# Dados fictícios para demonstração
usuarios_demo = [
    {
        "id": 1,
        "nome": "Carlos Bertoldo",
        "email": "carlos@ferreiracosta.com",
        "role": "Admin",
        "ativo": True,
        "ultimoLogin": "2025-01-02T10:30:00Z"
    },
    {
        "id": 2,
        "nome": "Maria Silva",
        "email": "maria@ferreiracosta.com", 
        "role": "Gestor",
        "ativo": True,
        "ultimoLogin": "2025-01-02T09:15:00Z"
    },
    {
        "id": 3,
        "nome": "João Santos",
        "email": "joao@ferreiracosta.com",
        "role": "Usuario",
        "ativo": True,
        "ultimoLogin": "2025-01-01T16:45:00Z"
    }
]

cursos_demo = [
    {
        "id": 1,
        "titulo": "Gestão de Vendas Ferreira Costa",
        "descricao": "Curso completo sobre técnicas de vendas e atendimento ao cliente",
        "categoria": "Vendas",
        "status": "Ativo",
        "duracao": "40 horas",
        "participantes": 156,
        "criadoEm": "2024-12-01T00:00:00Z"
    },
    {
        "id": 2,
        "titulo": "Segurança no Trabalho",
        "descricao": "Normas de segurança e prevenção de acidentes",
        "categoria": "Segurança",
        "status": "Ativo", 
        "duracao": "20 horas",
        "participantes": 89,
        "criadoEm": "2024-11-15T00:00:00Z"
    },
    {
        "id": 3,
        "titulo": "Atendimento ao Cliente",
        "descricao": "Excelência no atendimento e fidelização de clientes",
        "categoria": "Atendimento",
        "status": "Rascunho",
        "duracao": "30 horas", 
        "participantes": 0,
        "criadoEm": "2025-01-01T00:00:00Z"
    }
]

arquivos_demo = [
    {
        "id": 1,
        "nome": "Manual_Vendas_2025.pdf",
        "tipo": "PDF",
        "tamanho": "2.5 MB",
        "categoria": "Documento",
        "cursoId": 1,
        "uploadEm": "2024-12-01T10:00:00Z"
    },
    {
        "id": 2,
        "nome": "Video_Seguranca_Trabalho.mp4",
        "tipo": "Video",
        "tamanho": "45.2 MB",
        "categoria": "Video",
        "cursoId": 2,
        "uploadEm": "2024-11-15T14:30:00Z"
    }
]

@app.route('/')
def home():
    return jsonify({
        "sistema": "Acervo Educacional Ferreira Costa",
        "versao": "1.0.0",
        "status": "Operacional",
        "timestamp": datetime.utcnow().isoformat() + "Z",
        "endpoints": {
            "usuarios": "/api/usuarios",
            "cursos": "/api/cursos", 
            "arquivos": "/api/arquivos",
            "auth": "/api/auth/login",
            "health": "/health",
            "swagger": "/swagger"
        }
    })

@app.route('/health')
def health():
    return jsonify({
        "status": "healthy",
        "timestamp": datetime.utcnow().isoformat() + "Z",
        "services": {
            "database": "connected",
            "storage": "available",
            "email": "operational"
        }
    })

@app.route('/swagger')
def swagger():
    return jsonify({
        "swagger": "2.0",
        "info": {
            "title": "Acervo Educacional API",
            "version": "1.0.0",
            "description": "API do Sistema Acervo Educacional Ferreira Costa"
        },
        "host": "localhost:5000",
        "basePath": "/api",
        "schemes": ["http", "https"],
        "paths": {
            "/usuarios": {
                "get": {
                    "summary": "Listar usuários",
                    "responses": {"200": {"description": "Lista de usuários"}}
                }
            },
            "/cursos": {
                "get": {
                    "summary": "Listar cursos", 
                    "responses": {"200": {"description": "Lista de cursos"}}
                }
            },
            "/arquivos": {
                "get": {
                    "summary": "Listar arquivos",
                    "responses": {"200": {"description": "Lista de arquivos"}}
                }
            }
        }
    })

@app.route('/api/usuarios')
def get_usuarios():
    return jsonify({
        "success": True,
        "data": usuarios_demo,
        "total": len(usuarios_demo),
        "message": "Usuários recuperados com sucesso"
    })

@app.route('/api/cursos')
def get_cursos():
    return jsonify({
        "success": True,
        "data": cursos_demo,
        "total": len(cursos_demo),
        "message": "Cursos recuperados com sucesso"
    })

@app.route('/api/arquivos')
def get_arquivos():
    return jsonify({
        "success": True,
        "data": arquivos_demo,
        "total": len(arquivos_demo),
        "message": "Arquivos recuperados com sucesso"
    })

@app.route('/api/auth/login', methods=['POST'])
def login():
    data = request.get_json()
    email = data.get('email', '')
    senha = data.get('senha', '')
    
    # Simulação de login (aceita qualquer credencial para demo)
    if email and senha:
        return jsonify({
            "success": True,
            "data": {
                "token": "demo_jwt_token_12345",
                "usuario": {
                    "id": 1,
                    "nome": "Demo User",
                    "email": email,
                    "role": "Admin"
                },
                "expiresIn": "24h"
            },
            "message": "Login realizado com sucesso"
        })
    else:
        return jsonify({
            "success": False,
            "message": "Email e senha são obrigatórios"
        }), 400

@app.route('/api/dashboard/stats')
def dashboard_stats():
    return jsonify({
        "success": True,
        "data": {
            "totalUsuarios": len(usuarios_demo),
            "totalCursos": len(cursos_demo),
            "totalArquivos": len(arquivos_demo),
            "cursosAtivos": len([c for c in cursos_demo if c["status"] == "Ativo"]),
            "usuariosAtivos": len([u for u in usuarios_demo if u["ativo"]]),
            "ultimaAtualizacao": datetime.utcnow().isoformat() + "Z"
        },
        "message": "Estatísticas do dashboard"
    })

if __name__ == '__main__':
    print("🚀 Iniciando API Demo - Sistema Acervo Educacional Ferreira Costa")
    print("📊 Endpoints disponíveis:")
    print("   • GET  /              - Informações da API")
    print("   • GET  /health        - Health Check")
    print("   • GET  /swagger       - Documentação Swagger")
    print("   • GET  /api/usuarios  - Listar usuários")
    print("   • GET  /api/cursos    - Listar cursos")
    print("   • GET  /api/arquivos  - Listar arquivos")
    print("   • POST /api/auth/login - Autenticação")
    print("   • GET  /api/dashboard/stats - Estatísticas")
    print("")
    app.run(host='0.0.0.0', port=5000, debug=True)

