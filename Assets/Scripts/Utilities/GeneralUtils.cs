using UnityEngine;
using UnityEngine.InputSystem;

namespace MemoryFishing.Utilities
{
    public static class GeneralUtils
    {
        public static string PlayerInputTag { get; } = "PlayerInput";

        private static PlayerInput cachedPlayerInput;

        public static PlayerInput GetPlayerInput()
        {
            if (cachedPlayerInput == null)
            {
                cachedPlayerInput = GameObject.FindWithTag(PlayerInputTag).GetComponent<PlayerInput>();
            }

            return cachedPlayerInput;
        }
    }
}