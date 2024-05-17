using MemoryFishing.FX.Animation;
using MemoryFishing.Gameplay.Fishing.Fish;
using MemoryFishing.Gameplay.Fishing.Player;
using UnityEngine;

public class FerrymanAnimation : GuyAnimation
{
    private Vector3 rotateDir;

    protected override void Start()
    {
        reelingController = fishingManager.GetComponent<ReelingController>();
    }

    public void CatchFish()
    {
        OnCatchFish();
    }

    public void StartFighting(FishBehaviour fishBehaviour)
    {
        OnStartFighting(fishBehaviour);
    }

    public void EndFighting()
    {
        OnEndFighting();
    }

    public void StartWindUp()
    {
        OnStartWindUp();
    }

    public void CastBobber()
    {
        OnCastBobber();
    }

    public void EnableFishing()
    {
        OnEnableFishing();
    }

    public void DisableFishing()
    {
        OnDisableFishing();
    }

    public void SetRotateTarget(Vector3 target)
    {
        rotateDir = (target - transform.position).normalized;
    }

    protected override Vector3 GetRotateDirection()
    {
        return rotateDir;
    }
}
