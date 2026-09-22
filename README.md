# DriveMatch

O DriveMatch é uma plataforma web que conecta alunos a instrutores autônomos de direção.

O projeto foi desenvolvido como um MVP completo e também como projeto de portfólio, com foco em demonstrar decisões reais de engenharia de software: modelagem de domínio, arquitetura em camadas, APIs REST, autenticação, regras de negócio, testes automatizados, frontend responsivo e documentação técnica.

## Sobre o projeto

A proposta do DriveMatch é facilitar o encontro e o gerenciamento da relação entre alunos e instrutores autônomos.

A plataforma permite que alunos encontrem instrutores compatíveis com suas necessidades, consultem horários disponíveis e solicitem aulas. Instrutores podem gerenciar sua disponibilidade, receber solicitações e acompanhar suas aulas.

O fluxo de aula inclui ainda um mecanismo de check-in por QR Code para confirmação de presença antes do início da aula.

## Funcionalidades

### Aluno

- Cadastro e autenticação.
- Criação e atualização de perfil.
- Gerenciamento dos dados da conta.
- Alteração de senha.
- Busca de instrutores.
- Consulta de informações e disponibilidade dos instrutores.
- Solicitação de aulas.
- Acompanhamento das solicitações realizadas.
- Visualização das aulas.
- Check-in de aula por QR Code.
- Dashboard com resumo das principais informações da conta.

### Instrutor

- Cadastro e autenticação.
- Criação e atualização de perfil profissional.
- Gerenciamento dos dados da conta.
- Alteração de senha.
- Ativação e desativação da visibilidade do perfil.
- Configuração da disponibilidade.
- Recebimento de solicitações de aula.
- Aceite e recusa de solicitações.
- Gerenciamento das aulas.
- Geração do QR Code para check-in.
- Controle do fluxo da aula.
- Dashboard com resumo de aulas, solicitações e situação do perfil.

### Fluxo de aula

O fluxo principal implementado é:

```text
Instrutor disponibiliza horário
        ↓
Aluno encontra o instrutor
        ↓
Aluno consulta a disponibilidade
        ↓
Aluno solicita uma aula
        ↓
Instrutor aceita a solicitação
        ↓
Aula é agendada
        ↓
Instrutor inicia o check-in
        ↓
QR Code temporário é gerado
        ↓
Aluno realiza o check-in
        ↓
Aula é iniciada
        ↓
Instrutor conclui a aula
```

O token utilizado no check-in é temporário e associado à aula correspondente.

## Arquitetura

O backend foi estruturado seguindo princípios de Clean Architecture, com separação entre domínio, aplicação, infraestrutura e camada de API.

```text
DriveMatch.Api
      │
      ▼
DriveMatch.Application
      │
      ▼
DriveMatch.Domain
      ▲
      │
DriveMatch.Infrastructure
```

### Domain

Contém o núcleo do negócio:

- entidades;
- enums;
- regras de negócio;
- exceções de domínio;
- comportamento das entidades.

### Application

Responsável pelos casos de uso da aplicação:

- commands e handlers;
- queries;
- contratos de repositório;
- DTOs;
- validações relacionadas aos casos de uso.

### Infrastructure

Implementa detalhes externos:

- Entity Framework Core;
- PostgreSQL;
- repositórios;
- persistência;
- migrations;
- implementações de infraestrutura.

### API

Responsável pela exposição HTTP da aplicação:

- endpoints REST;
- autenticação JWT;
- autorização;
- configuração de dependências;
- documentação OpenAPI.

## Frontend

O frontend foi desenvolvido em Angular e organizado por funcionalidades.

A aplicação possui áreas específicas para aluno e instrutor, além de uma página pública de apresentação do projeto.

Entre as decisões adotadas estão:

- standalone components;
- lazy loading;
- guards de autenticação, papel e perfil;
- componentes compartilhados;
- layout autenticado reutilizável;
- integração com API REST;
- persistência controlada da sessão;
- interface responsiva para desktop e dispositivos móveis;
- PWA.

## Tecnologias

### Backend

- C#
- .NET 10
- ASP.NET Core
- Entity Framework Core
- PostgreSQL
- JWT Bearer Authentication
- OpenAPI

### Frontend

- Angular
- TypeScript
- SCSS
- Font Awesome
- PWA

### Testes e infraestrutura

- xUnit
- testes unitários
- testes de integração
- Docker
- Docker Compose
- Git
- GitHub

## Estrutura do repositório

```text
drivematch/
│
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
├── docs/
│   ├── architecture/
│   ├── business-rules/
│   ├── domain/
│   └── use-cases/
│
├── docker-compose.yml
└── README.md
```

## Testes

O projeto possui testes unitários e testes de integração cobrindo os principais fluxos e regras de negócio da aplicação.

Os testes de integração executam a API utilizando um banco PostgreSQL real e isolado por meio de **Testcontainers**, permitindo validar os fluxos da aplicação em um ambiente próximo ao comportamento real de persistência.

A suíte do backend possui atualmente **281 testes automatizados**, incluindo cenários relacionados a:

- autenticação e autorização;
- conta do usuário;
- perfis de aluno e instrutor;
- disponibilidade;
- busca de instrutores;
- solicitações de aula;
- aulas;
- check-in;
- regras de domínio;
- persistência e endpoints.

Além dos testes automatizados, os principais fluxos do MVP foram validados manualmente de ponta a ponta no frontend, incluindo comportamento em desktop e dispositivos móveis.

## Executando o projeto

### Pré-requisitos

- .NET 10 SDK
- Node.js
- Angular CLI
- Docker
- Docker Compose

### Infraestrutura

Na raiz do repositório:

```bash
docker compose up -d
```

### Backend

```bash
dotnet restore
dotnet build
dotnet run --project src/DriveMatch.Api
```

### Frontend

```bash
cd frontend/drivematch-web
npm install
ng serve
```

A aplicação estará disponível por padrão em:

```text
http://localhost:4200
```

## Executando os testes

Na raiz do repositório:

```bash
dotnet test
```

Para validar o build do frontend:

```bash
cd frontend/drivematch-web
ng build
```

## Segurança e privacidade

O DriveMatch utiliza autenticação baseada em JWT e autorização de acordo com o papel do usuário.

Entre as medidas e decisões implementadas estão:

- senhas armazenadas por meio de hash;
- endpoints protegidos por autenticação;
- separação de acesso entre alunos e instrutores;
- validação da identidade do usuário autenticado nos fluxos protegidos;
- tokens temporários para o processo de check-in;
- armazenamento apenas dos dados necessários aos fluxos implementados no MVP.

O projeto também considera princípios de minimização e uso responsável de dados pessoais. Por se tratar de um projeto de portfólio, não devem ser utilizados dados pessoais reais ou sensíveis em ambientes de demonstração.

## Documentação

A documentação técnica detalhada está disponível em [`docs/`](docs/README.md).

Ela inclui:

- visão do produto;
- requisitos;
- arquitetura;
- regras de negócio;
- modelo de domínio;
- fluxos de usuário;
- fluxo completo de aula.

## Decisões de escopo

O objetivo do MVP foi construir e validar o fluxo principal da plataforma sem introduzir complexidade desnecessária.

Por isso, algumas funcionalidades foram deliberadamente mantidas fora desta versão, como:

- pagamentos;
- carteira digital;
- chat em tempo real;
- videochamadas;
- aplicativo mobile nativo;
- integrações com órgãos de trânsito;
- funcionalidades baseadas em IA.

Outras possibilidades de evolução, como mecanismos avançados de recomendação e compatibilidade, podem ser avaliadas em versões futuras.

## Status

**MVP funcionalmente concluído.**

Os principais fluxos da plataforma estão implementados e foram validados por testes automatizados e testes manuais.

O projeto encontra-se em etapa de fechamento técnico, documentação e publicação da versão de portfólio.

## Objetivo profissional

O DriveMatch foi desenvolvido como projeto de portfólio para demonstrar competências em engenharia e desenvolvimento de software além da experiência profissional anterior.

O projeto reúne, em uma aplicação completa, práticas relacionadas a:

- arquitetura de software;
- desenvolvimento backend com .NET e C#;
- APIs REST;
- desenvolvimento frontend com Angular;
- modelagem de domínio;
- persistência com PostgreSQL;
- autenticação e autorização;
- testes automatizados;
- containerização;
- documentação técnica;
- organização e evolução de um produto do planejamento ao MVP.

## Autora

**Isadora Silvino**

Software Developer | .NET | C# | Angular

- LinkedIn: https://www.linkedin.com/in/isadorasilvino
- GitHub: https://github.com/isadorasilvino

---

Este projeto foi desenvolvido para fins de estudo, demonstração técnica e portfólio.