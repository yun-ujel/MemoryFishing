using System.Collections;
using UnityEngine;

using MemoryFishing.Gameplay;
using MemoryFishing.Gameplay.Fishing.Player;
using MemoryFishing.Gameplay.Fishing.Player.EventArgs;
using MemoryFishing.Gameplay.Fishing.Fish;
using MemoryFishing.Utilities;
using MemoryFishing.UI.Dialogue;

using DS;

namespace MemoryFishing.FX.Cutscenes
{
    public class StartCutsceneManager : MonoBehaviour
    {
        [Header("Dialogue")]
        [SerializeField] private DialogueController dialogueController;

        [Space]

        [SerializeField] private DSDialogue startDialogue;

        [Header("References")]
        [SerializeField] private PlayerFishingManager fishingManager;
        [SerializeField] private PlayerManager playerManager;

        private BobberCastController bobberCaster;
        private FishFightController fishFighter;

        [Space]

        [SerializeField] private PaulFish paulFish;

        [Space]
        
        [SerializeField] private BoatMovement boat;

        [Header("Boat")]
        [SerializeField] private Vector3 boatStartPosition = new(21, 0, 23);
        [SerializeField] private Vector3 boatStartRotation = new(0, -155, 0);

        private void Start()
        {
            bobberCaster = fishingManager.GetComponent<BobberCastController>();
            fishFighter = fishingManager.GetComponent<FishFightController>();

            boat.transform.SetPositionAndRotation(boatStartPosition, Quaternion.Euler(boatStartRotation));

            playerManager.SwitchToEmptyState();
            playerManager.EnablePlayerStateSwitching = false;

            fishingManager.OnCatchFishEvent += OnPaulFishCaught;

            _ = StartCoroutine(PlayCutscene());
        }

        private void OnPaulFishCaught(object sender, OnCatchFishEventArgs args)
        {
            dialogueController.ReadDialogue(startDialogue.dialogue);

            fishingManager.OnCatchFishEvent -= OnPaulFishCaught;
            dialogueController.OnCloseDialogueEvent += OnStartDialogueClosed;
        }

        private void OnStartDialogueClosed(object sender, DialogueController.OnCloseDialogueEventArgs args)
        {
            playerManager.EnablePlayerStateSwitching = true;
            playerManager.SwitchToBoatState();

            dialogueController.OnCloseDialogueEvent -= OnStartDialogueClosed;
        }

        private IEnumerator PlayCutscene()
        {
            boat.SetMoveInput(new(0, 1));

            yield return new WaitForSeconds(7.5f);

            boat.SetMoveInput(new(0, 0));

            yield return new WaitForSeconds(2f);
            CastBobber(Vector3.zero);

            yield return new WaitForSeconds(1f);

            playerManager.SwitchToFishingState();
            fishFighter.StartFighting(paulFish, true);
        }

        private void CastBobber(Vector3 targetPosition)
        {
            Vector3 direction = targetPosition - bobberCaster.transform.position;
            direction.DirectionMagnitude(out Vector3 normalized, out float magnitude);

            bobberCaster.CastBobber(normalized, magnitude);
        }
    }
}