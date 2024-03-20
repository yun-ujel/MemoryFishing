using UnityEngine;

using MemoryFishing.Gameplay.Fishing.Player;
using MemoryFishing.Utilities;
using MemoryFishing.Gameplay.Fishing.Fish;
using MemoryFishing.Gameplay.Fishing.Player.EventArgs;

namespace MemoryFishing.Gameplay.Fishing
{
    public class FishingSpot : MonoBehaviour
    {
        [SerializeField] private PlayerFishingManager fishingManager;
        [SerializeField] private BobberCastController bobberCastController;
        
        [Space, SerializeField] private GameObject fishPrefab;
        private FishBehaviour fishBehaviour;

        [Space, SerializeField, Range(0f, 10f)] private float spotRadius;

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, spotRadius);
        }
#endif

        private void Start()
        {
            fishingManager.OnBobberLandEvent += OnBobberLand;
            fishBehaviour = fishPrefab.GetComponent<FishBehaviour>();
        }

        private void OnBobberLand(object sender, OnBobberLandEventArgs args)
        {
            float sqrDist = VectorUtils.SqrDistance(args.BobberPosition, transform.position);
            
            if (sqrDist <= Mathf.Pow(spotRadius, 2))
            {
                InstantiateFish(args.BobberPosition);
            }
        }

        private void InstantiateFish(Vector3 bobberPos)
        {
            FishBehaviour newFish = Instantiate(fishBehaviour, transform.position, Quaternion.identity);

            bobberCastController.FishGainedInterest(newFish, newFish.GetApproachTime(bobberPos));
        }
    }
}