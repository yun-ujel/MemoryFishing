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

        public static float Remap(this float input, float inputMin, float inputMax, float outputMin, float outputMax)
        {
            return (input - inputMin) / (inputMax - inputMin) * (outputMax - outputMin) + outputMin;
        }

        public static float Remap01(this float input, float inputMin, float inputMax)
        {
            return Remap(input, inputMin, inputMax, 0.0f, 1.0f);
        }

        public static bool IsInRangeOf(this int value, int length)
        {
            return value >= 0 && value < length;
        }

        public static bool IsInRangeOf(this int value, object[] array)
        {
            return IsInRangeOf(value, array.Length);
        }

        public static bool CoinFlip()
        {
            return Random.Range(0, 2) == 0;
        }
    }
}