using UnityEngine;

namespace DownhillDuo.Telemetry
{
    /// <summary>
    /// Deriva os valores do HUD a partir do Rigidbody. Calcula no FixedUpdate;
    /// o HUD só lê (e a uma frequência baixa).
    /// Convenção da força g: "força específica" (o que um acelerómetro mediria), em g,
    /// no espaço local do veículo: x = direita, y = cima, z = frente.
    /// Parado no plano: (0, +1, 0). Curva à direita: x positivo.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class TelemetryCalculator : MonoBehaviour
    {
        [SerializeField, Tooltip("Constante de tempo do filtro da força g (s). Maior = ponteiro mais calmo e mais atrasado.")]
        float gFilterTau = 0.15f;

        [SerializeField, Tooltip("Altitude (m) que corresponde a Y = 0 no mundo. Percurso com altitudes reais como Y: deixar 0. Cena de teste: ajustar para que o topo da rampa fique perto de 3900 m.")]
        float altitudeAtWorldY0 = 0f;

        public float SpeedKmh { get; private set; }
        public float PitchDeg { get; private set; }   // + nariz para cima
        public float RollDeg { get; private set; }    // + inclinado para a direita
        public Vector3 GLocal { get; private set; }   // em g
        public float DistanceKm { get; private set; }
        public float AltitudeM { get; private set; }  // acima do nível do mar; alimenta o HUD e a densidade do ar

        Rigidbody rb;
        Vector3 lastVelocity;
        float distanceMeters;

        const float G = 9.80665f;

        void Awake()
        {
            rb = GetComponent<Rigidbody>();
            AltitudeM = transform.position.y + altitudeAtWorldY0; // valor válido antes do 1.º FixedUpdate
        }

        void FixedUpdate()
        {
            float dt = Time.fixedDeltaTime;
            Vector3 v = rb.linearVelocity; // Unity 6. Em versões anteriores: rb.velocity

            // Força g (específica): (a - g) / 9.81, filtrada
            Vector3 a = (v - lastVelocity) / dt;
            lastVelocity = v;
            Vector3 local = transform.InverseTransformDirection(a - Physics.gravity) / G;
            GLocal = Vector3.Lerp(GLocal, local, 1f - Mathf.Exp(-dt / gFilterTau));

            // Velocidade e distância
            float speed = v.magnitude;
            SpeedKmh = speed * 3.6f;
            distanceMeters += speed * dt;
            DistanceKm = distanceMeters / 1000f;

            AltitudeM = transform.position.y + altitudeAtWorldY0;

            // Inclinação (aproximação suficiente para HUD; evita o salto 0/360 dos ângulos de Euler)
            PitchDeg = Mathf.Asin(Mathf.Clamp(transform.forward.y, -1f, 1f)) * Mathf.Rad2Deg;
            RollDeg = Mathf.Asin(Mathf.Clamp(-transform.right.y, -1f, 1f)) * Mathf.Rad2Deg;
        }

        public void ResetTrip()
        {
            distanceMeters = 0f;
            DistanceKm = 0f;
        }
    }
}
