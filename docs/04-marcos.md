# Marcos e critérios de saída

Cada marco só termina quando o critério é cumprido. Marque `[x]` ao concluir.

- [ ] **M0: Ambiente.** Unity 6 LTS, Input System, TMP, repositório Git com `.gitignore`. *Saída:* projeto abre sem erros; scripts compilam.
- [ ] **M1: Rampa + caixa.** Cube com `Rigidbody` a deslizar numa rampa. *Saída:* entendo massa, atrito, `FixedUpdate` e layers.
- [ ] **M2: Chassis com 4 raycasts.** `RaycastWheel` x4, rodas de raios diferentes. *Saída:* assenta no chão sem saltar nem afundar; nivelado em plano.
- [ ] **M3: Rolamento e atrito lateral.** *Saída:* desce a rampa em linha reta e curva sem derrapar como sabão nem recusar curvar.
- [ ] **M4: Arrasto + gravidade + densidade por altitude.** `Atmosphere` e testes de EditMode a passar. *Saída:* velocidade terminal credível numa descida longa e `AirDensity` coerente com a altitude (≈ 0,83 a 3900 m; ≈ 1,01 a 2000 m).
- [ ] **M5: Direção e freio.** *Saída:* curva por ângulo limitado; freio progressivo (gatilho analógico) para o carro numa descida.
- [ ] **M6: Lean.** Primeiro só lateral (piloto de trás), depois frente/trás e `Tuck`. *Saída:* numa curva forte o carro levanta rodas interiores e o lean o salva (**este é o teste da ideia do jogo**).
- [ ] **M6b: Copiloto automático** (`AutoCopilot`). *Saída:* um jogador consegue completar curvas controlando só o piloto da frente.
- [ ] **M7: HUD, olhar para trás, primeiro percurso.** Velocidade, altitude, inclinação, força g, km, mapa; câmara traseira; um percurso de 1 a 3 km. *Saída:* uma descida completa jogável com HUD legível.
- [ ] **M8: Co-op local + ghost de contra-relógio.** *Saída:* 2 jogadores, 1 por piloto; ghost de volta anterior.
- [ ] **M9: Build para o pitch.** Build de PC estável, vídeo de 30 a 60 s, folha de uma página, devlog público. *Saída:* kit de pitch pronto (ver `05-publicacao.md`).

## Regra de ouro

Se o protótipo com cubos cinzentos **não** for divertido no M6, nenhum cenário bonito o salva. Pare e reavalie a mecânica antes de seguir.
