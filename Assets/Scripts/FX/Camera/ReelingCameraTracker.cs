using MemoryFishing.Gameplay.Enumerations;

namespace MemoryFishing.FX.Camera
{
    public class ReelingCameraTracker : FightingCameraTracker
    {
        public override bool TryTrackingConditions(PlayerState playerState, FishingState fishingState)
        {
            return fishingState == FishingState.Reeling || fishingState == FishingState.Exhausted;
        }
    }
}