using UnityEngine;
using UnityEngine.InputSystem;

namespace DownhillDuo.Vehicle
{
    /// <summary>
    /// Protótipo: 1 jogador controla tudo. As ações são criadas em código para não depender de
    /// um asset .inputactions. Quando entrar o co-op local, trocar por um asset com 2 esquemas
    /// (um por piloto) e manter o VehicleInput como contrato.
    /// </summary>
    public class PlayerInputProvider : MonoBehaviour
    {
        [SerializeField] VehicleController vehicle;
        [SerializeField, Tooltip("Opcional: se atribuído, controla o lean lateral do piloto de trás.")]
        AutoCopilot autoCopilot;

        InputAction steer, brake, frontLean, rearLean;

        void Awake()
        {
            steer = new InputAction("Steer", InputActionType.Value);
            steer.AddCompositeBinding("1DAxis")
                 .With("Negative", "<Keyboard>/a")
                 .With("Positive", "<Keyboard>/d");
            steer.AddBinding("<Gamepad>/leftStick/x");

            brake = new InputAction("Brake", InputActionType.Value);
            brake.AddBinding("<Keyboard>/space");
            brake.AddBinding("<Gamepad>/rightTrigger");

            frontLean = new InputAction("FrontLean", InputActionType.Value);
            frontLean.AddCompositeBinding("1DAxis")
                     .With("Negative", "<Keyboard>/s")
                     .With("Positive", "<Keyboard>/w");
            frontLean.AddBinding("<Gamepad>/leftStick/y");

            rearLean = new InputAction("RearLean", InputActionType.Value);
            rearLean.AddCompositeBinding("2DVector")
                    .With("Up", "<Keyboard>/i")
                    .With("Down", "<Keyboard>/k")
                    .With("Left", "<Keyboard>/j")
                    .With("Right", "<Keyboard>/l");
            rearLean.AddBinding("<Gamepad>/rightStick");
        }

        void OnEnable()
        {
            steer.Enable(); brake.Enable(); frontLean.Enable(); rearLean.Enable();
        }

        void OnDisable()
        {
            steer.Disable(); brake.Disable(); frontLean.Disable(); rearLean.Disable();
        }

        void OnDestroy()
        {
            steer.Dispose(); brake.Dispose(); frontLean.Dispose(); rearLean.Dispose();
        }

        void Update()
        {
            Vector2 rear = rearLean.ReadValue<Vector2>();
            if (autoCopilot != null) rear.x = autoCopilot.LeanX;

            vehicle.Command = new VehicleInput
            {
                steer = steer.ReadValue<float>(),
                brake = brake.ReadValue<float>(),
                frontLean = new Vector2(0f, frontLean.ReadValue<float>()),
                rearLean = rear
            };
        }
    }
}
