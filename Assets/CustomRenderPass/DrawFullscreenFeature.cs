using UnityEngine;
using UnityEngine.Rendering.Universal;

public enum BufferType
{
    CameraColor,
    Custom 
}

public class DrawFullscreenFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class Settings
    {
        // Where/when the render pass should be injected during the rendering process.
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
        
        // blit properties
        public Material blitMaterial = null;
        public int blitMaterialPassIndex = -1;
        public BufferType sourceType = BufferType.CameraColor;
        public BufferType destinationType = BufferType.CameraColor;
        public string sourceTextureId = "_SourceTexture";
        public string destinationTextureId = "_DestinationTexture";
    }
    
    // References to pass and its settings.
    public Settings settings = new Settings();
    DrawFullscreenPass blitPass;

    // Gets called every time serialization happens.
    // Gets called when you enable/disable the renderer feature.
    // Gets called when you change a property in the inspector of the renderer feature.
    public override void Create()
    {
        // constructor of pass
        blitPass = new DrawFullscreenPass(name);
    }

    // Injects one or multiple render passes in the renderer.
    // Gets called when setting up the renderer, once per-camera.
    // Gets called every frame, once per-camera.
    // Will not be called if the renderer feature is disabled in the renderer inspector.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (settings.blitMaterial == null)
        {
            Debug.LogWarningFormat("Missing Blit Material. {0} blit pass will not execute. Check for missing reference in the assigned renderer.", GetType().Name);
            return;
        }

        blitPass.renderPassEvent = settings.renderPassEvent;
        blitPass.settings = settings;
        
        // queue up multiple passes after each other
        renderer.EnqueuePass(blitPass);
    }
}