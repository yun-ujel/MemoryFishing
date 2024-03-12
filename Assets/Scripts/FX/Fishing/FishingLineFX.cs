using UnityEngine;

using MemoryFishing.Gameplay.Fishing.Player;

namespace MemoryFishing.FX.Fishing
{
    [RequireComponent(typeof(LineRenderer))]
    public class FishingLineFX : MonoBehaviour
    {
        private LineRenderer lineRenderer;
        
        [SerializeField] private ReelingController reelingController;

        private void Start()
        {
            lineRenderer = GetComponent<LineRenderer>();
            reelingController.OnStartReelingEvent += StartReeling;
        }

        private void StartReeling(object sender, ReelingController.OnStartReelingEventArgs e)
        {

        }
    }
}