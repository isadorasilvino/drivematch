\# DriveMatch — Business Rules



\## 1. Objetivo



Este documento centraliza as regras de negócio que devem ser respeitadas pela aplicação.



As regras de negócio não devem depender exclusivamente da interface do usuário e deverão ser protegidas também no backend.



\---



\## RN-001 — Tipos de usuário



O sistema possui dois tipos principais de usuário:



\* `STUDENT`

\* `INSTRUCTOR`



\---



\## RN-002 — Perfil de instrutor ativo



Somente instrutores com perfil profissional `ACTIVE` poderão aparecer nas pesquisas dos alunos.



\---



\## RN-003 — Disponibilidade



Um instrutor somente poderá receber solicitações para horários compatíveis com sua disponibilidade.



\---



\## RN-004 — Conflito de agenda



Um instrutor não poderá possuir duas aulas confirmadas que ocupem o mesmo intervalo.



\---



\## RN-005 — Solicitação pendente



Uma nova solicitação deverá iniciar com status:



`PENDING`



\---



\## RN-006 — Aceite de solicitação



Antes de aceitar uma solicitação, o sistema deverá verificar novamente a disponibilidade do horário.



\---



\## RN-007 — Confirmação



Uma solicitação aceita deverá resultar em um agendamento confirmado.



\---



\## RN-008 — Aula confirmada



Somente aulas confirmadas poderão ser iniciadas.



\---



\## RN-009 — Check-in

O processo de check-in deverá ser iniciado pelo instrutor para uma aula agendada.

Ao iniciar o check-in, o backend deverá gerar um token temporário e único associado à aula, com validade de 15 minutos.

O frontend poderá representar esse token através de um QR Code apresentado pelo instrutor.

Caso o token expire antes da confirmação do aluno, o instrutor poderá iniciar novamente o processo de check-in. Nesse caso, um novo token deverá ser gerado e o token anterior deixará de ser válido.

\---

\## RN-010 — Validação do token de check-in

Para que o check-in seja considerado válido, o token deverá:

* corresponder ao token ativo da aula;
* estar dentro do período de validade;
* pertencer a uma aula em estado `CHECK_IN`;
* ser utilizado pelo aluno associado à aula.

Somente o aluno autenticado associado à aula poderá confirmar o check-in.

Após uma confirmação válida, o token deverá ser invalidado e não poderá ser reutilizado.

\---

\## RN-011 — Presença

A presença somente poderá ser registrada após a validação bem-sucedida do check-in.

Após a validação:

* `CheckInAt` deverá registrar a data e hora da confirmação;
* `StartedAt` deverá registrar o início da aula;
* o token de check-in deverá ser invalidado;
* sua data de expiração deverá ser removida.

\---

\## RN-012 — Início da aula

Uma aula somente poderá entrar em `IN_PROGRESS` após um check-in válido realizado pelo aluno associado à aula.

A confirmação válida do check-in deverá realizar a transição da aula de `CHECK_IN` para `IN_PROGRESS`.


\---



\## RN-013 — Encerramento



Uma aula somente poderá ser encerrada quando estiver `IN\_PROGRESS`.



\---



\## RN-014 — Aula concluída



Uma aula `COMPLETED` não poderá retornar para um estado anterior.



\---



\## RN-015 — Avaliação



Um aluno somente poderá avaliar uma aula `COMPLETED`.



\---



\## RN-016 — Avaliação única



Cada aluno poderá realizar no máximo uma avaliação por aula.



\---



\## RN-017 — Compatibilidade



O cálculo de compatibilidade deverá utilizar dados do aluno e do instrutor.



O cálculo deverá ser determinístico.



Para os mesmos dados de entrada, o sistema deverá produzir o mesmo resultado.



\---



\## RN-018 — Privacidade



Informações pessoais não necessárias para a descoberta de instrutores não deverão ser expostas publicamente.



\---



\## RN-019 — Cancelamento



Uma aula não poderá ser cancelada após ter sido concluída.



\---



\## RN-020 — Integridade da agenda



A criação e confirmação de aulas deverá ser protegida contra condições de concorrência que possam resultar em dupla reserva do mesmo horário.



\---



\## 2. Princípios



As regras de negócio devem ser:



\* Independentes da interface.

\* Testáveis.

\* Centralizadas quando possível.

\* Explicitamente documentadas.

\* Protegidas no backend.

