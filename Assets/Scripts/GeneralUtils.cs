using UnityEngine;
using UnityEngine.InputSystem;

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

    public static Vector3 OnZAxis(this Vector2 vector)
    {
        return new Vector3(vector.x, 0f, vector.y);
    }

    public static Vector2 DegreesToVector(float angle)
    {
        float radians = angle * Mathf.Deg2Rad;

        return new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
    }
}
