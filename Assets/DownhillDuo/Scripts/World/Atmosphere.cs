using UnityEngine;

namespace DownhillDuo.World
{
    /// <summary>
    /// Atmosfera padrão ISA (troposfera, até ~11 km). Matemática pura e estática: testável em EditMode.
    /// Densidade do ar = pressão / (R · T). A pressão cai com a altitude; a temperatura também.
    /// O desvio de temperatura permite simular um dia mais frio (ar mais denso) ou mais quente.
    /// </summary>
    public static class Atmosphere
    {
        const float SeaLevelPressure = 101325f;    // Pa
        const float SeaLevelTemperature = 288.15f; // K
        const float LapseRate = 0.0065f;           // K/m
        const float GasConstantAir = 287.05f;      // J/(kg·K)
        const float PressureExponent = 5.25588f;   // g·M/(R·L)

        /// <param name="altitudeMeters">Altitude acima do nível do mar (m).</param>
        /// <param name="temperatureOffsetC">Desvio em relação à atmosfera padrão (°C). Negativo = mais frio = mais denso.</param>
        /// <returns>Densidade do ar em kg/m³.</returns>
        public static float Density(float altitudeMeters, float temperatureOffsetC = 0f)
        {
            float h = Mathf.Clamp(altitudeMeters, -500f, 11000f);
            float pressure = SeaLevelPressure * Mathf.Pow(1f - LapseRate * h / SeaLevelTemperature, PressureExponent);
            float temperature = SeaLevelTemperature - LapseRate * h + temperatureOffsetC;
            return pressure / (GasConstantAir * temperature);
        }
    }
}
