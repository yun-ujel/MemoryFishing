using UnityEngine;

using MemoryFishing.Gameplay.Inventory;
using MemoryFishing.Utilities;

using static MemoryFishing.Utilities.GeneralUtils;
using static MemoryFishing.Utilities.VectorUtils;

namespace MemoryFishing.Gameplay.Fishing.Fish
{
    public class PaulFish : FishBehaviour
    {
        private Vector3 direction;
        private Rigidbody body;

        [SerializeField] private PlayerDirection playerDirection;

        [Space, SerializeField] private float driftDuration = 2f;
        [SerializeField] private float startSpeed = 1f;

        private float speed;
        float t;

        private void Start()
        {
            body = GetComponent<Rigidbody>();
        }

        #region Override Methods

        #region Approaching

        public override void ApproachBobber(Vector3 bobberPos, float delta)
        {
            transform.position = bobberPos;
        }

        public override float GetApproachTime(Vector3 bobberPos)
        {
            return 0f;
        }

        #endregion

        #region Fighting

        public override void InitiateFighting(Vector3 playerPos, Vector3 fishPos, int fightCount)
        {
            base.InitiateFighting(playerPos, fishPos, fightCount);

            Vector3 dir = playerDirection.GetLookDirection(transform.position);
            float angle = VectorToDegrees(dir);

            angle += 60f;
            direction = DegreesToVector(angle).OnZAxis();

            speed = startSpeed;
            t = 0f;
        }

        public override float GetExhaustionMultiplier(float dot)
        {
            return 1f;
        }

        public override float UpdateFishExhaustion(float delta, Vector3 input)
        {
            float dot = Vector3.Dot(input, GetFishDirection());

            if (dot > 0.9f)
            {
                return 0.2f * delta;
            }

            return -1f * delta;
        }

        public override Vector3 GetFishDirection()
        {
            return direction;
        }

        public override void MoveFishFighting(float delta)
        {
            body.AddForce(direction * speed, ForceMode.Acceleration);
        }

        public override void UpdateFishFighting(float delta)
        {
            t += delta / driftDuration;

            speed = Mathf.Lerp(startSpeed, 0, t);
        }

        #endregion

        #region Catching / Reeling

        public override float GetReelDuration()
        {
            return 3f;
        }
        public override float UpdateReawakenDuration(float startingDistance, float distanceLeft, float delta)
        {
            return 1f;
        }

        public override InventoryItem GetItem()
        {
            return null;
        }

        #endregion

        #endregion
    }
}