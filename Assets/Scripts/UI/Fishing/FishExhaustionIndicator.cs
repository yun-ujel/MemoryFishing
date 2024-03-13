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

        [SerializeField, Range(0, 1)] private float minSpriteAlpha = 0.2f;
        [SerializeField, Range(0, 1)] private float maxSpriteAlpha = 0.6f;

        private bool reeling;

        private float FillAmount
        {
            get => radialSprite.fillAmount;
            set => radialSprite.fillAmount = value;
        }

        private float Alpha
        {
            get => radialSprite.color.a;
            set
            {
                Color color = radialSprite.color;
                color.a = value;

                radialSprite.color = color;
            }
        }

        private void Start()
        {
            reelingController.OnStartReelingEvent += StartReeling;
            reelingController.OnEndReelingEvent += EndReeling;
        }

        private void StartReeling(object sender, ReelingController.OnStartReelingEventArgs args)
        {
            reeling = true;

            FillAmount = 0;
            Alpha = minSpriteAlpha;
        }

        private void EndReeling(object sender, ReelingController.OnEndReelingEventArgs args)
        {
            reeling = false;

            FillAmount = 1;
            Alpha = maxSpriteAlpha;
        }

        private void Update()
        {
            if (reeling)
            {
                float exhaustion = reelingController.FishExhaustion;

                FillAmount = exhaustion;
                Alpha = Mathf.Lerp(minSpriteAlpha, maxSpriteAlpha, exhaustion);
            }
        }
    }
}