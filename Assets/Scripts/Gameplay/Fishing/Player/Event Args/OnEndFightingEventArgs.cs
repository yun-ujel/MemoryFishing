using UnityEngine;

using MemoryFishing.Gameplay.Fishing.Fish;

namespace MemoryFishing.Gameplay.Fishing.Player.EventArgs
{
    public class OnEndFightingEventArgs : System.EventArgs
    {
        public FishBehaviour FishBehaviour { get; private set; }
        public Vector3 PlayerPos { get; private set; }
        public Vector3 FishPos { get; private set; }

        public OnEndFightingEventArgs(FishBehaviour fishBehaviour, Vector3 playerPos, Vector3 fishPos)
        {
            FishBehaviour = fishBehaviour;
            PlayerPos = playerPos;
            FishPos = fishPos;
        }
    }
}