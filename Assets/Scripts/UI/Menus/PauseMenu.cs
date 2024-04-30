using UnityEngine;
using UnityEngine.EventSystems;

namespace MemoryFishing.UI.Menus
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] private int startingSubmenuIndex = 0;

        [Space]

        [SerializeField] private GameObject menuOverlay;
        [SerializeField] private Submenu[] submenus;

        private void Start()
        {
            PauseController.Instance.OnPauseEvent += OnPaused;
            TogglePause(PauseController.Instance.Paused);
        }

        private void OnPaused(object sender, PauseController.OnPauseEventArgs args)
        {
            TogglePause(args.Paused);
        }

        public void TogglePause(bool paused)
        {
            if (paused)
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