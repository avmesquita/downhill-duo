using UnityEngine;

namespace DownhillDuo.Vehicle
{
    /// <summary>
    /// A tripulação como massas pontuais móveis. Sem ragdoll, sem animação:
    /// cada piloto desloca-se numa pequena área e o centro de massa do Rigidbody
    /// é recalculado a cada passo de física.
    /// Convenção de eixos do lean: x = lateral (+ direita), y = frente (+) / trás (-).
    /// </summary>
    public class CrewMass : MonoBehaviour
    {
        [System.Serializable]
        public class Pilot
        {
            public string label = "Piloto";
            public float mass = 75f;
            public Vector3 seatLocal;                              // posição neutra no espaço local do veículo
            public Vector2 leanRange = new Vector2(0.15f, 0.20f);  // alcance em metros: x lateral, y frente/trás
            [Range(0.5f, 20f)] public float response = 6f;         // maior = corpo responde mais depressa

            [HideInInspector] public Vector2 target;               // -1..1
            [HideInInspector] public Vector2 current;              // -1..1, suavizado

            public Vector3 LocalPosition =>
                seatLocal + new Vector3(current.x * leanRange.x, 0f, current.y * leanRange.y);
        }

        [SerializeField] float chassisMass = 60f;
        [SerializeField] Vector3 chassisCenterLocal = new Vector3(0f, 0.15f, 0f);
        [SerializeField] Pilot front = new Pilot
        {
            label = "Frente",
            seatLocal = new Vector3(0f, 0.35f, 0.35f),
            leanRange = new Vector2(0f, 0.20f)   // o da frente só inclina frente/trás
        };
        [SerializeField] Pilot rear = new Pilot
        {
            label = "Trás",
            seatLocal = new Vector3(0f, 0.35f, -0.45f)
        };

        /// <summary>0 = pilotos eretos (mais arrasto), 1 = ambos agachados à frente (menos arrasto).</summary>
        public float Tuck01 { get; private set; }
        public float TotalMass => chassisMass + front.mass + rear.mass;

        /// <summary>Define a massa do Rigidbody uma única vez (mudar massa a cada passo é desperdício).</summary>
        public void Init(Rigidbody rb)
        {
            rb.mass = TotalMass;
        }

        public void SetLean(Vector2 frontLean, Vector2 rearLean)
        {
            front.target = ClampLean(frontLean);
            rear.target = ClampLean(rearLean);
        }

        /// <summary>Chamar uma vez por FixedUpdate.</summary>
        public void Step(Rigidbody rb, float dt)
        {
            Smooth(front, dt);
            Smooth(rear, dt);

            Vector3 com = (chassisCenterLocal * chassisMass
                         + front.LocalPosition * front.mass
                         + rear.LocalPosition * rear.mass) / TotalMass;
            rb.centerOfMass = com;

            Tuck01 = (front.current.y + rear.current.y) * 0.25f + 0.5f; // média de -1..1 mapeada para 0..1
        }

        static Vector2 ClampLean(Vector2 v) =>
            new Vector2(Mathf.Clamp(v.x, -1f, 1f), Mathf.Clamp(v.y, -1f, 1f));

        static void Smooth(Pilot p, float dt)
        {
            float k = 1f - Mathf.Exp(-p.response * dt); // suavização exponencial independente do dt
            p.current = Vector2.Lerp(p.current, p.target, k);
        }
    }
}
