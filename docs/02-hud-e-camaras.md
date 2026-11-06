# HUD, mapa e câmaras

## Valores do HUD

Todos calculados no `FixedUpdate` por `TelemetryCalculator`; o `HudView` só lê.

| Valor | Cálculo | Nota |
|---|---|---|
| Velocidade | `|v| · 3.6` (km/h) | |
| Inclinação (pitch/roll) | `asin(forward.y)` e `asin(-right.y)` | Evita o salto 0°/360° dos ângulos de Euler. Aproximação suficiente para HUD. Pitch + = nariz para cima; roll + = inclinado para a direita |
| Força g | `(Δv/Δt − g) / 9.81`, no espaço local | "Força específica" (o que um acelerómetro mede). Parado: `(0, 1, 0)`. Curva à direita: `x > 0`. Filtro passa-baixo (`gFilterTau` ≈ 0,15 s) |
| Distância | `∫ |v| dt` | Com spline de percurso, pode vir do progresso na spline |
| Altitude | `transform.position.y + altitudeAtWorldY0` (m) | Mesma fonte da densidade do ar (ver `01-fisica.md`). Mostrada em metros inteiros. Percurso com altitudes reais como Y: offset 0 |

## Custo

- HUD atualiza a ~12 Hz, não a cada frame.
- `TMP_Text.SetText("{0:1}", valor)` formata sem alocar strings novas.
- Sem `string.Format`, interpolação ou concatenação em `Update`.

## Mapa

Duas opções, da mais barata para a mais cara:

1. **Spline 2D na UI:** projetar a spline do percurso no plano XZ e desenhá-la num painel (linha), com um marcador para o carro. Sem câmara extra.
2. **Câmara ortográfica de cima → `RenderTexture`:** mais flexível (mostra terreno), mas renderiza a cena mais uma vez.

Comece pela opção 1.

## Olhar para trás

- Segurar um botão troca para uma câmara virada para trás (Cinemachine: mudança de prioridade). Custo zero fora do momento do olhar.
- **Evitar retrovisor permanente com `RenderTexture`**: renderiza a cena duas vezes, caro em consola.
- Co-op local com **split-screen** também duplica renderização. Preferir câmara única com o piloto de trás a poder olhar para trás por botão.
