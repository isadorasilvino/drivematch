Claro — abaixo está o mesmo conteúdo, sem alteração textual, apenas com a formatação Markdown aplicada de forma consistente.

 # Arquitetura do DriveMatch

 ## 1\. Visão geral

 O DriveMatch é uma aplicação web desenvolvida como projeto de portfólio, composta por:

 - frontend em Angular;
- API em ASP.NET Core;
- backend estruturado em camadas;
- PostgreSQL como banco de dados relacional;
- Entity Framework Core para persistência;
- autenticação baseada em JWT;
- testes unitários e de integração automatizados.

 A arquitetura foi organizada para manter as responsabilidades do domínio, dos casos de uso, da infraestrutura e da exposição HTTP separadas.

 O backend utiliza quatro projetos principais:

```
DriveMatch.Api
DriveMatch.Application
DriveMatch.Domain
DriveMatch.Infrastructure
```

 Além deles, a solution possui dois projetos de testes:

```
DriveMatch.UnitTests
DriveMatch.IntegrationTests
```

 O frontend Angular é mantido no mesmo repositório, dentro de:

```
frontend/drivematch-web
```

 ## 2\. Estrutura geral do repositório

 A organização principal do projeto é:

```
drivematch/
├── src/
│   ├── DriveMatch.Api/
│   ├── DriveMatch.Application/
│   ├── DriveMatch.Domain/
│   └── DriveMatch.Infrastructure/
│
├── tests/
│   ├── DriveMatch.UnitTests/
│   └── DriveMatch.IntegrationTests/
│
├── frontend/
│   └── drivematch-web/
│
└── docs/
```

 Essa estrutura mantém backend, frontend, testes e documentação dentro do mesmo repositório.

 ## 3\. Dependências entre as camadas

 As dependências entre os projetos do backend seguem esta direção:

```
                    ┌─────────────────┐
                    │ DriveMatch.Api  │
                    └────────┬────────┘
                             │
                   ┌─────────┴─────────┐
                   ▼                   ▼
        ┌──────────────────┐  ┌──────────────────────┐
        │   Application    │◄─│    Infrastructure    │
        └────────┬─────────┘  └──────────┬───────────┘
                 │                       │
                 └───────────┬───────────┘
                             ▼
                    ┌─────────────────┐
                    │     Domain      │
                    └─────────────────┘
```

 Na implementação atual:

 - `DriveMatch.Domain` não possui referência para os demais projetos;
- `DriveMatch.Application` referencia `DriveMatch.Domain`;
- `DriveMatch.Infrastructure` referencia `DriveMatch.Application` e `DriveMatch.Domain`;
- `DriveMatch.Api` referencia `DriveMatch.Application` e `DriveMatch.Infrastructure`.

 Essa organização mantém o domínio independente das tecnologias utilizadas nas camadas externas.

 ## 4\. Domain

 O projeto `DriveMatch.Domain` concentra o modelo de domínio e as regras diretamente relacionadas às entidades e conceitos centrais da aplicação.

 Sua estrutura inclui:

```
DriveMatch.Domain/
├── Common/
├── Entities/
├── Enums/
├── Exceptions/
└── ValueObjects/
```

 Entre as principais entidades persistidas estão:

 - `User`;
- `StudentProfile`;
- `InstructorProfile`;
- `Availability`;
- `LessonRequest`;
- `Lesson`;
- `Review`.

 O domínio também possui objetos de valor, como `Money`, utilizado para representar o preço da aula do instrutor.

 Essa camada não depende de ASP.NET Core, Entity Framework Core, PostgreSQL ou da implementação da interface web.

 ## 5\. Application

 O projeto `DriveMatch.Application` contém os casos de uso da aplicação e as abstrações necessárias para executá-los.

 Sua estrutura principal inclui:

```
DriveMatch.Application/
├── Abstractions/
├── Common/
├── DependencyInjection/
├── DTOs/
└── Features/
```

 Os casos de uso são organizados por funcionalidade.

 Entre as áreas existentes estão:

 - Auth
- Availabilities
- Instructors
- LessonRequests
- Lessons
- Reviews
- Students
- Users

 ### Exemplos de operações implementadas

 #### Usuários e autenticação

 - registro de usuário;
- login;
- consulta da própria conta;
- atualização de nome e e-mail;
- alteração de senha.

 #### Alunos

 - criação do perfil;
- consulta do próprio perfil;
- atualização do perfil.

 #### Instrutores

 - criação do perfil;
- consulta do próprio perfil;
- atualização do perfil;
- ativação e desativação;
- busca de instrutores.

 #### Disponibilidade

 - criação;
- atualização;
- ativação e desativação;
- consulta da própria agenda;
- geração dos horários disponíveis;
- consulta da próxima data disponível.

 #### Solicitações de aula

 - criação;
- consulta das solicitações enviadas;
- consulta das solicitações recebidas;
- aceite;
- recusa;
- cancelamento.

 #### Aulas

 - consulta das aulas;
- início do check-in;
- confirmação do check-in;
- conclusão;
- cancelamento;
- registro de não comparecimento.

 #### Avaliações

 - criação de avaliação após a aula.

 Os handlers desses casos de uso são registrados no container de injeção de dependência pela extensão `AddApplication()`.

 ## 6\. Abstrações

 A camada Application define contratos utilizados pelos casos de uso sem depender das implementações concretas da infraestrutura.

 Entre eles estão abstrações para:

 - persistência;
- repositórios;
- unidade de trabalho;
- autenticação;
- geração de token;
- hash de senha;
- acesso à data e hora.

 A implementação concreta desses contratos fica em `DriveMatch.Infrastructure`.

 Esse mecanismo permite que os casos de uso dependam de interfaces, enquanto detalhes técnicos permanecem fora da camada de aplicação.

 ## 7\. Infrastructure

 O projeto `DriveMatch.Infrastructure` implementa os detalhes técnicos necessários pela aplicação.

 Sua estrutura inclui:

```
DriveMatch.Infrastructure/
├── Configurations/
├── DependencyInjection/
├── Persistence/
├── Repositories/
└── Services/
```

 Entre suas responsabilidades estão:

 - configuração do Entity Framework Core;
- acesso ao PostgreSQL;
- implementação dos repositórios;
- implementação da unidade de trabalho;
- geração de JWT;
- hash e validação de senha;
- implementação do provedor de data e hora;
- configuração do fuso horário utilizado pela agenda.

 A configuração dessas dependências é centralizada em:

 `InfrastructureDependencyInjection`

 e disponibilizada por:

 `AddInfrastructure(configuration)`

 ## 8\. Persistência

 A persistência utiliza:

 - Entity Framework Core;
- Npgsql;
- PostgreSQL.

 O contexto principal é:

 `DriveMatchDbContext`

 Ele implementa também a abstração:

 `IUnitOfWork`

 O contexto possui conjuntos para:

 - `Users`
- `StudentProfiles`
- `InstructorProfiles`
- `Availabilities`
- `LessonRequests`
- `Lessons`
- `Reviews`

 As configurações das entidades são carregadas automaticamente a partir do assembly da infraestrutura.

```
modelBuilder.ApplyConfigurationsFromAssembly(
    typeof(DriveMatchDbContext).Assembly);
```

 O schema do banco é versionado por migrations do Entity Framework Core.

 ## 9\. Relacionamentos principais

 De forma simplificada, o modelo persistido possui os seguintes relacionamentos:

```
User
 ├── StudentProfile
 └── InstructorProfile

InstructorProfile
 └── Availability

StudentProfile ─────┐
                    ├── LessonRequest
InstructorProfile ──┘

LessonRequest
 └── Lesson

StudentProfile ─────┐
InstructorProfile ──┼── Lesson
LessonRequest ──────┘

Lesson
 └── Review
```

 Cada usuário possui um papel definido no sistema e pode possuir o perfil correspondente a esse papel.

 As solicitações conectam alunos e instrutores.

 Quando uma solicitação é aceita, ela dá origem a uma aula.

 Após uma aula elegível para avaliação, uma avaliação pode ser associada à aula.

 ## 10\. Repositórios

 A infraestrutura possui implementações concretas para os principais repositórios da aplicação:

 - `UserRepository`
- `StudentProfileRepository`
- `InstructorProfileRepository`
- `AvailabilityRepository`
- `LessonRequestRepository`
- `LessonRepository`
- `ReviewRepository`

 Essas implementações são registradas por injeção de dependência e consumidas pelos casos de uso através das interfaces definidas pela camada Application.

 O `UserRepository` também participa do fluxo de autenticação através da abstração específica utilizada para localizar usuários durante o login.

 ## 11\. API

 O projeto `DriveMatch.Api` é responsável pela exposição HTTP da aplicação.

 Sua estrutura inclui:

```
DriveMatch.Api/
├── Configuration/
├── Controllers/
├── Endpoints/
├── Extensions/
├── Middlewares/
├── OpenApi/
└── Properties/
```

 A aplicação utiliza endpoints organizados por área funcional.

 No bootstrap da API são registrados:

 - `UserEndpoints`
- `StudentEndpoints`
- `InstructorEndpoints`
- `AvailabilityEndpoints`
- `LessonRequestEndpoints`
- `LessonEndpoints`
- `ReviewEndpoints`
- `AuthEndpoints`

 A API é responsável por receber as requisições HTTP, aplicar autenticação e autorização quando necessário e encaminhar as operações aos casos de uso da camada Application.

 ## 12\. Autenticação e autorização

 A autenticação utiliza JWT Bearer.

 O token é validado considerando:

 - issuer;
- audience;
- chave de assinatura;
- tempo de validade.

 O `ClockSkew` está configurado como zero, evitando tolerância adicional após o vencimento do token.

 A API registra:

```
AddAuthentication(...)
AddJwtBearer(...)
AddAuthorization()
```

 e o pipeline HTTP utiliza:

```
UseAuthentication()
UseAuthorization()
```

 A geração do token é implementada pela infraestrutura através do serviço:

 `JwtTokenService`

 ## 13\. Senhas

 As senhas não são persistidas em texto puro.

 A infraestrutura utiliza:

 `BCrypt.Net-Next`

 e encapsula a operação através da abstração de hash de senha utilizada pela aplicação.

 Dessa forma, os casos de uso não dependem diretamente da biblioteca utilizada para geração e verificação dos hashes.

 ## 14\. OpenAPI

 A API possui documentação OpenAPI.

 Em ambientes de desenvolvimento e staging são disponibilizados:

 - documento OpenAPI;
- interface Scalar para exploração da API.

 A documentação também possui configuração para autenticação Bearer, permitindo trabalhar com endpoints protegidos durante desenvolvimento e homologação.

 ## 15\. CORS

 A API possui uma política de CORS chamada:

 `Frontend`

 Na configuração atual são permitidas as origens:

```
http://localhost:4200
https://drivematch-hml.pages.dev
```

 A política permite os headers e métodos necessários para comunicação entre o frontend e a API.

 ## 16\. Data, hora e agenda

 A aplicação possui uma abstração de tempo:

 `IDateTimeProvider`

 implementada pela infraestrutura através de:

 `SystemDateTimeProvider`

 A configuração da agenda também utiliza um `TimeZoneInfo` configurado externamente através de:

 `Scheduling:TimeZoneId`

 Isso evita espalhar dependências diretas de relógio e configuração de fuso horário pelos casos de uso.

 # Frontend

 ## 17\. Visão geral

 O frontend está localizado em:

 `frontend/drivematch-web`

 e utiliza Angular.

 Na versão atualmente implementada, as principais dependências incluem:

 - Angular 22;
- Angular Router;
- Angular Forms;
- Angular HttpClient;
- Angular Service Worker;
- RxJS;
- Font Awesome;
- QRCode.

 A aplicação utiliza componentes standalone e carregamento lazy das principais páginas através do Angular Router.

 ## 18\. Organização das rotas

 A aplicação possui uma área pública e duas áreas autenticadas principais.

 ### Área pública

```
/
/login
/register
```

 A rota raiz apresenta a página pública de apresentação do DriveMatch.

 ### Área do aluno

```
/student
/student/profile
/student/instructors
/student/instructors/:instructorProfileId/availability
/student/lesson-requests
/student/lessons
/student/lessons/check-in
```

 ### Área do instrutor

```
/instructor
/instructor/profile
/instructor/availability
/instructor/lesson-requests
/instructor/lessons
```

 As áreas autenticadas utilizam `AuthenticatedLayoutComponent`.

 ## 19\. Proteção de rotas

 O frontend possui guards responsáveis por restringir o acesso conforme autenticação, papel e existência do perfil.

 Entre eles estão:

 - `authGuard`
- `roleGuard`
- `studentProfileGuard`
- `instructorProfileGuard`

 O `authGuard` protege áreas autenticadas.

 O `roleGuard` restringe as áreas específicas de aluno e instrutor.

 Os guards de perfil impedem o acesso aos fluxos que dependem da existência do respectivo perfil antes que ele tenha sido configurado.

 ## 20\. Comunicação HTTP

 O Angular utiliza HttpClient para comunicação com a API.

 Um interceptor de autenticação é registrado globalmente:

 `authInterceptor`

 através de:

```
provideHttpClient(
  withInterceptors([authInterceptor]),
)
```

 Isso centraliza o envio das informações necessárias para autenticação das chamadas protegidas.

 ## 21\. PWA

 O frontend possui suporte a Progressive Web App.

 O Angular Service Worker é registrado fora do ambiente de desenvolvimento:

```
provideServiceWorker('ngsw-worker.js', {
  enabled: !isDevMode(),
  registrationStrategy: 'registerWhenStable:30000',
})
```

 O projeto também possui manifesto e assets específicos para instalação e utilização como aplicação web progressiva.

 # Testes

 ## 22\. Estratégia de testes

 O backend possui dois projetos de testes:

 - `DriveMatch.UnitTests`
- `DriveMatch.IntegrationTests`

 A separação permite validar tanto regras isoladas quanto o comportamento integrado da aplicação.

 ## 23\. Testes unitários

 Os testes unitários utilizam:

 - xUnit
- Microsoft.NET.Test.Sdk
- coverlet.collector

 O projeto referencia diretamente:

 - `DriveMatch.Domain`
- `DriveMatch.Application`

 Os testes unitários verificam principalmente comportamentos de domínio e casos de uso sem depender da execução completa da API.

 ## 24\. Testes de integração

 Os testes de integração utilizam:

 - xUnit
- Microsoft.AspNetCore.Mvc.Testing
- Testcontainers.PostgreSql
- coverlet.collector

 O projeto referencia:

 - `DriveMatch.Api`
- `DriveMatch.Infrastructure`

 A infraestrutura de testes utiliza:

 `WebApplicationFactory<Program>`

 para executar a aplicação durante os testes.

 Também é utilizado um container PostgreSQL através de Testcontainers.

 Com isso, os testes de integração podem executar os fluxos da API contra uma instância PostgreSQL criada especificamente para os testes, aproximando o ambiente automatizado do comportamento real da aplicação.

 ## 25\. Fluxo simplificado de uma requisição

 De forma geral, uma operação iniciada no frontend percorre o seguinte caminho:

```
Angular
   │
   ▼
HTTP Request
   │
   ▼
DriveMatch.Api
   │
   ▼
Application Handler
   │
   ├── Domain
   │
   ▼
Repository Interface
   │
   ▼
Infrastructure Repository
   │
   ▼
Entity Framework Core
   │
   ▼
PostgreSQL
```

 A resposta percorre o caminho inverso até o frontend.

 Essa separação permite que regras de negócio e casos de uso permaneçam desacoplados da interface HTTP e da tecnologia de persistência.

 ## 26\. Resumo das tecnologias

 | Área | Tecnologia |
| --- | --- |
| Backend | .NET 10 |
| API | ASP.NET Core |
| Frontend | Angular 22 |
| Banco de dados | PostgreSQL |
| ORM | Entity Framework Core |
| Provider PostgreSQL | Npgsql |
| Autenticação | JWT Bearer |
| Hash de senha | BCrypt |
| Documentação da API | OpenAPI + Scalar |
| Testes | xUnit |
| Testes HTTP | WebApplicationFactory |
| Banco nos testes de integração | Testcontainers PostgreSQL |
| PWA | Angular Service Worker |
| QR Code | qrcode |

## 27\. Princípios adotados

 A arquitetura atual do DriveMatch busca manter:

 - separação clara de responsabilidades;
- domínio independente de infraestrutura;
- casos de uso organizados por funcionalidade;
- inversão de dependência através de abstrações;
- persistência isolada na infraestrutura;
- autenticação centralizada;
- componentes de frontend reutilizáveis;
- proteção de rotas por autenticação, papel e perfil;
- testes unitários para regras isoladas;
- testes de integração contra infraestrutura próxima da real;
- documentação mantida junto ao código.