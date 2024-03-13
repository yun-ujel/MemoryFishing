using UnityEngine;
using UnityEngine.InputSystem;

namespace MemoryFishing.Utilities
{
    public static class VectorUtils
    {
        public static float SqrDistance(Vector3 target, Vector3 current)
        {
            return (target - current).sqrMagnitude;
        }

        public static Vector3 OnZAxis(this Vector2 vector)
        {
            return new Vector3(vector.x, 0f, vector.y);
        }

        public static Vector2 OnYAxis(this Vector3 vector)
        {
            return new Vector2(vector.x, vector.z);
        }

        public static Vector3 ExcludeYAxis(this Vector3 vector)
        {
            return new Vector3(vector.x, 0f, vector.z);
        }

        public static Vector2 DegreesToVector(float angle)
        {
            float radians = angle * Mathf.Deg2Rad;

            return new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
        }

        public static float VectorToDegrees(Vector2 vector)
        {
            float radians = Mathf.Acos(vector.x);
            return radians * Mathf.Rad2Deg;
        }
    }
}