# DriveMatch — Regras de Negócio

Este documento centraliza as principais regras de negócio implementadas no DriveMatch.

As regras descritas aqui representam o comportamento esperado do domínio e dos casos de uso da aplicação. Sempre que aplicável, elas são protegidas no backend e não dependem exclusivamente de validações da interface.

---

## 1. Usuários e perfis

### RN-001 — Tipos de usuário

O DriveMatch possui dois tipos de usuário:

- `Student`
- `Instructor`

Cada usuário possui apenas um papel no sistema.

---

### RN-002 — Estado do usuário

Um usuário pode estar nos estados:

- `Active`
- `Inactive`

Novos usuários são criados como `Active`.

Usuários inativos não podem autenticar-se na aplicação.

---

### RN-003 — Perfil de aluno

Usuários do tipo `Student` podem possuir um perfil de aluno contendo:

- cidade;
- estado;
- nível de experiência;
- informação sobre possuir veículo;
- informação sobre disponibilizar veículo próprio para as aulas.

O aluno não pode informar que disponibiliza veículo próprio para as aulas caso tenha informado que não possui veículo.

---

### RN-004 — Perfil de instrutor

Usuários do tipo `Instructor` podem possuir um perfil profissional contendo:

- descrição profissional;
- anos de experiência;
- cidade;
- estado;
- preço da aula;
- atendimento a alunos iniciantes;
- atendimento a alunos experientes;
- aceitação de veículo do aluno.

Os anos de experiência não podem ser negativos.

O valor monetário da aula não pode ser negativo.

---

### RN-005 — Estado do perfil do instrutor

O perfil de instrutor pode estar nos estados:

- `Draft`;
- `Active`;
- `Inactive`.

Um novo perfil de instrutor é criado como `Draft`.

---

### RN-006 — Ativação do perfil do instrutor

Um perfil de instrutor somente pode ser ativado quando possuir pelo menos uma disponibilidade ativa.

Instrutores cujo perfil não esteja `Active` não ficam disponíveis para descoberta e agendamento pelos alunos.

---

## 2. Disponibilidade

### RN-007 — Configuração de disponibilidade

Uma disponibilidade pertence a um único perfil de instrutor e define:

- dia da semana;
- horário inicial;
- horário final;
- duração da aula;
- intervalo entre aulas;
- estado ativo ou inativo.

Uma nova disponibilidade é criada como ativa.

---

### RN-008 — Intervalo da disponibilidade

O horário inicial de uma disponibilidade deve ser anterior ao horário final.

A janela configurada deve comportar pelo menos uma aula completa.

---

### RN-009 — Duração das aulas

As durações de aula permitidas são:

- 30 minutos;
- 40 minutos;
- 45 minutos;
- 50 minutos;
- 60 minutos.

---

### RN-010 — Intervalo entre aulas

Os intervalos permitidos entre aulas são:

- 0 minutos;
- 5 minutos;
- 10 minutos;
- 15 minutos;
- 20 minutos;
- 30 minutos.

---

### RN-011 — Geração de horários

Os horários disponíveis são calculados a partir da janela de disponibilidade, da duração da aula e do intervalo configurado entre aulas.

Somente slots completos dentro da janela configurada são considerados válidos.

---

### RN-012 — Disponibilidades ativas

Somente disponibilidades ativas podem gerar horários disponíveis para os alunos.

Disponibilidades inativas permanecem cadastradas, mas não participam da oferta de horários.

---

### RN-013 — Horários passados

Datas anteriores à data atual não possuem horários disponíveis.

Para a data atual, horários cujo início já tenha ocorrido não podem ser oferecidos para novas solicitações.

---

### RN-014 — Ocupação da agenda

Um horário que conflite com uma aula que bloqueie a agenda do instrutor não pode ser apresentado como disponível.

Dois intervalos possuem conflito quando há sobreposição entre seus horários de início e fim.

---

## 3. Solicitações de aula

### RN-015 — Criação da solicitação

Uma nova solicitação de aula é criada com status `Pending`.

A solicitação registra:

- aluno;
- instrutor;
- data;
- horário inicial;
- horário final;
- uso ou não do veículo do aluno;
- mensagem opcional do aluno.

---

### RN-016 — Instrutor disponível para solicitação

Uma solicitação somente pode ser criada para um instrutor cujo perfil esteja `Active`.

O horário solicitado deve corresponder a um slot válido de uma disponibilidade ativa do instrutor.

Solicitações para horários passados não são permitidas.

---

### RN-017 — Uso do veículo do aluno

Quando a solicitação indicar uso do veículo do aluno, o instrutor deve aceitar essa modalidade.

Caso o instrutor não aceite veículo do aluno, a solicitação não pode ser criada com essa opção.

---

### RN-018 — Estados da solicitação

Uma solicitação de aula pode assumir os estados:

- `Pending`;
- `Accepted`;
- `Confirmed`;
- `Rejected`;
- `Cancelled`;
- `Expired`.

As transições de estado devem respeitar o fluxo permitido pelo domínio.

---

### RN-019 — Aceite da solicitação

Somente o instrutor associado à solicitação pode aceitá-la.

Somente solicitações `Pending` podem ser aceitas.

Antes do aceite, o sistema deve validar novamente se o horário ainda pertence a uma disponibilidade válida do instrutor.

---

### RN-020 — Conflito no aceite

Antes de confirmar uma solicitação, o sistema deve verificar novamente a agenda do instrutor.

Caso exista conflito com outra aula que ocupe o mesmo intervalo, a solicitação não pode ser confirmada.

---

### RN-021 — Confirmação e criação da aula

Ao aceitar com sucesso uma solicitação:

1. a solicitação passa de `Pending` para `Accepted`;
2. a solicitação passa de `Accepted` para `Confirmed`;
3. uma nova aula é criada;
4. a aula inicia com status `Scheduled`.

---

### RN-022 — Rejeição da solicitação

Somente uma solicitação `Pending` pode ser rejeitada.

A rejeição altera seu status para `Rejected`.

---

### RN-023 — Cancelamento da solicitação

Somente uma solicitação `Pending` pode ser cancelada.

O cancelamento altera seu status para `Cancelled`.

---

### RN-024 — Expiração da solicitação

Somente uma solicitação `Pending` pode expirar.

A expiração altera seu status para `Expired`.

---

## 4. Aulas

### RN-025 — Criação da aula

Uma aula deve estar associada a:

- um aluno;
- um instrutor;
- uma solicitação de aula;
- uma data;
- um horário inicial;
- um horário final.

Aluno e instrutor devem ser usuários distintos.

O horário inicial deve ser anterior ao horário final.

Uma nova aula é criada com status `Scheduled`.

---

### RN-026 — Estados da aula

Uma aula pode assumir os estados:

- `Scheduled`;
- `CheckIn`;
- `InProgress`;
- `Completed`;
- `Cancelled`;
- `NotAttended`.

As operações disponíveis dependem do estado atual da aula.

---

### RN-027 — Cancelamento da aula

Uma aula somente pode ser cancelada enquanto estiver `Scheduled`.

Ao ser cancelada, passa para `Cancelled` e registra o momento do cancelamento.

---

### RN-028 — Ausência

Uma aula somente pode ser marcada como não comparecida enquanto estiver `Scheduled`.

Ao ser marcada como não comparecida, passa para `NotAttended`.

---

## 5. Check-in

### RN-029 — Início do check-in

O check-in pode ser iniciado para uma aula:

- `Scheduled`; ou
- que já esteja em `CheckIn`.

O início do processo gera um token temporário e altera a aula para `CheckIn`.

---

### RN-030 — Token de check-in

O token de check-in:

- deve ser único para o processo iniciado;
- possui validade de 15 minutos;
- fica associado à aula;
- pode ser representado pelo frontend através de QR Code.

Caso o check-in seja iniciado novamente, um novo token é gerado, substituindo o anterior.

---

### RN-031 — Confirmação do check-in

Para confirmar o check-in:

- a aula deve estar em `CheckIn`;
- o token deve ser informado;
- o token deve corresponder ao token ativo da aula;
- o token não pode estar expirado;
- a confirmação deve ser realizada pelo aluno associado à aula.

---

### RN-032 — Início efetivo da aula

Após a confirmação válida do check-in:

- o momento do check-in é registrado;
- o momento de início da aula é registrado;
- o token é invalidado;
- sua expiração é removida;
- a aula passa para `InProgress`.

Um token confirmado não pode ser reutilizado.

---

## 6. Conclusão da aula

### RN-033 — Conclusão

Somente uma aula em `InProgress` pode ser concluída.

Ao ser concluída:

- o momento da conclusão é registrado;
- o status passa para `Completed`.

Uma aula concluída não retorna aos estados anteriores através do fluxo normal do domínio.

---

## 7. Avaliações

### RN-034 — Permissão para avaliar

Somente o aluno associado à aula pode avaliá-la.

---

### RN-035 — Aula concluída

Uma avaliação somente pode ser criada para uma aula com status `Completed`.

---

### RN-036 — Avaliação única

Cada aula pode possuir no máximo uma avaliação.

Uma nova avaliação não pode ser criada quando já existir uma avaliação associada à aula.

---

### RN-037 — Nota da avaliação

A nota deve possuir valor entre:

- 1;
- 2;
- 3;
- 4;
- 5.

O comentário da avaliação é opcional.

---

## 8. Busca de instrutores

### RN-038 — Instrutores pesquisáveis

A descoberta de instrutores considera apenas perfis disponíveis para os alunos.

A pesquisa pode considerar informações como:

- cidade;
- estado;
- nível de experiência do aluno;
- utilização de veículo próprio;
- preço máximo da aula.

As preferências de atendimento do instrutor devem ser consideradas na compatibilidade da busca.

---

### RN-039 — Privacidade na descoberta

Somente informações necessárias para apresentação e descoberta do instrutor devem ser expostas aos alunos.

Dados internos ou pessoais que não façam parte desse fluxo não devem ser expostos publicamente.

---

## 9. Integridade

### RN-040 — Integridade da agenda

A confirmação de uma solicitação deve impedir que o instrutor possua aulas conflitantes no mesmo intervalo.

A validação da agenda deve ocorrer novamente no momento do aceite, mesmo que o horário estivesse disponível quando a solicitação foi criada.

---

### RN-041 — Autorização sobre recursos

Operações relacionadas a perfis, disponibilidades, solicitações, aulas e avaliações devem validar se o usuário autenticado possui permissão para atuar sobre o recurso solicitado.

Um usuário não deve conseguir executar operações pertencentes a outro aluno ou instrutor apenas conhecendo o identificador do recurso.

---

## 10. Princípios das regras de negócio

As regras de negócio do DriveMatch devem ser:

- independentes da interface;
- testáveis;
- centralizadas quando possível;
- explicitamente documentadas;
- protegidas no backend;
- aplicadas de acordo com o usuário autenticado e o estado atual das entidades.
