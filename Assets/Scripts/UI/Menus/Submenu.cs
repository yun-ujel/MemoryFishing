using UnityEngine.EventSystems;
using UnityEngine;

namespace MemoryFishing.UI.Menus
{
    [System.Serializable]
    public class Submenu
    {
        [SerializeField] private GameObject menuParent;
        [SerializeField] private GameObject firstSelected;

        public void OpenMenu()
        {
            menuParent.SetActive(true);
            EventSystem.current.SetSelectedGameObject(firstSelected);
        }

        public void CloseMenu()
        {
            menuParent.SetActive(false);
        }
    }
}