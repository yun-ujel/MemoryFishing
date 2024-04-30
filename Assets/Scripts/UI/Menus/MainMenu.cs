using UnityEngine;

namespace MemoryFishing.UI.Menus
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] protected int startingSubmenuIndex = 0;

        [Space]

        [SerializeField] protected Submenu[] submenus;

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