using UnityEngine;

using MemoryFishing.Gameplay.Fishing.Player;
using MemoryFishing.Utilities;
using MemoryFishing.Gameplay.Fishing.Fish;
using MemoryFishing.Gameplay.Fishing.Player.EventArgs;

namespace MemoryFishing.Gameplay.Fishing
{
    public class FishingSpot : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerFishingManager fishingManager;
        [SerializeField] private BobberCastController bobberCastController;

        [Space, SerializeField] private GameObject fishPrefab;
        private FishBehaviour fishBehaviour;

        [Header("Spot Settings")]
        [SerializeField, Range(0f, 10f)] private float spotRadius;
        private bool bobberInRange;
        private Vector3 bobberPos;

        [Space, SerializeField, Range(0f, 2f)] private float timeToAppear = 0.5f;
        private float fishAppearCounter;

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, spotRadius);
        }
#endif

        private void Start()
        {
            fishingManager.OnBobberLandEvent += OnBobberLand;
            fishingManager.OnRecallBobberEvent += OnRecallBobber;
            fishBehaviour = fishPrefab.GetComponent<FishBehaviour>();
        }

        private void Update()
        {
            if (!bobberInRange)
            {
                return;
            }

            fishAppearCounter -= Time.deltaTime;

            if (fishAppearCounter <= 0)
            {
                InstantiateFish(bobberPos);
            }
        }

        private void OnBobberLand(object sender, OnBobberLandEventArgs args)
        {
            float sqrDist = VectorUtils.SqrDistance(args.BobberPosition, transform.position);
            
            if (sqrDist <= Mathf.Pow(spotRadius, 2))
            {
                bobberInRange = true;
                fishAppearCounter = timeToAppear;

                bobberPos = args.BobberPosition;
            }
        }

        private void OnRecallBobber(object sender, OnRecallBobberEventArgs args)
        {
            bobberInRange = false;
        }

        private void InstantiateFish(Vector3 bobberPos)
        {
            FishBehaviour newFish = Instantiate(fishBehaviour, transform.position, Quaternion.identity);

            bobberCastController.StartFishApproaching(newFish, newFish.GetApproachTime(bobberPos));

            bobberInRange = false;
        }
    }
}