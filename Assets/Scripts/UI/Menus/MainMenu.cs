using UnityEngine;
using UnityEngine.SceneManagement;

namespace MemoryFishing.UI.Menus
{
    public class MainMenu : Menu
    {
        [SerializeField] private int firstScene = 1;
        public void StartGame()
        {
            SceneManager.LoadScene(firstScene);
        }
    }
}