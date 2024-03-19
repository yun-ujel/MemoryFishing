using UnityEngine;

using MemoryFishing.Gameplay.Fishing.Player;
using MemoryFishing.Gameplay.Fishing.Fish;
using MemoryFishing.Gameplay.Fishing.Enumerations;

using static MemoryFishing.Utilities.GeneralUtils;

namespace MemoryFishing.FX.Fishing
{
    public class FishingLineFX : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerFishingManager fishingManager;
        [SerializeField] private BobberCastController castController;

        [Space, SerializeField] private LineRenderer line;

        [Header("Cast Settings")]
        [SerializeField] private float peakHeightMultiplier;
        [SerializeField, Range(0f, 1f)] private float peak;

        [Space, SerializeField] private AnimationCurve risingHeightCurve;
        [SerializeField] private AnimationCurve fallingHeightCurve;

        [Header("Line Settings")]
        [SerializeField] private AnimationCurve lineCurve;
        
        [Space, SerializeField] private Transform rodEnd;
        [SerializeField] private Transform bobber;

        private float timeSinceCastStart;
        private float castTimeToLand;
        private float castPeakHeight;

        private Vector3 targetCastPosition;
        private Vector3 startingCastPosition;

        private void Start()
        {
            castController.OnCastBobberEvent += OnCastBobber;
        }

        private void OnCastBobber(object sender, BobberCastController.OnCastBobberEventArgs args)
        {
            timeSinceCastStart = 0f;

            castTimeToLand = args.TimeToLand;
            castPeakHeight = args.Magnitude * peakHeightMultiplier;

            targetCastPosition = args.TargetPosition;
            startingCastPosition = rodEnd.position;
        }

        private void Update()
        {
            if (fishingManager.State == FishingState.Casting)
            {
                timeSinceCastStart += Time.deltaTime;
                float t = timeSinceCastStart / castTimeToLand;

                Vector3 bobberPos = Vector3.Lerp(startingCastPosition, targetCastPosition, t);

                if (t <= peak)
                {
                    float curveT = risingHeightCurve.Evaluate(t.Remap01(0, peak));
                    bobberPos.y = Mathf.LerpUnclamped(startingCastPosition.y, startingCastPosition.y + castPeakHeight, curveT);
                }
                else
                {
                    float curveT = fallingHeightCurve.Evaluate(t.Remap01(peak, 1));
                    bobberPos.y = Mathf.LerpUnclamped(startingCastPosition.y + castPeakHeight, targetCastPosition.y, curveT);
                }

                SetLinePositions(rodEnd.position, bobberPos);
                return;
            }

            if (fishingManager.State != FishingState.None)
            {
                SetLinePositions(rodEnd.position, bobber.position);
                return;
            }
        }

        private void SetLinePositions(Vector3 playerPos, Vector3 bobberPos)
        {
            for (int i = 0; i < line.positionCount; i++)
            {
                float t = (float)i / (line.positionCount - 1);
                float curveT = lineCurve.Evaluate(t);

                Vector3 pos = Vector3.Lerp(playerPos, bobberPos, t);
                pos.y = Mathf.Lerp(playerPos.y, bobberPos.y, curveT);

                line.SetPosition(i, pos);
            }
        }
    }
}