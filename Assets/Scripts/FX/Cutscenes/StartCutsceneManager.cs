using System.Collections;
using UnityEngine;

using MemoryFishing.Gameplay;
using MemoryFishing.Gameplay.Fishing.Player;
using MemoryFishing.Utilities;

namespace MemoryFishing.FX.Cutscenes
{
    public class StartCutsceneManager : MonoBehaviour
    {
        [Header("Boat")]
        [SerializeField] private BoatMovement boat;

        [Space]

        [SerializeField] private Vector3 boatStartPosition = new(21, 0, 23);
        [SerializeField] private Vector3 boatStartRotation = new(0, -155, 0);

        [Header("Bobber Cast")]
        [SerializeField] private BobberCastController bobberCast;

        private void Start()
        {
            boat.transform.position = boatStartPosition;
            boat.transform.rotation = Quaternion.Euler(boatStartRotation);

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
            Vector3 direction = targetPosition - bobberCast.transform.position;
            direction.DirectionMagnitude(out Vector3 normalized, out float magnitude);

            bobberCast.CastBobber(normalized, magnitude);
        }
    }
}