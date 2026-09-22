Segue o conteúdo em Markdown, mantendo o texto e a estrutura apresentados, apenas corrigindo a formatação para Markdown consistente.

 # DriveMatch — User Flows

 ## 1\. Objetivo

 Este documento descreve os principais fluxos de interação dos usuários com o DriveMatch no escopo atual do MVP.

 Os fluxos representam as jornadas disponíveis para os dois perfis da plataforma:

 - Aluno;
- Instrutor.

 Eles servem como referência para a implementação das interfaces, APIs, casos de uso e regras de negócio.

---

 ## 2\. Atores

 O DriveMatch possui dois tipos de usuário:

 ### Aluno

 Usuário que procura instrutores, consulta horários disponíveis, solicita aulas, acompanha solicitações e aulas, realiza check-in e avalia aulas concluídas.

 ### Instrutor

 Usuário que mantém um perfil profissional, configura disponibilidade, recebe solicitações de aula e conduz o ciclo de vida das aulas.

---

 ## 3\. Fluxos de acesso e conta

 ### UC-001 — Cadastro

 #### Objetivo

 Permitir a criação de uma conta no DriveMatch.

 #### Fluxo principal

 1. Usuário acessa a tela de cadastro.
2. Informa:
   - nome;
   - e-mail;
   - senha;
   - tipo de usuário (`Student` ou `Instructor`).
3. Sistema valida os dados.
4. Sistema cria a conta.
5. Cadastro é confirmado.

 #### Exceções

 - E-mail já cadastrado.
- Dados inválidos.
- Senha incompatível com as regras de validação.

---

 ### UC-002 — Login

 #### Objetivo

 Permitir que um usuário cadastrado acesse a aplicação.

 #### Fluxo principal

 1. Usuário informa e-mail e senha.
2. Sistema valida as credenciais.
3. Sistema autentica o usuário.
4. Uma sessão autenticada é criada.
5. O usuário é direcionado para a área correspondente ao seu papel.

 #### Exceções

 - Credenciais inválidas.
- Usuário inativo.

---

 ### UC-003 — Primeiro acesso

 Após a autenticação, o DriveMatch verifica se o usuário possui o perfil correspondente ao seu papel.

 #### Aluno sem perfil

 O usuário é direcionado para:

 `/student/profile`

 Após criar o perfil, passa a ter acesso às demais funcionalidades destinadas ao aluno.

 #### Instrutor sem perfil

 O usuário é direcionado para:

 `/instructor/profile`

 Após criar o perfil profissional, passa a ter acesso às demais funcionalidades destinadas ao instrutor.

---

 ### UC-004 — Gerenciamento da conta

 #### Objetivo

 Permitir que um usuário autenticado mantenha seus dados de conta.

 #### Fluxos disponíveis

 O usuário pode:

 - consultar seus dados;
- alterar nome;
- alterar e-mail;
- alterar senha.

 #### Alteração de nome ou e-mail

 1. Usuário acessa seu perfil.
2. Altera nome e/ou e-mail.
3. Sistema valida os novos dados.
4. Sistema atualiza a conta.
5. Os dados da sessão são atualizados para refletir as alterações.

 #### Exceções

 - E-mail já utilizado por outra conta.
- Conta não encontrada.
- Dados inválidos.

 #### Alteração de senha

 1. Usuário informa a senha atual.
2. Informa a nova senha.
3. Sistema valida a senha atual.
4. Sistema valida a nova senha.
5. A senha da conta é atualizada.

 #### Exceções

 - Senha atual incorreta.
- Nova senha inválida.
- Conta não encontrada.

---

 ## 4\. Fluxos do aluno

 ### UC-005 — Configuração do perfil

 #### Objetivo

 Permitir que o aluno informe os dados utilizados durante sua experiência na plataforma.

 #### Informações

 O aluno informa:

 - cidade;
- estado;
- nível de experiência;
- se possui veículo;
- se possui veículo próprio disponível para as aulas.

 #### Fluxo principal

 1. Aluno acessa seu perfil.
2. Preenche as informações.
3. Sistema valida os dados.
4. Sistema cria o perfil.

 O aluno também poderá editar posteriormente essas informações.

---

 ### UC-006 — Dashboard do aluno

 #### Objetivo

 Apresentar uma visão resumida da situação atual do aluno na plataforma.

 O dashboard disponibiliza informações rápidas sobre suas atividades e atalhos para os principais fluxos da aplicação.

 O acesso depende da existência de um perfil de aluno.

---

 ### UC-007 — Buscar instrutores

 #### Objetivo

 Permitir que o aluno encontre instrutores compatíveis com suas necessidades.

 #### Fluxo principal

 1. Aluno acessa a busca de instrutores.
2. Informa os critérios desejados.
3. Sistema consulta os instrutores disponíveis.
4. Apenas instrutores ativos são considerados.
5. Sistema apresenta os resultados compatíveis.

 #### Critérios utilizados

 A busca considera informações como:

 - cidade;
- estado;
- nível de experiência;
- utilização de veículo próprio;
- preço máximo por aula.

---

 ### UC-008 — Consultar disponibilidade do instrutor

 #### Objetivo

 Permitir que o aluno consulte datas e horários disponíveis antes de solicitar uma aula.

 #### Fluxo principal

 1. Aluno seleciona um instrutor.
2. Sistema consulta a próxima data disponível.
3. Aluno seleciona uma data.
4. Sistema calcula os horários disponíveis para aquela data.
5. Os horários livres são apresentados ao aluno.

 A disponibilidade considera as configurações cadastradas pelo instrutor e os horários que não podem mais receber novas aulas.

---

 ### UC-009 — Solicitar aula

 #### Objetivo

 Permitir que o aluno solicite uma aula com um instrutor.

 #### Fluxo principal

 1. Aluno seleciona um instrutor.
2. Consulta sua disponibilidade.
3. Seleciona uma data.
4. Seleciona um horário disponível.
5. Informa se utilizará veículo próprio.
6. Opcionalmente adiciona uma mensagem ao instrutor.
7. Sistema valida a solicitação.
8. Sistema cria a solicitação de aula.

 #### Validações

 O sistema verifica, entre outras regras:

 - existência do perfil do aluno;
- existência do perfil do instrutor;
- status ativo do instrutor;
- disponibilidade para o horário solicitado;
- compatibilidade com o uso de veículo próprio.

---

 ### UC-010 — Acompanhar solicitações

 #### Objetivo

 Permitir que o aluno acompanhe as solicitações de aula enviadas.

 #### Fluxo principal

 1. Aluno acessa suas solicitações.
2. Sistema recupera as solicitações pertencentes ao aluno.
3. Sistema apresenta seus respectivos estados.

 O aluno pode acompanhar se uma solicitação ainda aguarda resposta, foi aceita, recusada ou cancelada.

---

 ### UC-011 — Cancelar solicitação

 #### Objetivo

 Permitir que o aluno cancele uma solicitação que ainda possa ser cancelada.

 #### Fluxo principal

 1. Aluno acessa suas solicitações.
2. Seleciona uma solicitação elegível para cancelamento.
3. Solicita o cancelamento.
4. Sistema valida a operação.
5. A solicitação é cancelada.

 Somente o aluno responsável pela solicitação pode realizar essa operação.

---

 ### UC-012 — Consultar aulas

 #### Objetivo

 Permitir que o aluno acompanhe suas aulas.

 #### Fluxo principal

 1. Aluno acessa a área de aulas.
2. Sistema recupera as aulas associadas ao usuário.
3. A aplicação apresenta as informações e ações disponíveis conforme o estado de cada aula.

---

 ### UC-013 — Cancelar aula

 #### Objetivo

 Permitir o cancelamento de uma aula quando seu estado atual permitir essa operação.

 #### Fluxo principal

 1. Usuário acessa uma aula elegível.
2. Solicita o cancelamento.
3. Sistema valida a associação do usuário com a aula.
4. Sistema valida o estado atual.
5. Aula é cancelada.

 O cancelamento pode ser realizado pelo aluno ou pelo instrutor associado à aula, respeitando as regras de negócio.

---

 ## 5\. Fluxos do instrutor

 ### UC-014 — Configuração do perfil profissional

 #### Objetivo

 Permitir que o instrutor configure as informações utilizadas para apresentar seus serviços aos alunos.

 #### Informações

 O instrutor informa:

 - descrição profissional;
- anos de experiência;
- cidade;
- estado;
- preço por aula;
- se aceita iniciantes;
- se aceita alunos experientes;
- se aceita veículo do aluno.

 #### Fluxo principal

 1. Instrutor acessa seu perfil.
2. Preenche as informações obrigatórias.
3. Sistema valida os dados.
4. Sistema cria o perfil profissional.

 O perfil poderá ser editado posteriormente.

---

 ### UC-015 — Ativação e desativação do perfil

 #### Objetivo

 Permitir que o instrutor controle sua disponibilidade pública na plataforma.

 #### Ativação

 1. Instrutor acessa seu perfil.
2. Solicita a ativação.
3. Sistema verifica se o perfil pode ser ativado.
4. Perfil passa para o estado ativo.

 Quando ativo, o instrutor pode ser encontrado pelos alunos.

 #### Desativação

 1. Instrutor acessa seu perfil.
2. Solicita a desativação.
3. Sistema altera o estado do perfil.

 Um perfil desativado não fica disponível para novas buscas e solicitações.

---

 ### UC-016 — Dashboard do instrutor

 #### Objetivo

 Apresentar uma visão resumida da atividade do instrutor.

 O dashboard disponibiliza informações rápidas sobre solicitações e aulas, além de atalhos para as principais áreas da aplicação.

 Também apresenta o estado atual do perfil profissional.

 Quando o perfil está ativo, essa informação é exibida de forma informativa.

 Quando está desativado, o dashboard destaca que sua ativação é necessária para que o instrutor possa ser encontrado por novos alunos.

---

 ### UC-017 — Configuração de disponibilidade

 #### Objetivo

 Permitir que o instrutor defina os períodos em que aceita aulas.

 #### Fluxo principal

 1. Instrutor acessa sua disponibilidade.
2. Define:
   - dia da semana;
   - horário inicial;
   - horário final;
   - duração das aulas;
   - intervalo entre aulas.
3. Sistema valida os dados.
4. Disponibilidade é criada.

 O instrutor também pode:

 - consultar suas disponibilidades;
- editar uma disponibilidade;
- ativar ou desativar uma disponibilidade.

---

 ### UC-018 — Receber solicitações

 #### Objetivo

 Permitir que o instrutor acompanhe solicitações enviadas pelos alunos.

 #### Fluxo principal

 1. Aluno cria uma solicitação.
2. Solicitação é registrada pelo sistema.
3. Instrutor acessa as solicitações recebidas.
4. Sistema apresenta as solicitações associadas ao instrutor.
5. Instrutor pode aceitar ou recusar as solicitações elegíveis.

---

 ### UC-019 — Aceitar solicitação

 #### Fluxo principal

 1. Instrutor seleciona uma solicitação pendente.
2. Solicita a aceitação.
3. Sistema valida a solicitação.
4. Sistema verifica a disponibilidade do horário.
5. Sistema verifica possíveis conflitos de agenda.
6. Solicitação é aceita.
7. A aula correspondente é disponibilizada no fluxo de aulas.

 #### Exceções

 - Solicitação inexistente.
- Horário indisponível.
- Conflito de agenda.
- Operação incompatível com o estado atual da solicitação.

---

 ### UC-020 — Recusar solicitação

 #### Fluxo principal

 1. Instrutor seleciona uma solicitação pendente.
2. Seleciona a opção de recusa.
3. Sistema valida a operação.
4. Solicitação é recusada.

 Somente o instrutor associado à solicitação pode realizar essa operação.

---

 ### UC-021 — Consultar aulas

 #### Objetivo

 Permitir que o instrutor acompanhe suas aulas e execute as ações correspondentes ao estado de cada uma.

 #### Fluxo principal

 1. Instrutor acessa a área de aulas.
2. Sistema recupera as aulas associadas ao instrutor.
3. Sistema apresenta as aulas e seus estados.
4. As ações disponíveis são apresentadas conforme o estado atual da aula.

---

 ## 6\. Fluxos compartilhados da aula

 ### UC-022 — Iniciar check-in

 #### Ator

 Instrutor.

 #### Objetivo

 Iniciar o processo de confirmação de presença para uma aula.

 #### Fluxo principal

 1. Instrutor acessa uma aula elegível para check-in.
2. Seleciona a opção para iniciar o check-in.
3. Backend valida a aula e o instrutor autenticado.
4. Sistema gera um token temporário e único.
5. Token recebe validade de 15 minutos.
6. Frontend representa o token por meio de QR Code.
7. QR Code é apresentado ao aluno.

 Após o início do processo, a aula permanece aguardando a confirmação do aluno.

---

 ### UC-023 — Confirmar check-in

 #### Ator

 Aluno.

 #### Pré-condições

 - A aula deve estar no estado correspondente ao processo de check-in.
- O aluno autenticado deve estar associado à aula.
- Deve existir um token válido.

 #### Fluxo principal

 1. Aluno realiza a leitura do QR Code.
2. Frontend obtém o token de check-in.
3. Token é enviado ao backend utilizando a autenticação do aluno.
4. Backend verifica:
   - associação do aluno à aula;
   - estado atual da aula;
   - correspondência do token;
   - validade temporal do token.
5. Sistema registra o check-in.
6. Sistema registra o início da aula.
7. Token utilizado é invalidado.
8. Aula passa para `IN_PROGRESS`.

 #### Token expirado

 1. Aluno tenta confirmar utilizando um token expirado.
2. Sistema rejeita a operação.
3. Instrutor poderá iniciar novamente o check-in.
4. Um novo token é gerado.
5. O token anterior deixa de ser válido.

 #### Token inválido

 1. Aluno apresenta um token diferente do token ativo.
2. Sistema rejeita a operação.
3. A presença não é registrada.
4. O estado da aula não é alterado.

 #### Aluno não associado

 1. Outro aluno tenta confirmar o check-in.
2. Sistema rejeita a operação por falta de permissão.
3. Nenhuma informação da aula é alterada.

---

 ### UC-024 — Encerrar aula

 #### Ator

 Instrutor.

 #### Fluxo principal

 1. Instrutor acessa uma aula em andamento.
2. Seleciona a opção de encerramento.
3. Sistema valida a associação do instrutor.
4. Sistema registra a conclusão.
5. Aula passa para `COMPLETED`.

 Somente aulas em estado compatível podem ser concluídas.

---

 ### UC-025 — Registrar não comparecimento

 #### Ator

 Instrutor.

 #### Objetivo

 Registrar que uma aula agendada não foi realizada por ausência.

 #### Fluxo principal

 1. Instrutor acessa uma aula elegível.
2. Seleciona a opção de não comparecimento.
3. Sistema valida a associação do instrutor e o estado da aula.
4. Sistema registra o não comparecimento.
5. Aula passa para o estado correspondente à ausência.

---

 ### UC-026 — Avaliar aula

 #### Ator

 Aluno.

 #### Pré-condições

 - A aula deve estar concluída.
- O aluno autenticado deve estar associado à aula.
- A aula ainda não pode possuir avaliação do aluno.

 #### Fluxo principal

 1. Aluno acessa suas aulas.
2. Sistema identifica uma aula concluída elegível para avaliação.
3. Aluno informa uma nota.
4. Opcionalmente informa um comentário.
5. Sistema valida a avaliação.
6. Avaliação é registrada.

 #### Exceções

 - Aula inexistente.
- Aula ainda não concluída.
- Aluno não associado à aula.
- Aula já avaliada.
- Nota ou dados da avaliação inválidos.

---

 ## 7\. Proteção dos fluxos

 As áreas internas do DriveMatch exigem autenticação.

 Além disso, o acesso é controlado pelo papel do usuário:

 - rotas de aluno exigem papel `Student`;
- rotas de instrutor exigem papel `Instructor`.

 As funcionalidades principais também exigem a existência do perfil correspondente.

 Um aluno autenticado sem perfil é direcionado para a criação do perfil de aluno.

 Um instrutor autenticado sem perfil é direcionado para a criação do perfil profissional.

 Essa separação impede que usuários acessem jornadas incompatíveis com seu papel ou utilizem funcionalidades que dependem de um perfil ainda inexistente.

---

 Sim. O problema está no fechamento do bloco de código do fluxo da seção 8: faltou o \`\`\` antes do `## 9`. Por isso, o Markdown interpreta a seção 9 como parte do código.

 A correção é esta:

 DriveMatch — User Flows

## 8\. Fluxo geral do MVP

```
                         VISITANTE
                             │
                ┌────────────┴────────────┐
                │                         │
             Cadastro                   Login
                │                         │
                └────────────┬────────────┘
                             │
                             ▼
                       Autenticação
                             │
                 ┌───────────┴───────────┐
                 │                       │
                 ▼                       ▼
               ALUNO                 INSTRUTOR
                 │                       │
                 ▼                       ▼
              Perfil                  Perfil
                 │                       │
                 ▼                       ▼
             Dashboard                Dashboard
                 │                       │
                 ▼                       ▼
        Buscar instrutores      Configurar disponibilidade
                 │                       │
                 ▼                       │
       Consultar disponibilidade        │
                 │                       │
                 ▼                       │
          Solicitar aula ────────────────┘
                 │
                 ▼
             PENDENTE
                 │
                 ▼
       Instrutor recebe solicitação
                 │
          ┌──────┴──────┐
          │             │
          ▼             ▼
       Recusar         Aceitar
          │             │
          ▼             ▼
      REJEITADA        Aula
                        │
                        ▼
                    Check-in
                        │
                        ▼
                  IN_PROGRESS
                        │
                        ▼
                    Conclusão
                        │
                        ▼
                    COMPLETED
                        │
                        ▼
                    Avaliação
```

 ## 9\. Escopo dos fluxos

 Os fluxos documentados representam o comportamento implementado no MVP atual do DriveMatch.

 Funcionalidades não implementadas não devem ser interpretadas como parte destes fluxos.

 O MVP não inclui:

 - pagamentos;
- repasses financeiros;
- chat em tempo real;
- notificações push;
- geolocalização em tempo real;
- integração com órgãos de trânsito;
- recursos administrativos avançados.