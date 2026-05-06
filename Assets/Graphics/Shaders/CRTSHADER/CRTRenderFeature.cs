using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CRTRenderFeature : ScriptableRendererFeature
{
    class CRTRenderPass : ScriptableRenderPass
    {
        public Material crtMaterial;
        private RTHandle tempTexture;
        private const string k_TempTextureName = "_TemporaryCRTRenderTexture";

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            RenderTextureDescriptor desc = renderingData.cameraData.cameraTargetDescriptor;
            desc.depthBufferBits = 0;
            RenderingUtils.ReAllocateIfNeeded(
                ref tempTexture,
                desc,
                FilterMode.Bilinear,
                TextureWrapMode.Clamp,
                name: k_TempTextureName);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (crtMaterial == null) return;

            CommandBuffer cmd = CommandBufferPool.Get("CRTRenderPass");
            RTHandle source = renderingData.cameraData.renderer.cameraColorTargetHandle;

            Blit(cmd, source, tempTexture, crtMaterial);
            Blit(cmd, tempTexture, source);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public void Dispose()
        {
            tempTexture?.Release();
        }
    }

    CRTRenderPass m_CRTPass;
    public Material crtMaterial;

    public override void Create()
    {
        m_CRTPass = new CRTRenderPass();
        m_CRTPass.renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
        m_CRTPass.crtMaterial = crtMaterial;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (!Application.isPlaying) return;
        renderer.EnqueuePass(m_CRTPass);
    }

    protected override void Dispose(bool disposing)
    {
        m_CRTPass?.Dispose();
    }
}