# Percursos

## Princípio

Para um carro de gravidade, o que decide o jogo é a **inclinação da estrada** e a **geometria das curvas**. Os dois vêm melhor de dados de elevação e mapa do que de vídeo.

A elevação também é **entrada da física**: o percurso de referência desce de ~3900 m para ~2000 m e a densidade do ar (logo o arrasto) acompanha a altitude. Por isso o Y do mundo deve carregar a altitude real (offset 0), ou o offset deve ser definido em `TelemetryCalculator.altitudeAtWorldY0`.

## Pipeline recomendado (só a partir do marco M7)

1. Escolher o trecho.
2. Traçar a linha central da estrada em mapa (OpenStreetMap).
3. Amostrar a elevação ao longo da linha (modelo digital de terreno aberto, ex.: Copernicus ou SRTM).
4. Suavizar a altimetria (dados de 30 m são grosseiros e ruidosos; sem suavização o perfil de declive mente).
5. Converter latitude/longitude para metros locais (projeção local, ex.: UTM ou equirretangular).
6. Importar como spline no Unity e gerar a malha da estrada ao longo dela.
7. Terreno em volta: heightmap do mesmo DEM, ou simples (planos + rochas) para o primeiro percurso.

Limitações a aceitar: curvas muito fechadas (grampos) perdem detalhe nos dados abertos; podem precisar de ajuste manual.

## Os vídeos do YouTube: para quê servem

**Servem para:**

- **Calibrar a física.** Se o vídeo permite estimar a velocidade (marcos no percurso, sobreposição de GPS, contagem de tempo entre referências), compare com o carro virtual no mesmo trecho. Se o virtual chega a 90 km/h onde o real fez 60, o arrasto ou o rolamento estão errados. Compare no **mesmo trecho e altitude**: a densidade do ar muda ao longo da descida.
- **Referência visual para a arte** (vegetação, piso, cores), feita à mão.

**Não servem para (nem usar):**

- Reconstruir a estrada 3D por structure-from-motion: com câmara única a escala é ambígua e a deriva acumula, e o declive é justamente o que sai pior.
- Descarregar, redistribuir ou embutir vídeo/frames no jogo ou no repositório (termos do YouTube e direitos de autor). Material de referência fica fora do repositório (`/ReferenceMedia/` está no `.gitignore`).

## Licenças e atribuição

- OpenStreetMap exige atribuição e tem condições próprias de licença de base de dados. Confirmar antes de distribuir.
- Cada fonte de elevação tem a sua licença. Registar fonte, versão e licença em `docs/06-decisoes.md` ao adotar uma.
- Antes de enviar um build a terceiros (publisher/port), conferir estas licenças.

## Escopo inicial

**Um percurso**, de 1 a 3 km, com 2 ou 3 curvas desafiantes e uma reta de velocidade. Um trecho de poucos km perde só algumas dezenas a poucas centenas de metros de altitude; o efeito completo da densidade (3900 → 2000 m) só aparece num percurso longo. Para testar o efeito antes disso, mude `altitudeAtWorldY0` e compare. Só se faz o segundo quando o primeiro for divertido.
