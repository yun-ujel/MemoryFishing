using System.Collections.Generic;

using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class KuwaharaPass : ScriptableRenderPass
{
    public KuwaharaPass(KuwaharaPassSettings settings, Material material)
    {
        this.settings = settings;
        this.material = material;
    }

    private KuwaharaPassSettings settings;
    private Material material;

    private RTHandle rtBaseColor;

    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        RTHandle camTarget = renderingData.cameraData.renderer.cameraColorTargetHandle;
        RTHandle depthTarget = renderingData.cameraData.renderer.cameraDepthTargetHandle;

        RenderTextureDescriptor colorDescriptor = renderingData.cameraData.cameraTargetDescriptor;
        colorDescriptor.depthBufferBits = 0;

        _ = RenderingUtils.ReAllocateIfNeeded(ref rtBaseColor, colorDescriptor);

        ConfigureTarget(camTarget, depthTarget);
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (material == null)
        {
            return;
        }

        CommandBuffer cmd = CommandBufferPool.Get();
        RTHandle camTarget = renderingData.cameraData.renderer.cameraColorTargetHandle;

        using (new ProfilingScope(cmd, new ProfilingSampler("Kuwahara Filter")))
        {
            material.SetInt("_KernelSize", settings.KernelSize);

            Blitter.BlitCameraTexture(cmd, camTarget, rtBaseColor, material, 0);
            Blitter.BlitCameraTexture(cmd, rtBaseColor, camTarget);
        }

        context.ExecuteCommandBuffer(cmd);
        cmd.Clear();

        CommandBufferPool.Release(cmd);
    }

    public void Dispose()
    {
        rtBaseColor?.Release();
    }
}