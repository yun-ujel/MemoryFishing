using UnityEngine;

namespace MemoryFishing.Gameplay.Fishing.Player.EventArgs
{
    public class OnRecallBobberEventArgs : System.EventArgs
    {
        public Vector3 BobberPosition { get; private set; }
        public Vector3 Direction { get; private set; }
        public float TimeToRecall { get; private set; }

        public OnRecallBobberEventArgs(Vector3 bobberPosition, Vector3 direction, float timeToRecall)
        {
            BobberPosition = bobberPosition;
            Direction = direction;
            TimeToRecall = timeToRecall;
        }
    }
}