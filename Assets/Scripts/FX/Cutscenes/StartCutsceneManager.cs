using System.Collections;
using UnityEngine;

using MemoryFishing.Gameplay;
using MemoryFishing.Gameplay.Fishing.Player;
using MemoryFishing.Utilities;

namespace MemoryFishing.FX.Cutscenes
{
    public class StartCutsceneManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private BoatMovement boat;
        [SerializeField] private BobberCastController bobberCaster;

        [Space]

        [SerializeField] private PlayerManager playerManager;

        [Header("Boat")]
        [SerializeField] private Vector3 boatStartPosition = new(21, 0, 23);
        [SerializeField] private Vector3 boatStartRotation = new(0, -155, 0);

        private void Start()
        {
            boat.transform.SetPositionAndRotation(boatStartPosition, Quaternion.Euler(boatStartRotation));

            playerManager.SwitchToEmptyState();

            _ = StartCoroutine(PlayCutscene());
        }

        private IEnumerator PlayCutscene()
        {
            boat.SetMoveInput(new(0, 1));

            yield return new WaitForSeconds(7.5f);

            boat.SetMoveInput(new(0, 0));

            yield return new WaitForSeconds(1f);
            CastBobber(Vector3.zero);
        }

        private void CastBobber(Vector3 targetPosition)
        {
            Vector3 direction = targetPosition - bobberCaster.transform.position;
            direction.DirectionMagnitude(out Vector3 normalized, out float magnitude);

            bobberCaster.CastBobber(normalized, magnitude);
        }
    }
}