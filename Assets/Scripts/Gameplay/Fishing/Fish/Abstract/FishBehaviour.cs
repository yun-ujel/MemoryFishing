using UnityEngine;

using MemoryFishing.Gameplay.Inventory;

namespace MemoryFishing.Gameplay.Fishing.Fish
{
    public abstract class FishBehaviour : MonoBehaviour
    {
        protected bool isFighting;

        #region Approaching

        public abstract float GetApproachTime(Vector3 bobberPos);

        public abstract void ApproachBobber(Vector3 bobberPos, float delta);

        #endregion

        #region Fighting
        public abstract void MoveFishFighting(float delta);
        public abstract void UpdateFishFighting(float delta);
        public abstract Vector3 GetFishDirection();

        public virtual void InitiateFighting(Vector3 playerPos, Vector3 fishPos)
        {
            isFighting = true;
        }
        public virtual void StopFighting()
        {
            isFighting = false;
        }

        public virtual float UpdateFishExhaustion(float delta, Vector3 input)
        {
            float dot = Vector3.Dot(input, GetFishDirection());

            return dot * GetExhaustionMultiplier(dot) * delta;
        }

        public abstract float GetExhaustionMultiplier(float dot);

        #endregion

        #region Reeling

        public abstract float UpdateReawakenDuration(float startingDistance, float distanceLeft, float delta);

        #endregion

        #region Catching

        public abstract InventoryItem GetItem();

        #endregion
    }
}