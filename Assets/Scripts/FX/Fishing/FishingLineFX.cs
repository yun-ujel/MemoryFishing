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

        [Header("Recall Settings")]
        [SerializeField] private AnimationCurve recallCurve;

        [Header("Line Settings")]
        [SerializeField] private AnimationCurve lineCurve;
        
        [Space, SerializeField] private Transform rodEnd;
        [SerializeField] private Transform bobber;

        private float counter;
        private float timeToLand;
        private float peakHeight;

        private float timeToRecall;

        private Vector3 targetPos;
        private Vector3 startPos;

        private void Start()
        {
            castController.OnCastBobberEvent += OnCastBobber;
            castController.OnRecallBobberEvent += RecallBobber;
        }

        private void OnCastBobber(object sender, BobberCastController.OnCastBobberEventArgs args)
        {
            counter = 0f;

            timeToLand = args.TimeToLand;
            peakHeight = args.Magnitude * peakHeightMultiplier;

            startPos = rodEnd.position;
            targetPos = args.TargetPosition;
        }

        private void RecallBobber(object sender, BobberCastController.OnRecallBobberEventArgs args)
        {
            timeToRecall = args.TimeToRecall;
            counter = 0f;

            startPos = args.BobberPosition;
            targetPos = rodEnd.position;
        }

        private void Update()
        {
            if (fishingManager.State == FishingState.None)
            {
                if (counter > 0f)
                {
                    counter = 0f;
                    SetLinePositions(rodEnd.position, rodEnd.position);
                }

                return;
            }

            if (fishingManager.State == FishingState.Casting)
            {
                counter += Time.deltaTime;
                float t = counter / timeToLand;

                Vector3 bobberPos = Vector3.Lerp(startPos, targetPos, t);

                if (t <= peak)
                {
                    float curveT = risingHeightCurve.Evaluate(t.Remap01(0, peak));
                    bobberPos.y = Mathf.LerpUnclamped(startPos.y, startPos.y + peakHeight, curveT);
                }
                else
                {
                    float curveT = fallingHeightCurve.Evaluate(t.Remap01(peak, 1));
                    bobberPos.y = Mathf.LerpUnclamped(startPos.y + peakHeight, targetPos.y, curveT);
                }

                SetLinePositions(rodEnd.position, bobberPos);
                return;
            }

            if (fishingManager.State == FishingState.Recall)
            {
                counter += Time.deltaTime;

                float t = recallCurve.Evaluate(counter / timeToRecall);

                Vector3 bobberPos = Vector3.Lerp(startPos, targetPos, t);
                SetLinePositions(rodEnd.position, bobberPos);
                return;
            }

            if (fishingManager.State != FishingState.WindUp)
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