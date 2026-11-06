# AGENTS.md

Instruções para agentes de IA (Claude, Copilot, Cursor, Codex, etc.) e para quem contribuir. Leia antes de mexer em qualquer coisa.

## 1. Missão

Simulador de corrida de gravidade para **dois pilotos** num veículo de metal sem motor (rodas pequenas à frente, grandes atrás). A mecânica central é o **peso da tripulação**: ambos inclinam frente/trás (carga por eixo e aerodinâmica) e o piloto de trás inclina também para os lados para segurar as curvas. Cenário de referência: descidas entre **~3900 m e ~2000 m de altitude**, onde a densidade do ar muda de forma relevante. Alvo: **PC primeiro**, depois consola (PS4 a verificar, ver secção 8). Publicação via parceiro (publisher ou empresa de port) depois de um protótipo jogável, ver `docs/05-publicacao.md`.

## 2. Premissas do dono do projeto (valem para qualquer decisão)

1. **Volume, velocidade e baixo custo operacional** são premissa, não otimização tardia. Poucos raycasts, sem alocações por frame, sem netcode, sem serviços pagos.
2. **Partir do problema original.** Ao corrigir um bug, reduzir ao caso mais simples que o reproduz. Não empilhar remendos: uma correção sobre um comportamento que ninguém entendeu cria um erro dentro de um erro.
3. **Pragmatismo.** Sem elogios vazios nem adulação. Humor e ironia leves são bem-vindos; floreio não.
4. O dono tem ~30 anos de desenvolvimento (C#, .NET, TypeScript, Delphi) e **nenhuma experiência prévia em Unity**. Não explique C#; explique conceitos do engine (ciclo de vida, `FixedUpdate`, Rigidbody, layers) quando forem relevantes, em poucas linhas.

## 3. Stack e convenções

- **Unity 6 LTS**, C#, URP. API de velocidade: `Rigidbody.linearVelocity` (em versões antigas seria `velocity`).
- **Input System** (pacote novo). O resto do código só conhece `VehicleInput`; nunca leia teclado/comando fora de `PlayerInputProvider`.
- **TextMeshPro** para HUD.
- Namespaces: `DownhillDuo.Vehicle`, `DownhillDuo.Telemetry`, `DownhillDuo.UI`. Um assembly: `DownhillDuo.Runtime`.
- Identificadores em inglês; comentários e documentação em português.
- Física só em `FixedUpdate`. Leitura de input e UI em `Update`.
- Fixed Timestep do projeto: **0.01 s**.
- Parâmetros tunáveis vão para `[SerializeField]`, nunca constantes enterradas no código. Afinar com o jogo a correr.
- Matemática pura (filtros, conversões, mapeamentos) em métodos estáticos fáceis de testar em EditMode.

## 4. Regras rígidas (não fazer sem pedir ao dono)

- **Não usar `WheelCollider`.** A decisão é raycast próprio (ver `docs/06-decisoes.md`, ADR-001).
- **Não criar ragdoll nem física articulada para os pilotos.** São massas pontuais.
- **Não mexer no `transform` do veículo** (nem `position`, nem `rotation`) fora do respawn. Só forças no `Rigidbody`.
- **Não introduzir netcode** (multiplayer online). Co-op é local; o contra-relógio usa ghost gravado.
- **Não construir pipeline de dados de percurso** antes do marco M7. Um percurso à mão primeiro.
- **Não descarregar, redistribuir nem embutir vídeos ou frames do YouTube.** Vídeo é referência visual e fonte de calibração, ver `docs/03-percursos.md`.
- **Não adicionar pacotes, serviços ou dependências** sem justificar o custo (build, licença, manutenção).
- **Não alterar `ProjectSettings/` sem avisar** o que mudou e porquê.
- **Não commitar** `Library/`, `Temp/`, `Logs/`, `UserSettings/`, `ReferenceMedia/`, `RawRouteData/` (já no `.gitignore`). **Nunca** ignorar ficheiros `.meta`.
- **Não tratar a densidade do ar como constante.** Ela vem de `Atmosphere.Density(altitude)`, e a altitude tem **uma única fonte** (`TelemetryCalculator.AltitudeM`), partilhada com o HUD.
- **Não alterar `LICENSE`, `NOTICE.md`, `COMMERCIAL.md` nem `CONTRIBUTING.md`** sem pedido explícito do dono. A licença do código é PolyForm Noncommercial; uso comercial exige licença separada.
- **Não introduzir código, arte, áudio ou dados de terceiros sem verificar a licença.** Proibido copiar código GPL/AGPL/CC BY-SA (inclui respostas de fóruns) ou de origem desconhecida: contamina a possibilidade de licenciar comercialmente. Registar a origem e a licença de qualquer exceção em `docs/06-decisoes.md`.
- **Não aceitar contribuição externa** sem os termos de `CONTRIBUTING.md`.
- **Não saltar marcos.** Ver `docs/04-marcos.md`.

## 5. Protocolo de depuração

1. Reproduza no cenário mais simples possível (uma rampa, uma roda, sem direção, sem lean).
2. Ative os gizmos (`OnDrawGizmosSelected` em `RaycastWheel` e `VehicleController`): raio, carga por roda e centro de massa dinâmico.
3. Altere **um** parâmetro de cada vez e registe o efeito.
4. Só depois de entender a causa, corrija. Se a correção exigir mais de ~20 linhas, pare e reveja a hipótese.
5. Tabela de sintomas comuns: `docs/01-fisica.md`, secção "Sintomas e causas prováveis".

## 6. Fluxo de trabalho

- O trabalho segue os marcos de `docs/04-marcos.md`. Cada marco tem **critério de saída** observável; só avance quando for cumprido.
- Ao concluir um marco, marque-o em `docs/04-marcos.md`.
- Decisão relevante (arquitetura, plataforma, licença, dependência) vira entrada em `docs/06-decisoes.md` com data e motivo.
- Mudança de comportamento da física ou do HUD atualiza o doc correspondente no mesmo commit.

## 7. Mapa do repositório

| Caminho | Conteúdo |
|---|---|
| `Assets/DownhillDuo/Scripts/Vehicle/` | `RaycastWheel`, `CrewMass`, `VehicleController`, `VehicleInput`, `PlayerInputProvider`, `AutoCopilot` |
| `Assets/DownhillDuo/Scripts/Telemetry/` | `TelemetryCalculator` (velocidade, altitude, pitch/roll, força g, distância) |
| `Assets/DownhillDuo/Scripts/World/` | `Atmosphere` (densidade do ar por altitude, matemática estática) |
| `Assets/DownhillDuo/Tests/EditMode/` | testes de matemática pura (`AtmosphereTests`) |
| `Assets/DownhillDuo/Scripts/UI/` | `HudView` |
| `docs/01-fisica.md` | modelo físico, parâmetros iniciais, depuração |
| `docs/02-hud-e-camaras.md` | HUD, mapa, olhar para trás |
| `docs/03-percursos.md` | como criar percursos, calibração, licenças |
| `docs/04-marcos.md` | roteiro e critérios de saída |
| `docs/05-publicacao.md` | estratégia de publicação e verificações de plataforma |
| `docs/06-decisoes.md` | registo de decisões |
| `LICENSE`, `NOTICE.md`, `COMMERCIAL.md`, `CONTRIBUTING.md` | licenciamento: código, documentação, conteúdo, uso comercial e contribuições |

## 8. Não assumir: verificar

- **Suporte a PS4 no Unity atual** e requisitos (subscrição Unity Pro, aprovação PlayStation Partners, certificação). A página atual da Unity cita PS5; PS4 não foi confirmado.
- Custos e condições de publishers/empresas de port.
- Licenças de qualquer dado de elevação ou mapa antes de partilhar builds.
- O código inicial **nunca foi compilado**. Se algo não compilar, corrija o código; não "contorne" a arquitetura.

## 9. Definição de pronto para uma mudança

- Compila sem avisos novos.
- Comportamento verificado no cenário mínimo (rampa) e descrito no commit.
- Sem alocações novas em `Update`/`FixedUpdate` (sem `new` de classes, LINQ, concatenação de strings).
- Documentação afetada atualizada.

## 10. Como falar com o dono

Português, direto, com os trade-offs de custo e complexidade à vista. Se existir uma solução simples e uma sofisticada, apresente a simples primeiro e diga o que a sofisticada compraria. Discorde quando houver razão.
