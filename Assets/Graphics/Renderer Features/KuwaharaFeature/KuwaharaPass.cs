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

        shaderTagIDList = new List<ShaderTagId>
        {
            new ShaderTagId("SRPDefaultUnlit"),
            new ShaderTagId("UniversalForward"),
            new ShaderTagId("UniversalForwardOnly")
        };

        renderStateBlock = new RenderStateBlock(RenderStateMask.Nothing);

        RenderQueueRange renderQueueRange = (settings.RenderQueueType == RenderQueueType.Transparent)
            ? RenderQueueRange.transparent
            : RenderQueueRange.opaque;

        filteringSettings = new FilteringSettings(renderQueueRange, settings.LayerMask);
    }

    private KuwaharaPassSettings settings;
    private Material material;

    private RTHandle rtBaseColor;

    private List<ShaderTagId> shaderTagIDList;
    private RenderStateBlock renderStateBlock;
    private FilteringSettings filteringSettings;

    public void SetStencilState(int reference, CompareFunction compareFunction, StencilOp passOp, StencilOp failOp, StencilOp zFailOp)
    {
        StencilState stencilState = StencilState.defaultValue;
        stencilState.enabled = true;
        stencilState.SetCompareFunction(compareFunction);
        stencilState.SetPassOperation(passOp);
        stencilState.SetFailOperation(failOp);
        stencilState.SetZFailOperation(zFailOp);

        renderStateBlock.mask |= RenderStateMask.Stencil;
        renderStateBlock.stencilReference = reference;
        renderStateBlock.stencilState = stencilState;
    }

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
        
        SortingCriteria sortingCriteria = (settings.RenderQueueType == RenderQueueType.Transparent)
            ? SortingCriteria.CommonTransparent
            : renderingData.cameraData.defaultOpaqueSortFlags;

        DrawingSettings drawingSettings = CreateDrawingSettings(shaderTagIDList, ref renderingData, sortingCriteria);

        using (new ProfilingScope(cmd, new ProfilingSampler("Draw Stylised Water")))
        {
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();

            material.SetInt("_KernelSize", settings.KernelSize);

            context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filteringSettings, ref renderStateBlock);
        }

        using (new ProfilingScope(cmd, new ProfilingSampler("Kuwahara Filter")))
        {
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