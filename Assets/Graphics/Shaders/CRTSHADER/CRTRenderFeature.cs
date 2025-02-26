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

            // Convert the RTHandle to a RenderTargetIdentifier using its underlying RenderTexture.
            source = new RenderTargetIdentifier(renderingData.cameraData.renderer.cameraColorTargetHandle.rt);

            CommandBuffer cmd = CommandBufferPool.Get("CRTRenderPass");
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.depthBufferBits = 0;
            cmd.GetTemporaryRT(tempTexture.id, opaqueDesc, FilterMode.Bilinear);

            // Blit from the camera's color target to a temporary RT using the CRT shader
            Blit(cmd, source, tempTexture.Identifier(), crtMaterial);
            // Blit the result back to the camera's color target
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
        // Only execute if the game is running.
        if (!Application.isPlaying)
        {
            return;
        }

        if (SanityBar.instance.sanityEffectHandler != null &&
            SanityBar.instance.sanityEffectHandler.IsPlayerInUnderworld)
        {
            // Skip the CRT effect when the player is in the underworld.
            return;
        }

        // Otherwise, enqueue the CRT render pass as usual.
        renderer.EnqueuePass(m_CRTPass);
    }
}