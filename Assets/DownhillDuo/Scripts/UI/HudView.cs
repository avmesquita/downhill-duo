using DownhillDuo.Telemetry;
using TMPro;
using UnityEngine;

namespace DownhillDuo.UI
{
    /// <summary>
    /// HUD de texto. Atualiza a ~12 Hz (não a cada frame) e usa TMP_Text.SetText com
    /// formatação numérica nativa do TMP, que não aloca strings novas (menos lixo na consola).
    /// Formato TMP: "{0:1}" = 1 casa decimal.
    /// Todos os campos de texto precisam estar atribuídos no Inspector.
    /// </summary>
    public class HudView : MonoBehaviour
    {
        [SerializeField] TelemetryCalculator telemetry;
        [SerializeField] TMP_Text speedText;
        [SerializeField] TMP_Text pitchText;
        [SerializeField] TMP_Text rollText;
        [SerializeField] TMP_Text gLateralText;
        [SerializeField] TMP_Text gLongitudinalText;
        [SerializeField] TMP_Text distanceText;
        [SerializeField] TMP_Text altitudeText;
        [SerializeField] float refreshHz = 12f;

        float nextRefresh;

        void Update()
        {
            if (Time.unscaledTime < nextRefresh) return;
            nextRefresh = Time.unscaledTime + 1f / refreshHz;

            speedText.SetText("{0:0} km/h", telemetry.SpeedKmh);
            pitchText.SetText("Pitch {0:1}°", telemetry.PitchDeg);
            rollText.SetText("Roll {0:1}°", telemetry.RollDeg);
            gLateralText.SetText("G lat {0:2}", telemetry.GLocal.x);
            gLongitudinalText.SetText("G lon {0:2}", telemetry.GLocal.z);
            distanceText.SetText("{0:2} km", telemetry.DistanceKm);
            altitudeText.SetText("{0:0} m", telemetry.AltitudeM);
        }
    }
}
