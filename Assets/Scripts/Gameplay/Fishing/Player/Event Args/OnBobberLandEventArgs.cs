using UnityEngine;

namespace MemoryFishing.Gameplay.Fishing.Player.EventArgs
{
    public class OnBobberLandEventArgs : System.EventArgs
    {
        public Vector3 BobberPosition { get; private set; }

        public OnBobberLandEventArgs(Vector3 bobberPosition)
        {
            BobberPosition = bobberPosition;
        }
    }
}