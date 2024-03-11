using UnityEngine;
using UnityEngine.InputSystem;

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

        public virtual void SubscribeToInputActions()
        {

        }
    }
}