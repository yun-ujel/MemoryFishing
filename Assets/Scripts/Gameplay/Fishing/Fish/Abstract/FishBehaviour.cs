using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MemoryFishing.Gameplay.Fishing.Fish
{
    public abstract class FishBehaviour : MonoBehaviour
    {
        public abstract Vector2 UpdateFishDirection(float delta);
        public abstract void InitiateFishing();

        public virtual float UpdateExhaustion(float delta, Vector2 input)
        {
            float dot = Vector2.Dot(input, UpdateFishDirection(delta));

            return dot * GetExhaustionMultiplier(dot);
        }

        public abstract float GetExhaustionMultiplier(float dot);
    }
}