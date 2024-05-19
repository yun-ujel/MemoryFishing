using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BloomPass : ScriptableRenderPass
{
    public BloomPass(BloomPassSettings passSettings, Material passMaterial)
    {
        shaderTagIds = new List<ShaderTagId>();
        
        for (int i = 0; i < passSettings.PassTags.Length; i++)
        {
            shaderTagIds.Add(new ShaderTagId(passSettings.PassTags[i]));
        }

        material = passMaterial;
        settings = passSettings;

        filteringSettings = FilteringSettings.defaultValue;
    }

    private Material material;
    private BloomPassSettings settings;

    private RTHandle bloomTarget;

    private RTHandle tempColorTarget;
    private RTHandle[] downsampleTargets;
    private int downsampleCount;

    private List<ShaderTagId> shaderTagIds;

    private FilteringSettings filteringSettings;

    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        RenderTextureDescriptor colorDesc = renderingData.cameraData.cameraTargetDescriptor;
        colorDesc.depthBufferBits = 0;

        RTHandle depthTarget = renderingData.cameraData.renderer.cameraDepthTargetHandle;

        _ = RenderingUtils.ReAllocateIfNeeded(ref bloomTarget, colorDesc);
        _ = RenderingUtils.ReAllocateIfNeeded(ref tempColorTarget, colorDesc);
        SetupDownsamples(colorDesc);

        ConfigureTarget(bloomTarget, depthTarget);
        ConfigureClear(ClearFlag.Color, Color.clear);
    }

    private void SetupDownsamples(RenderTextureDescriptor colorDesc)
    {
        downsampleTargets = new RTHandle[settings.Downsamples];
        downsampleCount = settings.Downsamples;
        RenderTextureDescriptor downsampleDesc = colorDesc;

        for (int i = 0; i < settings.Downsamples; i++)
        {
            if (downsampleDesc.height < 2)
            {
                downsampleCount = i;
                break;
            }

            downsampleDesc.width /= 2;
            downsampleDesc.height /= 2;

            _ = RenderingUtils.ReAllocateIfNeeded(ref downsampleTargets[i], downsampleDesc);
        }
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (material == null)
        {
            return;
        }

        CommandBuffer cmd = CommandBufferPool.Get();

        SortingCriteria sortingCriteria = renderingData.cameraData.defaultOpaqueSortFlags;
        RTHandle cameraTarget = renderingData.cameraData.renderer.cameraColorTargetHandle;

        using (new ProfilingScope(cmd, new ProfilingSampler("Bloom Pass")))
        {
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();

            DrawingSettings drawingSettings = CreateDrawingSettings(shaderTagIds, ref renderingData, sortingCriteria);
            context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filteringSettings);

            material.SetFloat("_DownDelta", settings.DownsampleDelta);
            material.SetFloat("_UpDelta", settings.UpsampleDelta);
            material.SetFloat("_Intensity", settings.Intensity);

            cmd.SetGlobalTexture("_BloomTexture", bloomTarget);

            Blitter.BlitCameraTexture(cmd, cameraTarget, tempColorTarget, material, 0);
            Blitter.BlitCameraTexture(cmd, tempColorTarget, downsampleTargets[0], material, 1);
            
            int i = 1;
            for (; i < downsampleCount; i++)
            {
                Blitter.BlitCameraTexture(cmd, downsampleTargets[i - 1], downsampleTargets[i], material, 1);
            }

            for (i -= 2; i >= 0; i--)
            {
                Blitter.BlitCameraTexture(cmd, downsampleTargets[i + 1], downsampleTargets[i], material, 2);
            }

            cmd.SetGlobalTexture("_BloomTexture", downsampleTargets[0]);
            Blitter.BlitCameraTexture(cmd, cameraTarget, tempColorTarget);
            Blitter.BlitCameraTexture(cmd, tempColorTarget, cameraTarget, material, 3);
        }

        context.ExecuteCommandBuffer(cmd);
        cmd.Clear();
        CommandBufferPool.Release(cmd);
    }

    public void Dispose()
    {
        bloomTarget?.Release();
        tempColorTarget?.Release();

        if (downsampleTargets != null)
        {
            for (int i = 0; i < downsampleTargets.Length; i++)
            {
                downsampleTargets[i]?.Release();
            }
        }
    }
}