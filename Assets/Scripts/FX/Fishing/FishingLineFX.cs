using UnityEngine;

using MemoryFishing.Gameplay.Fishing.Player;
using MemoryFishing.Gameplay.Fishing.Fish;

namespace MemoryFishing.FX.Fishing
{
    [RequireComponent(typeof(LineRenderer))]
    public class FishingLineFX : MonoBehaviour
    {
        private LineRenderer line;
        
        [SerializeField] private ReelingController reelingController;

        [Space]

        [SerializeField] private AnimationCurve lineCurve;

        private Transform player;
        private Transform fish;

        private void Start()
        {
            line = GetComponent<LineRenderer>();
            reelingController.OnStartReelingEvent += StartReeling;
        }

        private void StartReeling(object sender, ReelingController.OnStartReelingEventArgs args)
        {
            MonoBehaviour reelingController = (MonoBehaviour)sender;

            player = reelingController.transform;
            fish = args.FishBehaviour.transform;
        }

        private void Update()
        {
            SetLinePositions(player.position, fish.position);
        }

        private void SetLinePositions(Vector3 playerPos, Vector3 fishPos)
        {
            float t = 0;
            float curveT = 0;

            for (int i = 0; i < line.positionCount; i++)
            {
                t = (float)i / (line.positionCount - 1);
                curveT = lineCurve.Evaluate(t);

                Vector3 pos = Vector3.Lerp(playerPos, fishPos, t);
                pos.y = Mathf.Lerp(playerPos.y, fishPos.y, curveT);

                line.SetPosition(i, pos);
            }
        }
    }
}