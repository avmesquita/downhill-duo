# Física do veículo

## Modelo

- **Chassis:** um `Rigidbody` com `Continuous Dynamic` e interpolação ligada.
- **Rodas:** 4 raycasts (`RaycastWheel`). Cada roda calcula mola + amortecedor, atrito lateral, rolamento e freio, e aplica a força no ponto de contacto. Raio por roda (0.12 m à frente, 0.20 m atrás).
- **Tripulação:** `CrewMass`. Piloto da frente e de trás são massas pontuais que deslizam numa pequena área. O centro de massa do `Rigidbody` é recalculado a cada passo (média ponderada de chassis + 2 pilotos).
- **Aerodinâmica:** arrasto `½ ρ Cd A v²`. A área frontal é interpolada pelo `Tuck01` (0 = eretos, 1 = agachados) e **ρ depende da altitude** (atmosfera ISA, ver secção abaixo).
- **Freio:** força contrária ao rolamento por roda, limitada por `μ·carga`. Sem bloqueio de roda por enquanto (futuro: ao exceder o limite, passar a atrito de deslizamento).
- **Gravidade:** a do Unity. É o único "motor".

## Eixos do lean

`x` = lateral (+ direita), `y` = frente (+) / trás (-). Piloto da frente: só `y` (alcance lateral 0). Piloto de trás: `x` e `y`.

## Efeitos esperados do lean

| Movimento | Efeito físico | Onde aparece |
|---|---|---|
| Frente/trás | Muda a carga por eixo (aderência de direção vs estabilidade) | `CrewMass` → centro de massa → carga nas rodas |
| Agachar | Reduz área frontal | `Tuck01` → arrasto |
| Lateral (trás) | Devolve carga às rodas interiores em curva | centro de massa lateral |

## Parâmetros iniciais (chutes educados; afinar com o jogo a correr)

| Parâmetro | Valor | Nota |
|---|---|---|
| Massa chassis | 60 kg | `CrewMass` |
| Massa de cada piloto | 75 kg | total ≈ 210 kg |
| Mola | 18 000 N/m | ~500 N por roda → ~3 cm de compressão estática |
| Amortecedor | 900 N·s/m | ≈ 0,45 do crítico; subir se "salta", baixar se "afunda" |
| Atrito lateral μ | 1.0 | baixar para derrapar mais |
| Rolamento | 0.015 | metal em asfalto ≈ 0.01 a 0.02 |
| Freio máx. por roda | 450 N | |
| Cd · A | 0.9 · (1.1 a 0.65 m²) | |
| Densidade do ar ρ | calculada: ≈ 0,83 kg/m³ a 3900 m, ≈ 1,01 kg/m³ a 2000 m | `Atmosphere.Density(altitude)`; não é parâmetro fixo |
| Desvio de temperatura | 0 °C | `isaTemperatureOffsetC`; negativo = ar mais denso |
| Fixed Timestep | 0.01 s | |

## Altitude e densidade do ar

O percurso de referência desce de **~3900 m para ~2000 m**. O ar é mais rarefeito no topo e vai ficando mais denso à medida que se desce, logo o arrasto, para a mesma velocidade e postura, **aumenta ao longo da corrida**.

Densidade pela atmosfera padrão ISA (`Atmosphere.Density`, matemática estática testada em `Tests/EditMode`):

| Altitude | ρ (kg/m³) | Arrasto relativo (mesma v e postura) |
|---|---|---|
| 3900 m | 0,828 | 1,00 |
| 3000 m | 0,909 | 1,10 |
| 2000 m | 1,006 | 1,22 |

Leitura prática:

- Em baixo o arrasto é cerca de **22% maior** que no topo; a velocidade terminal teórica é cerca de **9% menor**. O efeito é real e perceptível numa descida longa, mas é **menor que o do `Tuck`** (área frontal 1,1 → 0,65 m² é uma redução de ~40%). Não o deixe mascarar um Cd mal calibrado.
- A altitude vem do `TelemetryCalculator` (`transform.position.y + altitudeAtWorldY0`) e a mesma fonte alimenta o HUD, para os dois nunca divergirem.
- Massa, rolamento e atrito **não** dependem da altitude.
- ISA é um dia "médio". O tempo real varia (temperatura, pressão do dia). `isaTemperatureOffsetC` cobre uma manhã fria de serra (ar mais denso); a variação de pressão do dia é de poucos por cento e fica fora de escopo.
- Ao calibrar com vídeo, use a densidade da **altitude do trecho filmado**, não a do topo.

## Sintomas e causas prováveis

| Sintoma | Causa provável |
|---|---|
| O carro afunda no chão ou fica "sentado" nos batentes | Mola fraca ou `springTravel` pequeno; massa errada |
| Salta ou vibra parado | Amortecedor baixo; `Fixed Timestep` alto |
| Parado, treme de um lado para o outro | Atrito lateral instável: baixar `lateralGrip` ou subir o passo de física |
| Desliza de lado como sabão | `lateralGrip` baixo, ou raycast a acertar no próprio carro (layer errada) |
| Não curva | `lateralGrip` alto demais ou ângulo de direção pequeno |
| Raycast "acerta" no veículo | `groundMask` inclui a layer **Vehicle** |
| Atravessa o chão em alta velocidade | Falta `Continuous Dynamic`, ou passo de física alto |
| Velocidade terminal absurda | Cd·A baixo ou rolamento baixo; calibrar com `docs/03-percursos.md` |
| O arrasto não muda ao descer | `VehicleController` sem `TelemetryCalculator` (cai em `fallbackAirDensity`) ou `altitudeAtWorldY0` mal definido; conferir `AirDensity` no Inspector em Debug |
| Capota "sem aviso" | Centro de massa alto; largura de via curta |
| Lean não tem efeito visível | `leanRange` pequeno; `response` baixo; massa do piloto baixa |

## Fora de escopo (por enquanto)

Fadiga de travão, deformação do chassis, vento, diferentes pisos com atrito variável, bloqueio de roda.
