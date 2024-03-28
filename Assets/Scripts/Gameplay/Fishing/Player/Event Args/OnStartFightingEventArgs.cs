using UnityEngine;

using MemoryFishing.Gameplay.Fishing.Fish;

namespace MemoryFishing.Gameplay.Fishing.Player.EventArgs
{
    public class OnStartFightingEventArgs : System.EventArgs
    {
        public FishBehaviour FishBehaviour { get; private set; }
        public Vector3 PlayerPos { get; private set; }
        public Vector3 FishPos { get; private set; }
        public int FightCount { get; private set; }
        public bool FirstFight => FightCount == 0;

        public OnStartFightingEventArgs(FishBehaviour fishBehaviour, Vector3 playerPos, Vector3 fishPos, int fightCount)
        {
            FishBehaviour = fishBehaviour;
            PlayerPos = playerPos;
            FishPos = fishPos;

            FightCount = fightCount;
        }
    }
}