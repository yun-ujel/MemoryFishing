using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MemoryFishing.Gameplay.Fishing.Fish
{
    public class BasicFish : FishBehaviour
    {
        private Vector2 direction;

        private float currentRotationDeg;

        private float prevRotationDeg;
        private float targetRotationDeg;

        [SerializeField, Range(0f, 720f)] private float maxAngleRange = 360f;
        [SerializeField, Range(0f, 360f)] private float angleOffset = 0f;

        [Space]

        [SerializeField, Range(0f, 10f)] private float maxHoldDuration = 3f;
        [SerializeField, Range(0f, 10f)] private float minHoldDuration = 1f;

        private float targetHoldDuration;
        private float currentHoldDuration;

        #region Override Methods
        public override void InitiateFishing()
        {

        }

        public override Vector2 UpdateFishDirection(float delta)
        {
            currentHoldDuration += delta;
            if (currentHoldDuration >= targetHoldDuration)
            {
                GetNewRotation();
            }

            LerpRotation();
            return GeneralUtils.DegreesToVector(currentRotationDeg + angleOffset);
        }

        public override float GetExhaustionMultiplier(float dot)
        {
            if (dot < 0f)
            {
                return 0.5f;
            }

            return 1f;
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

        private void LerpRotation()
        {
            float t = currentHoldDuration / targetHoldDuration;

            currentRotationDeg = Mathf.Lerp(prevRotationDeg, targetRotationDeg, t);
        }
        #endregion
    }
}