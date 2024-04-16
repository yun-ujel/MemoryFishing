using UnityEngine;
using UnityEngine.InputSystem;

using MemoryFishing.Utilities;

namespace MemoryFishing.Gameplay
{
    public abstract class PlayerController : MonoBehaviour
    {
        protected PlayerInput playerInput;

        public virtual void Start()
        {
            playerInput = GeneralUtils.GetPlayerInput();
            SubscribeToInputActions();
        }

        public abstract void SubscribeToInputActions();

        public abstract void UnsubscribeFromInputActions();
    }
}