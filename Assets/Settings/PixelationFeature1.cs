using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PixelationFeature : ScriptableRendererFeature
{
    class PixelationPass : ScriptableRenderPass
    {
        private Material pixelationMat;
        private RenderTargetIdentifier source;
        private RenderTargetHandle tempTexture;
        private string profilerTag;

        public PixelationPass(Material mat, string tag)
        {
            this.pixelationMat = mat;
            this.profilerTag = tag;
            tempTexture.Init("_TempPixelationTex");
        }

        public void Setup(RenderTargetIdentifier source)
        {
            this.source = source;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (pixelationMat == null) return;

            CommandBuffer cmd = CommandBufferPool.Get(profilerTag);

            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            cmd.GetTemporaryRT(tempTexture.id, opaqueDesc);

            Blit(cmd, source, tempTexture.Identifier(), pixelationMat);
            Blit(cmd, tempTexture.Identifier(), source);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }

    [System.Serializable]
    public class PixelationSettings
    {
        public Material material;
    }

    public PixelationSettings settings = new PixelationSettings();
    PixelationPass pass;

    public override void Create()
    {
        pass = new PixelationPass(settings.material, "PixelationPass");
        pass.renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        pass.Setup(renderer.cameraColorTarget);
        renderer.EnqueuePass(pass);
    }
}
