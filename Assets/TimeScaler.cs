using UnityEngine;

using MemoryFishing.UI.Menus;

public class TimeScaler : MonoBehaviour
{
    [SerializeField] private float timeScale = 1f;

    void Update()
    {
        if (PauseController.Instance.Paused)
        {
            return;
        }

        Time.timeScale = timeScale;
    }
}
