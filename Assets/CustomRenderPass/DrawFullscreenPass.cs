using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Draws full screen mesh using given material and pass and reading from source target.
/// </summary>
internal class DrawFullscreenPass : ScriptableRenderPass
{
    public FilterMode filterMode { get; set; }
    
    // We will store our pass settings in this variable.
    public DrawFullscreenFeature.Settings settings;

    RenderTargetIdentifier source;
    RenderTargetIdentifier destination;
    
    // It is good to cache the shader property IDs here.
    int temporaryRTId = Shader.PropertyToID("_TempRT");

    int sourceId;
    int destinationId;
    bool isSourceAndDestinationSameTarget;

    // The profiler tag that will show up in the frame debugger.
    string m_ProfilerTag;

    // The constructor of the pass. Here you can set any material properties
    // that do not need to be updated on a per-frame basis.
    public DrawFullscreenPass(string tag)
    {
        // set profiler tag in constructor
        m_ProfilerTag = tag;
    }

    // Gets called by the renderer before executing the pass.
    // Can be used to configure render targets and their clearing state.
    // Can be user to create temporary render target textures.
    // If this method is not overriden, the render pass will render to the active camera render target.
    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        // Grab the camera target descriptor. Use this when creating a temporary render texture.
        RenderTextureDescriptor blitTargetDescriptor = renderingData.cameraData.cameraTargetDescriptor;
        
        // Set the number of depth bits needed for the temporary render texture.
        blitTargetDescriptor.depthBufferBits = 0;
        
        // Enable these if pass requires access to the CameraDepthTexture or the CameraNormalsTexture.
        // ConfigureInput(ScriptableRenderPassInput.Depth);
        // ConfigureInput(ScriptableRenderPassInput.Normal);

        isSourceAndDestinationSameTarget = settings.sourceType == settings.destinationType &&
            (settings.sourceType == BufferType.CameraColor || settings.sourceTextureId == settings.destinationTextureId);

        var renderer = renderingData.cameraData.renderer;

        if (settings.sourceType == BufferType.CameraColor)
        {
            sourceId = -1;
            
            // Grab the color buffer from the renderer camera color target.
            source = renderer.cameraColorTarget;
        }
        else
        {
            sourceId = Shader.PropertyToID(settings.sourceTextureId);
            cmd.GetTemporaryRT(sourceId, blitTargetDescriptor, filterMode);
            source = new RenderTargetIdentifier(sourceId);
        }

        if (isSourceAndDestinationSameTarget)
        {
            destinationId = temporaryRTId;
            
            // Create a temporary render texture using the descriptor from above.
            cmd.GetTemporaryRT(destinationId, blitTargetDescriptor, filterMode);
            destination = new RenderTargetIdentifier(destinationId);
        }
        else if (settings.destinationType == BufferType.CameraColor)
        {
            destinationId = -1;
            destination = renderer.cameraColorTarget;
        }
        else
        {
            destinationId = Shader.PropertyToID(settings.destinationTextureId);
            
            // Create a temporary render texture using the descriptor from above.
            cmd.GetTemporaryRT(destinationId, blitTargetDescriptor, filterMode);
            destination = new RenderTargetIdentifier(destinationId);
        }
    }

    // The actual execution of the pass. This is where custom rendering occurs.
    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        // Grab a command buffer. We put the actual execution of the pass inside of a profiling scope.
        CommandBuffer cmd = CommandBufferPool.Get(m_ProfilerTag);

        // Can't read and write to same color target, create a temp render target to blit. 
        if (isSourceAndDestinationSameTarget)
        {
            // Blit from source to destination and back. This is needed for a two-pass shader.
            Blit(cmd, source, destination, settings.blitMaterial, settings.blitMaterialPassIndex);
            Blit(cmd, destination, source);
        }
        else
        {
            Blit(cmd, source, destination, settings.blitMaterial, settings.blitMaterialPassIndex);
        }

        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    // Called upon finish rendering a camera. You can use this callback to release any resources created
    // by this render
    // pass that need to be cleanup once camera has finished rendering.
    // This method be called for all cameras in a camera stack.
    public override void OnCameraCleanup(CommandBuffer cmd)
    {
        if (destinationId != -1)
            cmd.ReleaseTemporaryRT(destinationId);

        if (source == destination && sourceId != -1)
            cmd.ReleaseTemporaryRT(sourceId);
    }
}