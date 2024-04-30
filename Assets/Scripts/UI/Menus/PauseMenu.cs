using UnityEngine;
using UnityEngine.EventSystems;

namespace MemoryFishing.UI.Menus
{
    public class PauseMenu : MainMenu
    {
        [SerializeField] protected GameObject menuOverlay;

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
    }
}