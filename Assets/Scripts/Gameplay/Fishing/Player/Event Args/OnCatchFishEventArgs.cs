using UnityEngine;

using MemoryFishing.Gameplay.Fishing.Fish;

namespace MemoryFishing.Gameplay.Fishing.Player.EventArgs
{
    public class OnCatchFishEventArgs
    {
        public FishBehaviour Fish { get; private set; }

        public Vector3 LastFishPosition { get; private set; }

        public OnCatchFishEventArgs(FishBehaviour fish, Vector3 lastFishPosition)
        {
            Fish = fish;
            LastFishPosition = lastFishPosition;
        }
    }
}