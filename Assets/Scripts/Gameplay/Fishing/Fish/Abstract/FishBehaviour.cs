using UnityEngine;

namespace MemoryFishing.Gameplay.Fishing.Fish
{
    public abstract class FishBehaviour : MonoBehaviour
    {
        public abstract Vector3 UpdateFishDirection(float delta);
        public abstract Vector3 GetFishDirection();

        public abstract void InitiateFishing(Vector3 playerPos, Vector3 fishPos);

        public virtual float UpdateExhaustion(float delta, Vector3 input)
        {
            float dot = Vector3.Dot(input, GetFishDirection());

            return dot * GetExhaustionMultiplier(dot) * delta;
        }

        public abstract float GetExhaustionMultiplier(float dot);
    }
}