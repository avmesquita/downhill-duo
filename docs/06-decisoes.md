# Registo de decisões

Formato: ID, decisão, motivo, consequência. Acrescentar entradas novas no fim, com data.

**ADR-001: Raycast próprio em vez de `WheelCollider`.**
Motivo: o `WheelCollider` foi pensado para carros com motor e tração e comporta-se de forma opaca num carro de gravidade com rodas de raios diferentes. Raycast dá controlo total da carga por roda, necessário para o efeito do lean.
Consequência: mais código inicial, menos luta contra uma caixa-preta.

**ADR-002: Tripulação como massas pontuais.**
Motivo: o efeito físico relevante é o deslocamento do centro de massa. Ragdoll seria caro, instável e desnecessário.
Consequência: dois pilotos = dois pontos com alcance e suavização.

**ADR-003: Copiloto automático por regra, não por replay de inputs.**
Motivo: repetir inputs gravados falha porque a trajetória nunca é igual à gravação. A regra `lean = k · g lateral` é determinística e quase gratuita.
Consequência: `AutoCopilot` substitui o segundo humano até existir co-op.

**ADR-004: PC primeiro, input abstrato.**
Motivo: validar a jogabilidade antes de gastar tempo com burocracia de consola.
Consequência: `VehicleInput` desacopla física e dispositivo; o port a consola não reescreve input.

**ADR-005: Percursos a partir de dados de elevação/mapa, vídeo só para calibrar.**
Motivo: reconstrução 3D a partir de vídeo de câmara única tem escala ambígua e deriva, e estima mal o declive. Reutilizar imagens de terceiros tem risco de direitos de autor.
Consequência: pipeline de dados só no M7; vídeos fora do repositório.

**ADR-006: Co-op local em vez de online.**
Motivo: física partilhada com latência é cara e frágil; local tem custo operacional zero.
Consequência: sem netcode. Contra-relógio com ghost gravado.

**ADR-007: Olhar para trás por troca de câmara.**
Motivo: retrovisor permanente com `RenderTexture` renderiza a cena duas vezes.
Consequência: custo zero fora do olhar.

**ADR-008: Publicação via parceiro depois de protótipo.**
Motivo: reduzir burocracia e focar na jogabilidade. Ver `05-publicacao.md`.
Consequência: o pitch exige build jogável, vídeo curto e folha de uma página.

**ADR-009: Densidade do ar dependente da altitude (atmosfera ISA).**
Contexto: o cenário real desce de ~3900 m para ~2000 m. O ar é mais rarefeito no topo; o arrasto sobe cerca de 22% ao longo da descida para a mesma velocidade e postura.
Decisão: `Atmosphere.Density(altitude, desvioDeTemperatura)` (função estática, testada em EditMode) alimenta o arrasto do `VehicleController`. A altitude vem do `TelemetryCalculator`, a mesma fonte do HUD.
Consequência: custo desprezível (uma potência por passo de física). Calibração de Cd deve usar a densidade da altitude do trecho de referência. O efeito é menor que o do `Tuck`; não substitui calibração.

**ADR-010: Repositório público com licença source-available (PolyForm Noncommercial) e uso comercial reservado.**
Contexto: o autor quer repositório público e algum controlo sobre a exploração do jogo, incluindo a possibilidade de integrar a equipa de quem queira produzi-lo. Licenças não protegem ideias nem impõem inclusão em equipas; só criam alavancagem.
Decisão: código sob PolyForm Noncommercial 1.0.0; documentação sob CC BY-NC 4.0 (atribuição); conteúdo, nome e logótipo com direitos reservados; uso comercial por licença separada (`COMMERCIAL.md`). Contribuições externas só com os termos de `CONTRIBUTING.md` (concessão que permite relicenciar), porque um DCO não basta.
Consequência: não é open source no sentido OSI (a restrição comercial a exclui), e a comunicação deve dizer *source-available*. Textos de contribuição e acordo comercial precisam de revisão jurídica. Opção descartada por ora: AGPL com licenciamento duplo (mais complexa).
