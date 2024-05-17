using System.Collections;
using UnityEngine;

using MemoryFishing.Gameplay;
using MemoryFishing.Gameplay.Fishing.Player;
using MemoryFishing.Gameplay.Fishing.Player.EventArgs;
using MemoryFishing.Gameplay.Fishing.Fish;

using MemoryFishing.FX.Camera;

using MemoryFishing.Utilities;
using MemoryFishing.UI.Dialogue;

using DS;
using MemoryFishing.FX.Animation;

namespace MemoryFishing.FX.Cutscenes
{
    public class StartCutsceneManager : MonoBehaviour
    {
        [Header("Dialogue")]
        [SerializeField] private DialogueController dialogueController;

        [Space]

        [Space, SerializeField] private DSDialogue startDialogue;

        [Header("References")]
        [SerializeField] private PlayerFishingManager fishingManager;
        [SerializeField] private PlayerManager playerManager;

        private BobberCastController bobberCaster;
        private FishFightController fishFighter;

        [Space, SerializeField] private PaulFish paulFish;
        [SerializeField] private BoatMovement boat;
        [SerializeField] private StartCutsceneCameraTracker cutsceneCameraTracker;

        [Space, SerializeField] private GameObject fishingSpots;

        [Header("Boat")]
        [SerializeField] private Vector3 boatStartPosition = new(21, 0, 23);
        [SerializeField] private Vector3 boatStartRotation = new(0, -155, 0);

        [Header("Animation")]
        [SerializeField] private GuyAnimation guyAnimation;
        [SerializeField] private FerrymanAnimation ferrymanAnimation;

        [Space]

        [SerializeField] private Animator guyAnimator;
        [SerializeField] private Transform rodTransform;

        private void Start()
        {
            bobberCaster = fishingManager.GetComponent<BobberCastController>();
            fishFighter = fishingManager.GetComponent<FishFightController>();

            boat.transform.SetPositionAndRotation(boatStartPosition, Quaternion.Euler(boatStartRotation));

            playerManager.SwitchToEmptyState();
            playerManager.EnablePlayerStateSwitching = false;

            fishingManager.OnEndFightingEvent += OnEndFighting;
            fishingManager.OnCatchFishEvent += OnPaulFishCaught;

            guyAnimation.enabled = false;
            guyAnimator.Play("Hide");
            ferrymanAnimation.EnableFishing();

            rodTransform.SetParent(ferrymanAnimation.transform.parent, false);

            _ = StartCoroutine(PlayCutscene());
        }

        private void OnEndFighting(object sender, OnEndFightingEventArgs args)
        {
            ferrymanAnimation.EndFighting();

            fishingManager.OnEndFightingEvent -= OnEndFighting;
        }

        private void OnPaulFishCaught(object sender, OnCatchFishEventArgs args)
        {
            dialogueController.ReadDialogue(startDialogue.dialogue);

            ferrymanAnimation.CatchFish();
            ferrymanAnimation.DisableFishing();

            guyAnimator.Play("ClimbUp");
            guyAnimation.enabled = true;

            playerManager.EnablePlayerStateSwitching = true;
            playerManager.SwitchToBoatState();

            fishingManager.OnCatchFishEvent -= OnPaulFishCaught;
            dialogueController.OnCloseDialogueEvent += OnStartDialogueClosed;
        }

        private void OnStartDialogueClosed(object sender, DialogueController.OnCloseDialogueEventArgs args)
        {
            fishingSpots.SetActive(true);

            rodTransform.SetParent(guyAnimator.transform.parent, false);

            dialogueController.OnCloseDialogueEvent -= OnStartDialogueClosed;
        }

        private IEnumerator PlayCutscene()
        {
            boat.SetMoveInput(new(0, 1));
            ferrymanAnimation.SetRotateTarget(Vector3.zero);

            yield return new WaitForSeconds(6f);

            boat.SetMoveInput(new(0, 0));
            ferrymanAnimation.StartWindUp();
            ferrymanAnimation.SetRotateTarget(Vector3.zero);

            yield return new WaitForSeconds(2f);
            CastBobber(Vector3.zero);
            ferrymanAnimation.CastBobber();

            yield return new WaitForSeconds(1f);

            cutsceneCameraTracker.UseTracker = false;
            playerManager.SwitchToFishingState();
            fishFighter.StartFighting(paulFish, true);
            ferrymanAnimation.StartFighting(paulFish);
        }

        private void CastBobber(Vector3 targetPosition)
        {
            Vector3 direction = targetPosition - bobberCaster.transform.position;
            direction.DirectionMagnitude(out Vector3 normalized, out float magnitude);

            bobberCaster.CastBobber(normalized, magnitude);
        }
    }
}