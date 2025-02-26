using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CRTRenderFeature : ScriptableRendererFeature
{
    class CRTRenderPass : ScriptableRenderPass
    {
        public Material crtMaterial;
        private RenderTargetIdentifier source;
        private RenderTargetHandle tempTexture;

        public CRTRenderPass()
        {
            tempTexture.Init("_TemporaryCRTRenderTexture");
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (crtMaterial == null)
                return;

            source = new RenderTargetIdentifier(renderingData.cameraData.renderer.cameraColorTargetHandle.rt);

            CommandBuffer cmd = CommandBufferPool.Get("CRTRenderPass");
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.depthBufferBits = 0;
            cmd.GetTemporaryRT(tempTexture.id, opaqueDesc, FilterMode.Bilinear);
            Blit(cmd, source, tempTexture.Identifier(), crtMaterial);
            Blit(cmd, tempTexture.Identifier(), source);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
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
        if (!Application.isPlaying)
        {
            return;
        }

        if (SanityBar.instance.sanityEffectHandler != null &&
            SanityBar.instance.sanityEffectHandler.IsPlayerInUnderworld)
        {
            return;
        }

        renderer.EnqueuePass(m_CRTPass);
    }
}