using UnityEngine;

namespace MemoryFishing.Gameplay.Inventory
{
    public class PlayerInventory
    {
        public int Capacity { get; private set; }
        public InventoryItem[] Items { get; private set; }

        public PlayerInventory(int capacity)
        {
            Capacity = capacity;
            Items = new InventoryItem[capacity];
        }
    }
}