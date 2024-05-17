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
        [SerializeField] protected Animator animator;
        [SerializeField] protected Animator rodAnimator;

        [Header("<u>Fishing</u>")]
        [SerializeField] protected PlayerFishingManager fishingManager;
        protected ReelingController reelingController;
        protected FishBehaviour fish;

        [Header("<u>Rotation</u>")]
        [SerializeField] protected Transform orientation;
        [SerializeField] protected PlayerDirection direction;

        [Space, SerializeField, Range(0f, 1f)] protected float rotationSpeed;

        [Header("<u>Reeling</u>")]
        [SerializeField] protected float minReelSpeed = 0.5f;
        [SerializeField] protected float maxReelSpeed = 2.5f;

        [Header("<u>Step</u>")]
        [SerializeField] protected float step;

        protected float rotationVelocity;

        protected bool isFishing;
        protected bool isFighting;
        protected bool isBobberOut;
        protected bool isExhausted;

        protected virtual void Start()
        {
            reelingController = fishingManager.GetComponent<ReelingController>();

            fishingManager.OnDisableFishingEvent += (s, a) => OnDisableFishing();
            fishingManager.OnEnableFishingEvent += (s, a) => OnEnableFishing();

            fishingManager.OnStartWindUpEvent += (s, a) => OnStartWindUp();
            fishingManager.OnCastBobberEvent += (s, a) => OnCastBobber();

            fishingManager.OnStartFightingEvent += (s, a) => OnStartFighting(a.FishBehaviour);
            fishingManager.OnEndFightingEvent += (s, a) => OnEndFighting();

            fishingManager.OnRecallBobberEvent += (s, a) => OnRecall();
            fishingManager.OnCatchFishEvent += (s, a) => OnCatchFish();
        }

        protected virtual void OnCatchFish()
        {
            isBobberOut = false;
            isExhausted = false;

            SetBool("BobberOut", false);
            SetBool("Exhausted", false);

            SetFloat("Reeling", 0f);
            SetFloat("ReelingSpeed", minReelSpeed);
        }

        protected virtual void OnStartFighting(FishBehaviour fishBehaviour)
        {
            isFighting = true;
            SetBool("Fighting", true);

            fish = fishBehaviour;
        }

        protected virtual void OnEndFighting()
        {
            isFighting = false;
            isExhausted = true;

            SetBool("Fighting", false);
            SetBool("Exhausted", true);
        }

        protected virtual Vector3 GetRotateDirection()
        {
            return direction.GetLookDirection(transform.position);
        }

        protected virtual void FixedUpdate()
        {
            if (isFishing && !isBobberOut)
            {
                Vector3 dir = Quaternion.Euler(0, 90, 0) * GetRotateDirection();
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

        protected virtual void RotateTowards(float targetYRotation, float delta)
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

        protected virtual void UpdateFighting()
        {
            Vector3 dir = direction.GetLookDirection(fish.transform.position);

            Vector3 forward = orientation.transform.forward.ExcludeYAxis().normalized;
            Vector3 right = Quaternion.Euler(0, 90, 0) * forward;

            float x = Vector3.Dot(dir, right);
            float y = Vector3.Dot(dir, forward);

            SetFloat("X", x);
            SetFloat("Y", y);
        }

        protected virtual void OnRecall()
        {
            isBobberOut = false;
            isExhausted = false;

            SetTrigger("Recall");
            SetBool("BobberOut", false);
            SetBool("Exhausted", false);
        }

        protected virtual void OnStartWindUp()
        {
            SetTrigger("WindUp");
        }

        protected virtual void OnCastBobber()
        {
            isBobberOut = true;

            SetBool("BobberOut", true);
        }

        protected virtual void OnEnableFishing()
        {
            isFishing = true;

            SetBool("FishingState", true);
        }

        protected virtual void OnDisableFishing()
        {
            isFishing = false;

            SetBool("FishingState", false);
        }

        protected virtual void SetBool(string name, bool value)
        {
            animator.SetBool(name, value);
            rodAnimator.SetBool(name, value);
        }

        protected virtual void SetFloat(string name, float value)
        {
            animator.SetFloat(name, value);
            rodAnimator.SetFloat(name, value);
        }

        protected virtual void SetTrigger(string name)
        {
            animator.SetTrigger(name);
            rodAnimator.SetTrigger(name);
        }
    }
}