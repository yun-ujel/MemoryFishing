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
        [SerializeField] private FishFightController fightController;

        [Header("Display")]
        [SerializeField] private Image radialSprite;

        [Space]

        [SerializeField] private Color circleColorMin;
        [SerializeField] private Color circleColorMax;

        [Space]

        [SerializeField] private Color circleColorReeling;

        private bool fighting;
        private bool fishing;

        float t;

        private void Start()
        {
            fishingManager.OnStartFightingEvent += StartFighting;
            fishingManager.OnEndFightingEvent += EndFighting;

            fishingManager.OnCatchFishEvent += (sender, args) => Hide();
            fishingManager.OnDisableFishingEvent += (sender, args) => Hide();

            SetFillAmount(0);
            radialSprite.color = circleColorMin;
        }

        private void Hide()
        {
            fighting = false;
            fishing = false;
            radialSprite.color = circleColorMin;

            SetFillAmount(0);
        }

        private void StartFighting(object sender, OnStartFightingEventArgs args)
        {
            fighting = true;
            fishing = true;

            SetFillAmount(0);
            radialSprite.color = circleColorMin;
        }

        private void EndFighting(object sender, OnEndFightingEventArgs args)
        {
            fighting = false;

            SetFillAmount(1);
            radialSprite.color = circleColorMax;

            t = 0f;
        }

        private void Update()
        {
            if (fighting)
            {
                float exhaustion = fightController.FishExhaustion;

                SetFillAmount(exhaustion);
                radialSprite.color = Color.Lerp(circleColorMin, circleColorMax, exhaustion);
                
                return;
            }

            if (fishing)
            {
                t += Time.deltaTime;
                radialSprite.color = Color.Lerp(circleColorMax, circleColorReeling, t);

                return;
            }
        }

        private void SetFillAmount(float value)
        {
            radialSprite.fillAmount = value;
        }
    }
}