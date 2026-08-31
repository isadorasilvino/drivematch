\# DriveMatch — Lesson Lifecycle



\## 1. Objetivo



Este documento define o ciclo de vida de uma aula no DriveMatch. O controle de estados é utilizado para garantir que as operações ocorram em uma ordem válida.



\---



\## 2. Estados



Uma aula pode possuir os seguintes estados:



| Estado | Descrição |

| --- | --- |

| `SCHEDULED` | Aula confirmada e aguardando início |

| `CHECK\_IN` | Processo de validação de presença iniciado |

| `IN\_PROGRESS` | Aula iniciada e presença validada |

| `COMPLETED` | Aula concluída |

| `CANCELLED` | Aula cancelada |

| `NOT\_ATTENDED` | Aula não realizada por ausência |



\---



\## 3. Transições permitidas



```text

&#x20;                   ┌───────────────┐

&#x20;                   │   SCHEDULED   │

&#x20;                   └───────┬───────┘

&#x20;                ┌──────────┼──────────┐

&#x20;                │          │          │

&#x20;                ▼          ▼          ▼

&#x20;            CANCELLED   CHECK\_IN   NOT\_ATTENDED

&#x20;                           │

&#x20;                           ▼

&#x20;                      IN\_PROGRESS

&#x20;                           │

&#x20;                           ▼

&#x20;                       COMPLETED



```



\---



\## 4. Regras de transição



\### `SCHEDULED` → `CHECK\_IN`



Permitido quando:



\* A aula está confirmada.

\* O horário de início está dentro da janela permitida.

\* O instrutor autenticado é o responsável pela aula.



\### `CHECK\_IN` → `IN\_PROGRESS`



Permitido quando:



\* O token de check-in é válido.

\* O token de check-in não expirou.

\* O aluno pertence à aula.

\* A aula ainda não foi iniciada.



\### `IN\_PROGRESS` → `COMPLETED`



Permitido quando:



\* A aula está em andamento.

\* O instrutor responsável solicita o encerramento.



\### `SCHEDULED` → `CANCELLED`



\* Permitido enquanto a aula ainda não tiver sido iniciada.



\### `SCHEDULED` → `NOT\_ATTENDED`



\* Permitido quando a aula não foi realizada e a ausência é registrada pelo instrutor.



\---



\## 5. Transições inválidas



O sistema não deve permitir:



\* `SCHEDULED` → `COMPLETED`

\* `SCHEDULED` → `IN\_PROGRESS`

\* `COMPLETED` → `IN\_PROGRESS`

\* `COMPLETED` → `SCHEDULED`

\* `CANCELLED` → `IN\_PROGRESS`

\* `NOT\_ATTENDED` → `IN\_PROGRESS`



> \*\*Nota:\*\* Essas restrições devem ser garantidas pela camada responsável pelas regras de negócio.



\---



\## 6. Invariantes



\* \*\*Invariante 1:\*\* Uma aula `COMPLETED` deve ter sido previamente iniciada.

\* \*\*Invariante 2:\*\* Uma aula `IN\_PROGRESS` deve possuir registro de check-in válido.

\* \*\*Invariante 3:\*\* Uma aula `CANCELLED` não pode ser iniciada.

\* \*\*Invariante 4:\*\* Uma aula `NOT\_ATTENDED` não pode ser iniciada.

\* \*\*Invariante 5:\*\* Uma aula concluída não pode ser modificada para um estado anterior.



\---



\## 7. Check-in



O check-in possui como objetivo validar a presença do aluno antes do início da aula.

O processo utiliza um token temporário gerado pelo backend e associado à aula. O frontend poderá representar esse token através de um QR Code apresentado pelo instrutor.

Fluxo principal:

```text
Instrutor
   ↓
Inicia processo de check-in
   ↓
Backend gera token temporário
   ↓
Token possui validade de 15 minutos
   ↓
Frontend representa o token em QR Code
   ↓
Aluno escaneia o QR Code
   ↓
Frontend envia o token ao backend
   ↓
Backend valida token e aluno autenticado
   ↓
Presença registrada
   ↓
Token invalidado
   ↓
IN_PROGRESS


```



> \*\*Atenção:\*\* O token representado pelo QR Code possui validade limitada para reduzir riscos de reutilização.



\---



\## 8. Encerramento



Após a conclusão da aula:



```text

IN\_PROGRESS

&#x20;   ↓

COMPLETED



```



O sistema deverá registrar:



\* Data/hora de início.

\* Data/hora do check-in.

\* Data/hora de encerramento.

\* Instrutor.

\* Aluno.

