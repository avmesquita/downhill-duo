using UnityEngine;

namespace DownhillDuo.Vehicle
{
    /// <summary>
    /// Comando abstrato do veículo. Quem produz (teclado, comando, IA, replay de ghost)
    /// não sabe nada da física; quem consome (VehicleController) não sabe nada do dispositivo.
    /// </summary>
    [System.Serializable]
    public struct VehicleInput
    {
        [Range(-1f, 1f)] public float steer;   // -1 esquerda, +1 direita
        [Range(0f, 1f)] public float brake;    // 0 solto, 1 freio total
        public Vector2 frontLean;              // x lateral, y frente(+)/trás(-), piloto da frente
        public Vector2 rearLean;               // x lateral (+ = direita), y frente(+)/trás(-), piloto de trás
    }
}
