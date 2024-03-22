using UnityEngine;

namespace MemoryFishing.Gameplay.Fishing.Player.EventArgs
{
    public class OnStartWindUpEventArgs : System.EventArgs
    {
        public Vector3 Direction { get; private set; }
        public float TimeToWindUp { get; private set; }
        public AnimationCurve WindUpCurve { get; private set; }

        public float StartingCastDistance { get; private set; }
        public float CastDistance { get; private set; }

        public OnStartWindUpEventArgs(Vector3 direction, float timeToWindUp, AnimationCurve windUpCurve, float startingCastDistance, float castDistance)
        {
            Direction = direction;
            TimeToWindUp = timeToWindUp;
            WindUpCurve = windUpCurve;
            StartingCastDistance = startingCastDistance;
            CastDistance = castDistance;
        }
    }
}