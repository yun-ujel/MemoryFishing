using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using MemoryFishing.Gameplay.Inventory;

namespace MemoryFishing.UI.Inventory
{
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField] private InventoryManager inventoryManager;
        [SerializeField] private Animator animator;

        [SerializeField] private HorizontalLayoutGroup layoutGroup;
        private GameObject[] inventorySlots;

        private void Start()
        {
            inventoryManager.OnInventoryOpenedEvent += OpenInventory;
            inventoryManager.OnInventoryClosedEvent += CloseInventory;

            int childCount = layoutGroup.transform.childCount;
            inventorySlots = new GameObject[childCount];

            for (int i = 0; i < childCount; i++)
            {
                inventorySlots[i] = layoutGroup.transform.GetChild(i).gameObject;
            }
        }

        private void OpenInventory(object sender, InventoryManager.OnInventoryOpenedEventArgs args)
        {
            animator.SetBool("Open", true);

            EventSystem.current.SetSelectedGameObject(inventorySlots[0]);
        }

        private void CloseInventory(object sender, InventoryManager.OnInventoryOpenedEventArgs args)
        {
            animator.SetBool("Open", false);
        }
    }
}