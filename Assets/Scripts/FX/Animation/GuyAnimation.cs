using UnityEngine;

using MemoryFishing.Gameplay;
using MemoryFishing.Gameplay.Fishing.Player;
using MemoryFishing.Gameplay.Fishing.Player.EventArgs;
using MemoryFishing.Gameplay.Fishing.Fish;

using MemoryFishing.Utilities;
using static MemoryFishing.Utilities.VectorUtils;

namespace MemoryFishing.FX.Animation
{
    public class GuyAnimation : MonoBehaviour
    {
        [Header("<u>References</u>")]
        [SerializeField] private Animator animator;

        [Header("<u>Fishing</u>")]
        [SerializeField] private PlayerFishingManager fishingManager;
        private ReelingController reelingController;
        private FishBehaviour fish;

        [Header("<u>Rotation</u>")]
        [SerializeField] private Transform orientation;
        [SerializeField] private PlayerDirection direction;

        [Space, SerializeField, Range(0f, 1f)] private float rotationSpeed;

        [Header("<u>Reeling</u>")]
        [SerializeField] private float minReelSpeed = 0.5f;
        [SerializeField] private float maxReelSpeed = 2.5f;

        [Header("<u>Step</u>")]
        [SerializeField] private float step;

        private float rotationVelocity;

        private bool isFishing;
        private bool isFighting;
        private bool isBobberOut;
        private bool isExhausted;

        private void Start()
        {
            reelingController = fishingManager.GetComponent<ReelingController>();

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
            isBobberOut = false;
            isExhausted = false;

            animator.SetBool("BobberOut", false);
            animator.SetBool("Exhausted", false);

            animator.SetFloat("Reeling", 0f);
            animator.SetFloat("ReelingSpeed", minReelSpeed);
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
            isExhausted = true;

            animator.SetBool("Fighting", false);
            animator.SetBool("Exhausted", true);
        }

        private void FixedUpdate()
        {
            if (isFishing && !isBobberOut)
            {
                Vector3 dir = Quaternion.Euler(0, 90, 0) * direction.GetLookDirection(transform.position);
                float angle = VectorToDegrees(dir);

                bool flipAngle = dir.z > 0f;
                angle = flipAngle ? (360f - angle) : angle;

                RotateTowards(angle, Time.fixedDeltaTime);

                return;
            }

            if (isExhausted)
            {
                float t = reelingController.CurrentReelSpeed / reelingController.MaxReelSpeed;
                float reelingSpeed = Mathf.Lerp(minReelSpeed, maxReelSpeed, t);
                animator.SetFloat("Reeling", t);
                animator.SetFloat("ReelingSpeed", reelingSpeed);

                return;
            }

            if (isFighting)
            {
                UpdateFighting();
                return;
            }
        }

        private void RotateTowards(float targetYRotation, float delta)
        {
            Vector3 currentRotation = orientation.localRotation.eulerAngles;
            float yRotation = currentRotation.y;

            yRotation = Mathf.SmoothDampAngle(yRotation, targetYRotation, ref rotationVelocity, delta / rotationSpeed);
            animator.SetFloat("Step", Mathf.Abs(rotationVelocity) / step);

            currentRotation.y = yRotation;
            orientation.localRotation = Quaternion.Euler(currentRotation);
        }

        private void UpdateFighting()
        {
            Vector3 dir = direction.GetLookDirection(fish.transform.position);

            Vector3 forward = orientation.transform.forward.ExcludeYAxis().normalized;
            Vector3 right = Quaternion.Euler(0, 90, 0) * forward;

            float x = Vector3.Dot(dir, right);
            float y = Vector3.Dot(dir, forward);

            animator.SetFloat("X", x);
            animator.SetFloat("Y", y);
        }

        private void OnRecall(object sender, OnRecallBobberEventArgs args)
        {
            isBobberOut = false;
            isExhausted = false;

            animator.SetTrigger("Recall");
            animator.SetBool("BobberOut", false);
            animator.SetBool("Exhausted", false);
        }

        private void OnStartWindUp(object sender, OnStartWindUpEventArgs args)
        {
            animator.SetTrigger("WindUp");
        }

        private void OnCastBobber(object sender, OnCastBobberEventArgs args)
        {
            isBobberOut = true;

            animator.SetBool("BobberOut", true);
        }

        private void OnEnableFishing(object sender, OnEnableFishingEventArgs args)
        {
            isFishing = true;

            animator.SetBool("FishingState", true);
        }

        private void OnDisableFishing(object sender, OnEnableFishingEventArgs args)
        {
            isFishing = false;

            animator.SetBool("FishingState", false);
        }
    }
}