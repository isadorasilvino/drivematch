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



O check-in deverá ocorrer através de um QR Code temporário gerado para a aula.



\---



\## RN-010 — Validação do QR Code



O QR Code deverá:



\* Pertencer à aula correta.

\* Estar dentro da validade.

\* Não ter sido invalidado.

\* Ser utilizado pelo aluno correto.



\---



\## RN-011 — Presença



A presença somente poderá ser registrada após a validação do check-in.



\---



\## RN-012 — Início da aula



Uma aula somente poderá entrar em `IN\_PROGRESS` após check-in válido.



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

