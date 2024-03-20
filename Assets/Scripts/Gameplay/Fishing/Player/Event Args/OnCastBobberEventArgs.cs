using UnityEngine;

namespace MemoryFishing.Gameplay.Fishing.Player.EventArgs
{
    public class OnCastBobberEventArgs : System.EventArgs
    {
        public Vector3 TargetPosition { get; private set; }
        public Vector3 Direction { get; private set; }
        public float Magnitude { get; private set; }

        public float TimeToLand { get; private set; }

        public OnCastBobberEventArgs(Vector3 targetPosition, Vector3 direction, float magnitude, float timeToLand)
        {
            TargetPosition = targetPosition;
            Direction = direction;
            Magnitude = magnitude;
            TimeToLand = timeToLand;
        }
    }
}