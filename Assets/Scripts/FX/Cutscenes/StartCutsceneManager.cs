using System.Collections;
using UnityEngine;

using MemoryFishing.Gameplay;
using MemoryFishing.Gameplay.Fishing.Player;
using MemoryFishing.Utilities;
using MemoryFishing.Gameplay.Fishing.Fish;

namespace MemoryFishing.FX.Cutscenes
{
    public class StartCutsceneManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private BoatMovement boat;

        [Space]

        [SerializeField] private PlayerFishingManager playerFishingManager;
        [SerializeField] private PlayerManager playerManager;

        private BobberCastController bobberCaster;
        private FishFightController fishFighter;

        [Space]

        [SerializeField] private PaulFish paulFish;

        [Header("Boat")]
        [SerializeField] private Vector3 boatStartPosition = new(21, 0, 23);
        [SerializeField] private Vector3 boatStartRotation = new(0, -155, 0);

        private void Start()
        {
            bobberCaster = playerFishingManager.GetComponent<BobberCastController>();
            fishFighter = playerFishingManager.GetComponent<FishFightController>();

            boat.transform.SetPositionAndRotation(boatStartPosition, Quaternion.Euler(boatStartRotation));

            playerManager.SwitchToEmptyState();
            playerManager.EnablePlayerStateSwitching = false;

            _ = StartCoroutine(PlayCutscene());
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