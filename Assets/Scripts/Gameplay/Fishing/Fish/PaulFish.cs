using UnityEngine;

using MemoryFishing.Gameplay.Inventory;
using MemoryFishing.Utilities;

using static MemoryFishing.Utilities.GeneralUtils;

namespace MemoryFishing.Gameplay.Fishing.Fish
{
    public class PaulFish : FishBehaviour
    {
        private Vector3 direction;
        private Rigidbody body;

        [SerializeField] private float speed;

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

            direction = CalculateDirection(playerPos, fishPos).OnZAxis();
        }

        private static Vector2 CalculateDirection(Vector3 playerPos, Vector3 fishPos)
        {
            Vector3 direction = (playerPos - fishPos).ExcludeYAxis().normalized;
            float angleToPlayer = VectorUtils.VectorToDegrees(direction.OnYAxis());

            bool flipAngle = fishPos.z > playerPos.z;
            angleToPlayer = flipAngle ? (360 - angleToPlayer) : angleToPlayer;

            float multiplier = CoinFlip() ? 1 : -1;
            float angleOffset = (Random.value * 90f) + 45f;

            return VectorUtils.DegreesToVector((angleOffset * multiplier) + angleToPlayer);
        }

        public override float GetExhaustionMultiplier(float dot)
        {
            if (dot > 0.8f)
            {
                return 0.2f;
            }

            return 0f;
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

        }

        #endregion

        #region Catching / Reeling

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