using UnityEngine;

namespace MemoryFishing.Gameplay.Inventory
{
    public class Inventory
    {
        public int Capacity { get; private set; }
        public InventoryItem[] Items { get; private set; }

        public Inventory(int capacity)
        {
            Capacity = capacity;
            Items = new InventoryItem[capacity];
        }

        #region Add Methods

        public bool TryAddItem(InventoryItem item, int space)
        {
            if (Items[space] != null)
            {
                return false;
            }

            Items[space] = item;
            return true;
        }

        public bool TryAddItem(InventoryItem item, out int index)
        {
            index = GetFirstEmptySpace();
            
            if (index == Capacity)
            {
                return false;
            }

            return TryAddItem(item, index);
        }

        public bool TryAddItem(InventoryItem item)
        {
            return TryAddItem(item, out int _);
        }

        public InventoryItem SwapItem(InventoryItem item, int space)
        {
            InventoryItem swapped = Items[space];
            Items[space] = item;

            return swapped;
        }

        public void MoveItem(int from, int to)
        {
            InventoryItem fromItem = Items[from];
            InventoryItem toItem = Items[to];

            Items[from] = fromItem;
            Items[to] = toItem;
        }

        #endregion

        #region Get Methods

        public int GetFirstEmptySpace()
        {
            for (int i = 0; i < Capacity; i++)
            {
                if (Items[i] == null)
                {
                    return i;
                }
            }

            return Capacity;
        }

        public int[] GetEmptySpaces()
        {
            int[] spaces = new int[Capacity];
            int spacesCount = 0;

            for (int i = 0; i < Capacity; i++)
            {
                if (Items[i] == null)
                {
                    spaces[spacesCount] = i;
                    spacesCount++;
                }
            }

            int[] emptySpaces = new int[spacesCount];
            spaces.CopyTo(emptySpaces, 0);

            return emptySpaces;
        }

        #endregion
    }
}