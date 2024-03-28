using UnityEngine;
using UnityEngine.UI;

using MemoryFishing.Gameplay.Fishing.Player;
using MemoryFishing.Gameplay.Fishing.Fish;
using MemoryFishing.Utilities;
using MemoryFishing.Gameplay;
using MemoryFishing.Gameplay.Fishing.Player.EventArgs;

namespace MemoryFishing.UI.Fishing
{
    public class FightDirectionIndicator : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerFishingManager fishingManager;
        [SerializeField] private PlayerDirection playerDirection;

        [Header("Display")]
        [SerializeField] private Vector3 offset;
        [SerializeField, Range(0f, 100f)] private float arrowAcceleration;

        [Space]

        [SerializeField] private Transform fishParent;
        [SerializeField] private Image fishArrow;

        private float FishAlpha
        {
            get => fishArrow.color.a;
            set
            {
                Color color = fishArrow.color;
                color.a = value;

                fishArrow.color = color;
            }
        }

        [Space]

        [SerializeField] private Transform playerParent;
        [SerializeField] private Image playerArrow;

        private float PlayerAlpha
        {
            get => playerArrow.color.a;
            set
            {
                Color color = playerArrow.color;
                color.a = value;

                playerArrow.color = color;
            }
        }

        private FishBehaviour fishBehaviour;
        private Vector3 currentFishDirection;

        private bool isFighting;
        private bool isFishing;

        private void Start()
        {
            fishingManager.OnStartFightingEvent += OnStartFighting;
            fishingManager.OnEndFightingEvent += OnEndFighting;

            fishingManager.OnCatchFishEvent += Hide;
        }

        private void Hide(object sender, OnCatchFishEventArgs args)
        {
            isFishing = false;
        }

        private void OnStartFighting(object sender, OnStartFightingEventArgs args)
        {
            isFighting = true;
            isFishing = true;

            fishBehaviour = args.FishBehaviour;
        }

        private void OnEndFighting(object sender, OnEndFightingEventArgs args)
        {
            isFighting = false;
        }

        private void Update()
        {
            AnimateArrows(Time.deltaTime);
        }

        private void AnimateArrows(float delta)
        {
            if (!isFishing)
            {
                PlayerAlpha = Mathf.MoveTowards(PlayerAlpha, 0f, delta * 10f);
                FishAlpha = Mathf.MoveTowards(FishAlpha, 0f, delta * 10f);
                return;
            }

            PlayerAlpha = Mathf.MoveTowards(PlayerAlpha, 1f, delta * 10f);

            if (isFighting)
            {
                FishAlpha = Mathf.MoveTowards(FishAlpha, 1f, delta * 10f);
                
                currentFishDirection = Vector3.MoveTowards(currentFishDirection, fishBehaviour.GetFishDirection(), delta * arrowAcceleration);
                fishParent.forward = currentFishDirection;
            }
            else
            {
                FishAlpha = Mathf.MoveTowards(FishAlpha, 0f, delta * 10f);
            }

            transform.position = fishBehaviour.transform.position + offset;
            playerParent.forward = playerDirection.GetLookDirection(fishBehaviour.transform.position);
        }
    }
}