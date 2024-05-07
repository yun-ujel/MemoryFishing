using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering.Universal;

[System.Serializable]
public class KuwaharaPassSettings
{
    [field: SerializeField] public bool RunInSceneView { get; set; } = false;
    [field: SerializeField] public RenderPassEvent RenderPassEvent { get; set; } = RenderPassEvent.BeforeRenderingPostProcessing;

    [field: Header("Kuwahara Settings"), SerializeField, Range(2, 8)] public int KernelSize { get; set; } = 2;

    [field: Header("Filter Settings"), SerializeField] public RenderQueueType RenderQueueType { get; set; }
    [field: SerializeField] public LayerMask LayerMask { get; set; }

    [field: Header("Stencil Settings"), SerializeField] public StencilStateData StencilSettings { get; set; }
}