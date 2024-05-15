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
        [SerializeField] private Animator rodAnimator;

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

            SetBool("BobberOut", false);
            SetBool("Exhausted", false);

            SetFloat("Reeling", 0f);
            SetFloat("ReelingSpeed", minReelSpeed);
        }

        private void OnStartFighting(object sender, OnStartFightingEventArgs args)
        {
            isFighting = true;
            SetBool("Fighting", true);

            fish = args.FishBehaviour;
        }

        private void OnEndFighting(object sender, OnEndFightingEventArgs args)
        {
            isFighting = false;
            isExhausted = true;

            SetBool("Fighting", false);
            SetBool("Exhausted", true);
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
                SetFloat("Reeling", t);
                SetFloat("ReelingSpeed", reelingSpeed);

                return;
            }

            if (isFighting)
            {
                UpdateFighting();
                return;
            }

            if (!isBobberOut)
            {
                RotateTowards(orientation.parent.eulerAngles.y, Time.fixedDeltaTime);
                return;
            }
        }

        private void RotateTowards(float targetYRotation, float delta)
        {
            Vector3 currentRotation = orientation.eulerAngles;
            float yRotation = currentRotation.y;

            yRotation = Mathf.SmoothDampAngle(yRotation, targetYRotation, ref rotationVelocity, delta / rotationSpeed);
            SetFloat("Step", Mathf.Abs(rotationVelocity) / step);

            currentRotation.y = yRotation;
            Quaternion target = Quaternion.Euler(currentRotation);
            target.x = 0f;
            target.z = 0f;

            orientation.rotation = target;
        }

        private void UpdateFighting()
        {
            Vector3 dir = direction.GetLookDirection(fish.transform.position);

            Vector3 forward = orientation.transform.forward.ExcludeYAxis().normalized;
            Vector3 right = Quaternion.Euler(0, 90, 0) * forward;

            float x = Vector3.Dot(dir, right);
            float y = Vector3.Dot(dir, forward);

            SetFloat("X", x);
            SetFloat("Y", y);
        }

        private void OnRecall(object sender, OnRecallBobberEventArgs args)
        {
            isBobberOut = false;
            isExhausted = false;

            SetTrigger("Recall");
            SetBool("BobberOut", false);
            SetBool("Exhausted", false);
        }

        private void OnStartWindUp(object sender, OnStartWindUpEventArgs args)
        {
            SetTrigger("WindUp");
        }

        private void OnCastBobber(object sender, OnCastBobberEventArgs args)
        {
            isBobberOut = true;

            SetBool("BobberOut", true);
        }

        private void OnEnableFishing(object sender, OnEnableFishingEventArgs args)
        {
            isFishing = true;

            SetBool("FishingState", true);
        }

        private void OnDisableFishing(object sender, OnEnableFishingEventArgs args)
        {
            isFishing = false;

            SetBool("FishingState", false);
        }

        private void SetBool(string name, bool value)
        {
            animator.SetBool(name, value);
            rodAnimator.SetBool(name, value);
        }

        private void SetFloat(string name, float value)
        {
            animator.SetFloat(name, value);
            rodAnimator.SetFloat(name, value);
        }

        private void SetTrigger(string name)
        {
            animator.SetTrigger(name);
            rodAnimator.SetTrigger(name);
        }
    }
}