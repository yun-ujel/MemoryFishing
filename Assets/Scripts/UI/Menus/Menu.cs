using MemoryFishing.Gameplay;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MemoryFishing.UI.Menus
{
    public class Menu : PlayerController
    {
        [SerializeField] protected int startingSubmenuIndex = 0;
        protected int currentMenuIndex = 0;

        [Space]

        [SerializeField] protected Submenu[] submenus;

        public override void Start()
        {
            base.Start();

            OpenSubmenu(startingSubmenuIndex);
        }

        public void OpenSubmenu(int submenu)
        {
            currentMenuIndex = submenu;

            for (int i = 0; i < submenus.Length; i++)
            {
                if (i == submenu)
                {
                    submenus[i].Open();
                    continue;
                }
                submenus[i].Close();
            }
        }

        public virtual void GoBack()
        {
            if (currentMenuIndex < 0 || currentMenuIndex >= submenus.Length)
            {
                return;
            }

            OpenSubmenu(submenus[currentMenuIndex].previousMenuIndex);
        }

        public override void SubscribeToInputActions()
        {
            playerInput.actions["UI/Cancel"].performed += ReceiveCancelInput;
        }

        public override void UnsubscribeFromInputActions()
        {
            playerInput.actions["UI/Cancel"].performed -= ReceiveCancelInput;
        }

        protected void ReceiveCancelInput(InputAction.CallbackContext ctx)
        {
            GoBack();
        }
    }
}