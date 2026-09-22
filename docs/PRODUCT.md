# DriveMatch — Visão do Produto

## 1. Visão geral

O DriveMatch é uma plataforma web que conecta alunos a instrutores autônomos de direção.

A proposta é centralizar em um único ambiente digital etapas que normalmente dependem de contatos informais e ferramentas separadas: descoberta de instrutores, consulta de disponibilidade, solicitação de aulas, gerenciamento da agenda e acompanhamento da realização das aulas.

O DriveMatch foi desenvolvido como um MVP funcional e como projeto de portfólio, com foco tanto na experiência dos usuários quanto na demonstração de práticas de engenharia de software aplicadas a um produto completo.

---

## 2. Problema

Alunos que procuram aulas de direção com instrutores autônomos podem encontrar dificuldades para:

- localizar profissionais disponíveis;
- entender quais instrutores atendem às suas necessidades;
- consultar horários disponíveis;
- solicitar uma aula de maneira organizada;
- acompanhar o estado de uma solicitação;
- confirmar e acompanhar as aulas agendadas.

Para o instrutor autônomo, também existe a necessidade de organizar:

- sua apresentação profissional;
- sua disponibilidade;
- as solicitações recebidas;
- as aulas agendadas;
- a confirmação de presença;
- o acompanhamento do fluxo das aulas.

O DriveMatch busca reunir esses processos em uma experiência única.

---

## 3. Usuários

O sistema possui dois tipos principais de usuário.

### 3.1 Aluno

Pessoa interessada em encontrar um instrutor autônomo e realizar aulas de direção.

No DriveMatch, o aluno pode:

- criar sua conta;
- criar e editar seu perfil;
- informar características relevantes para as aulas;
- pesquisar instrutores;
- consultar informações dos instrutores;
- visualizar horários disponíveis;
- solicitar uma aula;
- acompanhar suas solicitações;
- visualizar suas aulas;
- realizar o check-in;
- acompanhar informações principais por meio do dashboard;
- gerenciar seus dados de conta e senha.

### 3.2 Instrutor

Profissional autônomo que disponibiliza horários para realização de aulas.

No DriveMatch, o instrutor pode:

- criar sua conta;
- criar e editar seu perfil profissional;
- informar experiência, localização, preço e características do atendimento;
- configurar sua disponibilidade;
- receber solicitações de alunos;
- aceitar ou recusar solicitações;
- acompanhar suas aulas;
- iniciar o processo de check-in;
- concluir aulas;
- controlar a visibilidade do seu perfil;
- acompanhar informações principais por meio do dashboard;
- gerenciar seus dados de conta e senha.

---

## 4. Proposta de valor

O DriveMatch organiza o relacionamento entre aluno e instrutor desde a descoberta do profissional até a realização da aula.

Para o aluno, a plataforma oferece uma maneira estruturada de encontrar instrutores e solicitar aulas a partir de horários efetivamente disponibilizados.

Para o instrutor, oferece uma forma centralizada de apresentar seu serviço, organizar disponibilidade, responder solicitações e acompanhar aulas.

Um dos principais elementos do produto é o fluxo de confirmação de presença por QR Code, utilizado antes do início da aula.

---

## 5. Fluxo principal do produto

O fluxo central do MVP é:

```text
Instrutor configura seu perfil
        ↓
Instrutor ativa a visibilidade do perfil
        ↓
Instrutor disponibiliza horários
        ↓
Aluno pesquisa instrutores
        ↓
Aluno consulta o instrutor e sua disponibilidade
        ↓
Aluno solicita uma aula
        ↓
Instrutor recebe a solicitação
        ↓
Instrutor aceita ou recusa
        ↓
Se aceita, a aula é agendada
        ↓
Instrutor inicia o check-in
        ↓
Sistema gera um QR Code temporário
        ↓
Aluno realiza o check-in
        ↓
Aula é iniciada
        ↓
Instrutor conclui a aula
```

Esse fluxo representa o núcleo funcional do DriveMatch.

---

## 6. Escopo do MVP entregue

O MVP foi definido para validar o fluxo principal entre aluno e instrutor sem introduzir funcionalidades que não fossem necessárias para essa experiência.

### 6.1 Conta e autenticação

O MVP inclui:

- cadastro de usuários;
- autenticação;
- diferenciação entre aluno e instrutor;
- autorização de acordo com o papel do usuário;
- persistência da sessão;
- consulta dos dados da própria conta;
- alteração de nome e e-mail;
- alteração de senha.

---

### 6.2 Perfil do aluno

O aluno possui um perfil com informações utilizadas durante sua experiência na plataforma.

O perfil permite registrar e atualizar informações como:

- cidade;
- estado;
- nível de experiência;
- posse de veículo;
- disponibilidade de veículo próprio para as aulas.

A existência do perfil é utilizada para controlar o acesso aos fluxos que dependem dessas informações.

---

### 6.3 Perfil do instrutor

O instrutor possui um perfil profissional contendo informações relevantes para sua apresentação e para o atendimento dos alunos.

Entre elas:

- descrição;
- anos de experiência;
- cidade;
- estado;
- preço da aula;
- atendimento a alunos iniciantes;
- atendimento a alunos com experiência;
- aceitação do veículo do aluno.

O instrutor também pode controlar o estado do seu perfil.

Quando o perfil está ativo, pode participar dos fluxos de descoberta disponíveis aos alunos.

Quando está desativado, sua visibilidade para novos alunos é interrompida até que seja novamente ativado.

---

### 6.4 Busca de instrutores

O aluno pode pesquisar instrutores disponíveis na plataforma.

A busca permite acessar informações relevantes do profissional antes da solicitação de uma aula.

O MVP não implementa um algoritmo avançado de matching ou um índice calculado de compatibilidade entre aluno e instrutor.

A decisão do aluno é baseada nas informações disponibilizadas pelo instrutor e nos recursos de busca implementados.

---

### 6.5 Disponibilidade

O instrutor pode cadastrar e gerenciar períodos de disponibilidade.

Esses horários representam os momentos em que está disponível para receber solicitações de aula.

O aluno pode consultar a disponibilidade antes de realizar uma solicitação.

As regras do domínio evitam situações incompatíveis com o fluxo de agendamento, como o uso indevido de horários já comprometidos.

---

### 6.6 Solicitação de aula

A solicitação de aula conecta o interesse do aluno à disponibilidade do instrutor.

O fluxo permite que:

1. o aluno selecione um horário disponível;
2. uma solicitação seja criada;
3. o instrutor visualize a solicitação;
4. o instrutor aceite ou recuse;
5. uma solicitação aceita resulte no agendamento da aula.

Aluno e instrutor podem acompanhar as informações correspondentes às suas solicitações.

---

### 6.7 Aulas

Após o aceite de uma solicitação, a aula passa a fazer parte do fluxo de acompanhamento dos usuários.

Aluno e instrutor possuem visualizações específicas de suas aulas.

O estado da aula evolui de acordo com as ações permitidas pelo domínio.

---

### 6.8 Check-in por QR Code

O DriveMatch utiliza um processo de check-in para confirmar a presença antes do início da aula.

O fluxo funciona da seguinte maneira:

1. o instrutor inicia o processo de check-in;
2. o sistema gera um token temporário associado à aula;
3. o token é representado por um QR Code;
4. o aluno utiliza o QR Code para realizar o check-in;
5. o sistema valida o token e a aula correspondente;
6. após a confirmação, o fluxo da aula pode continuar.

O token possui validade limitada e não é reutilizável fora das condições permitidas pelo fluxo.

Esse mecanismo adiciona uma etapa explícita de confirmação entre os dois participantes.

---

### 6.9 Dashboard do aluno

A área inicial do aluno apresenta um resumo das informações mais importantes para seu uso cotidiano da plataforma.

O dashboard oferece acesso rápido aos principais fluxos e informações relacionadas às aulas e solicitações do aluno.

O objetivo não é substituir as telas especializadas, mas fornecer uma visão resumida do estado atual da conta.

---

### 6.10 Dashboard do instrutor

A área inicial do instrutor apresenta um resumo das informações relevantes para a gestão de sua atividade na plataforma.

O dashboard reúne informações relacionadas a aulas, solicitações e acesso rápido aos principais fluxos.

Também apresenta o estado de visibilidade do perfil.

Quando o perfil está ativo, essa informação é apresentada de maneira discreta.

Quando está desativado, o dashboard destaca que é necessário ativá-lo para que o instrutor volte a ficar disponível para novos alunos.

A alteração do estado continua sendo realizada na tela de perfil.

---

### 6.11 Página pública

O DriveMatch possui uma página pública de apresentação.

Ela explica:

- o que é o projeto;
- para que serve;
- como funciona o fluxo principal;
- os principais recursos;
- o caráter de projeto de portfólio;
- informações sobre sua autoria.

A página também apresenta informações relacionadas à privacidade e ao tratamento limitado de dados dentro do contexto do projeto.

---

### 6.12 Experiência responsiva

As principais telas do MVP foram desenvolvidas para funcionar tanto em desktop quanto em dispositivos móveis.

A responsividade faz parte da experiência entregue pelo produto e foi validada durante os testes manuais.

---

## 7. Privacidade e dados

O MVP procura limitar os dados armazenados às informações necessárias para os fluxos implementados.

Entre os dados associados à conta e aos perfis estão informações como:

- nome;
- e-mail;
- dados de perfil necessários ao uso da plataforma;
- informações relacionadas a solicitações e aulas.

Senhas não são armazenadas em texto puro.

O acesso aos fluxos protegidos depende de autenticação e das permissões correspondentes ao papel do usuário.

O projeto considera princípios de minimização e uso responsável de dados pessoais.

Por ser uma aplicação de portfólio e demonstração técnica, ambientes públicos de demonstração não devem ser utilizados para inserir dados pessoais reais ou sensíveis.

Essas decisões representam medidas adotadas no projeto e não constituem, isoladamente, uma declaração de conformidade jurídica integral com a LGPD.

---

## 8. Decisões de escopo

Durante a concepção e o desenvolvimento, diferentes funcionalidades foram consideradas.

O MVP final priorizou o fluxo necessário para conectar aluno e instrutor e permitir o gerenciamento completo de uma aula.

Algumas ideias inicialmente avaliadas foram retiradas do escopo ou reservadas para possíveis evoluções.

### 8.1 Pagamentos

O DriveMatch não processa pagamentos no MVP.

Não existem:

- carteira digital;
- repasse de valores;
- retenção de pagamento;
- divisão financeira por ausência;
- integração com gateways de pagamento.

Questões financeiras relacionadas à contratação do serviço permanecem fora do fluxo implementado.

---

### 8.2 Matching e compatibilidade

Durante a concepção do produto foi considerada a criação de um mecanismo de compatibilidade entre aluno e instrutor.

Esse mecanismo não faz parte do MVP entregue.

O sistema permite que o aluno encontre instrutores e consulte suas características, mas não atribui um índice ou pontuação automática de compatibilidade.

Um mecanismo de recomendação pode ser avaliado futuramente caso existam dados e necessidades suficientes para justificar sua implementação.

---

### 8.3 Comunicação em tempo real

Chat, chamadas de voz e videochamadas não fazem parte do MVP.

A proposta desta versão é validar o fluxo de descoberta, solicitação, agendamento e realização das aulas.

---

### 8.4 Aplicativo nativo

O MVP é uma aplicação web responsiva.

Não existe aplicativo mobile nativo específico para Android ou iOS.

---

### 8.5 Integrações externas

O MVP não possui integração com órgãos de trânsito ou sistemas governamentais.

O DriveMatch também não se apresenta como substituto de sistemas oficiais relacionados à habilitação ou regulamentação de trânsito.

---

## 9. Fora do escopo atual

Não fazem parte do MVP:

- pagamentos dentro da plataforma;
- carteira digital;
- chat em tempo real;
- chamadas de áudio ou vídeo;
- aplicativo mobile nativo;
- integração com órgãos de trânsito;
- matching avançado;
- pontuação automática de compatibilidade;
- funcionalidades baseadas em inteligência artificial.

A ausência desses recursos é uma decisão de escopo e não impede a validação do fluxo principal do produto.

---

## 10. Possíveis evoluções

O DriveMatch pode evoluir futuramente conforme novas necessidades sejam identificadas.

Possibilidades incluem:

- mecanismos de recomendação;
- compatibilidade baseada em preferências;
- notificações mais avançadas;
- melhorias na experiência de busca;
- recursos adicionais de gestão para instrutores;
- métricas e informações adicionais nos dashboards;
- expansão dos mecanismos de comunicação;
- integrações externas pertinentes ao domínio.

Esses itens representam possibilidades e não compromissos de implementação.

---

## 11. Critérios de conclusão do MVP

O MVP é considerado funcionalmente concluído quando o fluxo principal pode ser executado de ponta a ponta:

- usuário consegue criar uma conta e autenticar-se;
- aluno e instrutor conseguem configurar seus perfis;
- instrutor consegue disponibilizar horários;
- aluno consegue encontrar um instrutor;
- aluno consegue consultar sua disponibilidade;
- aluno consegue solicitar uma aula;
- instrutor consegue aceitar ou recusar a solicitação;
- uma solicitação aceita gera o fluxo de aula correspondente;
- aluno e instrutor conseguem acompanhar suas aulas;
- check-in pode ser realizado por meio do fluxo de QR Code;
- a aula pode seguir até sua conclusão;
- os dados da conta podem ser gerenciados;
- os principais estados podem ser acompanhados pelos dashboards.

Esses fluxos foram implementados e validados no MVP.

---

## 12. Estado atual

O **MVP do DriveMatch está funcionalmente concluído**.

Os principais fluxos foram implementados e validados por meio de testes automatizados e testes manuais de ponta a ponta.

A etapa atual do projeto é de fechamento da documentação e preparação da versão final de portfólio.

---

## 13. Objetivo do projeto

Além de validar a proposta funcional do DriveMatch, o projeto foi desenvolvido como uma demonstração prática de engenharia de software.

O objetivo é apresentar a construção de um produto desde a definição do problema e das regras de negócio até sua implementação, testes, documentação e disponibilização como projeto de portfólio.

As decisões técnicas da solução são detalhadas separadamente na documentação de arquitetura.

---

[Voltar para o índice da documentação](README.md)