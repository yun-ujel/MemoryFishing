using UnityEngine;
using UnityEngine.EventSystems;

namespace MemoryFishing.UI.Pause
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] private int startingSubmenuIndex = 0;

        [Space]

        [SerializeField] private GameObject menuOverlay;
        [SerializeField] private Menu[] submenus;

        [System.Serializable]
        public class Menu
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

        private void Start()
        {
            PauseController.Instance.OnPauseEvent += OnPaused;
        }

        private void OnPaused(object sender, PauseController.OnPauseEventArgs args)
        {
            if (args.Paused)
            {
                menuOverlay.SetActive(true);
                OpenMenu(startingSubmenuIndex);
                return;
            }
            menuOverlay.SetActive(false);
            OpenMenu(-1);
        }

        public void OpenMenu(int menu)
        {
            for (int i = 0; i < submenus.Length; i++)
            {
                if (i == menu)
                {
                    submenus[i].OpenMenu();
                    continue;
                }
                submenus[i].CloseMenu();
            }
        }
    }
}