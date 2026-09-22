# DriveMatch — Requisitos do MVP

## 1. Objetivo

Este documento descreve os requisitos funcionais e não funcionais do MVP do DriveMatch.

Os requisitos apresentados correspondem ao comportamento consolidado da aplicação após sua implementação.

Funcionalidades consideradas durante a concepção, mas que não fazem parte do MVP entregue, são registradas separadamente como fora do escopo ou possíveis evoluções.

---

# 2. Requisitos funcionais

## 2.1 Conta e autenticação

### RF-01 — Cadastro de usuário

O sistema deve permitir o cadastro de um usuário como:

- aluno;
- instrutor.

O cadastro deve solicitar os dados necessários para criação da conta, incluindo nome, e-mail e senha.

---

### RF-02 — Autenticação

O sistema deve permitir que um usuário cadastrado realize login utilizando suas credenciais.

Após uma autenticação válida, o sistema deve identificar:

- o usuário;
- seu papel;
- sua sessão autenticada.

---

### RF-03 — Autorização por papel

O sistema deve diferenciar as permissões de alunos e instrutores.

Um aluno não deve acessar funcionalidades exclusivas de instrutores.

Um instrutor não deve acessar funcionalidades exclusivas de alunos.

---

### RF-04 — Sessão do usuário

O frontend deve manter as informações necessárias para preservar a sessão autenticada durante a utilização da aplicação.

Sessões inválidas ou expiradas não devem permitir acesso às áreas protegidas.

---

### RF-05 — Consulta da própria conta

O usuário autenticado deve conseguir consultar os dados de sua própria conta.

Entre os dados disponibilizados estão:

- nome;
- e-mail;
- papel do usuário.

---

### RF-06 — Atualização da própria conta

O usuário autenticado deve conseguir atualizar:

- nome;
- e-mail.

A alteração deve ser refletida nas áreas da interface que apresentam os dados da conta.

---

### RF-07 — Alteração de senha

O usuário autenticado deve conseguir alterar sua senha.

A operação deve exigir as informações necessárias para validar a alteração antes da definição da nova senha.

---

## 2.2 Perfil do aluno

### RF-08 — Criação do perfil do aluno

Um usuário com papel de aluno deve conseguir criar seu perfil.

O perfil deve permitir o registro das informações utilizadas nos fluxos do MVP.

---

### RF-09 — Informações do perfil do aluno

O perfil do aluno deve armazenar:

- cidade;
- estado;
- nível de experiência;
- informação sobre posse de veículo;
- informação sobre disponibilidade de veículo próprio para aulas.

---

### RF-10 — Atualização do perfil do aluno

O aluno deve conseguir atualizar as informações de seu perfil.

---

### RF-11 — Dependência de perfil

Funcionalidades do aluno que dependem de suas informações de perfil devem exigir a existência de um perfil válido.

Quando necessário, o usuário deve ser direcionado para concluir seu perfil antes de continuar.

---

## 2.3 Perfil do instrutor

### RF-12 — Criação do perfil do instrutor

Um usuário com papel de instrutor deve conseguir criar seu perfil profissional.

---

### RF-13 — Informações do perfil do instrutor

O perfil do instrutor deve permitir o registro de:

- descrição;
- anos de experiência;
- cidade;
- estado;
- preço da aula;
- aceitação de alunos iniciantes;
- aceitação de alunos com experiência;
- aceitação do veículo do aluno.

---

### RF-14 — Atualização do perfil do instrutor

O instrutor deve conseguir atualizar as informações de seu perfil profissional.

---

### RF-15 — Dependência de perfil do instrutor

Funcionalidades que dependem do perfil profissional devem exigir a existência de um perfil válido.

---

### RF-16 — Controle de visibilidade do perfil

O instrutor deve conseguir controlar o estado de seu perfil.

O perfil pode ser ativado ou desativado de acordo com as regras do sistema.

---

### RF-17 — Perfil desativado

Quando o perfil estiver desativado, o instrutor não deve permanecer disponível para descoberta por novos alunos.

O próprio instrutor deve continuar podendo acessar sua conta e gerenciar seu perfil.

---

## 2.4 Busca de instrutores

### RF-18 — Pesquisa de instrutores

O aluno deve conseguir pesquisar instrutores disponíveis na plataforma.

A busca deve considerar apenas instrutores que possam participar do fluxo de descoberta de acordo com as regras do sistema.

---

### RF-19 — Informações do instrutor

O aluno deve conseguir visualizar informações necessárias para avaliar um instrutor antes de solicitar uma aula.

Essas informações devem ser derivadas do perfil profissional do instrutor.

---

### RF-20 — Consulta da disponibilidade

O aluno deve conseguir consultar os horários disponibilizados pelo instrutor.

---

## 2.5 Disponibilidade do instrutor

### RF-21 — Cadastro de disponibilidade

O instrutor deve conseguir cadastrar períodos em que está disponível para receber solicitações de aula.

---

### RF-22 — Consulta da própria disponibilidade

O instrutor deve conseguir visualizar sua disponibilidade cadastrada.

---

### RF-23 — Gerenciamento da disponibilidade

O instrutor deve conseguir gerenciar os períodos disponibilizados conforme as operações permitidas pela aplicação.

---

### RF-24 — Proteção contra conflitos

O sistema deve impedir operações de disponibilidade incompatíveis com horários já comprometidos ou com as regras de agendamento.

---

## 2.6 Solicitações de aula

### RF-25 — Criação de solicitação

O aluno deve conseguir solicitar uma aula a partir de uma disponibilidade válida de um instrutor.

---

### RF-26 — Informações da solicitação

A solicitação deve identificar as informações necessárias para relacionar:

- aluno;
- instrutor;
- horário solicitado;
- estado da solicitação.

---

### RF-27 — Consulta das solicitações pelo aluno

O aluno deve conseguir acompanhar as solicitações realizadas por ele.

---

### RF-28 — Consulta das solicitações pelo instrutor

O instrutor deve conseguir acompanhar as solicitações recebidas.

---

### RF-29 — Aceite da solicitação

O instrutor deve conseguir aceitar uma solicitação válida.

O aceite deve produzir os efeitos necessários para transformar aquele compromisso em uma aula dentro do fluxo do sistema.

---

### RF-30 — Recusa da solicitação

O instrutor deve conseguir recusar uma solicitação válida.

---

### RF-31 — Consistência do horário

O sistema deve impedir que o mesmo horário seja comprometido de forma incompatível por múltiplas solicitações ou aulas.

---

## 2.7 Aulas

### RF-32 — Criação da aula

Uma solicitação aceita deve resultar na aula correspondente conforme as regras de negócio.

---

### RF-33 — Consulta das aulas pelo aluno

O aluno deve conseguir visualizar suas aulas.

---

### RF-34 — Consulta das aulas pelo instrutor

O instrutor deve conseguir visualizar suas aulas.

---

### RF-35 — Estado da aula

A aula deve possuir um estado que represente sua posição dentro do fluxo.

As transições devem obedecer às regras de negócio e às ações permitidas para cada participante.

---

### RF-36 — Proteção das operações da aula

Ações relacionadas a uma aula devem validar:

- identidade do usuário autenticado;
- participação do usuário naquela aula;
- papel necessário para a operação;
- estado atual da aula.

---

## 2.8 Check-in

### RF-37 — Início do check-in

O instrutor responsável pela aula deve conseguir iniciar o processo de check-in quando as condições da aula permitirem.

---

### RF-38 — Geração do token

O início do check-in deve gerar um token temporário associado à aula.

---

### RF-39 — Representação por QR Code

O token de check-in deve poder ser apresentado ao aluno por meio de um QR Code.

---

### RF-40 — Realização do check-in

O aluno participante da aula deve conseguir utilizar o token correspondente para realizar o check-in.

---

### RF-41 — Validação do token

O sistema deve validar o token antes de confirmar o check-in.

A validação deve impedir o uso de tokens inválidos, incompatíveis com a aula ou fora das condições permitidas.

---

### RF-42 — Continuidade da aula

Após o check-in válido, a aula deve poder avançar para os próximos estados previstos pelo fluxo.

---

### RF-43 — Conclusão da aula

O instrutor deve conseguir concluir uma aula quando as condições necessárias forem atendidas.

---

## 2.9 Dashboard do aluno

### RF-44 — Resumo do aluno

A página inicial autenticada do aluno deve apresentar um resumo das informações relevantes para seu uso da plataforma.

---

### RF-45 — Informações de aulas do aluno

O dashboard deve apresentar informações resumidas relacionadas às aulas do aluno, incluindo sua próxima aula quando aplicável.

Quando não houver aula correspondente, a interface deve apresentar um estado vazio adequado.

---

### RF-46 — Informações de solicitações do aluno

O dashboard deve apresentar informações resumidas relacionadas às solicitações do aluno quando aplicável.

---

### RF-47 — Acessos rápidos do aluno

O dashboard deve oferecer acesso aos principais fluxos disponíveis ao aluno.

---

## 2.10 Dashboard do instrutor

### RF-48 — Resumo do instrutor

A página inicial autenticada do instrutor deve apresentar um resumo das informações relevantes para sua utilização da plataforma.

---

### RF-49 — Informações de aulas do instrutor

O dashboard deve apresentar informações resumidas relacionadas às aulas do instrutor, incluindo sua próxima aula quando aplicável.

Quando não houver aula correspondente, a interface deve apresentar um estado vazio adequado.

---

### RF-50 — Informações de solicitações do instrutor

O dashboard deve apresentar informações resumidas relacionadas às solicitações recebidas pelo instrutor quando aplicável.

---

### RF-51 — Estado do perfil no dashboard

O dashboard deve informar se o perfil do instrutor está ativo ou desativado.

Quando ativo, a informação pode ser apresentada de forma discreta.

Quando desativado, a interface deve destacar que o perfil precisa ser ativado para voltar a ficar disponível para novos alunos.

A alteração do estado deve continuar sendo realizada na área de perfil.

---

### RF-52 — Acessos rápidos do instrutor

O dashboard deve oferecer acesso aos principais fluxos disponíveis ao instrutor.

---

## 2.11 Página pública

### RF-53 — Página inicial pública

O sistema deve possuir uma página pública acessível sem autenticação.

---

### RF-54 — Apresentação do produto

A página pública deve apresentar informações que permitam compreender:

- o que é o DriveMatch;
- para que serve;
- seus principais recursos;
- o fluxo geral da plataforma.

---

### RF-55 — Identificação como projeto de portfólio

A apresentação pública deve deixar claro que o DriveMatch é um projeto desenvolvido para fins de portfólio e demonstração técnica.

---

### RF-56 — Autoria

A página pública pode apresentar informações profissionais relacionadas à autoria do projeto e meios públicos de acesso aos respectivos perfis profissionais.

---

### RF-57 — Informações de privacidade

A página pública deve apresentar informações compatíveis com o contexto de privacidade e tratamento de dados adotado pelo projeto.

---

# 3. Requisitos não funcionais

## RNF-01 — Segurança das senhas

Senhas não devem ser armazenadas em texto puro.

---

## RNF-02 — Autenticação da API

Endpoints protegidos devem exigir autenticação válida.

---

## RNF-03 — Autorização

A aplicação deve validar as permissões necessárias para operações protegidas.

A existência de uma sessão autenticada não deve, isoladamente, permitir acesso a operações pertencentes a outro papel ou usuário.

---

## RNF-04 — Validação de entrada

Dados recebidos pela aplicação devem ser validados antes da execução de operações que dependam deles.

---

## RNF-05 — Persistência

Os dados persistentes do MVP devem ser armazenados em PostgreSQL por meio da camada de infraestrutura da aplicação.

---

## RNF-06 — Separação de responsabilidades

O backend deve manter separação entre:

- domínio;
- aplicação;
- infraestrutura;
- exposição da API.

---

## RNF-07 — API REST

A comunicação entre frontend e backend deve ocorrer por meio de endpoints HTTP da API.

---

## RNF-08 — Responsividade

As principais interfaces devem ser utilizáveis em desktop e dispositivos móveis.

---

## RNF-09 — Componentização

Elementos de interface reutilizáveis devem ser componentizados quando essa reutilização fizer sentido para a aplicação.

---

## RNF-10 — Carregamento do frontend

O frontend deve utilizar estratégias de organização e carregamento compatíveis com a estrutura Angular adotada no projeto, incluindo carregamento sob demanda das áreas apropriadas.

---

## RNF-11 — Testabilidade

As regras e casos de uso relevantes devem possuir estrutura que permita testes automatizados.

---

## RNF-12 — Testes unitários

O projeto deve possuir testes unitários para comportamentos e regras relevantes do domínio e da aplicação.

---

## RNF-13 — Testes de integração

O projeto deve possuir testes de integração para validar os principais fluxos envolvendo API e persistência.

Os testes de integração devem utilizar infraestrutura isolada apropriada para evitar dependência de um banco compartilhado entre execuções.

---

## RNF-14 — Build

Backend e frontend devem poder ser compilados sem erros antes da publicação de uma versão.

---

## RNF-15 — Containerização da infraestrutura

O projeto deve fornecer uma forma reproduzível de inicializar os serviços de infraestrutura necessários ao desenvolvimento local utilizando containers.

---

## RNF-16 — Documentação

O repositório deve possuir documentação suficiente para apresentar:

- produto;
- requisitos;
- arquitetura;
- regras de negócio;
- modelo de domínio;
- principais fluxos;
- execução do projeto.

---

## RNF-17 — Minimização de dados

O MVP deve evitar solicitar ou armazenar dados pessoais que não sejam necessários aos fluxos implementados.

---

## RNF-18 — Dados de demonstração

Ambientes públicos utilizados para demonstração e portfólio não devem ser tratados como ambientes apropriados para armazenamento de dados pessoais reais ou sensíveis.

---

# 4. Regras gerais de acesso

## 4.1 Usuário não autenticado

Um usuário não autenticado pode acessar as áreas públicas da aplicação, incluindo:

- apresentação do projeto;
- login;
- cadastro.

Áreas autenticadas devem exigir sessão válida.

---

## 4.2 Aluno autenticado

Um aluno autenticado pode acessar os fluxos destinados ao papel de aluno.

Funcionalidades que dependam da existência do perfil podem exigir que ele conclua seu perfil antes de continuar.

---

## 4.3 Instrutor autenticado

Um instrutor autenticado pode acessar os fluxos destinados ao papel de instrutor.

Funcionalidades que dependam da existência do perfil profissional podem exigir que ele conclua seu perfil antes de continuar.

---

# 5. Fora do escopo do MVP

Os seguintes recursos não constituem requisitos do MVP entregue:

- processamento de pagamentos;
- carteira digital;
- repasse financeiro;
- regras financeiras de no-show;
- chat em tempo real;
- chamadas de áudio;
- videochamadas;
- aplicativo mobile nativo;
- integração com órgãos de trânsito;
- algoritmo avançado de matching;
- índice ou pontuação automática de compatibilidade;
- funcionalidades baseadas em inteligência artificial.

A ausência desses recursos é deliberada e não caracteriza requisito incompleto do MVP.

---

# 6. Possíveis evoluções

Funcionalidades futuras podem ser avaliadas de acordo com a evolução do produto.

Entre as possibilidades estão:

- recomendações de instrutores;
- mecanismos de compatibilidade;
- notificações mais avançadas;
- melhorias nos filtros e mecanismos de busca;
- novas métricas nos dashboards;
- recursos adicionais para gestão do instrutor;
- integrações externas pertinentes ao domínio;
- mecanismos adicionais de comunicação.

Esses itens não constituem requisitos da versão atual.

---

# 7. Critérios funcionais de conclusão

O MVP atende ao fluxo principal quando é possível executar de ponta a ponta:

```text
Cadastro
   ↓
Autenticação
   ↓
Criação do perfil
   ↓
Disponibilização de horário pelo instrutor
   ↓
Busca do instrutor pelo aluno
   ↓
Consulta da disponibilidade
   ↓
Solicitação de aula
   ↓
Aceite pelo instrutor
   ↓
Agendamento da aula
   ↓
Geração do check-in
   ↓
Leitura/validação do QR Code
   ↓
Realização da aula
   ↓
Conclusão
```

Também devem estar disponíveis os fluxos complementares necessários ao MVP:

- gerenciamento da própria conta;
- alteração de senha;
- gerenciamento dos perfis;
- ativação/desativação do perfil do instrutor;
- acompanhamento de solicitações;
- acompanhamento de aulas;
- dashboards de aluno e instrutor;
- apresentação pública do projeto.

---

# 8. Estado dos requisitos

Os requisitos descritos neste documento representam o **MVP funcionalmente concluído do DriveMatch**.

Os fluxos principais foram implementados e validados por testes automatizados e testes manuais de ponta a ponta.

Requisitos e ideias existentes em versões anteriores da documentação que não estejam presentes neste documento não devem ser interpretados automaticamente como pendências do MVP.

---

[Voltar para o índice da documentação](README.md)