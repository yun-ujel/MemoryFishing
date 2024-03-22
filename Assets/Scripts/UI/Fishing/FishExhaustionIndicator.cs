using UnityEngine;

using MemoryFishing.Gameplay.Fishing.Player;
using UnityEngine.UI;
using MemoryFishing.Gameplay.Fishing.Player.EventArgs;

namespace MemoryFishing.UI.Fishing
{
    public class FishExhaustionIndicator : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerFishingManager fishingManager;
        [SerializeField] private ReelingController reelingController;

        [Header("Display")]
        [SerializeField] private Image radialSprite;

        [Space]

        [SerializeField] private Color circleColorMin;
        [SerializeField] private Color circleColorMax;

        private bool reeling;

        private void Start()
        {
            fishingManager.OnStartReelingEvent += StartReeling;
            fishingManager.OnEndReelingEvent += EndReeling;

            fishingManager.OnCatchFishEvent += Hide;

            SetFillAmount(0);
            radialSprite.color = circleColorMin;
        }

        private void Hide(object sender, OnCatchFishEventArgs args)
        {
            SetFillAmount(0);
            radialSprite.color = circleColorMin;
        }

        private void StartReeling(object sender, OnStartReelingEventArgs args)
        {
            reeling = true;

            SetFillAmount(0);
            radialSprite.color = circleColorMin;
        }

        private void EndReeling(object sender, OnEndReelingEventArgs args)
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