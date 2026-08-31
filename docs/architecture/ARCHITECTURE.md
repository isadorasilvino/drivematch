\# DriveMatch — Arquitetura



\## 1. Visão geral



A plataforma DriveMatch seguirá uma arquitetura de Monólito Modular baseada em princípios de Clean Architecture.



O sistema será composto por uma API backend, uma aplicação web frontend, um banco de dados relacional e componentes de infraestrutura necessários para execução, testes e implantação.



A arquitetura foi definida buscando equilíbrio entre:



\* Manutenibilidade.

\* Testabilidade.

\* Separação de responsabilidades.

\* Baixo acoplamento.

\* Clareza das regras de negócio.

\* Segurança.

\* Capacidade de evolução.

\* Simplicidade de implantação.

\* Complexidade adequada ao escopo do MVP.



\---



\## 2. Estilo arquitetural



O backend será desenvolvido como um \*\*Monólito Modular\*\*.



A aplicação possuirá um único backend e um único processo principal, porém suas funcionalidades serão organizadas em módulos e responsabilidades bem definidos.



Os principais módulos previstos são:



\* Autenticação.

\* Alunos.

\* Instrutores.

\* Disponibilidade.

\* Solicitações de aulas.

\* Aulas.

\* Avaliações.

\* Matching/Compatibilidade.



\### 2.1 Por que um Monólito Modular?



A utilização de microserviços não é necessária para o MVP.



O domínio inicial, o volume esperado de utilização e a finalidade do projeto não justificam a complexidade operacional de uma arquitetura distribuída.



O Monólito Modular permite:



\* manter a aplicação simples de desenvolver e executar;

\* manter limites claros entre os módulos;

\* facilitar testes;

\* reduzir complexidade de infraestrutura;

\* permitir evolução futura;

\* possibilitar uma eventual extração de módulos para serviços independentes caso isso seja justificado por requisitos futuros.



A decisão evita complexidade prematura sem abrir mão de uma organização arquitetural adequada.



\---



\## 3. Clean Architecture



O backend seguirá princípios de Clean Architecture.



A solução será composta inicialmente pelos seguintes projetos:



\* `DriveMatch.Api`

\* `DriveMatch.Application`

\* `DriveMatch.Domain`

\* `DriveMatch.Infrastructure`



A representação conceitual será:



```text

\&#x20;                   ┌─────────────────────┐

\&#x20;                   │   DriveMatch.Api    │

\&#x20;                   │                     │

\&#x20;                   │ Controllers         │

\&#x20;                   │ HTTP                │

\&#x20;                   │ Middleware          │

\&#x20;                   └──────────┬──────────┘

\&#x20;                              │

\&#x20;                              ▼

\&#x20;                   ┌─────────────────────┐

\&#x20;                   │ Application         │

\&#x20;                   │                     │

\&#x20;                   │ Use Cases           │

\&#x20;                   │ DTOs                │

\&#x20;                   │ Interfaces          │

\&#x20;                   │ Validações          │

\&#x20;                   └──────────┬──────────┘

\&#x20;                              │

\&#x20;                              ▼

\&#x20;                   ┌─────────────────────┐

\&#x20;                   │ Domain              │

\&#x20;                   │                     │

\&#x20;                   │ Entities            │

\&#x20;                   │ Value Objects       │

\&#x20;                   │ Business Rules      │

\&#x20;                   └─────────────────────┘

\&#x20;                              ▲

\&#x20;                              │

\&#x20;                   ┌──────────┴──────────┐

\&#x20;                   │ Infrastructure      │

\&#x20;                   │                     │

\&#x20;                   │ EF Core             │

\&#x20;                   │ PostgreSQL          │

\&#x20;                   │ Persistência        │

\&#x20;                   │ Serviços externos   │

\&#x20;                   └─────────────────────┘



```



\---



\## 4. Direção das dependências



As dependências deverão respeitar a seguinte direção:



```text

Api

\&#x20;↓

Application

\&#x20;↓

Domain



```



A infraestrutura implementará contratos definidos pelas camadas internas:



```text

Infrastructure

\&#x20;    ↓

Application

\&#x20;    ↓

Domain



```



O objetivo é evitar que as regras de negócio dependam de detalhes tecnológicos.



Por exemplo, o domínio não deverá conhecer:



\* Entity Framework Core.

\* PostgreSQL.

\* ASP.NET Core.

\* Angular.

\* HTTP.

\* JWT.

\* bibliotecas específicas de infraestrutura.



\---



\## 5. Projeto DriveMatch.Domain



O projeto DriveMatch.Domain representa o núcleo do negócio.



Será responsável por conter conceitos e regras que representam o domínio da plataforma.



Entre os conceitos inicialmente previstos estão:



\* Usuário.

\* Aluno.

\* Instrutor.

\* Perfil do aluno.

\* Perfil profissional do instrutor.

\* Disponibilidade.

\* Solicitação de aula.

\* Aula.

\* Avaliação.



Também poderão existir:



\* Enums.

\* Value Objects.

\* Exceções de domínio.

\* Regras de negócio.



O domínio deverá permanecer independente de infraestrutura e apresentação.



\---



\## 6. Projeto DriveMatch.Application



O projeto DriveMatch.Application será responsável pela implementação dos casos de uso da aplicação.



Entre os casos de uso inicialmente previstos estão:



\* Cadastro de usuário.

\* Autenticação.

\* Criação de perfil de aluno.

\* Criação de perfil profissional.

\* Busca de instrutores.

\* Cálculo de compatibilidade.

\* Solicitação de aula.

\* Aceite de solicitação.

\* Recusa de solicitação.

\* Cancelamento.

\* Início da aula.

\* Check-in.

\* Encerramento da aula.

\* Registro de ausência.

\* Criação de avaliação.



Também serão mantidos nessa camada:



\* DTOs.

\* Interfaces.

\* Validações de aplicação.

\* Serviços de aplicação.

\* Orquestração dos casos de uso.



A camada Application não deverá conter detalhes de persistência ou comunicação HTTP.



\---



\## 7. Projeto DriveMatch.Infrastructure



O projeto DriveMatch.Infrastructure será responsável pela implementação das dependências externas da aplicação.



Entre suas responsabilidades estarão:



\* Persistência de dados.

\* Entity Framework Core.

\* PostgreSQL.

\* Implementação dos repositórios.

\* Configurações de banco de dados.

\* Geração de token temporário para check-in.

\* Integrações externas.

\* Implementações relacionadas à autenticação quando necessário.

\* Serviços de infraestrutura.



Essa camada poderá depender de tecnologias específicas.



\---



\## 8. Projeto DriveMatch.Api



O projeto DriveMatch.Api será a porta de entrada HTTP da aplicação.



Suas responsabilidades incluirão:



\* Controllers.

\* Configuração da API.

\* Autenticação.

\* Autorização.

\* Injeção de dependência.

\* Middleware.

\* Tratamento global de exceções.

\* Configuração do Swagger/OpenAPI.

\* Configurações relacionadas ao HTTP.

\* Health Checks.



Os controllers deverão permanecer enxutos.



Eles serão responsáveis principalmente por:



\* Receber requisições HTTP.

\* Validar informações relacionadas ao transporte da requisição.

\* Invocar casos de uso.

\* Retornar respostas HTTP adequadas.



As regras de negócio não deverão ficar diretamente nos controllers.



\---



\## 9. Frontend



O frontend será desenvolvido utilizando:



\* Angular.

\* TypeScript.

\* HTML.

\* CSS.



O frontend será responsável pela interface utilizada por alunos e instrutores.



A comunicação com o backend ocorrerá através da API REST.



```text

Angular

\&#x20;  ↓

HTTP

\&#x20;  ↓

DriveMatch.Api



```



O frontend não deverá possuir regras de negócio críticas que precisem ser protegidas.



Validações de experiência do usuário poderão existir no frontend, porém regras de negócio deverão ser novamente validadas no backend.



\---



\## 10. Banco de dados



O banco de dados principal será o PostgreSQL.



O acesso aos dados será realizado utilizando Entity Framework Core.



As alterações estruturais do banco serão controladas através de migrations.



```text

Application

\&#x20;   ↓

Infrastructure

\&#x20;   ↓

Entity Framework Core

\&#x20;   ↓

PostgreSQL



```



A estrutura do banco será derivada do modelo de domínio e das necessidades de persistência da aplicação.



\---



\## 11. Autenticação e autorização



A autenticação será baseada em JWT.



O sistema possuirá inicialmente dois tipos principais de usuário:



\* `STUDENT`

\* `INSTRUCTOR`



A autorização será realizada no backend.



As restrições de acesso não deverão depender exclusivamente da interface frontend.



Por exemplo, esconder um botão no Angular não será considerado uma medida de segurança.



O backend deverá validar se o usuário autenticado possui permissão para executar determinada operação.



\---



\## 12. Check-in das aulas

O processo de check-in utilizará um token temporário associado à aula.

O backend será responsável por gerar esse token, enquanto o frontend poderá representá-lo através de um QR Code apresentado pelo instrutor.

O token não deverá expor informações sensíveis e possuirá validade de 15 minutos.

O backend deverá validar:

* autenticidade do token;
* validade temporal;
* aula associada;
* aluno associado;
* estado atual da aula;
* permissões do usuário autenticado.

Fluxo conceitual:

```text
Instrutor
   ↓
Inicia processo de check-in
   ↓
Backend gera token temporário
   ↓
Frontend representa o token em QR Code
   ↓
Aluno escaneia o QR Code
   ↓
Frontend envia o token ao backend
   ↓
Backend valida token e ownership do aluno
   ↓
Check-in confirmado
   ↓
Token é invalidado
   ↓
Aula passa para IN_PROGRESS



```



\---



\## 13. Testes



O projeto possuirá testes automatizados separados em:



```text

tests/

├── DriveMatch.UnitTests/

└── DriveMatch.IntegrationTests/



```



\### 13.1 Testes unitários



Serão utilizados para validar principalmente:



\* Regras de negócio.

\* Entidades.

\* Value Objects.

\* Cálculo de compatibilidade.

\* Transições de estado.

\* Validações.

\* Casos de uso isolados.



\### 13.2 Testes de integração



Serão utilizados para validar a integração entre componentes.



Entre os cenários previstos:



\* API.

\* Banco de dados.

\* Entity Framework Core.

\* Autenticação.

\* Endpoints.

\* Persistência.

\* Fluxos críticos.



O objetivo não será buscar uma quantidade arbitrária de testes, mas garantir cobertura dos comportamentos relevantes do sistema.



\---



\## 14. Containerização



Docker será utilizado para proporcionar ambientes reproduzíveis de desenvolvimento e execução.



A infraestrutura inicial deverá contemplar:



\* API.

\* PostgreSQL.



O objetivo será permitir que o ambiente possa ser inicializado de maneira consistente sem depender de configurações manuais específicas da máquina do desenvolvedor.



\---



\## 15. Integração contínua



O projeto utilizará GitHub Actions para automação do processo de integração contínua.



O pipeline deverá executar pelo menos:



```text

Push / Pull Request

\&#x20;       ↓

Restore

\&#x20;       ↓

Build

\&#x20;       ↓

Testes unitários

\&#x20;       ↓

Testes de integração

\&#x20;       ↓

Validação



```



A pipeline deverá impedir que alterações com falhas conhecidas sejam integradas à branch principal.



\---



\## 16. Observabilidade



A aplicação possuirá mecanismos básicos de observabilidade.



Inicialmente serão utilizados:



\* Logging estruturado.

\* Tratamento global de exceções.

\* Identificador de requisição/correlação.

\* Health Checks.



A implementação deverá permitir diagnosticar erros sem depender exclusivamente de mensagens apresentadas ao usuário.



\---



\## 17. Princípios arquiteturais



A implementação deverá priorizar:



\* Separação de responsabilidades.

\* Baixo acoplamento.

\* Alta coesão.

\* Inversão de dependência.

\* Testabilidade.

\* Clareza das regras de negócio.

\* Segurança por padrão.

\* Simplicidade.

\* Evolução incremental.

\* Código legível e manutenível.



\---



\## 18. Decisões de complexidade



O projeto evitará a adoção prematura de tecnologias e padrões que não sejam justificados pelos requisitos atuais.



Não serão utilizados inicialmente:



\* Microserviços.

\* Kubernetes.

\* Service Mesh.

\* Message Broker.

\* Arquitetura distribuída.

\* Event Sourcing.

\* CQRS formal com infraestrutura adicional.



Essas tecnologias poderão ser avaliadas futuramente caso novos requisitos justifiquem sua adoção.



\---



\## 19. Justificativa arquitetural



A arquitetura escolhida busca equilibrar qualidade técnica e velocidade de desenvolvimento.



O DriveMatch é um projeto de portfólio, mas deve representar um sistema que poderia evoluir para um produto real.



Por isso, a arquitetura foi projetada para demonstrar:



\* Conhecimento de arquitetura de software.

\* Organização de código.

\* Separação de responsabilidades.

\* Aplicação de princípios de engenharia.

\* Testabilidade.

\* Segurança.

\* Automação.

\* Capacidade de evolução.



