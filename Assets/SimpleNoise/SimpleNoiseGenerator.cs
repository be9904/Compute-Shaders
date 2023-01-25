using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleNoiseGenerator : MonoBehaviour
{
    [SerializeField] private ComputeShader computeShader;
    public RenderTexture renderTexture;
    
    // Start is called before the first frame update
    void Start()
    {
        renderTexture = new RenderTexture(512, 512, 24);
        renderTexture.enableRandomWrite = true;
        renderTexture.Create();
        
        computeShader.SetFloat("Resolution", renderTexture.width);
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(25, 25, 100, 50), "Generate Noise"))
        {
            computeShader.SetTexture(0, "Result", renderTexture);
            computeShader.Dispatch(
                0, 
                renderTexture.width / 8, 
                renderTexture.height / 8, 
                1
            );
        }
        if (GUI.Button(new Rect(150, 25, 100, 50), "Show UV"))
        {
            computeShader.SetTexture(1, "Result", renderTexture);
            computeShader.Dispatch(
                1, 
                renderTexture.width / 8, 
                renderTexture.height / 8, 
                1
            );
        }
    }
}
