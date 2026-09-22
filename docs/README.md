# Documentação do DriveMatch

Esta pasta reúne a documentação funcional e técnica do DriveMatch.

Enquanto o [`README.md`](../README.md) da raiz apresenta uma visão geral do projeto, os documentos desta pasta detalham o produto, requisitos, arquitetura, regras de negócio, modelo de domínio e principais fluxos da aplicação.

## Índice

### Produto

[`PRODUCT.md`](PRODUCT.md)

Apresenta a visão do produto, o problema abordado, os usuários da plataforma, os objetivos do DriveMatch e o escopo definido para o MVP.

### Requisitos

[`REQUIREMENTS.md`](REQUIREMENTS.md)

Documenta os requisitos funcionais e não funcionais da aplicação, organizando os comportamentos esperados dos principais fluxos do sistema.

### Arquitetura

[`architecture/ARCHITECTURE.md`](architecture/ARCHITECTURE.md)

Descreve a arquitetura utilizada no projeto, a separação de responsabilidades entre as camadas e as principais decisões técnicas adotadas.

### Regras de negócio

[`business-rules/BUSINESS-RULES.md`](business-rules/BUSINESS-RULES.md)

Centraliza as regras que controlam o comportamento do domínio, incluindo perfis, disponibilidade, solicitações, aulas e check-in.

### Modelo de domínio

[`domain/DOMAIN-MODEL.md`](domain/DOMAIN-MODEL.md)

Apresenta as principais entidades do DriveMatch, seus relacionamentos, responsabilidades e conceitos de domínio.

### Fluxos de usuário

[`use-cases/USER-FLOWS.md`](use-cases/USER-FLOWS.md)

Documenta os principais fluxos executados por alunos e instrutores durante a utilização da plataforma.

### Fluxo de aula

[`use-cases/LESSON-FLOW.md`](use-cases/LESSON-FLOW.md)

Detalha o ciclo de uma aula, desde a disponibilização de um horário pelo instrutor até o check-in e a conclusão da aula.

## Estrutura

```text
docs/
│
├── README.md
├── PRODUCT.md
├── REQUIREMENTS.md
│
├── architecture/
│   └── ARCHITECTURE.md
│
├── business-rules/
│   └── BUSINESS-RULES.md
│
├── domain/
│   └── DOMAIN-MODEL.md
│
└── use-cases/
    ├── USER-FLOWS.md
    └── LESSON-FLOW.md
```

## Organização da documentação

A documentação foi separada por responsabilidade para evitar concentrar diferentes níveis de informação em um único arquivo.

- **Produto** explica o que é o DriveMatch e qual problema ele busca resolver.
- **Requisitos** descrevem o comportamento esperado do sistema.
- **Arquitetura** explica como a solução foi estruturada tecnicamente.
- **Regras de negócio** registram restrições e comportamentos do domínio.
- **Modelo de domínio** descreve os principais conceitos e relacionamentos.
- **Fluxos** mostram como esses elementos se combinam durante o uso da aplicação.

## Estado da documentação

Esta documentação acompanha o **MVP funcionalmente concluído** do DriveMatch.

Os documentos refletem as decisões consolidadas durante o desenvolvimento e devem descrever o comportamento efetivamente implementado no MVP.

Funcionalidades consideradas como possíveis evoluções futuras devem ser identificadas explicitamente como fora do escopo atual, evitando confusão entre o produto implementado e ideias avaliadas durante a concepção.

---

[Voltar para o README principal](../README.md)