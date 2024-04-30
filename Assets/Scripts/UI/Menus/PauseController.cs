using UnityEngine;
using UnityEngine.InputSystem;

using MemoryFishing.Gameplay;

namespace MemoryFishing.UI.Menus
{
    public class PauseController : PlayerController
    {
        [Space, SerializeField] private bool startPaused;

        public class OnPauseEventArgs : System.EventArgs
        {
            public bool Paused { get; private set; }

            public OnPauseEventArgs(bool paused)
            {
                Paused = paused;
            }
        }

        public event System.EventHandler<OnPauseEventArgs> OnPauseEvent;

        public static PauseController Instance { get; private set; } = null;
        public bool Paused { get; private set; } = false;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(this);
                return;
            }

            Instance = this;
        }

        public override void Start()
        {
            base.Start();

            if (startPaused)
            {
                OpenPauseMenu();
                return;
            }

            ClosePauseMenu();
        }

        public override void SubscribeToInputActions()
        {
            playerInput.actions["Player/Pause"].performed += ReceivePauseInput;
            playerInput.actions["UI/Pause"].performed += ReceivePauseInput;
        }

        public override void UnsubscribeFromInputActions()
        {
            playerInput.actions["Player/Pause"].performed -= ReceivePauseInput;
            playerInput.actions["UI/Pause"].performed -= ReceivePauseInput;
        }

        private void ReceivePauseInput(InputAction.CallbackContext ctx)
        {
            if (Paused)
            {
                ClosePauseMenu();
                return;
            }

            OpenPauseMenu();
        }

        public void OpenPauseMenu()
        {
            playerInput.SwitchCurrentActionMap("UI");
            Time.timeScale = 0f;

            Paused = true;
            OnPauseEvent?.Invoke(this, new OnPauseEventArgs(true));
        }

        public void ClosePauseMenu()
        {
            playerInput.SwitchCurrentActionMap("Player");
            Time.timeScale = 1f;

            Paused = false;
            OnPauseEvent?.Invoke(this, new OnPauseEventArgs(false));
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}