using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MemoryFishing.Gameplay.Fishing.Fish
{
    public class BasicFish : FishBehaviour
    {
        [Header("Direction")]
        [SerializeField, Range(0f, 720f)] private float maxAngleRange = 360f;
        [SerializeField, Range(0f, 360f)] private float angleOffset = 0f;

        [Space]

        [SerializeField, Range(0f, 10f)] private float maxHoldDuration = 3f;
        [SerializeField, Range(0f, 10f)] private float minHoldDuration = 1f;

        [Header("Exhaustion")]
        [SerializeField, Range(0f, 1f)] private float positiveExhaustMultiplier = 0.1f;
        [SerializeField, Range(0f, 1f)] private float negativeExhaustMultiplier = 0.05f;

        private float currentRotationDeg;

        private float prevRotationDeg;
        private float targetRotationDeg;

        private float targetHoldDuration;
        private float currentHoldDuration;

        #region Override Methods
        public override void InitiateFishing()
        {

        }

        public override Vector3 UpdateFishDirection(float delta)
        {
            currentHoldDuration += delta;
            if (currentHoldDuration >= targetHoldDuration)
            {
                GetNewRotation();
            }

            LerpRotation(currentHoldDuration / targetHoldDuration);
            return GetFishDirection();
        }

        public override Vector3 GetFishDirection()
        {
            Vector2 direction = GeneralUtils.DegreesToVector(currentRotationDeg + angleOffset);
            return direction.OnZAxis();
        }

        public override float GetExhaustionMultiplier(float dot)
        {
            if (dot < 0f)
            {
                return negativeExhaustMultiplier;
            }

            return positiveExhaustMultiplier;
        }
        #endregion

        #region Private Methods
        private void GetNewRotation()
        {
            prevRotationDeg = currentRotationDeg;

            targetRotationDeg = Random.value * maxAngleRange;
            targetHoldDuration = Random.Range(minHoldDuration, maxHoldDuration);
            currentHoldDuration = 0f;
        }

        private void LerpRotation(float t)
        {
            currentRotationDeg = Mathf.Lerp(prevRotationDeg, targetRotationDeg, t);
        }
        #endregion
    }
}