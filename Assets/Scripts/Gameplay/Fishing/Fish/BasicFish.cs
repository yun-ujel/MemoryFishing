using UnityEngine;

using MemoryFishing.Utilities;
using MemoryFishing.Gameplay.Inventory;

namespace MemoryFishing.Gameplay.Fishing.Fish
{
    [RequireComponent(typeof(Rigidbody))]
    public class BasicFish : FishBehaviour
    {
        private Rigidbody body;

        [Header("Direction")]
        [SerializeField, Range(0f, 720f)] private float maxAngleRange = 270f;

        [Space]

        [SerializeField, Range(0f, 10f)] private float maxHoldDuration = 3f;
        [SerializeField, Range(0f, 10f)] private float minHoldDuration = 1f;

        [Header("Movement")]
        [SerializeField, Range(0f, 10f)] private float maxDistanceFromStart = 3f;
        [SerializeField, Range(0f, 10f)] private float speed = 2f;
        [SerializeField] private float acceleration = 20f;

        [Space]

        [SerializeField, Range(0f, 10f)] private float idleFriction = 1f;

        [Header("Exhaustion")]
        [SerializeField, Range(0f, 1f)] private float positiveExhaustMultiplier = 0.1f;
        [SerializeField, Range(0f, 1f)] private float negativeExhaustMultiplier = 0.05f;

        private float angleOffset = 0f;

        private float currentRotationDeg, prevRotationDeg, targetRotationDeg;
        private float targetHoldDuration, currentHoldDuration;

        private float currentExhaustion;

        private Vector3 reelStartPos;
        private Vector3 playerPos;

        private Vector3 approachStartPos;

        private void Start()
        {
            body = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            if (!isReeling)
            {
                // Apply Friction
                body.AddForce(body.velocity * -idleFriction);
            }
        }

        #region Override Methods
        
        #region Reeling
        public override void InitiateReeling(Vector3 playerPos, Vector3 fishPos)
        {
            base.InitiateReeling(playerPos, fishPos);

            currentExhaustion = 0f;
            GetNewTarget();

            reelStartPos = fishPos;
            this.playerPos = playerPos;

            angleOffset = CalculateAngleOffset(playerPos, fishPos);
        }

        public override void UpdateFishReeling(float delta)
        {
            currentHoldDuration += delta;
            float t = currentHoldDuration / targetHoldDuration;

            if (currentHoldDuration >= targetHoldDuration)
            {
                GetNewTarget();
            }

            LerpRotation(t);
        }

        public override void MoveFishReeling(float delta)
        {
            float maxDistance = Mathf.Lerp(maxDistanceFromStart, 0f, currentExhaustion);
            Vector3 directionToCenter = (reelStartPos - transform.position).normalized;

            Vector3 desiredDirection = GetFishDirection();

            float t = VectorUtils.SqrDistance(transform.position, reelStartPos) / Mathf.Pow(maxDistance, 2);

            Vector3 direction = Vector3.Lerp(desiredDirection, directionToCenter, t).ExcludeYAxis();
            body.velocity = Vector3.MoveTowards(body.velocity, direction * speed, acceleration * delta);
        }

        public override float UpdateFishExhaustion(float delta, Vector3 input)
        {
            currentExhaustion += base.UpdateFishExhaustion(delta, input);
            currentExhaustion = Mathf.Clamp01(currentExhaustion);

            return base.UpdateFishExhaustion(delta, input);
        }

        public override Vector3 GetFishDirection()
        {
            Vector2 direction = VectorUtils.DegreesToVector(currentRotationDeg + angleOffset);
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

        public override void StopReeling()
        {
            base.StopReeling();
            DriftToCenter();
        }

        #endregion

        #region Approaching

        public override float GetApproachTime(Vector3 bobberPos)
        {
            Vector3 randomDir = VectorUtils.DegreesToVector(Random.value * 360f).OnZAxis();
            
            approachStartPos = bobberPos + (randomDir * 10f);
            transform.position = approachStartPos;

            return 1f;
        }

        public override void ApproachBobber(Vector3 bobberPos, float t)
        {
            transform.position = Vector3.Lerp(approachStartPos, bobberPos, t);
        }

        #endregion

        #region Catching

        public override InventoryItem GetItem()
        {
            return null;
        }

        #endregion

        #endregion

        #region Private Methods

        private float CalculateAngleOffset(Vector3 playerPos, Vector3 fishPos)
        {
            Vector3 direction = (playerPos - fishPos).ExcludeYAxis().normalized;
            float angleToPlayer = VectorUtils.VectorToDegrees(direction.OnYAxis());

            bool flipAngle = fishPos.z > playerPos.z;
            angleToPlayer = flipAngle ? (360 - angleToPlayer) : angleToPlayer;

            float offset = (360f - maxAngleRange) / 2f;
            return offset + angleToPlayer;
        }

        private void GetNewTarget()
        {
            currentRotationDeg = targetRotationDeg;
            prevRotationDeg = currentRotationDeg;

            targetRotationDeg = Random.value * maxAngleRange;
            targetHoldDuration = Random.Range(minHoldDuration, maxHoldDuration);
            currentHoldDuration = 0f;
        }

        private void LerpRotation(float t)
        {
            currentRotationDeg = Mathf.Lerp(prevRotationDeg, targetRotationDeg, t);
        }

        private void DriftToCenter()
        {
            Vector3 towardsStart = (reelStartPos - transform.position).normalized;
            Vector3 towardsPlayer = (playerPos - transform.position).normalized;

            float dot = Mathf.Clamp01(Vector3.Dot(towardsStart, towardsPlayer));

            body.velocity *= dot;
        }

        #endregion
    }
}