# Downhill Duo (nome de trabalho)

Simulador de corrida de gravidade para dois pilotos, inspirado nos coches de metal das descidas andinas: sem motor, rodas pequenas à frente e grandes atrás. A graça está no **peso da tripulação**: os pilotos inclinam o corpo para a frente e para trás (aerodinâmica e distribuição de carga) e o piloto de trás também inclina para os lados para segurar o carro nas curvas. A descida vai de ~3900 m a ~2000 m, e o ar mais denso lá em baixo aumenta o arrasto.

**Estado:** pré-protótipo. Os scripts foram escritos sem editor Unity à mão e **não foram compilados nem testados**. Trate-os como ponto de partida e corrija o que o compilador reclamar. Os valores de física são chutes educados, não verdades.

## Princípios

1. Volume, velocidade e baixo custo operacional são premissa (poucos raycasts, sem netcode, HUD a baixa frequência).
2. Partir do problema original. Nada de geringonças para casos simples.
3. Protótipo antes de pipeline, pipeline antes de polimento.

## Início rápido (cerca de 15 minutos)

1. Unity Hub → **Unity 6 LTS** → template **3D (URP)**, num projeto novo.
2. Package Manager: instale **Input System** (aceite reiniciar o editor) e importe **TextMeshPro Essentials** (`Window > TextMeshPro > Import TMP Essential Resources`).
3. `Edit > Project Settings > Player > Active Input Handling` = **Input System Package (New)**.
4. `Edit > Project Settings > Time > Fixed Timestep` = **0.01**.
5. Copie para o projeto: `Assets/DownhillDuo/`, `docs/`, `AGENTS.md`, `CLAUDE.md`, `.github/` e `.gitignore`. Depois `git init`.
6. Crie as layers **Ground** e **Vehicle** (`Edit > Project Settings > Tags and Layers`).
7. Monte a cena mínima abaixo e carregue em Play.

### Cena mínima (marcos M1 a M3)

- **Rampa:** Cube, scale `(6, 0.5, 60)`, rotação X = `15°`, layer **Ground**.
- **Car** (GameObject vazio, layer **Vehicle**, posição acima da rampa): `Rigidbody`, `BoxCollider` (size `1, 0.3, 2`, center `0, 0.35, 0`), `VehicleController`, `CrewMass`, `TelemetryCalculator`, `PlayerInputProvider`.
- **4 filhos de `Car`**, cada um com `RaycastWheel`:

| Roda | Posição local | Raio | Steerable |
|---|---|---|---|
| Frente esq. | `(-0.45, 0.35, 0.75)` | 0.12 | sim |
| Frente dir. | `(0.45, 0.35, 0.75)` | 0.12 | sim |
| Trás esq. | `(-0.45, 0.43, -0.75)` | 0.20 | não |
| Trás dir. | `(0.45, 0.43, -0.75)` | 0.20 | não |

  As rodas traseiras ficam 0,08 m mais altas porque o raio é 0,08 m maior; assim o chassis assenta nivelado.
- No `VehicleController`: arraste as 4 rodas para `wheels`, o `CrewMass` para `crew` e defina `groundMask` = **Ground** (nunca inclua **Vehicle**, senão o raycast acerta no próprio carro).
- No `PlayerInputProvider`: arraste o `VehicleController` (e, opcionalmente, um `AutoCopilot`).
- No `TelemetryCalculator`: `altitudeAtWorldY0` define a altitude correspondente a Y = 0. Para a rampa de teste, ajuste para que o topo fique perto de 3900 m (a rampa é curta, então o efeito da altitude é pequeno; para vê-lo, compare dois valores).
- HUD (M7): Canvas com 7 `TMP_Text` (velocidade, pitch, roll, g lateral, g longitudinal, distância, altitude) e um `HudView`.

### Controlos de protótipo (1 jogador a controlar tudo)

| Ação | Teclado | Comando |
|---|---|---|
| Direção | A / D | Stick esquerdo X |
| Freio | Espaço | Gatilho direito |
| Piloto da frente: frente/trás | W / S | Stick esquerdo Y |
| Piloto de trás: lateral e frente/trás | J L / I K | Stick direito |

## Estrutura

```
AGENTS.md                      regras para agentes e para quem for contribuir
docs/                          física, HUD, percursos, marcos, publicação, decisões
Assets/DownhillDuo/Scripts/
  Vehicle/                     rodas por raycast, tripulação, controlador, input
  Telemetry/                   velocidade, altitude, inclinação, força g, distância
  World/                       atmosfera (densidade do ar por altitude)
Assets/DownhillDuo/Tests/      testes de EditMode (matemática pura)
  UI/                          HUD
```

## Licença

- **Código:** [PolyForm Noncommercial 1.0.0](LICENSE). Gratuito para uso pessoal e não comercial. **Isto é *source-available*, não open source no sentido estrito da OSI.**
- **Uso comercial:** requer licença separada, ver [COMMERCIAL.md](COMMERCIAL.md).
- **Documentação:** CC BY-NC 4.0. **Conteúdo do jogo, nome e logótipo:** direitos reservados. Detalhes em [NOTICE.md](NOTICE.md).
- **Contribuições:** ver [CONTRIBUTING.md](CONTRIBUTING.md).

## Documentos

- [Física do veículo](docs/01-fisica.md)
- [HUD, mapa e câmaras](docs/02-hud-e-camaras.md)
- [Percursos](docs/03-percursos.md)
- [Marcos e critérios](docs/04-marcos.md)
- [Estratégia de publicação](docs/05-publicacao.md)
- [Registo de decisões](docs/06-decisoes.md)
