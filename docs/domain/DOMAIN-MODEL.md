**# DriveMatch — Modelo de Domínio**



**## 1. Objetivo**



**Este documento descreve os principais conceitos do domínio do DriveMatch, seus relacionamentos e responsabilidades.**



**O modelo de domínio será utilizado como referência para a implementação das entidades, regras de negócio e persistência.**



**---**



**## 2. Entidades**



**### User**



**Representa a conta de acesso à plataforma.**



**Responsabilidades:**



**- Identificação do usuário.**

**- Autenticação.**

**- Controle de papel.**

**- Controle de status da conta.**



**Principais atributos:**



**- Id.**

**- Name.**

**- Email.**

**- PasswordHash.**

**- Role.**

**- Status.**

**- CreatedAt.**

**- UpdatedAt.**



**---**



**### StudentProfile**



**Representa as informações específicas de um aluno.**



**Principais atributos:**



**- Id.**

**- UserId.**

**- City.**

**- State.**

**- ExperienceLevel.**

**- OwnsVehicle.**

**- HasOwnVehicleForLessons.**

**- CreatedAt.**

**- UpdatedAt.**



**---**



**### InstructorProfile**



**Representa o perfil profissional de um instrutor.**



**Principais atributos:**



**- Id.**

**- UserId.**

**- Description.**

**- ExperienceYears.**

**- City.**

**- State.**

**- PricePerLesson.**

**- AcceptsBeginners.**

**- AcceptsExperiencedStudents.**

**- AcceptsStudentVehicle.**

**- Status.**

**- CreatedAt.**

**- UpdatedAt.**



**Somente instrutores com status `ACTIVE` poderão aparecer nas buscas.**



**---**



**### Availability**



**Representa um intervalo recorrente de disponibilidade de um instrutor.**



**Principais atributos:**



**- Id.**

**- InstructorProfileId.**

**- DayOfWeek.**

**- StartTime.**

**- EndTime.**

**- IsActive.**



**Os horários representam disponibilidade recorrente por dia da semana.**



**---**



**### LessonRequest**



**Representa uma solicitação de aula realizada por um aluno para um instrutor.**



**Principais atributos:**



**- Id.**

**- StudentId.**

**- InstructorId.**

**- RequestedDate.**

**- StartTime.**

**- EndTime.**

**- UsesStudentVehicle.**

**- StudentMessage.**

**- Status.**

**- CreatedAt.**

**- UpdatedAt.**



**Uma solicitação poderá ser aceita, recusada, cancelada ou expirar.**



**---**



**### Lesson**



**Representa uma aula efetivamente agendada.**



**Principais atributos:**



**- Id.**

**- StudentId.**

**- InstructorId.**

**- LessonRequestId.**

**- ScheduledDate.**

**- StartTime.**

**- EndTime.**

**- Status.**

**- StartedAt.**

**- CheckInAt.**

**- CompletedAt.**

**- CancelledAt.**

**- CreatedAt.**



**A aula possui ciclo de vida próprio.**



**---**



**### Review**



**Representa a avaliação realizada por um aluno após uma aula concluída.**



**Principais atributos:**



**- Id.**

**- LessonId.**

**- StudentId.**

**- InstructorId.**

**- Rating.**

**- Comment.**

**- CreatedAt.**



**Uma aula poderá possuir no máximo uma avaliação.**



**---**



**## 3. Enumerações**



**### UserRole**



**```text**

**STUDENT**

**INSTRUCTOR**

**```**



**### UserStatus**



**```text**

**ACTIVE**

**INACTIVE**

**```**



**### InstructorProfileStatus**



**```text**

**DRAFT**

**ACTIVE**

**INACTIVE**

**```**



**### LessonRequestStatus**



**```text**

**PENDING**

**ACCEPTED**

**CONFIRMED**

**REJECTED**

**CANCELLED**

**EXPIRED**

**```**



**### LessonStatus**



**```text**

**SCHEDULED**

**CHECK\_IN**

**IN\_PROGRESS**

**COMPLETED**

**CANCELLED**

**NOT\_ATTENDED**

**```**



**---**



**## 4. Relacionamentos**



**```text**

**User**

&#x20;**├── 0..1 StudentProfile**

&#x20;**└── 0..1 InstructorProfile**



**InstructorProfile**

&#x20;**└── N Availability**



**StudentProfile**

&#x20;**└── N LessonRequest**



**InstructorProfile**

&#x20;**└── N LessonRequest**



**LessonRequest**

&#x20;**└── 0..1 Lesson**



**Lesson**

&#x20;**└── 0..1 Review**

**```**



**---**



**## 5. Fluxo principal do domínio**



**```text**

**Student**

&#x20;   **↓**

**Busca instrutor**

&#x20;   **↓**

**Seleciona horário**

&#x20;   **↓**

**LessonRequest**

&#x20;   **↓**

**PENDING**

&#x20;   **↓**

**Instrutor aceita**

&#x20;   **↓**

**CONFIRMED**

&#x20;   **↓**

**Lesson**

&#x20;   **↓**

**SCHEDULED**

&#x20;   **↓**

**Check-in**

&#x20;   **↓**

**IN\_PROGRESS**

&#x20;   **↓**

**COMPLETED**

&#x20;   **↓**

**Review**

**```**



**---**



**## 6. Regras importantes**



**### Instrutor ativo**



**Somente instrutores com perfil ACTIVE poderão ser encontrados pelos alunos.**



**### Conflito de agenda**



**Um instrutor não poderá possuir duas aulas confirmadas no mesmo intervalo.**



**### Disponibilidade**



**Uma solicitação deverá respeitar a disponibilidade configurada pelo instrutor.**



**### Check-in**



**Uma aula somente poderá entrar em IN\_PROGRESS após check-in válido.**



**### Conclusão**



**Uma aula somente poderá ser concluída quando estiver IN\_PROGRESS.**



**### Avaliação**



**Uma avaliação somente poderá ser criada para uma aula COMPLETED.**



**### Avaliação única**



**Uma aula poderá possuir no máximo uma avaliação.**



**---**



**## 7. Decisões de modelagem**



**### User separado de perfil**



**A conta de autenticação é separada dos dados específicos de aluno e instrutor.**



**Isso permite manter responsabilidades distintas e possibilita evolução futura dos papéis.**



**### LessonRequest separado de Lesson**



**Uma solicitação representa uma intenção de agendamento.**



**Uma aula representa um compromisso efetivamente confirmado.**



**A separação evita misturar estados de solicitação com estados de execução da aula.**



**### Informações específicas da aula**



**Dados que podem variar entre aulas devem ser armazenados na solicitação ou na aula, em vez de depender exclusivamente do perfil do usuário.**



**Exemplo:**



**`UsesStudentVehicle`**



**A preferência geral pode pertencer ao perfil, mas a decisão específica da aula pertence à solicitação.**



**### Matching**



**O cálculo de compatibilidade será inicialmente tratado como comportamento da aplicação, não como uma entidade persistida.**



**O score poderá ser calculado dinamicamente a partir das características do aluno e dos instrutores.**



**---**



**## 8. Escopo inicial**



**O modelo não contempla inicialmente:**



**- Pagamentos.**

**- Assinaturas.**

**- Chat.**

**- Mensagens.**

**- Cupons.**

**- Promoções.**

**- Veículos como entidade independente.**

**- Notificações complexas.**

**- Certificações.**

**- Favoritos.**



**Esses conceitos poderão ser adicionados futuramente caso novos requisitos justifiquem sua existência.**

**```**

