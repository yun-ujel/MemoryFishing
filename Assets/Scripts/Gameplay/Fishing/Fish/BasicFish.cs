using UnityEngine;

namespace MemoryFishing.Gameplay.Fishing.Fish
{
    public class BasicFish : FishBehaviour
    {
        [Header("Direction")]
        [SerializeField, Range(0f, 720f)] private float maxAngleRange = 270f;

        [Space]

        [SerializeField, Range(0f, 10f)] private float maxHoldDuration = 3f;
        [SerializeField, Range(0f, 10f)] private float minHoldDuration = 1f;

        [Header("Movement")]
        [SerializeField, Range(0f, 10f)] private float maxDistanceFromStart = 3f;
        [SerializeField, Range(0f, 10f)] private float speed = 2f;

        [Header("Exhaustion")]
        [SerializeField, Range(0f, 1f)] private float positiveExhaustMultiplier = 0.1f;
        [SerializeField, Range(0f, 1f)] private float negativeExhaustMultiplier = 0.05f;

        private float angleOffset = 0f;

        private float currentRotationDeg, prevRotationDeg, targetRotationDeg;
        private float targetHoldDuration, currentHoldDuration;

        private float currentExhaustion;

        private Vector3 startPos;

        #region Override Methods
        public override void InitiateFishing(Vector3 playerPos, Vector3 fishPos)
        {
            startPos = fishPos;
            angleOffset = CalculateAngleOffset(playerPos, fishPos);
        }

        public override void UpdateFish(float delta)
        {
            currentHoldDuration += delta;
            float t = currentHoldDuration / targetHoldDuration;

            if (currentHoldDuration >= targetHoldDuration)
            {
                GetNewRotation();
            }

            LerpRotation(t);

            Vector3 targetPos = transform.position + GetFishDirection();
            Vector3 clampedPos = ClampToCircle(targetPos, startPos, Mathf.Lerp(maxDistanceFromStart, 0f, currentExhaustion));
            transform.position = Vector3.MoveTowards(transform.position, clampedPos, speed * delta);
        }

        public override float UpdateFishExhaustion(float delta, Vector3 input)
        {
            currentExhaustion += base.UpdateFishExhaustion(delta, input);

            return base.UpdateFishExhaustion(delta, input);
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

        private float CalculateAngleOffset(Vector3 playerPos, Vector3 fishPos)
        {
            float angleToPlayer = Vector3.Angle((playerPos - fishPos).ExcludeYAxis().normalized, Vector3.right);

            bool flipAngle = fishPos.z > playerPos.z;
            angleToPlayer = flipAngle ? (360 - angleToPlayer) : angleToPlayer;

            float offset = (360f - maxAngleRange) / 2f;
            return offset + angleToPlayer;
        }

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

        private Vector3 ClampToCircle(Vector3 position, Vector3 center, float radius)
        {
            Ray ray = new Ray(center, position - center);
            float sqrDistance = Vector3.SqrMagnitude(position - center);

            if (sqrDistance > Mathf.Pow(radius, 2))
            {
                return ray.GetPoint(radius);
            }

            return position;
        }
        #endregion
    }
}