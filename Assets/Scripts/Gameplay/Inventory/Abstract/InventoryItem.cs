using UnityEngine;

namespace MemoryFishing.Gameplay.Inventory
{
    [CreateAssetMenu(fileName = "New Item", menuName = "ScriptableObjects/Item")]
    public class InventoryItem : ScriptableObject
    {
        [field: SerializeField] public Texture2D Texture { get; private set; }
        [field: SerializeField, Multiline] public string Description { get; private set; }
    }
}