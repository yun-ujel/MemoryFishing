using UnityEngine;

using MemoryFishing.Gameplay.Fishing.Player;
using MemoryFishing.Gameplay.Fishing.Fish;

using static MemoryFishing.Utilities.GeneralUtils;

namespace MemoryFishing.FX.Fishing
{
    public class FishingLineFX : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ReelingController reelingController;
        [SerializeField] private BobberCastController castController;

        [Space, SerializeField] private LineRenderer line;

        [Header("Cast Settings")]
        [SerializeField] private float peakHeightMultiplier;
        [SerializeField, Range(0f, 1f)] private float peak;

        [Space, SerializeField] private AnimationCurve risingHeightCurve;
        [SerializeField] private AnimationCurve fallingHeightCurve;

        [Header("Line Settings")]
        [SerializeField] private AnimationCurve lineCurve;
        [SerializeField] private Transform rodEnd;

        private bool isCasting;
        private bool isWaiting;
        private bool isReeling;

        private float timeSinceCastStart;
        private float castTimeToLand;
        private float castPeakHeight;

        private Vector3 targetCastPosition;
        private Vector3 startingCastPosition;

        private Transform fish;

        private void Start()
        {
            reelingController.OnStartReelingEvent += OnStartReeling;
            reelingController.OnEndReelingEvent += OnEndReeling;

            castController.OnCastBobberEvent += OnCastBobber;
            castController.OnBobberLandEvent += OnBobberLand;
        }

        private void OnCastBobber(object sender, BobberCastController.OnCastBobberEventArgs args)
        {
            isCasting = true;
            timeSinceCastStart = 0f;

            castTimeToLand = args.TimeToLand;
            castPeakHeight = args.Magnitude * peakHeightMultiplier;

            targetCastPosition = args.TargetPosition + (Vector3.down * 0.5f);
            startingCastPosition = rodEnd.position;
        }

        private void OnBobberLand(object sender, BobberCastController.OnBobberLandEventArgs args)
        {
            isCasting = false;

            SetLinePositions(rodEnd.position, targetCastPosition);
        }

        private void OnStartReeling(object sender, ReelingController.OnStartReelingEventArgs args)
        {
            fish = args.FishBehaviour.transform;

            isReeling = true;
        }

        private void OnEndReeling(object sender, ReelingController.OnEndReelingEventArgs args)
        {
            isReeling = false;
        }

        private void Update()
        {
            if (isCasting)
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

            if (isReeling)
            {
                SetLinePositions(rodEnd.position, fish.position);
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