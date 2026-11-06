using UnityEngine;

namespace DownhillDuo.Vehicle
{
    /// <summary>
    /// Roda simulada por raycast (sem WheelCollider): mola + amortecedor,
    /// atrito lateral, resistência ao rolamento e freio.
    /// O transform desta roda é o ponto de montagem da suspensão (topo da mola).
    /// Todas as forças são aplicadas no ponto de contacto do Rigidbody.
    /// </summary>
    public class RaycastWheel : MonoBehaviour
    {
        [Header("Geometria")]
        [SerializeField] float radius = 0.15f;
        [SerializeField] float maxSpringLength = 0.12f; // suspensão totalmente estendida (m)
        [SerializeField] float springTravel = 0.06f;    // curso de compressão (m)
        [SerializeField] bool steerable;
        [SerializeField] Transform visual;              // opcional: malha da roda (NÃO filha desta roda)

        [Header("Suspensão")]
        [SerializeField] float springRate = 18000f;     // N/m
        [SerializeField] float damperRate = 900f;       // N·s/m

        [Header("Atrito")]
        [SerializeField] float lateralGrip = 1.0f;      // μ lateral
        [SerializeField] float longitudinalGrip = 0.9f; // μ longitudinal (limita o freio)
        [SerializeField] float rollingResistance = 0.015f;
        [SerializeField] float maxBrakeForce = 450f;    // N nesta roda com freio a 100%

        public bool Grounded { get; private set; }
        public float Load { get; private set; }         // força normal (N), para debug/HUD

        Rigidbody body;
        float effectiveMass;
        float lastSpringLength;
        float spinAngle;

        /// <param name="rb">Rigidbody do veículo.</param>
        /// <param name="massShare">Massa (kg) que esta roda "segura" para o atrito lateral. Ex.: massa total / nº de rodas.</param>
        public void Init(Rigidbody rb, float massShare)
        {
            body = rb;
            effectiveMass = massShare;
            lastSpringLength = maxSpringLength;
        }

        public void SetSteerAngle(float degrees)
        {
            if (steerable) transform.localRotation = Quaternion.Euler(0f, degrees, 0f);
        }

        public void Simulate(float brake01, LayerMask groundMask, float dt)
        {
            float minSpringLength = maxSpringLength - springTravel;
            float rayLength = maxSpringLength + radius;

            if (!Physics.Raycast(transform.position, -transform.up, out RaycastHit hit,
                    rayLength, groundMask, QueryTriggerInteraction.Ignore))
            {
                Grounded = false;
                Load = 0f;
                lastSpringLength = maxSpringLength;
                UpdateVisual(maxSpringLength, 0f, dt);
                return;
            }

            Grounded = true;

            // 1) Suspensão
            float springLength = Mathf.Clamp(hit.distance - radius, minSpringLength, maxSpringLength);
            float compression = maxSpringLength - springLength;
            float compressionSpeed = (lastSpringLength - springLength) / dt;
            lastSpringLength = springLength;

            float normalForce = Mathf.Max(0f, springRate * compression + damperRate * compressionSpeed);
            Load = normalForce;
            Vector3 force = transform.up * normalForce;

            // 2) Eixos no plano do chão
            Vector3 n = hit.normal;
            Vector3 fwd = Vector3.ProjectOnPlane(transform.forward, n).normalized;
            Vector3 right = Vector3.Cross(n, fwd);
            Vector3 v = body.GetPointVelocity(hit.point);
            float vLong = Vector3.Dot(v, fwd);
            float vLat = Vector3.Dot(v, right);

            // 3) Atrito lateral: tenta anular a velocidade lateral, limitado pelo atrito disponível
            float maxLat = lateralGrip * normalForce;
            float fLat = Mathf.Clamp(-vLat * effectiveMass / dt, -maxLat, maxLat);

            // 4) Longitudinal: rolamento + freio, sempre contra o movimento
            float dir = Mathf.Clamp(vLong / 0.5f, -1f, 1f); // suaviza perto de zero (evita tremor)
            float desired = -dir * (rollingResistance * normalForce + brake01 * maxBrakeForce);
            float maxLong = longitudinalGrip * normalForce;
            float fLong = Mathf.Clamp(desired, -maxLong, maxLong);

            force += fwd * fLong + right * fLat;
            body.AddForceAtPosition(force, hit.point);

            UpdateVisual(springLength, vLong, dt);
        }

        void UpdateVisual(float springLength, float vLong, float dt)
        {
            if (visual == null) return;
            spinAngle += vLong / Mathf.Max(radius, 0.01f) * Mathf.Rad2Deg * dt;
            visual.position = transform.position - transform.up * springLength;
            visual.rotation = transform.rotation * Quaternion.Euler(spinAngle, 0f, 0f);
        }

        void OnDrawGizmosSelected()
        {
            Vector3 top = transform.position;
            Vector3 center = top - transform.up * maxSpringLength;
            Gizmos.color = Grounded ? Color.green : Color.yellow;
            Gizmos.DrawLine(top, center);
            Gizmos.DrawWireSphere(center, radius);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(top, top + transform.up * (Load * 0.0005f)); // 1 m = 2000 N
        }
    }
}
