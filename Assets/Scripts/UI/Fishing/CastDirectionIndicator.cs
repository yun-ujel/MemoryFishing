using UnityEngine;

using MemoryFishing.Gameplay;
using MemoryFishing.Gameplay.Fishing.Player;
using MemoryFishing.Gameplay.Fishing.Player.EventArgs;

namespace MemoryFishing.UI.Fishing
{
    public class CastDirectionIndicator : MonoBehaviour
    {
        [SerializeField] private PlayerFishingManager fishingManager;
        [SerializeField] private PlayerDirection playerDirection;
        [SerializeField] private Transform player;

        [Space, SerializeField] private RectTransform arrowParent;

        private bool windingUp;
        private float windUpCounter;

        private AnimationCurve windUpCurve;
        private float timeToWindUp;

        private float startScale;
        private float targetScale;

        private float ArrowScale
        {
            get
            {
                return arrowParent.localScale.x;
            }

            set
            {
                arrowParent.localScale = Vector3.one * value;
            }
        }

        private void Start()
        {
            fishingManager.OnStartWindUpEvent += StartWindUp;
            fishingManager.OnCastBobberEvent += (sender, args) => HideArrow();

            fishingManager.OnDisableFishingEvent += (sender, args) => HideArrow();
        }

        private void HideArrow()
        {
            windingUp = false;
            arrowParent.gameObject.SetActive(false);
        }

        private void StartWindUp(object sender, OnStartWindUpEventArgs args)
        {
            arrowParent.gameObject.SetActive(true);
            
            startScale = args.StartingCastDistance;
            targetScale = args.CastDistance + startScale;

            ArrowScale = startScale;
            
            windingUp = true;
            windUpCounter = 0f;

            windUpCurve = args.WindUpCurve;
            timeToWindUp = args.TimeToWindUp;
        }

        private void Update()
        {
            if (!windingUp)
            {
                return;
            }

            windUpCounter += Time.deltaTime;

            float t = windUpCurve.Evaluate(windUpCounter / timeToWindUp);
            ArrowScale = Mathf.Lerp(startScale, targetScale, t);
            arrowParent.forward = playerDirection.GetLookDirection(player.transform.position);
        }
    }
}