using UnityEngine;

using MemoryFishing.Gameplay.Inventory;

namespace MemoryFishing.Gameplay.Fishing.Fish
{
    public abstract class FishBehaviour : MonoBehaviour
    {
        protected bool isReeling;

        #region Reeling
        public abstract void MoveFishReeling(float delta);
        public abstract void UpdateFishReeling(float delta);
        public abstract Vector3 GetFishDirection();

        public virtual void InitiateReeling(Vector3 playerPos, Vector3 fishPos)
        {
            isReeling = true;
        }
        public virtual void StopReeling()
        {
            isReeling = false;
        }

        public virtual float UpdateFishExhaustion(float delta, Vector3 input)
        {
            float dot = Vector3.Dot(input, GetFishDirection());

            return dot * GetExhaustionMultiplier(dot) * delta;
        }

        public abstract float GetExhaustionMultiplier(float dot);
        #endregion

        #region Approaching

        public abstract float GetApproachTime(Vector3 bobberPos);

        public abstract void ApproachBobber(Vector3 bobberPos, float delta);

        #endregion

        #region Catching

        public abstract InventoryItem GetItem();

        #endregion
    }
}