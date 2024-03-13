using UnityEngine;

namespace MemoryFishing.Gameplay.Fishing.Fish
{
    public abstract class FishBehaviour : MonoBehaviour
    {
        protected bool isReeling;

        public abstract void MoveFish(float delta);
        public abstract void UpdateFish(float delta);
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
    }
}