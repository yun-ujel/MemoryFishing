using UnityEngine;

using MemoryFishing.Gameplay.Fishing.Player;
using MemoryFishing.Gameplay.Fishing.Fish;

using static MemoryFishing.Utilities.GeneralUtils;
using MemoryFishing.Gameplay.Fishing.Player.EventArgs;
using MemoryFishing.Gameplay.Enumerations;

namespace MemoryFishing.FX.Fishing
{
    public class FishingLineFX : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerFishingManager fishingManager;

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
        private bool recalling;

        private Vector3 targetPos;
        private Vector3 startPos;

        private Vector3 bobberPos;

        private void Start()
        {
            fishingManager.OnCastBobberEvent += OnCastBobber;
            fishingManager.OnRecallBobberEvent += RecallBobber;
        }

        private void OnCastBobber(object sender, OnCastBobberEventArgs args)
        {
            counter = 0f;

            timeToLand = args.TimeToLand;
            peakHeight = args.Magnitude * peakHeightMultiplier;

            startPos = rodEnd.position;
            targetPos = args.TargetPosition;
        }

        private void RecallBobber(object sender, OnRecallBobberEventArgs args)
        {
            timeToRecall = args.TimeToRecall;
            counter = 0f;
            recalling = true;

            startPos = bobberPos;
            targetPos = rodEnd.position;
        }

        private void Update()
        {
            if (fishingManager.State == FishingState.None || fishingManager.State == FishingState.WindUp)
            {
                if (recalling)
                {
                    counter += Time.deltaTime;

                    float t = recallCurve.Evaluate(counter / timeToRecall);

                    bobberPos = Vector3.Lerp(startPos, targetPos, t);
                    SetLinePositions(rodEnd.position, bobberPos);

                    recalling = counter < timeToRecall;

                    return;
                }

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

                bobberPos = Vector3.Lerp(startPos, targetPos, t);

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
        }

        private void SetLinePositions(Vector3 start, Vector3 end)
        {
            for (int i = 0; i < line.positionCount; i++)
            {
                float t = (float)i / (line.positionCount - 1);
                float curveT = lineCurve.Evaluate(t);

                Vector3 pos = Vector3.Lerp(start, end, t);
                pos.y = Mathf.Lerp(start.y, end.y, curveT);

                line.SetPosition(i, pos);
            }
        }
    }
}