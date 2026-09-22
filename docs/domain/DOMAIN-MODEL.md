# DriveMatch — Modelo de Domínio

## 1. Objetivo

Este documento descreve o modelo de domínio implementado no DriveMatch, incluindo suas entidades, value objects, enumerações, relacionamentos e principais comportamentos.

O modelo representa os conceitos centrais utilizados pelo MVP e serve como referência para as camadas de Application, Infrastructure e API.

As regras de negócio completas estão documentadas em [`../business-rules/BUSINESS-RULES.md`](../business-rules/BUSINESS-RULES.md).

---

## 2. Visão geral

O domínio do DriveMatch é composto pelas seguintes entidades principais:

- `User`
- `StudentProfile`
- `InstructorProfile`
- `Availability`
- `LessonRequest`
- `Lesson`
- `Review`

Também fazem parte do domínio:

### Value objects

- `Money`
- `AvailabilitySlot`

### Enumerações

- `UserRole`
- `UserStatus`
- `ExperienceLevel`
- `InstructorProfileStatus`
- `LessonRequestStatus`
- `LessonStatus`

---

# 3. Entidades

## 3.1 User

Representa a conta de acesso de um usuário ao DriveMatch.

A entidade concentra os dados utilizados para identificação, autenticação e autorização básica.

### Atributos

- `Id`
- `Name`
- `Email`
- `PasswordHash`
- `Role`
- `Status`
- `CreatedAt`
- `UpdatedAt`

### Papel do usuário

O papel é representado por `UserRole`:

- `Student`
- `Instructor`

O papel é definido na criação da conta.

### Status da conta

O status é representado por `UserStatus`:

- `Active`
- `Inactive`

Uma nova conta é criada com status `Active`.

### Comportamentos

A entidade permite:

- atualizar nome e e-mail;
- alterar o hash da senha;
- ativar a conta;
- desativar a conta.

O e-mail é normalizado para letras minúsculas.

Na persistência, o e-mail possui índice único.

---

## 3.2 StudentProfile

Representa os dados específicos de um usuário com papel de aluno.

### Atributos

- `Id`
- `UserId`
- `City`
- `State`
- `ExperienceLevel`
- `OwnsVehicle`
- `HasOwnVehicleForLessons`
- `CreatedAt`
- `UpdatedAt`

### Nível de experiência

O nível de experiência é representado por `ExperienceLevel`:

- `Beginner`
- `Experienced`

### Preferências relacionadas a veículo

`OwnsVehicle` indica se o aluno possui veículo.

`HasOwnVehicleForLessons` indica se o aluno pretende disponibilizar veículo próprio para utilização durante as aulas.

Um aluno não pode informar que disponibiliza veículo próprio para as aulas quando não possui veículo.

### Comportamentos

O perfil permite:

- atualizar localização;
- atualizar nível de experiência;
- atualizar preferências relacionadas ao veículo.

Cidade e estado são obrigatórios.

O estado é normalizado para letras maiúsculas.

### Relacionamento com User

Cada `StudentProfile` pertence a exatamente um `User`.

Um usuário pode possuir no máximo um perfil de aluno.

---

## 3.3 InstructorProfile

Representa o perfil profissional de um instrutor.

### Atributos

- `Id`
- `UserId`
- `Description`
- `ExperienceYears`
- `City`
- `State`
- `PricePerLesson`
- `AcceptsBeginners`
- `AcceptsExperiencedStudents`
- `AcceptsStudentVehicle`
- `Status`
- `CreatedAt`
- `UpdatedAt`

### Preferências de atendimento

O instrutor informa se:

- aceita alunos iniciantes;
- aceita alunos experientes;
- aceita utilização do veículo do aluno.

### Preço

O preço da aula é representado pelo value object `Money`.

### Status do perfil

O status é representado por `InstructorProfileStatus`:

- `Draft`
- `Active`
- `Inactive`

Um novo perfil é criado inicialmente como `Draft`.

O instrutor pode posteriormente ativar ou desativar o perfil.

Somente perfis ativos podem ser disponibilizados para busca pelos alunos.

### Comportamentos

O perfil permite:

- atualizar descrição profissional;
- atualizar anos de experiência;
- atualizar localização;
- atualizar preço;
- atualizar preferências de atendimento;
- ativar o perfil;
- desativar o perfil.

Os anos de experiência não podem ser negativos.

Cidade e estado são obrigatórios.

O estado é normalizado para letras maiúsculas.

### Relacionamento com User

Cada `InstructorProfile` pertence a exatamente um `User`.

Um usuário pode possuir no máximo um perfil de instrutor.

---

## 3.4 Availability

Representa uma configuração recorrente de disponibilidade semanal de um instrutor.

Uma disponibilidade define uma janela de atendimento para determinado dia da semana e as regras utilizadas para gerar os horários disponíveis dentro dessa janela.

### Atributos

- `Id`
- `InstructorProfileId`
- `DayOfWeek`
- `StartTime`
- `EndTime`
- `LessonDurationMinutes`
- `BreakDurationMinutes`
- `IsActive`

### Duração da aula

As durações permitidas são:

- 30 minutos;
- 40 minutos;
- 45 minutos;
- 50 minutos;
- 60 minutos.

### Intervalo entre aulas

Os intervalos permitidos são:

- 0 minutos;
- 5 minutos;
- 10 minutos;
- 15 minutos;
- 20 minutos;
- 30 minutos.

### Comportamentos

A disponibilidade permite:

- atualizar sua configuração;
- ativar;
- desativar;
- gerar os slots disponíveis;
- verificar se determinado intervalo corresponde a um slot válido.

### Geração de slots

Os slots são calculados dinamicamente a partir de:

- horário inicial;
- horário final;
- duração da aula;
- intervalo entre aulas.

A geração continua enquanto uma aula completa puder ser encaixada dentro da janela configurada.

Cada slot gerado é representado por `AvailabilitySlot`.

### Validações principais

- o horário inicial deve ser anterior ao horário final;
- a duração da aula deve utilizar um dos valores permitidos;
- o intervalo deve utilizar um dos valores permitidos;
- a janela configurada deve comportar pelo menos uma aula completa.

### Relacionamento

Cada `Availability` pertence a um `InstructorProfile`.

Um instrutor pode possuir múltiplas disponibilidades.

---

## 3.5 LessonRequest

Representa uma solicitação de aula criada por um aluno para um instrutor.

A solicitação existe antes da criação da aula e representa a intenção de realizar um agendamento.

### Atributos

- `Id`
- `StudentId`
- `InstructorId`
- `RequestedDate`
- `StartTime`
- `EndTime`
- `UsesStudentVehicle`
- `StudentMessage`
- `Status`
- `CreatedAt`
- `UpdatedAt`

### Status

O ciclo de vida é representado por `LessonRequestStatus`:

- `Pending`
- `Accepted`
- `Confirmed`
- `Rejected`
- `Cancelled`
- `Expired`

Uma nova solicitação é criada como `Pending`.

### Transições

Fluxo de confirmação:

```text
Pending
   ↓
Accepted
   ↓
Confirmed
```

Outras transições possíveis a partir de `Pending`:

```text
Pending → Rejected
Pending → Cancelled
Pending → Expired
```

As transições são controladas pela própria entidade.

### Comportamentos

A solicitação permite:

- aceitar;
- confirmar;
- recusar;
- cancelar;
- expirar.

### Informações específicas da solicitação

`UsesStudentVehicle` registra se o veículo do aluno será utilizado naquela solicitação específica.

`StudentMessage` permite uma mensagem opcional do aluno e é normalizada antes de ser armazenada.

### Relacionamentos

Cada solicitação pertence a:

- um `StudentProfile`;
- um `InstructorProfile`.

Uma solicitação pode originar no máximo uma `Lesson`.

---

## 3.6 Lesson

Representa uma aula efetivamente agendada.

A entidade possui ciclo de vida próprio, separado da solicitação que originou o agendamento.

### Atributos

- `Id`
- `StudentId`
- `InstructorId`
- `LessonRequestId`
- `ScheduledDate`
- `StartTime`
- `EndTime`
- `Status`
- `StartedAt`
- `CheckInAt`
- `CheckInToken`
- `CheckInTokenExpiresAt`
- `CompletedAt`
- `CancelledAt`
- `CreatedAt`

### Status

O ciclo de vida é representado por `LessonStatus`:

- `Scheduled`
- `CheckIn`
- `InProgress`
- `Completed`
- `Cancelled`
- `NotAttended`

Uma nova aula é criada como `Scheduled`.

### Fluxo principal

```text
Scheduled
    ↓
CheckIn
    ↓
InProgress
    ↓
Completed
```

Também existem os estados terminais:

```text
Scheduled → Cancelled
Scheduled → NotAttended
```

### Check-in

O check-in ocorre em duas etapas.

#### Início

O instrutor inicia o processo.

A aula passa para:

```text
CheckIn
```

Nesse momento:

- um token aleatório é gerado;
- o token recebe validade de 15 minutos.

O token é armazenado temporariamente em `CheckInToken`.

A expiração é registrada em `CheckInTokenExpiresAt`.

#### Confirmação

O aluno confirma o check-in utilizando o token.

Para ser aceito:

- o token deve ser informado;
- deve corresponder ao token da aula;
- não pode estar expirado.

Após confirmação:

- `CheckInAt` recebe o horário atual;
- `StartedAt` recebe o horário atual;
- o token é removido;
- a expiração é removida;
- a aula passa para `InProgress`.

### Conclusão

Uma aula somente pode ser concluída quando estiver em `InProgress`.

Ao concluir:

- `CompletedAt` é registrado;
- o status passa para `Completed`.

### Cancelamento

Uma aula pode ser cancelada quando estiver em `Scheduled`.

Ao cancelar:

- `CancelledAt` é registrado;
- o status passa para `Cancelled`.

### Não comparecimento

Uma aula em `Scheduled` pode ser marcada como `NotAttended`.

### Relacionamentos

Cada `Lesson` está relacionada a:

- um `StudentProfile`;
- um `InstructorProfile`;
- uma `LessonRequest`.

Uma solicitação pode possuir no máximo uma aula.

Uma aula pode possuir no máximo uma avaliação.

---

## 3.7 Review

Representa a avaliação realizada pelo aluno sobre uma aula concluída.

### Atributos

- `Id`
- `LessonId`
- `StudentId`
- `InstructorId`
- `Rating`
- `Comment`
- `CreatedAt`

### Nota

A avaliação utiliza uma nota inteira entre:

```text
1 e 5
```

Valores fora desse intervalo não são aceitos pelo domínio.

### Comentário

O comentário é opcional.

Quando informado, ele é normalizado antes de ser armazenado.

### Relacionamentos

Cada avaliação pertence a:

- uma `Lesson`;
- um `StudentProfile`;
- um `InstructorProfile`.

Uma aula pode possuir no máximo uma avaliação.

Essa unicidade também é garantida na persistência por um índice único sobre `LessonId`.

---

# 4. Value Objects

## 4.1 Money

Representa um valor monetário.

### Propriedades

- `Amount`
- `Currency`

### Regras

- o valor não pode ser negativo;
- a moeda deve ser informada;
- o valor é armazenado com duas casas decimais;
- a moeda é normalizada para letras maiúsculas.

Quando nenhuma moeda é especificada, o valor padrão é:

```text
BRL
```

No perfil do instrutor, `Money` é utilizado para representar `PricePerLesson`.

---

## 4.2 AvailabilitySlot

Representa um horário de aula calculado a partir de uma disponibilidade.

### Propriedades

- `StartTime`
- `EndTime`

O slot não é uma entidade persistida.

Ele é calculado dinamicamente pela entidade `Availability`.

---

# 5. Enumerações

## 5.1 UserRole

```text
Student
Instructor
```

---

## 5.2 UserStatus

```text
Active
Inactive
```

---

## 5.3 ExperienceLevel

```text
Beginner
Experienced
```

---

## 5.4 InstructorProfileStatus

```text
Draft
Active
Inactive
```

---

## 5.5 LessonRequestStatus

```text
Pending
Accepted
Confirmed
Rejected
Cancelled
Expired
```

---

## 5.6 LessonStatus

```text
Scheduled
CheckIn
InProgress
Completed
Cancelled
NotAttended
```

Os enums são persistidos como texto pelo Entity Framework Core.

---

# 6. Relacionamentos

A visão simplificada dos principais relacionamentos é:

```text
User
 ├── 0..1 StudentProfile
 └── 0..1 InstructorProfile


InstructorProfile
 └── 0..N Availability


StudentProfile
 ├── 0..N LessonRequest
 ├── 0..N Lesson
 └── 0..N Review


InstructorProfile
 ├── 0..N LessonRequest
 ├── 0..N Lesson
 └── 0..N Review


LessonRequest
 └── 0..1 Lesson


Lesson
 └── 0..1 Review
```

### Integridade dos relacionamentos

A persistência utiliza chaves estrangeiras para relacionar as entidades.

Os perfis de aluno e instrutor possuem `UserId` único, garantindo no máximo um perfil de cada tipo por usuário.

`LessonRequestId` é único em `Lesson`, garantindo que uma solicitação origine no máximo uma aula.

`LessonId` é único em `Review`, garantindo no máximo uma avaliação por aula.

---

# 7. Fluxo principal do domínio

Uma representação simplificada do fluxo principal é:

```text
User
 ↓
StudentProfile
 ↓
Busca por InstructorProfile ativo
 ↓
Availability
 ↓
AvailabilitySlot
 ↓
LessonRequest
 ↓
Pending
 ↓
Accepted
 ↓
Confirmed
 ↓
Lesson
 ↓
Scheduled
 ↓
CheckIn
 ↓
InProgress
 ↓
Completed
 ↓
Review
```

O fluxo completo possui regras adicionais de autorização, compatibilidade, disponibilidade, conflitos e transições de estado documentadas em:

[`../business-rules/BUSINESS-RULES.md`](../business-rules/BUSINESS-RULES.md)

---

# 8. Decisões de modelagem

## 8.1 User separado dos perfis

Os dados de autenticação e identificação ficam em `User`.

As informações específicas de cada papel ficam em:

- `StudentProfile`;
- `InstructorProfile`.

Essa separação mantém responsabilidades distintas entre conta e perfil de negócio.

---

## 8.2 LessonRequest separado de Lesson

Uma solicitação representa uma intenção de agendamento.

Uma aula representa um compromisso efetivamente criado a partir de uma solicitação confirmada.

Essa separação evita misturar o ciclo de aprovação de uma solicitação com o ciclo de execução de uma aula.

---

## 8.3 Disponibilidade separada de slots

`Availability` representa uma configuração recorrente.

`AvailabilitySlot` representa um horário calculado a partir dessa configuração.

Os slots não são persistidos como entidades independentes.

Isso permite gerar os horários dinamicamente utilizando:

- janela de atendimento;
- duração da aula;
- intervalo entre aulas.

---

## 8.4 Preço representado por Money

O preço da aula não é representado apenas por um `decimal`.

O domínio utiliza `Money`, que agrupa:

- valor;
- moeda.

Essa modelagem mantém explícito o significado monetário da informação.

---

## 8.5 Informações específicas da aula

Dados que podem variar de um agendamento para outro pertencem à solicitação ou à aula, e não apenas ao perfil.

Um exemplo é:

```text
UsesStudentVehicle
```

O perfil do aluno informa sua preferência geral relacionada ao veículo, enquanto a solicitação registra a decisão para aquele agendamento específico.

---

## 8.6 Check-in pertencente à Lesson

O processo de check-in faz parte do ciclo de vida da aula.

Por isso, token, expiração e registros temporais relacionados ao check-in pertencem à entidade `Lesson`.

Não existe uma entidade persistida separada para check-in.

---

# 9. Persistência e restrições relevantes

Embora detalhes completos de infraestrutura estejam documentados em `ARCHITECTURE.md`, algumas restrições de persistência fazem parte da compreensão do modelo.

### User

- e-mail único;
- `Name`: máximo de 150 caracteres;
- `Email`: máximo de 255 caracteres;
- `PasswordHash`: máximo de 500 caracteres.

### StudentProfile

- `UserId` único;
- cidade: máximo de 120 caracteres;
- estado: máximo de 2 caracteres.

### InstructorProfile

- `UserId` único;
- descrição: máximo de 1000 caracteres;
- cidade: máximo de 120 caracteres;
- estado: máximo de 2 caracteres;
- preço persistido com precisão `10,2`;
- moeda com máximo de 3 caracteres.

### LessonRequest

- mensagem do aluno: máximo de 1000 caracteres;
- índice para instrutor, data solicitada e status.

### Lesson

- `LessonRequestId` único;
- token de check-in: máximo de 32 caracteres;
- índice para instrutor, data agendada e status.

### Review

- `LessonId` único;
- comentário: máximo de 2000 caracteres.

### Availability

Possui índice composto por:

- `InstructorProfileId`;
- `DayOfWeek`;
- `IsActive`.

---

# 10. Escopo atual

O modelo do MVP não contempla:

- pagamentos;
- assinaturas;
- chat;
- sistema próprio de mensagens;
- cupons;
- promoções;
- veículos como entidade independente;
- sistema complexo de notificações;
- certificações;
- favoritos.

Esses conceitos podem ser incorporados futuramente caso novos requisitos justifiquem sua inclusão no domínio.

---

# 11. Referências

Para uma visão complementar do sistema:

- [`../PRODUCT.md`](../PRODUCT.md) — visão do produto e escopo;
- [`../REQUIREMENTS.md`](../REQUIREMENTS.md) — requisitos funcionais e não funcionais;
- [`../business-rules/BUSINESS-RULES.md`](../business-rules/BUSINESS-RULES.md) — regras de negócio;
- [`../architecture/ARCHITECTURE.md`](../architecture/ARCHITECTURE.md) — arquitetura técnica;
- [`../use-cases/USER-FLOWS.md`](../use-cases/USER-FLOWS.md) — fluxos de usuário;
- [`../use-cases/LESSON-FLOW.md`](../use-cases/LESSON-FLOW.md) — fluxo de aulas.