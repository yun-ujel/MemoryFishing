using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MemoryFishing.FX.Animation
{
    public class OnTossFinished : StateMachineBehaviour
    {
        public event System.EventHandler<System.EventArgs> OnTossFinishedEvent;

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);

            OnTossFinishedEvent?.Invoke(this, new());
        }
    }
}