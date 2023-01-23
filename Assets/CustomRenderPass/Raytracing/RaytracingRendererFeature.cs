using UnityEngine;
using UnityEngine.Rendering.Universal;

public class RaytracingRendererFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class PassSettings
    {
        // Where/when the render pass should be injected during the rendering process.
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
        
        // Used for any potential down-sampling we will do in the pass.
        [Range(1,4)] public int downsample = 1;
        
        // A variable that's specific to the use case of our pass.
        [Range(0, 20)] public int blurStrength = 5;
        
        // additional properties ...
    }
    
    public override void Create()
    {
        throw new System.NotImplementedException();
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        throw new System.NotImplementedException();
    }
}
