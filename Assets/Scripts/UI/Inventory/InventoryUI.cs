using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using MemoryFishing.Gameplay.Inventory;

namespace MemoryFishing.UI.Inventory
{
    public class InventoryUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private InventoryManager inventoryManager;
        
        [Space, SerializeField] private Animator animator;
        [SerializeField] private HorizontalLayoutGroup layoutGroup;
        private InventorySlot[] inventorySlots;

        private void Start()
        {
            inventoryManager.OnInventoryOpenedEvent += OpenInventory;
            inventoryManager.OnInventoryClosedEvent += CloseInventory;

            inventoryManager.OnGetItemEvent += GetItem;

            int childCount = layoutGroup.transform.childCount;
            inventorySlots = new InventorySlot[childCount];

            for (int i = 0; i < childCount; i++)
            {
                inventorySlots[i] = layoutGroup.transform.GetChild(i).GetComponent<InventorySlot>();
            }
        }

        private void GetItem(object sender, InventoryManager.OnGetItemEventArgs args)
        {
            inventorySlots[args.Slot].AddItem(args.Item);
        }

        private void OpenInventory(object sender, InventoryManager.OnInventoryOpenedEventArgs args)
        {
            animator.SetBool("Open", true);

            EventSystem.current.SetSelectedGameObject(inventorySlots[0].gameObject);
        }

        private void CloseInventory(object sender, InventoryManager.OnInventoryOpenedEventArgs args)
        {
            animator.SetBool("Open", false);
        }
    }
}