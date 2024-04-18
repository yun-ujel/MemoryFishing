using UnityEngine;

using MemoryFishing.Gameplay.Fishing.Fish;

namespace MemoryFishing.Gameplay.Fishing.Player.EventArgs
{
    public class OnFishApproachingEventArgs : System.EventArgs
    {
        public FishBehaviour FishBehaviour { get; private set; }
        public float ApproachTime { get; private set; }

        public OnFishApproachingEventArgs(FishBehaviour fishBehaviour, float approachTime)
        {
            FishBehaviour = fishBehaviour;
            ApproachTime = approachTime;
        }
    }
}