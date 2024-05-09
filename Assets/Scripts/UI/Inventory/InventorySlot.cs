using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

using MemoryFishing.Gameplay.Inventory;

namespace MemoryFishing.UI.Inventory
{
    public class InventorySlot : MonoBehaviour
    {
        [SerializeField] private RawImage image;

        public void AddItem(InventoryItem item)
        {
            image.enabled = true;
            image.texture = item.Texture;
        }
    }
}