using UnityEngine;
using UnityEngine.UI;

using MemoryFishing.Gameplay.Fishing.Player;
using MemoryFishing.Gameplay.Fishing.Fish;
using MemoryFishing.Utilities;
using MemoryFishing.Gameplay;

namespace MemoryFishing.UI.Fishing
{
    public class ReelDirectionIndicator : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ReelingController reelingController;
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

        private bool isReeling;
        private bool isFishing;

        private void Start()
        {
            reelingController.OnStartReelingEvent += OnStartReeling;
            reelingController.OnEndReelingEvent += OnEndReeling;
        }

        private void OnStartReeling(object sender, ReelingController.OnStartReelingEventArgs args)
        {
            isReeling = true;
            isFishing = true;

            fishBehaviour = args.FishBehaviour;
        }

        private void OnEndReeling(object sender, ReelingController.OnEndReelingEventArgs args)
        {
            isReeling = false;
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
                return;
            }

            PlayerAlpha = Mathf.MoveTowards(PlayerAlpha, 1f, delta * 10f);

            if (isReeling)
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