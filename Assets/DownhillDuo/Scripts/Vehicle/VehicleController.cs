using DownhillDuo.Telemetry;
using DownhillDuo.World;
using UnityEngine;

namespace DownhillDuo.Vehicle
{
    /// <summary>
    /// Orquestra um passo de física: tripulação → direção → rodas → arrasto.
    /// Regra: nada aqui mexe no transform do veículo; só forças no Rigidbody.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class VehicleController : MonoBehaviour
    {
        [Header("Componentes")]
        [SerializeField] RaycastWheel[] wheels;
        [SerializeField] CrewMass crew;
        [SerializeField] LayerMask groundMask = ~0; // NUNCA incluir a layer do próprio veículo

        [Header("Direção")]
        [SerializeField] float maxSteerAngle = 20f; // graus
        [SerializeField] float steerRate = 90f;     // graus/s

        [Header("Aerodinâmica (arrasto = ½ ρ Cd A v²; ρ depende da altitude)")]
        [SerializeField, Tooltip("Fonte da altitude. Se vazio, procura no mesmo GameObject.")]
        TelemetryCalculator telemetry;
        [SerializeField, Tooltip("Desvio de temperatura em relação à atmosfera padrão ISA (°C). Manhã fria na serra: valor negativo.")]
        float isaTemperatureOffsetC = 0f;
        [SerializeField, Tooltip("Densidade (kg/m³) usada só se não houver TelemetryCalculator.")]
        float fallbackAirDensity = 1.2f;
        [SerializeField] float dragCoefficient = 0.9f;
        [SerializeField] float frontalAreaUpright = 1.1f; // m², pilotos eretos
        [SerializeField] float frontalAreaTucked = 0.65f; // m², pilotos agachados

        /// <summary>Escrito por quem produz o input; lido no FixedUpdate.</summary>
        public VehicleInput Command;

        public Rigidbody Body { get; private set; }
        public float AirDensity { get; private set; } // kg/m³, para debug/HUD

        float steerAngle;

        void Awake()
        {
            Body = GetComponent<Rigidbody>();
            if (telemetry == null) telemetry = GetComponent<TelemetryCalculator>();
            Body.interpolation = RigidbodyInterpolation.Interpolate;
            Body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            crew.Init(Body);
            float share = wheels.Length > 0 ? crew.TotalMass / wheels.Length : crew.TotalMass;
            foreach (var w in wheels) w.Init(Body, share);
        }

        void FixedUpdate()
        {
            float dt = Time.fixedDeltaTime;

            crew.SetLean(Command.frontLean, Command.rearLean);
            crew.Step(Body, dt);

            float targetSteer = Mathf.Clamp(Command.steer, -1f, 1f) * maxSteerAngle;
            steerAngle = Mathf.MoveTowards(steerAngle, targetSteer, steerRate * dt);

            float brake = Mathf.Clamp01(Command.brake);
            foreach (var w in wheels)
            {
                w.SetSteerAngle(steerAngle);
                w.Simulate(brake, groundMask, dt);
            }

            ApplyDrag();
        }

        void ApplyDrag()
        {
            AirDensity = telemetry != null
                ? Atmosphere.Density(telemetry.AltitudeM, isaTemperatureOffsetC)
                : fallbackAirDensity;

            Vector3 v = Body.linearVelocity; // Unity 6. Em versões anteriores: Body.velocity
            float speed = v.magnitude;
            if (speed < 0.01f) return;

            float area = Mathf.Lerp(frontalAreaUpright, frontalAreaTucked, crew.Tuck01);
            float force = 0.5f * AirDensity * dragCoefficient * area * speed * speed;
            Body.AddForce(-v / speed * force);
        }

        void OnDrawGizmosSelected()
        {
            if (Body == null) return;
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(Body.worldCenterOfMass, 0.05f); // centro de massa dinâmico
        }
    }
}
