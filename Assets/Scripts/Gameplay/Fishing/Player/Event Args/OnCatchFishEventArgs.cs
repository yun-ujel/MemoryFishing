using UnityEngine;

using MemoryFishing.Gameplay.Inventory;
using MemoryFishing.Gameplay.Fishing.Fish;

namespace MemoryFishing.Gameplay.Fishing.Player.EventArgs
{
    public class OnCatchFishEventArgs
    {
        public InventoryItem Item { get; private set; }
        public FishBehaviour Fish { get; private set; }

        public Vector3 LastFishPosition { get; private set; }

        public OnCatchFishEventArgs(InventoryItem item, FishBehaviour fish, Vector3 lastFishPosition)
        {
            Item = item;
            Fish = fish;
            LastFishPosition = lastFishPosition;
        }
    }
}