DriveMatch — Fluxo de Aula
1. Objetivo

Este documento descreve o ciclo de vida de uma aula no DriveMatch, desde a solicitação realizada pelo aluno até sua conclusão, cancelamento, registro de ausência e eventual avaliação.

O fluxo é dividido em duas etapas principais:

solicitação de aula (LessonRequest);

aula confirmada (Lesson).

As transições de estado são controladas pelo domínio para impedir operações incompatíveis com o estado atual das entidades.

2. Visão geral do fluxo
Aluno seleciona instrutor e horário
        ↓
Solicitação criada
        ↓
      PENDING
        ↓
Instrutor analisa solicitação
        ↓
   ┌────┴─────┐
   ↓          ↓
REJECTED    ACCEPTED
              ↓
          CONFIRMED
              ↓
       Aula é criada
              ↓
          SCHEDULED
              ↓
     Instrutor inicia
         o check-in
              ↓
           CHECK_IN
              ↓
      Aluno confirma
         o check-in
              ↓
         IN_PROGRESS
              ↓
     Instrutor conclui
              ↓
          COMPLETED
              ↓
      Aluno pode avaliar


Também existem fluxos alternativos de cancelamento, expiração da solicitação e ausência.

3. Solicitação de aula

Antes de existir uma aula, existe uma solicitação (LessonRequest).

3.1 Estados da solicitação
Estado	Descrição
Pending	Solicitação criada e aguardando decisão do instrutor
Accepted	Solicitação aceita pelo instrutor
Confirmed	Solicitação confirmada e convertida em aula
Rejected	Solicitação recusada pelo instrutor
Cancelled	Solicitação cancelada pelo aluno
Expired	Solicitação pendente expirada
3.2 Criação da solicitação

A solicitação é criada pelo aluno.

Para criar uma solicitação:

o aluno autenticado deve possuir perfil;

o instrutor deve existir;

o perfil do instrutor deve estar ativo;

a data e o horário solicitados não podem estar no passado;

o horário deve estar dentro de uma disponibilidade ativa do instrutor;

caso o aluno informe que utilizará veículo próprio, o instrutor deve aceitar essa modalidade;

o horário inicial deve ser anterior ao horário final.

A solicitação armazena:

aluno;

instrutor;

data solicitada;

horário inicial;

horário final;

utilização ou não do veículo do aluno;

mensagem opcional do aluno;

status;

data de criação;

data da última alteração.

Ao ser criada:

LessonRequest
    ↓
Pending

3.3 Aceite da solicitação

Somente o instrutor associado à solicitação pode aceitá-la.

Antes do aceite, o sistema verifica novamente:

se a disponibilidade correspondente ainda existe;

se o horário continua válido dentro da disponibilidade;

se já existe uma aula confirmada que conflite com o intervalo solicitado.

Se houver conflito de agenda, a solicitação não pode ser aceita.

Quando o aceite é realizado:

Pending
   ↓
Accepted
   ↓
Confirmed


Na mesma operação é criada uma Lesson associada à solicitação.

A nova aula é criada inicialmente com:

Scheduled

Portanto, na implementação atual, Accepted é um estado intermediário utilizado durante o processo de confirmação da solicitação.

3.4 Recusa da solicitação

Somente o instrutor associado à solicitação pode recusá-la.

A transição permitida é:

Pending
   ↓
Rejected


Uma solicitação recusada não gera uma aula.

3.5 Cancelamento da solicitação

Enquanto a solicitação estiver pendente, o aluno responsável pode cancelá-la.

Pending
   ↓
Cancelled


Uma solicitação já confirmada não utiliza esse fluxo de cancelamento, pois nesse momento já existe uma Lesson.

O cancelamento passa então a ocorrer sobre a própria aula.

3.6 Expiração da solicitação

O domínio também prevê o estado:

Pending
   ↓
Expired


A transição somente pode ocorrer enquanto a solicitação estiver pendente.

Esse estado faz parte do modelo de domínio da solicitação.

4. Aula

Uma aula (Lesson) é criada quando uma solicitação é aceita e confirmada pelo instrutor.

Ela mantém referência à solicitação que lhe deu origem.

4.1 Estados da aula
Estado	Descrição
Scheduled	Aula confirmada e agendada
CheckIn	Processo de check-in iniciado
InProgress	Presença confirmada e aula iniciada
Completed	Aula concluída
Cancelled	Aula cancelada
NotAttended	Aula marcada como não realizada por ausência
5. Transições da aula

O fluxo principal é:

Scheduled
    ↓
 CheckIn
    ↓
InProgress
    ↓
Completed


A partir de Scheduled, também são possíveis:

Scheduled ───→ Cancelled

Scheduled ───→ NotAttended


Assim, o fluxo geral pode ser representado como:

                    ┌─────────────┐
                    │  Scheduled  │
                    └──────┬──────┘
                           │
              ┌────────────┼────────────┐
              │            │            │
              ↓            ↓            ↓
          Cancelled     CheckIn     NotAttended
                           │
                           ↓
                      InProgress
                           │
                           ↓
                       Completed

6. Check-in

O check-in valida a presença do aluno e inicia efetivamente a aula.

6.1 Início do check-in

O processo é iniciado pelo instrutor responsável pela aula.

A aula precisa estar em um dos seguintes estados:

Scheduled;

CheckIn.

Ao iniciar o processo, o backend:

gera um novo token;

associa o token à aula;

define sua validade para 15 minutos;

altera o status da aula para CheckIn.

Scheduled
    ↓
Instrutor inicia check-in
    ↓
Token temporário é gerado
    ↓
CheckIn


Caso o check-in seja iniciado novamente enquanto a aula já estiver em CheckIn, um novo token é gerado e sua validade é renovada.

6.2 Representação por QR Code

O token gerado pelo backend pode ser representado pelo frontend através de um QR Code.

O QR Code contém as informações necessárias para que o aluno confirme o check-in da aula correspondente.

Instrutor inicia check-in
        ↓
Backend gera token
        ↓
Frontend apresenta QR Code
        ↓
Aluno escaneia o QR Code
        ↓
Frontend envia token ao backend

6.3 Confirmação pelo aluno

Somente o aluno associado à aula pode confirmar o check-in.

Para a confirmação ser válida:

a aula deve estar em CheckIn;

o token deve ser informado;

o token deve corresponder ao token armazenado na aula;

o token não pode estar expirado;

o usuário autenticado deve ser o aluno da aula.

Quando o token é validado:

CheckInAt recebe a data e hora atual;

StartedAt recebe a data e hora atual;

o token é removido;

a data de expiração do token é removida;

o status passa para InProgress.

CheckIn
   ↓
Token validado
   ↓
Presença registrada
   ↓
Token invalidado
   ↓
InProgress


Um token utilizado com sucesso não permanece disponível para reutilização.

7. Aula em andamento

Uma aula entra em InProgress exclusivamente após a confirmação válida do check-in.

CheckIn
   ↓
Confirmação do aluno
   ↓
InProgress


Nesse momento existem registros de:

data/hora do check-in;

data/hora de início da aula.

A partir de InProgress, a transição normal permitida é a conclusão da aula.

8. Conclusão da aula

Somente o instrutor responsável pode concluir a aula.

A aula precisa estar em:

InProgress

A transição é:

InProgress
    ↓
Completed


Ao concluir:

CompletedAt recebe a data e hora da conclusão;

o status passa para Completed.

Uma aula não pode ser concluída diretamente a partir de Scheduled ou CheckIn.

9. Cancelamento da aula

Uma aula confirmada pode ser cancelada pelo aluno ou pelo instrutor associado a ela.

Na implementação atual, o cancelamento da Lesson somente é permitido enquanto ela estiver:

Scheduled

A transição é:

Scheduled
    ↓
Cancelled


Ao cancelar:

CancelledAt recebe a data e hora do cancelamento;

o status passa para Cancelled.

Depois que o check-in foi iniciado, a aula não pode utilizar essa transição de cancelamento.

10. Não comparecimento

O instrutor responsável pode marcar uma aula como não realizada.

A operação somente é válida quando a aula está em:

Scheduled

A transição é:

Scheduled
    ↓
NotAttended


Uma aula em CheckIn, InProgress, Completed ou Cancelled não pode ser marcada como NotAttended.

11. Avaliação

Depois que uma aula é concluída, o aluno responsável pode avaliar o instrutor.

A avaliação somente pode ser criada quando:

a aula existe;

o usuário autenticado é o aluno associado à aula;

a aula está em Completed;

ainda não existe avaliação para aquela aula.

Cada aula pode possuir apenas uma avaliação.

A avaliação contém:

aula;

aluno;

instrutor;

nota;

comentário opcional;

data de criação.

A nota deve estar entre:

1 e 5

Comentários vazios ou contendo apenas espaços são tratados como ausência de comentário.

Fluxo:

Completed
    ↓
Aluno avalia
    ↓
Nota de 1 a 5
    ↓
Avaliação registrada

12. Transições inválidas

As regras de domínio impedem transições incompatíveis com o estado atual.

Exemplos:

Scheduled   → Completed
Scheduled   → InProgress
CheckIn     → Completed
CheckIn     → Cancelled
CheckIn     → NotAttended
InProgress  → Scheduled
InProgress  → Cancelled
Completed   → InProgress
Completed   → Scheduled
Cancelled   → InProgress
NotAttended → InProgress


Também não é possível executar novamente operações que exigem um estado anterior depois que a entidade já avançou para outro estado.

13. Invariantes principais

O ciclo de vida da aula mantém as seguintes invariantes:

Uma aula é criada a partir de uma solicitação aceita e confirmada.

Toda aula nova começa em Scheduled.

Uma aula InProgress passou por confirmação válida de check-in.

Uma aula Completed necessariamente esteve anteriormente em InProgress.

Uma aula Cancelled não pode ser iniciada.

Uma aula NotAttended não pode ser iniciada.

O check-in somente pode ser confirmado pelo aluno associado à aula.

A conclusão somente pode ser realizada pelo instrutor associado à aula.

O não comparecimento somente pode ser registrado pelo instrutor associado à aula.

O cancelamento da aula somente pode ser realizado pelo aluno ou instrutor associados.

Uma avaliação somente pode existir para uma aula Completed.

Cada aula pode possuir no máximo uma avaliação.

14. Dados temporais registrados

Ao longo do ciclo de vida, a aula pode registrar:

Campo	Momento
CreatedAt	criação da aula
CheckInTokenExpiresAt	geração do token de check-in
CheckInAt	confirmação do check-in pelo aluno
StartedAt	início efetivo da aula
CompletedAt	conclusão da aula
CancelledAt	cancelamento da aula

CheckInToken e CheckInTokenExpiresAt são removidos após uma confirmação de check-in bem-sucedida.

15. Resumo do ciclo completo
ALUNO
  │
  │ solicita aula
  ↓
PENDING
  │
  ├──────────────→ CANCELLED
  │
  ├──────────────→ REJECTED
  │
  ├──────────────→ EXPIRED
  │
  ↓
ACCEPTED
  │
  ↓
CONFIRMED
  │
  │ cria Lesson
  ↓
SCHEDULED
  │
  ├──────────────→ CANCELLED
  │
  ├──────────────→ NOT_ATTENDED
  │
  ↓
CHECK_IN
  │
  │ aluno confirma token
  ↓
IN_PROGRESS
  │
  │ instrutor conclui
  ↓
COMPLETED
  │
  │ aluno pode avaliar
  ↓
REVIEW