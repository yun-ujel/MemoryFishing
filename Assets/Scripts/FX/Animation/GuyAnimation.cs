using UnityEngine;

using MemoryFishing.Gameplay;
using MemoryFishing.Gameplay.Fishing.Player;
using MemoryFishing.Gameplay.Fishing.Player.EventArgs;

namespace MemoryFishing.FX.Animation
{
    public class GuyAnimation : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Animator animator;

        [Space, SerializeField] private PlayerManager playerManager;
        [SerializeField] private PlayerFishingManager fishingManager;

        [Space, SerializeField] private PlayerDirection direction;

        private void Start()
        {
            fishingManager.OnDisableFishingEvent += OnDisableFishing;
            fishingManager.OnEnableFishingEvent += OnEnableFishing;

            fishingManager.OnStartWindUpEvent += OnStartWindUp;
            fishingManager.OnCastBobberEvent += OnCastBobber;

            fishingManager.OnRecallBobberEvent += OnRecall;
        }

        private void OnRecall(object sender, OnRecallBobberEventArgs args)
        {
            animator.SetTrigger("Recall");
        }

        private void OnStartWindUp(object sender, OnStartWindUpEventArgs args)
        {
            animator.SetTrigger("WindUp");
            animator.SetBool("BobberOut", false);
        }

        private void OnCastBobber(object sender, OnCastBobberEventArgs args)
        {
            animator.SetBool("BobberOut", true);
        }

        private void OnEnableFishing(object sender, OnEnableFishingEventArgs args)
        {
            animator.SetBool("FishingState", true);
        }

        private void OnDisableFishing(object sender, OnEnableFishingEventArgs args)
        {
            animator.SetBool("FishingState", false);
            animator.SetBool("BobberOut", false);
        }
    }
}