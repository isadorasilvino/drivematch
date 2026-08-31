\# DriveMatch — Requirements Specification



\## 1. Objetivo



Este documento define os requisitos funcionais, requisitos não funcionais e regras de negócio do DriveMatch.



Os requisitos descritos neste documento representam o escopo funcional planejado para o MVP e servirão como referência para arquitetura, implementação, testes e evolução do sistema.



\---



\## 2. Priorização



Os requisitos são classificados de acordo com a seguinte prioridade:



| Prioridade | Descrição |

|---|---|

| Must | Necessário para o MVP |

| Should | Importante, mas pode ser postergado |

| Could | Desejável para versões futuras |



\---



\## 3. Requisitos Funcionais



\### 3.1 Autenticação e Usuários



\#### RF-001 — Cadastro de usuário



\*\*Prioridade:\*\* Must



O sistema deve permitir que uma pessoa crie uma conta informando os dados necessários para autenticação.



\---



\#### RF-002 — Autenticação



\*\*Prioridade:\*\* Must



O sistema deve permitir que usuários cadastrados realizem autenticação utilizando suas credenciais.



\---



\#### RF-003 — Encerramento de sessão



\*\*Prioridade:\*\* Must



O sistema deve permitir que o usuário encerre sua sessão.



\---



\#### RF-004 — Recuperação de acesso



\*\*Prioridade:\*\* Should



O sistema deve permitir que usuários recuperem o acesso à conta em caso de esquecimento das credenciais.



\---



\#### RF-005 — Perfil do usuário



\*\*Prioridade:\*\* Must



O sistema deve permitir que usuários visualizem e atualizem seus dados de perfil.



\---



\### 3.2 Perfil do Aluno



\#### RF-006 — Cadastro de perfil do aluno



\*\*Prioridade:\*\* Must



O aluno deve poder informar dados necessários para utilização da plataforma.



\---



\#### RF-007 — Preferências de aprendizagem



\*\*Prioridade:\*\* Must



O aluno deve poder informar características relacionadas às suas necessidades de aprendizagem.



Exemplos:



\* Nível de experiência.

\* Se já possui experiência dirigindo.

\* Necessidade de aulas para iniciantes.

\* Utilização de veículo próprio.

\* Preferências de disponibilidade.



\---



\#### RF-008 — Histórico de aulas do aluno



\*\*Prioridade:\*\* Must



O aluno deve poder visualizar seu histórico de aulas realizadas e agendadas.



\---



\### 3.3 Perfil do Instrutor



\#### RF-009 — Cadastro de perfil profissional



\*\*Prioridade:\*\* Must



O instrutor deve poder criar e manter um perfil profissional.



\---



\#### RF-010 — Informações profissionais



\*\*Prioridade:\*\* Must



O instrutor deve poder informar características de seus serviços.



Entre elas:



\* Regiões atendidas.

\* Preço das aulas.

\* Descrição profissional.

\* Experiência.

\* Tipos de alunos atendidos.

\* Condições para utilização de veículo próprio.



\---



\#### RF-011 — Configuração de público atendido



\*\*Prioridade:\*\* Must



O instrutor deve poder informar se aceita:



\* Alunos iniciantes.

\* Alunos que já sabem dirigir.

\* Alunos utilizando veículo próprio.



\---



\#### RF-012 — Configuração de disponibilidade



\*\*Prioridade:\*\* Must



O instrutor deve poder configurar os períodos em que possui disponibilidade para aulas.



\---



\#### RF-013 — Histórico de aulas do instrutor



\*\*Prioridade:\*\* Must



O instrutor deve poder visualizar o histórico de aulas realizadas e agendadas.



\---



\### 3.4 Busca de Instrutores



\#### RF-014 — Pesquisa de instrutores



\*\*Prioridade:\*\* Must



O aluno deve poder pesquisar instrutores disponíveis na plataforma.



\---



\#### RF-015 — Filtro por localização



\*\*Prioridade:\*\* Must



O aluno deve poder filtrar instrutores de acordo com regiões atendidas.



\---



\#### RF-016 — Filtro por disponibilidade



\*\*Prioridade:\*\* Must



O aluno deve poder filtrar instrutores de acordo com sua disponibilidade.



\---



\#### RF-017 — Filtro por características



\*\*Prioridade:\*\* Should



O aluno deve poder filtrar instrutores de acordo com características específicas das aulas.



\---



\#### RF-018 — Visualização de perfil do instrutor



\*\*Prioridade:\*\* Must



O aluno deve poder visualizar as informações públicas do perfil de um instrutor.



\---



\#### RF-019 — Visualização de avaliações



\*\*Prioridade:\*\* Must



O aluno deve poder visualizar avaliações realizadas por outros alunos.



\---



\### 3.5 Compatibilidade entre aluno e instrutor



\#### RF-020 — Cálculo de compatibilidade



\*\*Prioridade:\*\* Must



O sistema deve calcular um índice de compatibilidade entre aluno e instrutor com base nas características e preferências informadas por ambos.



\---



\#### RF-021 — Exibição do índice de compatibilidade



\*\*Prioridade:\*\* Must



O sistema deve apresentar o índice de compatibilidade ao aluno durante a busca ou visualização dos instrutores.



\---



\#### RF-022 — Priorização por compatibilidade



\*\*Prioridade:\*\* Should



O sistema poderá priorizar instrutores com maior compatibilidade nos resultados de pesquisa.



\---



\### 3.6 Solicitação de aulas



\#### RF-023 — Solicitação de aula



\*\*Prioridade:\*\* Must



O aluno deve poder solicitar uma aula com um instrutor.



\---



\#### RF-024 — Informações da solicitação



\*\*Prioridade:\*\* Must



A solicitação deve conter as informações necessárias para que o instrutor possa avaliar o pedido.



Entre elas:



\* Data.

\* Horário.

\* Duração.

\* Local.

\* Utilização de veículo próprio.

\* Observações adicionais.



\---



\#### RF-025 — Recebimento de solicitações



\*\*Prioridade:\*\* Must



O instrutor deve poder visualizar solicitações de aulas recebidas.



\---



\#### RF-026 — Aceite de solicitação



\*\*Prioridade:\*\* Must



O instrutor deve poder aceitar uma solicitação de aula.



\---



\#### RF-027 — Recusa de solicitação



\*\*Prioridade:\*\* Must



O instrutor deve poder recusar uma solicitação de aula.



\---



\#### RF-028 — Cancelamento de solicitação



\*\*Prioridade:\*\* Should



O aluno deve poder cancelar uma solicitação de aula enquanto ela estiver em estado compatível com cancelamento.



\---



\### 3.7 Agenda



\#### RF-029 — Visualização da agenda



\*\*Prioridade:\*\* Must



O instrutor deve poder visualizar suas aulas agendadas.



\---



\#### RF-030 — Visualização de agenda do instrutor



\*\*Prioridade:\*\* Must



O aluno deve poder consultar a disponibilidade do instrutor antes de realizar uma solicitação.



\---



\#### RF-031 — Bloqueio de horários



\*\*Prioridade:\*\* Should



O instrutor deve poder bloquear períodos de sua agenda em que não estará disponível.



\---



\#### RF-032 — Prevenção de conflito de horários



\*\*Prioridade:\*\* Must



O sistema deve impedir que duas aulas sejam confirmadas para o mesmo instrutor no mesmo período.



\---



\### 3.8 Gerenciamento de aulas



\#### RF-033 — Início da aula



\*\*Prioridade:\*\* Must



O instrutor deve poder iniciar uma aula agendada através da plataforma.



\---



\#### RF-034 — Geração de token de check-in

**Prioridade:** Must

Ao iniciar o processo de check-in de uma aula, o backend deve gerar um token temporário e único associado à aula.

O token deve possuir validade de 15 minutos.

O frontend poderá representar esse token através de um QR Code apresentado pelo instrutor.

Caso o token expire antes da confirmação, o instrutor deve poder iniciar novamente o processo de check-in, gerando um novo token e invalidando o anterior.

\---

\#### RF-035 — Check-in do aluno

**Prioridade:** Must

O aluno deve poder realizar o check-in através da leitura do QR Code apresentado pelo instrutor.

A confirmação do check-in deve utilizar o token temporário associado à aula.

O backend deve validar:

* o aluno autenticado;
* a associação do aluno à aula;
* o estado atual da aula;
* a correspondência do token informado;
* a validade temporal do token.

\---

\#### RF-036 — Validação da presença

**Prioridade:** Must

Após a validação bem-sucedida do check-in, o sistema deve:

* registrar a data e hora do check-in;
* registrar o início da aula;
* invalidar o token utilizado;
* alterar o estado da aula para `IN_PROGRESS`.

\---



\#### RF-037 — Encerramento da aula



\*\*Prioridade:\*\* Must



O instrutor deve poder encerrar uma aula em andamento.



\---



\#### RF-038 — Registro da aula



\*\*Prioridade:\*\* Must



O sistema deve armazenar o registro da aula realizada.



\---



\#### RF-039 — Aula não realizada



\*\*Prioridade:\*\* Should



O instrutor deve poder registrar que uma aula agendada não foi realizada, informando o motivo quando necessário.



\---



\### 3.9 Avaliações



\#### RF-040 — Avaliação do instrutor



\*\*Prioridade:\*\* Must



O aluno deve poder avaliar um instrutor após uma aula concluída.



\---



\#### RF-041 — Nota da avaliação



\*\*Prioridade:\*\* Must



A avaliação deve permitir que o aluno atribua uma nota ao instrutor.



\---



\#### RF-042 — Comentário da avaliação



\*\*Prioridade:\*\* Should



O aluno poderá adicionar um comentário à avaliação.



\---



\#### RF-043 — Média de avaliações



\*\*Prioridade:\*\* Must



O sistema deve calcular a média das avaliações recebidas pelo instrutor.



\---



\### 3.10 Notificações



\#### RF-044 — Notificação de nova solicitação



\*\*Prioridade:\*\* Should



O instrutor deve ser notificado quando receber uma nova solicitação de aula.



\---



\#### RF-045 — Notificação de decisão



\*\*Prioridade:\*\* Should



O aluno deve ser notificado quando uma solicitação for aceita ou recusada.



\---



\#### RF-046 — Notificação de alteração de aula



\*\*Prioridade:\*\* Could



O sistema poderá notificar usuários sobre alterações relevantes em seus agendamentos.



\---



\## 4. Requisitos Não Funcionais



\### RNF-001 — Segurança



A aplicação deve implementar mecanismos adequados de autenticação, autorização e proteção de dados.



\---



\### RNF-002 — API REST



O backend deve disponibilizar uma API seguindo princípios REST.



\---



\### RNF-003 — Documentação da API



A API deve possuir documentação utilizando OpenAPI/Swagger.



\---



\### RNF-004 — Arquitetura



A aplicação deve utilizar uma arquitetura organizada em camadas, mantendo separação entre domínio, aplicação, infraestrutura e apresentação.



\---



\### RNF-005 — Testabilidade



As regras de negócio devem ser implementadas de forma que possam ser testadas automaticamente.



\---



\### RNF-006 — Testes automatizados



O projeto deve possuir testes unitários e testes de integração para os principais fluxos.



\---



\### RNF-007 — Qualidade de código



O código deve seguir princípios de Clean Code e boas práticas de desenvolvimento.



\---



\### RNF-008 — Controle de versão



O código-fonte deve ser versionado utilizando Git.



\---



\### RNF-009 — Integração contínua



O projeto deve possuir pipeline de CI para validação automática do código.



\---



\### RNF-010 — Containerização



A aplicação deve possuir suporte à execução através de containers.



\---



\### RNF-011 — Banco de dados



A aplicação deve utilizar PostgreSQL como banco de dados principal.



\---



\### RNF-012 — Observabilidade



A aplicação deve possuir mecanismos básicos de logging e monitoramento de erros.



\---



\### RNF-013 — Responsividade



A interface web deve ser responsiva e utilizável em dispositivos desktop e mobile.



\---



\### RNF-014 — Manutenibilidade



A arquitetura deve permitir evolução das funcionalidades sem acoplamento excessivo entre componentes.



\---



\### RNF-015 — Configuração por ambiente



Informações específicas de ambiente devem ser configuráveis sem alteração do código-fonte.



\---



\## 5. Regras de Negócio



\### RN-001 — Tipos de usuário



O sistema deverá diferenciar usuários do tipo:



\* Aluno.

\* Instrutor.



\---



\### RN-002 — Perfil profissional



Somente usuários com perfil de instrutor poderão disponibilizar serviços de aulas.



\---



\### RN-003 — Disponibilidade



Um instrutor somente poderá receber solicitações em horários configurados como disponíveis.



\---



\### RN-004 — Conflito de agenda



Um instrutor não poderá possuir duas aulas confirmadas que ocupem o mesmo intervalo de tempo.



\---



\### RN-005 — Solicitação pendente



Uma solicitação de aula deverá permanecer pendente até que o instrutor aceite ou recuse o pedido.



\---



\### RN-006 — Aula confirmada



Uma aula somente poderá ser iniciada caso esteja previamente confirmada.



\---



\### RN-007 — Check-in



O check-in somente poderá ser realizado para uma aula válida e dentro do período permitido para início da aula.



\---



\### RN-008 — QR Code temporário



O QR Code utilizado para check-in deverá possuir validade limitada e não poderá ser reutilizado indefinidamente.



\---



\### RN-009 — Presença



O registro de presença deverá ocorrer somente após a validação do token de check-in enviado pelo aluno autenticado.



\---



\### RN-010 — Encerramento



Uma aula somente poderá ser encerrada após ter sido iniciada.



\---



\### RN-011 — Avaliação



Um aluno somente poderá avaliar um instrutor após uma aula concluída entre ambos.



\---



\### RN-012 — Avaliação única



Um aluno não poderá registrar múltiplas avaliações para a mesma aula.



\---



\### RN-013 — Perfil público



Somente informações definidas como públicas pelo sistema deverão ser exibidas no perfil público do instrutor.



\---



\### RN-014 — Compatibilidade



O índice de compatibilidade deverá ser calculated utilizando critérios definidos pelo sistema e deverá possuir comportamento determinístico para os mesmos dados de entrada.



\---



\## 6. Critérios Gerais de Aceitação



Uma funcionalidade será considerada concluída quando:



1\. Sua regra de negócio estiver implementada.

2\. Seus principais cenários estiverem cobertos por testes automatizados.

3\. O comportamento esperado estiver documentado quando necessário.

4\. O código estiver integrado à arquitetura existente.

5\. A implementação passar pelo pipeline de CI.

6\. Não existirem erros críticos conhecidos relacionados à funcionalidade.



\---



\## 7. Rastreabilidade



Os requisitos deverão posteriormente ser relacionados aos seguintes artefatos:



```text

Requirement

&#x20;   ↓

Business Rule

&#x20;   ↓

Use Case

&#x20;   ↓

API Endpoint / UI Flow

&#x20;   ↓

Automated Tests

