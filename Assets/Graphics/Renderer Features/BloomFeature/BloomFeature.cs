using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BloomFeature : ScriptableRendererFeature
{
    [SerializeField] private BloomPassSettings settings;

    private BloomPass bloomPass;
    private Material material;

    public override void Create()
    {
        material = CoreUtils.CreateEngineMaterial("Screen/Bloom");

        bloomPass = new BloomPass(settings, material)
        {
            renderPassEvent = settings.RenderPassEvent
        };
    }
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (renderingData.cameraData.cameraType == CameraType.Game)
        {
            renderer.EnqueuePass(bloomPass);
        }
    }

    protected override void Dispose(bool disposing)
    {
        bloomPass.Dispose();
    }
}