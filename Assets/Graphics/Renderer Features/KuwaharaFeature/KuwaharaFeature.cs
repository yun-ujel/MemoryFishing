using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class KuwaharaFeature : ScriptableRendererFeature
{
    [SerializeField] private KuwaharaPassSettings settings;

    private Material material;
    private KuwaharaPass kuwaharaPass;

    public override void Create()
    {
        material = CoreUtils.CreateEngineMaterial("Screen/Kuwahara");
        kuwaharaPass = new KuwaharaPass(settings, material)
        {
            renderPassEvent = settings.RenderPassEvent
        };
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
#if UNITY_EDITOR
        if (settings.RunInSceneView && renderingData.cameraData.cameraType == CameraType.SceneView)
        {
            renderer.EnqueuePass(kuwaharaPass);
            return;
        }
#endif
        if (renderingData.cameraData.cameraType == CameraType.Game)
        {
            renderer.EnqueuePass(kuwaharaPass);
        }
    }

    protected override void Dispose(bool disposing)
    {
        kuwaharaPass.Dispose();
    }
}