// JavaScript Customizado para Swagger UI - Ferreira Costa
(function() {
    'use strict';

    // Aguarda o carregamento completo do Swagger UI
    window.addEventListener('load', function() {
        setTimeout(initializeCustomizations, 1000);
    });

    function initializeCustomizations() {
        addCustomHeader();
        addEnvironmentSelector();
        addQuickActions();
        addKeyboardShortcuts();
        addResponseTimeTracker();
        addTokenManager();
        enhanceErrorMessages();
        addThemeToggle();
        
        console.log('🎉 Swagger UI customizado para Ferreira Costa carregado com sucesso!');
    }

    // Adiciona header customizado com informações da empresa
    function addCustomHeader() {
        const info = document.querySelector('.swagger-ui .info');
        if (info && !document.querySelector('.fc-custom-header')) {
            const header = document.createElement('div');
            header.className = 'fc-custom-header';
            header.innerHTML = `
                <div style="background: linear-gradient(135deg, #DC2626 0%, #B91C1C 100%); color: white; padding: 20px; border-radius: 8px; margin-bottom: 20px; text-align: center;">
                    <h2 style="margin: 0; font-size: 24px; font-weight: 700;">🏢 Sistema Acervo Educacional</h2>
                    <p style="margin: 8px 0 0 0; opacity: 0.9;">Ferreira Costa - Tecnologia e Inovação</p>
                    <div style="margin-top: 12px; font-size: 14px;">
                        <span style="background: rgba(255,255,255,0.2); padding: 4px 8px; border-radius: 4px; margin: 0 4px;">📚 Cursos</span>
                        <span style="background: rgba(255,255,255,0.2); padding: 4px 8px; border-radius: 4px; margin: 0 4px;">📁 Arquivos</span>
                        <span style="background: rgba(255,255,255,0.2); padding: 4px 8px; border-radius: 4px; margin: 0 4px;">👥 Usuários</span>
                        <span style="background: rgba(255,255,255,0.2); padding: 4px 8px; border-radius: 4px; margin: 0 4px;">📊 Relatórios</span>
                    </div>
                </div>
            `;
            info.parentNode.insertBefore(header, info);
        }
    }

    // Adiciona seletor de ambiente
    function addEnvironmentSelector() {
        const topbar = document.querySelector('.swagger-ui .topbar');
        if (topbar && !document.querySelector('.fc-env-selector')) {
            const envSelector = document.createElement('div');
            envSelector.className = 'fc-env-selector';
            envSelector.innerHTML = `
                <select id="envSelect" style="
                    background: rgba(255,255,255,0.2); 
                    color: white; 
                    border: 1px solid rgba(255,255,255,0.3); 
                    border-radius: 4px; 
                    padding: 6px 12px; 
                    margin-right: 16px;
                    font-family: 'Barlow', sans-serif;
                    font-weight: 500;
                ">
                    <option value="prod">🌐 Produção</option>
                    <option value="dev">🔧 Desenvolvimento</option>
                    <option value="local" selected>💻 Local</option>
                </select>
            `;
            
            const wrapper = topbar.querySelector('.topbar-wrapper');
            if (wrapper) {
                wrapper.appendChild(envSelector);
                
                document.getElementById('envSelect').addEventListener('change', function(e) {
                    const baseUrls = {
                        'prod': 'https://acervo.ferreiracosta.com',
                        'dev': 'https://acervo-dev.ferreiracosta.com',
                        'local': 'http://localhost:5000'
                    };
                    
                    const newUrl = baseUrls[e.target.value];
                    if (newUrl && window.ui) {
                        window.ui.specActions.updateUrl(newUrl + '/api/docs/v1/swagger.json');
                        showNotification(`Ambiente alterado para: ${e.target.options[e.target.selectedIndex].text}`, 'success');
                    }
                });
            }
        }
    }

    // Adiciona ações rápidas
    function addQuickActions() {
        const authWrapper = document.querySelector('.swagger-ui .auth-wrapper');
        if (authWrapper && !document.querySelector('.fc-quick-actions')) {
            const quickActions = document.createElement('div');
            quickActions.className = 'fc-quick-actions';
            quickActions.innerHTML = `
                <div style="margin-top: 16px; padding: 16px; background: #F3F4F6; border-radius: 8px;">
                    <h4 style="margin: 0 0 12px 0; color: #374151; font-weight: 600;">⚡ Ações Rápidas</h4>
                    <div style="display: flex; gap: 8px; flex-wrap: wrap;">
                        <button onclick="fcQuickLogin()" style="background: #16A34A; color: white; border: none; padding: 6px 12px; border-radius: 4px; font-size: 12px; cursor: pointer;">🔑 Login Demo</button>
                        <button onclick="fcClearAuth()" style="background: #DC2626; color: white; border: none; padding: 6px 12px; border-radius: 4px; font-size: 12px; cursor: pointer;">🚪 Logout</button>
                        <button onclick="fcExportCollection()" style="background: #2563EB; color: white; border: none; padding: 6px 12px; border-radius: 4px; font-size: 12px; cursor: pointer;">📥 Exportar</button>
                        <button onclick="fcShowStats()" style="background: #F59E0B; color: white; border: none; padding: 6px 12px; border-radius: 4px; font-size: 12px; cursor: pointer;">📊 Estatísticas</button>
                    </div>
                </div>
            `;
            authWrapper.appendChild(quickActions);
        }
    }

    // Adiciona atalhos de teclado
    function addKeyboardShortcuts() {
        document.addEventListener('keydown', function(e) {
            // Ctrl/Cmd + K: Foco na busca
            if ((e.ctrlKey || e.metaKey) && e.key === 'k') {
                e.preventDefault();
                const searchInput = document.querySelector('.swagger-ui .filter input');
                if (searchInput) {
                    searchInput.focus();
                    showNotification('🔍 Busca ativada', 'info');
                }
            }
            
            // Ctrl/Cmd + Enter: Executar requisição focada
            if ((e.ctrlKey || e.metaKey) && e.key === 'Enter') {
                e.preventDefault();
                const executeBtn = document.querySelector('.swagger-ui .btn.execute:not([disabled])');
                if (executeBtn) {
                    executeBtn.click();
                    showNotification('🚀 Requisição executada', 'success');
                }
            }
            
            // Esc: Fechar modais
            if (e.key === 'Escape') {
                const modals = document.querySelectorAll('.swagger-ui .modal');
                modals.forEach(modal => {
                    if (modal.style.display !== 'none') {
                        modal.style.display = 'none';
                    }
                });
            }
        });
    }

    // Rastreador de tempo de resposta
    function addResponseTimeTracker() {
        const originalFetch = window.fetch;
        window.fetch = function(...args) {
            const startTime = performance.now();
            
            return originalFetch.apply(this, args).then(response => {
                const endTime = performance.now();
                const duration = Math.round(endTime - startTime);
                
                // Adiciona tempo de resposta ao header
                setTimeout(() => {
                    const responseHeaders = document.querySelector('.swagger-ui .response-col_description');
                    if (responseHeaders && !responseHeaders.querySelector('.fc-response-time')) {
                        const timeSpan = document.createElement('span');
                        timeSpan.className = 'fc-response-time';
                        timeSpan.innerHTML = `<br><small style="color: #16A34A; font-weight: 600;">⏱️ ${duration}ms</small>`;
                        responseHeaders.appendChild(timeSpan);
                    }
                }, 100);
                
                return response;
            });
        };
    }

    // Gerenciador de token
    function addTokenManager() {
        window.fcTokenManager = {
            save: function(token) {
                localStorage.setItem('fc_api_token', token);
                localStorage.setItem('fc_token_timestamp', Date.now().toString());
                showNotification('🔐 Token salvo com sucesso', 'success');
            },
            
            load: function() {
                const token = localStorage.getItem('fc_api_token');
                const timestamp = localStorage.getItem('fc_token_timestamp');
                
                if (token && timestamp) {
                    const age = Date.now() - parseInt(timestamp);
                    const hours = Math.floor(age / (1000 * 60 * 60));
                    
                    if (hours < 24) {
                        return token;
                    } else {
                        this.clear();
                        showNotification('⚠️ Token expirado, faça login novamente', 'warning');
                    }
                }
                return null;
            },
            
            clear: function() {
                localStorage.removeItem('fc_api_token');
                localStorage.removeItem('fc_token_timestamp');
                showNotification('🚪 Token removido', 'info');
            }
        };
        
        // Auto-carrega token salvo
        const savedToken = window.fcTokenManager.load();
        if (savedToken) {
            setTimeout(() => {
                const authInput = document.querySelector('.swagger-ui input[placeholder*="Bearer"]');
                if (authInput) {
                    authInput.value = savedToken;
                    showNotification('🔄 Token carregado automaticamente', 'info');
                }
            }, 2000);
        }
    }

    // Melhora mensagens de erro
    function enhanceErrorMessages() {
        const observer = new MutationObserver(function(mutations) {
            mutations.forEach(function(mutation) {
                mutation.addedNodes.forEach(function(node) {
                    if (node.nodeType === 1 && node.classList && node.classList.contains('response')) {
                        enhanceErrorResponse(node);
                    }
                });
            });
        });
        
        observer.observe(document.body, { childList: true, subtree: true });
    }

    function enhanceErrorResponse(responseElement) {
        const statusCode = responseElement.querySelector('.response-col_status');
        if (statusCode && (statusCode.textContent.startsWith('4') || statusCode.textContent.startsWith('5'))) {
            const errorHelp = document.createElement('div');
            errorHelp.className = 'fc-error-help';
            errorHelp.innerHTML = getErrorHelp(statusCode.textContent);
            responseElement.appendChild(errorHelp);
        }
    }

    function getErrorHelp(statusCode) {
        const errorMessages = {
            '400': '❌ <strong>Dados inválidos:</strong> Verifique os parâmetros enviados',
            '401': '🔐 <strong>Não autorizado:</strong> Token necessário ou inválido',
            '403': '🚫 <strong>Acesso negado:</strong> Permissões insuficientes',
            '404': '🔍 <strong>Não encontrado:</strong> Recurso não existe',
            '422': '⚠️ <strong>Dados inválidos:</strong> Verifique os campos obrigatórios',
            '429': '⏰ <strong>Muitas requisições:</strong> Aguarde antes de tentar novamente',
            '500': '🔥 <strong>Erro interno:</strong> Problema no servidor',
            '502': '🌐 <strong>Gateway error:</strong> Problema de conectividade',
            '503': '🔧 <strong>Serviço indisponível:</strong> Servidor em manutenção'
        };
        
        const message = errorMessages[statusCode] || '❓ <strong>Erro desconhecido:</strong> Consulte a documentação';
        
        return `
            <div style="background: #FEF2F2; border: 1px solid #FECACA; border-radius: 6px; padding: 12px; margin-top: 8px;">
                <div style="color: #DC2626; font-size: 14px;">${message}</div>
            </div>
        `;
    }

    // Toggle de tema (claro/escuro)
    function addThemeToggle() {
        const topbar = document.querySelector('.swagger-ui .topbar .topbar-wrapper');
        if (topbar && !document.querySelector('.fc-theme-toggle')) {
            const themeToggle = document.createElement('button');
            themeToggle.className = 'fc-theme-toggle';
            themeToggle.innerHTML = '🌙';
            themeToggle.style.cssText = `
                background: rgba(255,255,255,0.2);
                border: 1px solid rgba(255,255,255,0.3);
                color: white;
                border-radius: 4px;
                padding: 6px 10px;
                margin-left: 8px;
                cursor: pointer;
                font-size: 16px;
            `;
            
            themeToggle.addEventListener('click', function() {
                document.body.classList.toggle('fc-dark-theme');
                this.innerHTML = document.body.classList.contains('fc-dark-theme') ? '☀️' : '🌙';
                
                const theme = document.body.classList.contains('fc-dark-theme') ? 'dark' : 'light';
                localStorage.setItem('fc_theme', theme);
                showNotification(`🎨 Tema ${theme === 'dark' ? 'escuro' : 'claro'} ativado`, 'info');
            });
            
            topbar.appendChild(themeToggle);
            
            // Carrega tema salvo
            const savedTheme = localStorage.getItem('fc_theme');
            if (savedTheme === 'dark') {
                document.body.classList.add('fc-dark-theme');
                themeToggle.innerHTML = '☀️';
            }
        }
    }

    // Funções globais para ações rápidas
    window.fcQuickLogin = function() {
        const demoCredentials = {
            email: 'admin@ferreiracosta.com',
            senha: 'Admin@123'
        };
        
        showNotification('🔄 Fazendo login demo...', 'info');
        
        // Simula login (em produção, faria requisição real)
        setTimeout(() => {
            const demoToken = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.demo.token';
            window.fcTokenManager.save(demoToken);
            
            const authInput = document.querySelector('.swagger-ui input[placeholder*="Bearer"]');
            if (authInput) {
                authInput.value = demoToken;
            }
            
            showNotification('✅ Login demo realizado com sucesso!', 'success');
        }, 1000);
    };

    window.fcClearAuth = function() {
        window.fcTokenManager.clear();
        
        const authInput = document.querySelector('.swagger-ui input[placeholder*="Bearer"]');
        if (authInput) {
            authInput.value = '';
        }
        
        showNotification('🚪 Logout realizado', 'info');
    };

    window.fcExportCollection = function() {
        const collection = {
            info: {
                name: 'Acervo Educacional - Ferreira Costa',
                description: 'Coleção de endpoints da API',
                version: '1.0.0'
            },
            endpoints: []
        };
        
        // Coleta endpoints visíveis
        document.querySelectorAll('.swagger-ui .opblock').forEach(block => {
            const method = block.querySelector('.opblock-summary-method')?.textContent;
            const path = block.querySelector('.opblock-summary-path')?.textContent;
            const description = block.querySelector('.opblock-summary-description')?.textContent;
            
            if (method && path) {
                collection.endpoints.push({ method, path, description });
            }
        });
        
        const blob = new Blob([JSON.stringify(collection, null, 2)], { type: 'application/json' });
        const url = URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = 'acervo-educacional-api-collection.json';
        a.click();
        URL.revokeObjectURL(url);
        
        showNotification('📥 Coleção exportada com sucesso!', 'success');
    };

    window.fcShowStats = function() {
        const endpoints = document.querySelectorAll('.swagger-ui .opblock').length;
        const tags = document.querySelectorAll('.swagger-ui .opblock-tag').length;
        const methods = {
            GET: document.querySelectorAll('.swagger-ui .opblock-get').length,
            POST: document.querySelectorAll('.swagger-ui .opblock-post').length,
            PUT: document.querySelectorAll('.swagger-ui .opblock-put').length,
            DELETE: document.querySelectorAll('.swagger-ui .opblock-delete').length,
            PATCH: document.querySelectorAll('.swagger-ui .opblock-patch').length
        };
        
        const stats = `
            📊 <strong>Estatísticas da API</strong><br>
            🔗 Endpoints: ${endpoints}<br>
            🏷️ Tags: ${tags}<br>
            📥 GET: ${methods.GET} | 📤 POST: ${methods.POST}<br>
            ✏️ PUT: ${methods.PUT} | 🗑️ DELETE: ${methods.DELETE} | 🔧 PATCH: ${methods.PATCH}
        `;
        
        showNotification(stats, 'info', 5000);
    };

    // Sistema de notificações
    function showNotification(message, type = 'info', duration = 3000) {
        const colors = {
            success: '#16A34A',
            error: '#DC2626',
            warning: '#F59E0B',
            info: '#2563EB'
        };
        
        const notification = document.createElement('div');
        notification.style.cssText = `
            position: fixed;
            top: 20px;
            right: 20px;
            background: ${colors[type]};
            color: white;
            padding: 12px 16px;
            border-radius: 6px;
            box-shadow: 0 4px 12px rgba(0,0,0,0.3);
            z-index: 10000;
            font-family: 'Barlow', sans-serif;
            font-weight: 500;
            max-width: 300px;
            animation: slideIn 0.3s ease-out;
        `;
        
        notification.innerHTML = message;
        document.body.appendChild(notification);
        
        setTimeout(() => {
            notification.style.animation = 'slideOut 0.3s ease-in';
            setTimeout(() => {
                if (notification.parentNode) {
                    notification.parentNode.removeChild(notification);
                }
            }, 300);
        }, duration);
    }

    // Adiciona animações CSS
    const style = document.createElement('style');
    style.textContent = `
        @keyframes slideIn {
            from { transform: translateX(100%); opacity: 0; }
            to { transform: translateX(0); opacity: 1; }
        }
        
        @keyframes slideOut {
            from { transform: translateX(0); opacity: 1; }
            to { transform: translateX(100%); opacity: 0; }
        }
        
        .fc-dark-theme {
            filter: invert(1) hue-rotate(180deg);
        }
        
        .fc-dark-theme img,
        .fc-dark-theme .swagger-ui .topbar {
            filter: invert(1) hue-rotate(180deg);
        }
    `;
    document.head.appendChild(style);

})();

