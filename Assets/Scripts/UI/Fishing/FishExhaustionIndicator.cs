using UnityEngine;

using MemoryFishing.Gameplay.Fishing.Player;
using UnityEngine.UI;

namespace MemoryFishing.UI.Fishing
{
    public class FishExhaustionIndicator : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ReelingController reelingController;

        [Header("Display")]
        [SerializeField] private Image radialSprite;

        [Space]

        [SerializeField] private Color circleColorMin;
        [SerializeField] private Color circleColorMax;

        private bool reeling;

        private void Start()
        {
            reelingController.OnStartReelingEvent += StartReeling;
            reelingController.OnEndReelingEvent += EndReeling;

            SetFillAmount(0);
            radialSprite.color = circleColorMin;
        }

        private void StartReeling(object sender, ReelingController.OnStartReelingEventArgs args)
        {
            reeling = true;

            SetFillAmount(0);
            radialSprite.color = circleColorMin;
        }

        private void EndReeling(object sender, ReelingController.OnEndReelingEventArgs args)
        {
            reeling = false;

            SetFillAmount(1);
            radialSprite.color = circleColorMax;
        }

        private void Update()
        {
            if (reeling)
            {
                float exhaustion = reelingController.FishExhaustion;

                SetFillAmount(exhaustion);
                radialSprite.color = Color.Lerp(circleColorMin, circleColorMax, exhaustion);
            }
        }

        private void SetFillAmount(float value)
        {
            radialSprite.fillAmount = value;
        }
    }
}