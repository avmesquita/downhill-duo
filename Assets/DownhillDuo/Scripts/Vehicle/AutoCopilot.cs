using DownhillDuo.Telemetry;
using UnityEngine;

namespace DownhillDuo.Vehicle
{
    /// <summary>
    /// Copiloto de trás automático (substitui o segundo jogador humano e o "ghost de copiloto").
    /// Regra deliberadamente simples: inclinar para dentro da curva proporcionalmente à força g lateral.
    /// Curva à direita → GLocal.x positivo → lean positivo (direita = dentro da curva).
    /// Repetir inputs gravados NÃO funciona aqui: a trajetória nunca é igual à da gravação.
    /// </summary>
    public class AutoCopilot : MonoBehaviour
    {
        [SerializeField] TelemetryCalculator telemetry;
        [SerializeField, Tooltip("Lean (-1..1) por g lateral.")] float gain = 1.2f;

        public float LeanX => Mathf.Clamp(gain * telemetry.GLocal.x, -1f, 1f);
    }
}
