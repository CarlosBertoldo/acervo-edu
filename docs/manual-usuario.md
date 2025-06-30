# Manual do Usuário - Sistema Acervo Educacional

**Versão:** 1.0  
**Data:** Dezembro 2024  
**Autor:** Manus AI  
**Público-alvo:** Administradores e Usuários do Sistema

---

## Introdução

Bem-vindo ao Sistema Acervo Educacional, uma plataforma completa para gerenciamento de cursos educacionais e seus respectivos arquivos. Este manual fornece instruções detalhadas sobre como utilizar todas as funcionalidades do sistema de forma eficiente e segura.

O Sistema Acervo Educacional foi projetado para simplificar o gerenciamento do ciclo de vida completo dos cursos, desde a concepção inicial até a veiculação final. A interface intuitiva baseada em Kanban permite visualização clara do status de cada curso e facilita a colaboração entre equipes.

### Objetivos do Sistema

O sistema tem como objetivo principal centralizar e organizar todo o acervo educacional da instituição, proporcionando controle total sobre cursos, arquivos e processos relacionados. A plataforma oferece recursos avançados de proteção de conteúdo, relatórios executivos e integração com sistemas corporativos.

A metodologia Kanban implementada no sistema permite acompanhamento visual do progresso dos cursos através de três estágios bem definidos: Backlog (cursos planejados), Em Desenvolvimento (cursos em produção) e Veiculado (cursos finalizados e disponíveis). Esta abordagem facilita a gestão de projetos e melhora a produtividade das equipes.

## Primeiros Passos

### Acessando o Sistema

Para acessar o Sistema Acervo Educacional, abra seu navegador web preferido e digite o endereço fornecido pelo administrador do sistema. Recomendamos utilizar navegadores modernos como Google Chrome, Mozilla Firefox, Microsoft Edge ou Safari para melhor experiência.

A tela de login apresenta campos para email e senha, além de opção para recuperação de senha caso necessário. Certifique-se de utilizar as credenciais fornecidas pelo administrador do sistema. O sistema implementa medidas de segurança que podem bloquear temporariamente o acesso após múltiplas tentativas de login incorretas.

### Recuperação de Senha

Caso tenha esquecido sua senha, clique no link "Esqueceu sua senha?" na tela de login. Digite seu email cadastrado no sistema e clique em "Enviar". Você receberá um email com instruções para criar uma nova senha. O link de recuperação tem validade limitada por motivos de segurança.

Ao criar uma nova senha, certifique-se de seguir as políticas de segurança do sistema: utilize pelo menos 8 caracteres, incluindo letras maiúsculas, minúsculas, números e símbolos especiais. Evite utilizar informações pessoais óbvias como datas de nascimento ou nomes.

### Interface Principal

Após o login bem-sucedido, você será direcionado para o dashboard principal do sistema. A interface é organizada de forma intuitiva com menu lateral para navegação entre diferentes seções e área principal para visualização de conteúdo.

O cabeçalho superior apresenta informações do usuário logado, notificações do sistema e opções de logout. O menu lateral oferece acesso rápido às principais funcionalidades: Dashboard, Kanban, Relatórios, Integração Senior e Configurações.

## Navegação e Layout

### Menu Principal

O menu lateral é o centro de navegação do sistema, organizado de forma hierárquica para facilitar o acesso às funcionalidades. Cada item do menu possui ícone intuitivo e descrição clara de sua função. O menu pode ser recolhido para maximizar a área de trabalho quando necessário.

A seção Dashboard oferece visão geral do sistema com estatísticas importantes e atividades recentes. O Kanban é onde ocorre o gerenciamento principal dos cursos. Relatórios fornece acesso a análises detalhadas e exportação de dados. A Integração Senior permite sincronização com sistemas corporativos.

### Área de Trabalho

A área principal de trabalho adapta-se automaticamente ao conteúdo selecionado, oferecendo visualização otimizada para cada funcionalidade. Controles de zoom e layout responsivo garantem boa experiência em diferentes tamanhos de tela e dispositivos.

Barras de ferramentas contextuais aparecem conforme necessário, oferecendo ações relevantes para o conteúdo sendo visualizado. Tooltips informativos fornecem ajuda contextual para elementos da interface que possam gerar dúvidas.

### Notificações

O sistema de notificações mantém usuários informados sobre eventos importantes como conclusão de uploads, atualizações de sincronização e alertas de sistema. Notificações aparecem temporariamente no canto superior direito e podem ser acessadas através do ícone de sino no cabeçalho.

Notificações são categorizadas por tipo e importância, utilizando cores e ícones distintos para facilitar identificação rápida. Usuários podem marcar notificações como lidas ou descartá-las conforme necessário.

## Gerenciamento de Cursos

### Visualização Kanban

O board Kanban é o coração do sistema, oferecendo visualização clara e intuitiva do status de todos os cursos. As três colunas representam os estágios do ciclo de vida: Backlog, Em Desenvolvimento e Veiculado. Cada curso é representado por um card informativo com dados essenciais.

Os cards apresentam informações organizadas de forma hierárquica: título do curso em destaque, seguido por código, descrição da academia, status atual, tipo de ambiente, tipo de acesso, data de início de operação e informações sobre origem e criador. Cores e ícones ajudam na identificação rápida de diferentes tipos de curso.

### Criação de Novos Cursos

Para criar um novo curso, clique no botão "Novo Curso" localizado no topo do board Kanban. Um formulário modal será aberto com campos obrigatórios e opcionais para cadastro completo do curso. Preencha todas as informações necessárias com atenção aos detalhes.

Campos obrigatórios incluem código do curso (deve ser único no sistema), nome do curso, descrição da academia e tipo de ambiente. Campos opcionais como data de início de operação e comentários internos podem ser preenchidos conforme disponibilidade de informações.

O sistema valida automaticamente a unicidade do código do curso e formatos de dados inseridos. Mensagens de erro claras indicam problemas que devem ser corrigidos antes do salvamento. Após criação, o curso aparece automaticamente na coluna Backlog.

### Edição de Cursos

Para editar um curso existente, clique no menu de três pontos no canto superior direito do card e selecione "Editar". O mesmo formulário de criação será aberto com dados atuais preenchidos. Faça as alterações necessárias e clique em "Salvar".

Cursos originados do sistema Senior têm campos específicos bloqueados para edição, indicados visualmente na interface. Apenas campos locais podem ser modificados para manter consistência com o sistema de origem. Comentários internos estão sempre disponíveis para edição independente da origem.

Todas as modificações são registradas automaticamente no log de auditoria do sistema, incluindo usuário responsável, timestamp e detalhes das alterações realizadas. Este histórico pode ser consultado através do menu de ações do curso.

### Movimentação entre Status

A movimentação de cursos entre as colunas do Kanban representa mudanças no status do projeto. Para mover um curso, clique no menu de três pontos do card e selecione a opção de movimentação desejada: "Mover para Em Desenvolvimento" ou "Mover para Veiculado".

O sistema solicita confirmação antes de executar a movimentação, permitindo cancelamento caso necessário. Após confirmação, o card é automaticamente transferido para a coluna apropriada e o log de atividades é atualizado com a mudança de status.

Movimentações são irreversíveis através da interface padrão por motivos de auditoria, mas administradores podem realizar ajustes através de funcionalidades administrativas específicas. Todas as movimentações são registradas com timestamp e usuário responsável.

### Filtros e Busca

O sistema oferece funcionalidades avançadas de filtro e busca para facilitar localização de cursos específicos em bases de dados extensas. A barra de busca no topo do Kanban permite pesquisa por nome, código ou descrição do curso com resultados em tempo real.

Filtros laterais permitem refinamento da visualização por múltiplos critérios: status, origem (manual ou Senior), tipo de ambiente, tipo de acesso, data de criação e criador. Filtros podem ser combinados para buscas muito específicas e salvos como favoritos para uso futuro.

A funcionalidade de busca implementa algoritmos inteligentes que consideram sinônimos e variações de escrita, melhorando a precisão dos resultados. Resultados são destacados visualmente para facilitar identificação dos termos pesquisados.

## Gerenciamento de Arquivos

### Acessando Arquivos de um Curso

Para acessar os arquivos vinculados a um curso, clique no menu de três pontos do card no Kanban e selecione "Gerenciar Arquivos". Um modal lateral será aberto apresentando todos os arquivos organizados por categoria em abas específicas.

As abas organizam arquivos por tipo: Todos (visão geral), Vídeos, Documentos e Outros. Cada aba apresenta contadores indicando quantidade de arquivos por categoria. A aba "Todos" oferece visão consolidada com filtros adicionais por tipo de arquivo.

### Upload de Arquivos

Para fazer upload de novos arquivos, clique no botão "Upload de Arquivos" no modal de gerenciamento. Uma interface de drag-and-drop será apresentada, permitindo arrastar arquivos diretamente do explorador de arquivos ou clicar para selecionar através de diálogo padrão.

O sistema suporta upload múltiplo simultâneo com validação automática de tipos de arquivo permitidos e tamanho máximo. Barras de progresso individuais mostram o status de cada upload em tempo real. Arquivos que não atendem aos critérios são rejeitados com mensagens explicativas claras.

Durante o upload, selecione a categoria apropriada para cada arquivo: Briefing de Desenvolvimento, Briefing de Execução, PPT, Caderno de Exercício, Plano de Aula, Vídeos, Podcast ou Outros Arquivos. A categorização correta facilita organização e localização posterior dos conteúdos.

### Visualização de Arquivos

O sistema oferece visualização integrada para diferentes tipos de arquivo sem necessidade de download. Para visualizar um arquivo, clique no ícone de olho no card do arquivo. Um modal de visualização será aberto com player ou visualizador apropriado para o tipo de conteúdo.

Vídeos são reproduzidos através de player customizado com controles avançados de proteção. Documentos PDF são exibidos em visualizador integrado com funcionalidades de zoom, rotação e navegação por páginas. Imagens são apresentadas em galeria com opções de zoom e navegação.

O sistema implementa proteções avançadas durante a visualização, incluindo bloqueio de menu contextual, teclas de atalho para download e captura de tela. Watermarks personalizáveis podem ser aplicados para identificação de propriedade do conteúdo.

### Compartilhamento de Arquivos

Para compartilhar arquivos com usuários externos ou sistemas, utilize a funcionalidade de geração de links de compartilhamento. Clique no menu de três pontos do arquivo e selecione "Compartilhar". Um modal será aberto com opções de configuração do compartilhamento.

Três tipos de compartilhamento estão disponíveis: Público (acesso livre), Restrito (requer autenticação) e Com Expiração (acesso limitado por tempo). Para cada tipo, configure as permissões apropriadas: apenas visualização, download permitido ou ambos.

Links de compartilhamento podem incluir proteções adicionais como restrição de domínio (arquivo só pode ser acessado de domínios específicos) e limite de acessos. O sistema registra todos os acessos aos links compartilhados para auditoria e controle.

### Código Embed

Para incorporar arquivos em sites externos ou sistemas de e-learning, utilize a funcionalidade de código embed. Clique no menu do arquivo e selecione "Gerar Embed". Um código HTML será gerado com iframe configurado para exibição segura do conteúdo.

O código embed inclui proteções automáticas contra download e captura, mantendo a segurança do conteúdo mesmo quando incorporado em sites externos. Configurações de tamanho, controles de player e aparência podem ser personalizadas conforme necessário.

Restrições de domínio podem ser aplicadas ao código embed, garantindo que o conteúdo só seja exibido em sites autorizados. Esta funcionalidade é especialmente importante para conteúdos proprietários que devem ter distribuição controlada.

## Dashboard e Relatórios

### Dashboard Principal

O dashboard oferece visão executiva do sistema com métricas importantes apresentadas em cards informativos e gráficos interativos. Estatísticas incluem total de cursos por status, distribuição por origem, atividade de usuários e utilização de armazenamento.

Gráficos de pizza mostram distribuição de cursos por status e origem, facilitando análise rápida da composição do acervo. Gráficos de linha apresentam tendências temporais como criação de cursos ao longo do tempo e atividade de usuários.

A seção de atividades recentes lista as últimas ações realizadas no sistema, incluindo criação de cursos, uploads de arquivos, movimentações de status e sincronizações com Senior. Esta informação é valiosa para acompanhamento da atividade geral do sistema.

### Relatórios Avançados

A seção de relatórios oferece análises detalhadas com múltiplas opções de filtro e exportação. Quatro tipos principais de relatório estão disponíveis: Cursos, Atividades, Arquivos e Sincronização Senior. Cada tipo oferece filtros específicos para análises direcionadas.

O relatório de cursos permite análise por status, origem, data de criação, tipo de ambiente e criador. Filtros de data oferecem períodos pré-definidos (última semana, último mês, último ano) ou seleção de intervalo personalizado. Resultados podem ser ordenados por diferentes critérios.

Relatórios de atividades apresentam logs de auditoria com filtros por usuário, tipo de ação, período e recursos afetados. Esta funcionalidade é essencial para compliance e investigação de incidentes. Detalhes incluem IP de origem, timestamp preciso e dados modificados.

### Exportação de Dados

Todos os relatórios podem ser exportados em três formatos: Excel (.xlsx), PDF (.pdf) e CSV (.csv). A exportação em Excel inclui formatação profissional com cabeçalhos, cores e gráficos quando aplicável. PDFs são otimizados para impressão com layout estruturado.

Arquivos CSV são ideais para análise em ferramentas externas como Power BI, Tableau ou planilhas personalizadas. O formato CSV utiliza codificação UTF-8 para suporte completo a caracteres especiais e acentuação em português.

Exportações são geradas em tempo real e disponibilizadas para download imediato. Para relatórios muito extensos, o sistema pode processar a exportação em background e notificar quando estiver pronta. Arquivos exportados incluem timestamp de geração e filtros aplicados.

## Integração com Senior

### Configuração da Integração

A integração com o sistema Senior permite sincronização automática de dados de cursos, mantendo consistência entre sistemas corporativos. Para configurar a integração, acesse a seção "Integração Senior" no menu principal e configure os parâmetros de conexão.

A configuração requer string de conexão com o banco de dados Senior, incluindo servidor, banco, usuário e senha. Teste a conectividade antes de salvar as configurações para garantir que a integração funcionará corretamente. O sistema valida permissões de acesso às views necessárias.

Configurações adicionais incluem frequência de sincronização automática (recomendado diariamente às 2h da manhã), estratégias de resolução de conflitos e filtros para sincronização seletiva de cursos conforme critérios específicos.

### Sincronização Manual

Para executar sincronização manual, clique no botão "Sincronizar Agora" na página de integração Senior. O processo será executado em tempo real com feedback visual do progresso. Logs detalhados mostram cursos processados, conflitos detectados e ações realizadas.

Durante a sincronização, o sistema compara dados entre Senior e base local, identificando cursos novos, modificados ou removidos. Conflitos são apresentados para resolução manual quando estratégias automáticas não são suficientes.

Resultados da sincronização incluem estatísticas detalhadas: cursos criados, atualizados, conflitos detectados e erros encontrados. Logs completos podem ser exportados para análise posterior ou documentação de compliance.

### Resolução de Conflitos

Conflitos ocorrem quando o mesmo curso foi modificado tanto no Senior quanto localmente desde a última sincronização. O sistema oferece quatro estratégias de resolução: Manter Local, Usar Senior, Ignorar e Resolução Manual.

A estratégia "Manter Local" preserva dados locais ignorando mudanças do Senior. "Usar Senior" sobrescreve dados locais com informações do Senior. "Ignorar" mantém status atual sem aplicar mudanças. "Resolução Manual" permite análise caso a caso.

Para resolução manual, o sistema apresenta interface comparativa mostrando dados atuais e propostas de mudança lado a lado. Usuários podem selecionar campos específicos para atualizar ou manter, oferecendo controle granular sobre a resolução de conflitos.

## Configurações e Administração

### Configurações do Sistema

Administradores têm acesso a configurações avançadas do sistema através da seção "Configurações" no menu principal. Configurações incluem parâmetros de segurança, limites de upload, políticas de retenção e integrações externas.

Configurações de segurança permitem ajustar políticas de senha, tempo de expiração de sessões, tentativas de login permitidas e configurações de auditoria. Estas configurações afetam todos os usuários do sistema e devem ser ajustadas conforme políticas organizacionais.

Limites de upload incluem tamanho máximo de arquivo, tipos permitidos e quotas por usuário ou curso. Configurações de retenção determinam por quanto tempo logs e backups são mantidos no sistema antes da limpeza automática.

### Gerenciamento de Usuários

O sistema atualmente opera com todos os usuários como administradores, mas a arquitetura suporta expansão para múltiplos níveis de acesso. Funcionalidades futuras incluirão criação de usuários, atribuição de roles e gerenciamento granular de permissões.

Logs de atividade de usuários podem ser consultados para auditoria e análise de uso. Informações incluem últimos acessos, ações realizadas, IPs utilizados e sessões ativas. Administradores podem revogar sessões específicas se necessário.

### Backup e Manutenção

O sistema executa backup automático diário, mas administradores podem executar backups manuais quando necessário. Acesse a seção de manutenção para executar backup imediato, verificar integridade de dados ou executar tarefas de limpeza.

Backups incluem dump completo do banco de dados PostgreSQL e arquivamento de logs importantes. Arquivos são comprimidos automaticamente para otimizar espaço de armazenamento. Configurações de retenção determinam por quanto tempo backups são mantidos.

Tarefas de manutenção incluem limpeza de arquivos temporários, otimização de índices de banco de dados, verificação de integridade de arquivos no S3 e análise de performance. Estas tarefas podem ser agendadas para execução automática em horários de baixa utilização.

---

**Manual elaborado por:** Manus AI  
**Última atualização:** Dezembro 2024  
**Versão:** 1.0

Para suporte técnico ou dúvidas sobre o sistema, entre em contato com o administrador responsável ou consulte a documentação técnica complementar.

