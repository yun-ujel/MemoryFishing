using UnityEngine.EventSystems;
using UnityEngine;

namespace MemoryFishing.UI.Menus
{
    [System.Serializable]
    public class Submenu
    {
        [SerializeField] private GameObject menuParent;
        [SerializeField] private GameObject firstSelected;

        [field: Space, SerializeField] public int previousMenuIndex;

        public void Open()
        {
            menuParent.SetActive(true);
            EventSystem.current.SetSelectedGameObject(firstSelected);
        }

        public void Close()
        {
            menuParent.SetActive(false);
        }
    }
}