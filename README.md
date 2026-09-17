# DriveMatch

> Plataforma que conecta alunos a instrutores autônomos de direção.

O **DriveMatch** é uma aplicação full stack desenvolvida para centralizar a busca, contratação e gestão de aulas entre alunos e instrutores autônomos de direção.

O projeto também funciona como um projeto público de portfólio, demonstrando decisões de arquitetura, modelagem de domínio, desenvolvimento de APIs REST, frontend, persistência, autenticação, testes automatizados e containerização.

---

## Funcionalidades

### Aluno

- Cadastro e autenticação.
- Criação de perfil.
- Busca de instrutores.
- Consulta de perfis e disponibilidade.
- Solicitação de aulas.
- Acompanhamento das solicitações.
- Consulta das aulas.
- Check-in da aula.
- Histórico de aulas.

### Instrutor

- Cadastro e autenticação.
- Criação de perfil profissional.
- Configuração de disponibilidade.
- Recebimento de solicitações de aula.
- Aceite e recusa de solicitações.
- Gerenciamento das aulas.
- Controle do ciclo de vida da aula.
- Geração de QR Code para check-in.
- Histórico de aulas.

---

## Check-in por QR Code

Um dos principais fluxos do DriveMatch é a validação de presença na aula.

O instrutor inicia o check-in e o backend gera um token temporário associado à aula. O frontend representa esse token por meio de um QR Code.

O aluno utiliza o QR Code para confirmar sua presença e o backend valida:

- identidade do usuário;
- associação do aluno à aula;
- estado atual da aula;
- token informado;
- validade temporal do token.

Após a confirmação, o token é invalidado e a aula pode seguir seu ciclo de execução.

---

## Arquitetura

O backend utiliza uma arquitetura de **Monólito Modular**, aplicando princípios de **Clean Architecture**.

```text
DriveMatch.Api
      |
      v
DriveMatch.Application
      |
      v
DriveMatch.Domain

DriveMatch.Infrastructure
      |
      +--> Application
      |
      +--> Domain
```

A solução está dividida em:

```text
src/
├── DriveMatch.Api
├── DriveMatch.Application
├── DriveMatch.Domain
└── DriveMatch.Infrastructure

tests/
├── DriveMatch.UnitTests
└── DriveMatch.IntegrationTests

frontend/
└── drivematch-web
```

### Domain

Contém o núcleo do negócio:

- entidades;
- enums;
- regras de negócio;
- transições de estado;
- conceitos do domínio.

### Application

Responsável pelos casos de uso:

- autenticação;
- perfis;
- disponibilidade;
- busca de instrutores;
- solicitações de aula;
- ciclo de vida das aulas;
- check-in;
- avaliações.

### Infrastructure

Implementa detalhes externos:

- Entity Framework Core;
- PostgreSQL;
- repositórios;
- persistência;
- autenticação;
- serviços de infraestrutura.

### API

Responsável pela interface HTTP:

- endpoints REST;
- autenticação e autorização;
- configuração da aplicação;
- injeção de dependência.

---

## Stack

### Backend

- C#
- .NET 10
- ASP.NET Core
- Entity Framework Core
- PostgreSQL
- JWT

### Frontend

- Angular 22
- TypeScript
- RxJS
- Font Awesome
- PWA / Angular Service Worker

### Testes e infraestrutura

- xUnit
- Testcontainers
- PostgreSQL 17
- Docker
- Docker Compose
- Vitest

---

## Testes automatizados

O projeto possui testes unitários e testes de integração.

Os testes de integração executam a API utilizando um banco PostgreSQL real e isolado por meio de **Testcontainers**, permitindo validar fluxos completos da aplicação.

Entre os cenários cobertos estão:

- autenticação;
- autorização;
- cadastro;
- perfis;
- disponibilidade;
- solicitações de aula;
- persistência;
- ciclo de vida da aula;
- check-in.

Para executar os testes do backend:

```bash
dotnet test DriveMatch.slnx
```

Para executar os testes do frontend:

```bash
cd frontend/drivematch-web
npm test -- --watch=false
```

---

## Executando o projeto

### Pré-requisitos

- .NET 10 SDK
- Node.js
- npm
- Docker Desktop

Clone o repositório e acesse a pasta:

```bash
git clone https://github.com/isadorasilvino/drivematch.git
cd drivematch
```

Crie um arquivo `.env` na raiz:

```env
JWT_KEY=defina-uma-chave-segura-para-desenvolvimento
```

Inicie o PostgreSQL e a API:

```bash
docker compose up --build
```

A API ficará disponível em:

```text
http://localhost:8080
```

### Frontend

Em outro terminal:

```bash
cd frontend/drivematch-web
npm install
npm start
```

---

## Segurança

O projeto utiliza autenticação baseada em JWT e autorização no backend.

Segredos, como a chave utilizada para assinatura dos tokens, não são versionados no repositório e devem ser fornecidos através de configuração de ambiente.

O arquivo `.env` local permanece ignorado pelo Git.

---

## Documentação

A documentação detalhada do projeto está disponível em [`docs/`](docs/).

Principais documentos:

- [Definição do produto](docs/PRODUCT.md)
- [Requisitos](docs/REQUIREMENTS.md)
- [Arquitetura](docs/architecture/ARCHITECTURE.md)
- [Regras de negócio](docs/business-rules/BUSINESS-RULES.md)
- [Modelo de domínio](docs/domain/DOMAIN-MODEL.md)
- [Fluxos de usuário](docs/use-cases/USER-FLOWS.md)
- [Fluxo de aula](docs/use-cases/LESSON-FLOW.md)

---

## Decisões de engenharia

O projeto busca evitar complexidade prematura.

Para o escopo atual, foi adotado um Monólito Modular em vez de microserviços, mantendo limites claros entre responsabilidades sem introduzir o custo operacional de uma arquitetura distribuída.

A arquitetura foi estruturada para permitir evolução futura caso novos requisitos justifiquem mudanças.

---

## Status

**MVP em fase final de desenvolvimento.**

Backend, frontend, persistência, autenticação, fluxos principais e testes automatizados já estão implementados.

As etapas finais incluem automação de integração contínua e preparação do projeto para publicação.

---

## Autora

**Isadora Silvino**

Software Engineer | .NET | C# | Angular
