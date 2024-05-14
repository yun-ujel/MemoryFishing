using UnityEngine;

using MemoryFishing.Gameplay;
using MemoryFishing.Gameplay.Fishing.Player;
using MemoryFishing.Gameplay.Fishing.Player.EventArgs;
using MemoryFishing.Gameplay.Fishing.Fish;
using MemoryFishing.Utilities;

namespace MemoryFishing.FX.Animation
{
    public class GuyAnimation : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Animator animator;

        [Space, SerializeField] private PlayerManager playerManager;
        [SerializeField] private PlayerFishingManager fishingManager;

        [Space, SerializeField] private PlayerDirection direction;
        [SerializeField] private Transform playerOrientation;

        private bool isFighting;
        private FishBehaviour fish;

        private void Start()
        {
            fishingManager.OnDisableFishingEvent += OnDisableFishing;
            fishingManager.OnEnableFishingEvent += OnEnableFishing;

            fishingManager.OnStartWindUpEvent += OnStartWindUp;
            fishingManager.OnCastBobberEvent += OnCastBobber;

            fishingManager.OnStartFightingEvent += OnStartFighting;
            fishingManager.OnEndFightingEvent += OnEndFighting;

            fishingManager.OnRecallBobberEvent += OnRecall;
            fishingManager.OnCatchFishEvent += OnCatchFish;
        }

        private void OnCatchFish(object sender, OnCatchFishEventArgs args)
        {
            animator.SetBool("BobberOut", false);
        }

        private void OnStartFighting(object sender, OnStartFightingEventArgs args)
        {
            isFighting = true;
            animator.SetBool("Fighting", true);

            fish = args.FishBehaviour;
        }

        private void OnEndFighting(object sender, OnEndFightingEventArgs args)
        {
            isFighting = false;
            animator.SetBool("Fighting", false);

            animator.SetFloat("X", 0f);
            animator.SetFloat("Y", 0f);
        }

        private void FixedUpdate()
        {
            if (!isFighting)
            {
                return;
            }

            Vector3 dir = direction.GetLookDirection(fish.transform.position);

            Vector3 forward = playerOrientation.transform.forward.ExcludeYAxis().normalized;
            Vector3 right = Quaternion.Euler(0, 90, 0) * forward;

            float x = Vector3.Dot(dir, right);
            float y = Vector3.Dot(dir, forward);

            animator.SetFloat("X", x);
            animator.SetFloat("Y", y);
        }

        private void OnRecall(object sender, OnRecallBobberEventArgs args)
        {
            animator.SetTrigger("Recall");
            animator.SetBool("BobberOut", false);
        }

        private void OnStartWindUp(object sender, OnStartWindUpEventArgs args)
        {
            animator.SetTrigger("WindUp");
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
        }
    }
}