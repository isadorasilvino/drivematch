\# DriveMatch — User Flows



\## 1. Objetivo



Este documento descreve os principais fluxos de interação dos usuários com a plataforma DriveMatch.



Os fluxos são utilizados como referência para definição de casos de uso, regras de negócio, APIs e interfaces.



\---



\## 2. Atores



O sistema possui dois atores principales:



\* \*\*Aluno\*\*

\* \*\*Instrutor\*\*



\---



\## 3. Fluxos do Aluno



\### UC-001 — Cadastro



\#### Objetivo



Permitir que uma pessoa crie uma conta na plataforma.



\#### Fluxo principal



1\. Usuário acessa a tela de cadastro.

2\. Informa os dados necessários.

3\. Sistema valida os dados.

4\. Sistema cria a conta.

5\. Sistema confirma o cadastro.



\#### Exceções



\* E-mail já cadastrado.

\* Dados inválidos.

\* Credenciais incompatíveis com as regras de segurança.



\---



\### UC-002 — Configuração do perfil



\#### Objetivo



Permitir que o aluno configure informações utilizadas pela plataforma.



\#### Fluxo principal



1\. Aluno acessa seu perfil.

2\. Informa suas características.

3\. Informa preferências relacionadas às aulas.

4\. Sistema valida os dados.

5\. Sistema salva as informações.



\---



\### UC-003 — Buscar instrutores



\#### Objetivo



Permitir que o aluno encontre instrutores compatíveis com suas necessidades.



\#### Fluxo principal



1\. Aluno acessa a busca.

2\. Informa região ou outros filtros.

3\. Sistema recupera instrutores ativos.

4\. Sistema aplica os filtros.

5\. Sistema calcula a compatibilidade.

6\. Sistema ordena os resultados.

7\. Sistema apresenta os instrutores.



\#### Resultado



O aluno recebe uma lista de instrutores que atendem aos critérios informados.



\---



\### UC-004 — Visualizar instrutor



\#### Objetivo



Permitir que o aluno analise um instrutor antes de realizar uma solicitação.



\#### Informações exibidas



\* Nome.

\* Foto.

\* Descrição.

\* Experiência.

\* Regiões atendidas.

\* Preço.

\* Disponibilidade.

\* Características das aulas.

\* Avaliações.

\* Índice de compatibilidade.



\---



\### UC-005 — Solicitar aula



\#### Objetivo



Permitir que um aluno solicite uma aula com um instrutor.



\#### Fluxo principal



1\. Aluno seleciona um instrutor.

2\. Sistema apresenta os horários disponíveis.

3\. Aluno seleciona um horário.

4\. Aluno informa os dados da aula.

5\. Sistema valida a disponibilidade.

6\. Sistema cria a solicitação.

7\. Solicitação recebe o status `PENDING`.



\#### Resultado



O instrutor recebe uma nova solicitação.



\---



\### UC-006 — Acompanhar solicitação



O aluno poderá consultar o status de suas solicitações.



\#### Estados possíveis



\* `PENDING`

\* `ACCEPTED`

\* `CONFIRMED`

\* `REJECTED`

\* `CANCELLED`

\* `EXPIRED`



\---



\## 4. Fluxos do Instrutor



\### UC-007 — Configuração do perfil profissional



\#### Objetivo



Permitir que o instrutor configure seus serviços.



\#### Informações



\* Descrição profissional.

\* Experiência.

\* Regiões atendidas.

\* Preço.

\* Tipos de alunos aceitos.

\* Aceitação de veículo próprio.

\* Disponibilidade.



\---



\### UC-008 — Configuração de disponibilidade



\#### Objetivo



Permitir que o instrutor defina os horários em que aceita aulas.



\#### Exemplo



\*\*Segunda-feira\*\*



\* 08:00 — 12:00

\* 14:00 — 18:00



\*\*Terça-feira\*\*



\* 08:00 — 12:00



\---



\### UC-009 — Recebimento de solicitação



\#### Fluxo principal



1\. Aluno cria uma solicitação.

2\. Sistema registra a solicitação como `PENDING`.

3\. Instrutor recebe a solicitação.

4\. Instrutor visualiza os detalhes.

5\. Instrutor decide aceitar ou recusar.



\---



\### UC-010 — Aceitar solicitação



\#### Fluxo principal



1\. Instrutor visualiza a solicitação.

2\. Sistema verifica se o horário continua disponível.

3\. Instrutor aceita.

4\. Sistema atualiza a solicitação.

5\. Sistema cria/confirma o agendamento.



\#### Exceção



Caso o horário não esteja mais disponível, a solicitação não poderá ser aceita.



\---



\### UC-011 — Recusar solicitação



\#### Fluxo principal



1\. Instrutor visualiza a solicitação.

2\. Instrutor seleciona \*\*Recusar\*\*.

3\. Sistema registra a decisão.

4\. Solicitação recebe o status `REJECTED`.



\---



\### UC-012 — Gerenciar agenda



O instrutor poderá visualizar suas aulas futuras e históricas.



\#### Informações apresentadas



\* Data.

\* Horário.

\* Aluno.

\* Status.

\* Duração.

\* Local.



\---



\## 5. Fluxos compartilhados



\### UC-013 — Check-in da aula



\#### Ator principal

Aluno.

\#### Ator secundário

Instrutor.

\#### Pré-condições

* A aula deve estar agendada.
* O instrutor autenticado deve estar associado à aula.
* O aluno autenticado deve estar associado à aula.

\#### Fluxo principal

1. O instrutor acessa uma aula agendada.
2. O instrutor inicia o processo de check-in.
3. O backend gera um token temporário e único associado à aula.
4. O token recebe validade de 15 minutos.
5. O frontend representa o token através de um QR Code apresentado pelo instrutor.
6. O aluno realiza a leitura do QR Code.
7. O frontend envia o token ao backend utilizando a autenticação do aluno.
8. O backend valida:
   * se a aula está em `CHECK_IN`;
   * se o aluno autenticado está associado à aula;
   * se o token corresponde ao token ativo;
   * se o token ainda está dentro do período de validade.
9. O sistema registra a data e hora do check-in em `CheckInAt`.
10. O sistema registra o início da aula em `StartedAt`.
11. O token utilizado é invalidado.
12. A data de expiração do token é removida.
13. A aula passa para `IN_PROGRESS`.

\#### Fluxo alternativo — Token expirado

1. O aluno tenta confirmar o check-in utilizando um token expirado.
2. O backend rejeita a confirmação.
3. A aula permanece em `CHECK_IN`.
4. O instrutor inicia novamente o processo de check-in.
5. O backend gera um novo token com um novo período de validade.
6. O token anterior deixa de ser válido.
7. O fluxo principal poderá ser retomado com o novo token.

\#### Fluxo alternativo — Token inválido

1. O aluno tenta confirmar o check-in utilizando um token que não corresponde ao token ativo da aula.
2. O backend rejeita a confirmação.
3. A presença não é registrada.
4. A aula permanece em `CHECK_IN`.

\#### Fluxo alternativo — Aluno não associado à aula

1. Um aluno autenticado que não está associado à aula tenta confirmar o check-in.
2. O backend rejeita a operação por falta de permissão.
3. A presença não é registrada.
4. O estado da aula não é alterado.

\#### Pós-condições

Após um check-in válido:

* a presença do aluno estará registrada;
* `CheckInAt` estará preenchido;
* `StartedAt` estará preenchido;
* o token de check-in estará invalidado;
* a aula estará em `IN_PROGRESS`.



\---



\### UC-014 — Encerramento da aula



\#### Fluxo principal



1\. Instrutor acessa a aula em andamento.

2\. Instrutor seleciona \*\*Encerrar aula\*\*.

3\. Sistema registra o horário de encerramento.

4\. Aula passa para `COMPLETED`.



\---



\### UC-015 — Registro de ausência



\#### Fluxo principal



1\. Instrutor acessa uma aula agendada.

2\. Instrutor informa que a aula não foi realizada.

3\. Sistema solicita o motivo quando aplicável.

4\. Sistema registra a ocorrência.

5\. Aula passa para `NOT\_ATTENDED`.



\---



\### UC-016 — Avaliação



\#### Fluxo principal



1\. Aula é concluída.

2\. Aluno acessa o histórico.

3\. Sistema identifica que a aula pode ser avaliada.

4\. Aluno informa uma nota.

5\. Aluno pode informar um comentário.

6\. Sistema registra a avaliação.



\---



\## 6. Fluxo geral



```text

&#x20;                   ALUNO

&#x20;                     │

&#x20;                     ▼

&#x20;                  Cadastro

&#x20;                     │

&#x20;                     ▼

&#x20;                  Perfil

&#x20;                     │

&#x20;                     ▼

&#x20;           Busca de instrutores

&#x20;                     │

&#x20;                     ▼

&#x20;              Compatibilidade

&#x20;                     │

&#x20;                     ▼

&#x20;           Visualização do perfil

&#x20;                     │

&#x20;                     ▼

&#x20;            Escolha de horário

&#x20;                     │

&#x20;                     ▼

&#x20;                Solicitação

&#x20;                     │

&#x20;                     ▼

&#x20;                  PENDING

&#x20;                     │

&#x20;                     │

&#x20;           ┌─────────┴─────────┐

&#x20;           │                   │

&#x20;           │     INSTRUTOR     │

&#x20;           │                   │

&#x20;           │                   ▼

&#x20;           │          Recebe solicitação

&#x20;           │                   │

&#x20;           │                   ▼

&#x20;           │           Aceitar / Recusar

&#x20;           │                   │

&#x20;           │         ┌─────────┴─────────┐

&#x20;           │         │                   │

&#x20;           │         ▼                   ▼

&#x20;           │      REJECTED           CONFIRMED

&#x20;           │                             │

&#x20;           │                             ▼

&#x20;           │                           Agenda

&#x20;           │                             │

&#x20;           │                             ▼

&#x20;           │                      Início da aula

&#x20;           │                             │

&#x20;           │                             ▼

&#x20;           │                          QR Code

&#x20;           │                             │

&#x20;           │                             ▼

&#x20;           │                         Check-in

&#x20;           │                             │

&#x20;           │                             ▼

&#x20;           │                        IN\_PROGRESS

&#x20;           │                             │

&#x20;           │                             ▼

&#x20;           │                       Encerramento

&#x20;           │                             │

&#x20;           │                             ▼

&#x20;           │                         COMPLETED

&#x20;           │                             │

&#x20;           │                             ▼

&#x20;           │                         Avaliação

&#x20;           │

&#x20;           └───────────────────────────────────┘

