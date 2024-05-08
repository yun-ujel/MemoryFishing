using UnityEngine;
using UnityEngine.EventSystems;

namespace MemoryFishing.UI.Menus
{
    public class PauseMenu : Menu
    {
        [SerializeField] protected GameObject menuOverlay;

        public override void Start()
        {
            base.Start();

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
                OpenSubmenu(startingSubmenuIndex);
                return;
            }
            menuOverlay.SetActive(false);
            OpenSubmenu(-1);
        }

        public override void GoBack()
        {
            if (currentMenuIndex < 0 || currentMenuIndex >= submenus.Length)
            {
                return;
            }

            if (submenus[currentMenuIndex].previousMenuIndex < 0)
            {
                PauseController.Instance.ClosePauseMenu();
            }

            base.GoBack();
        }
    }
}