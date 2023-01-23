using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleNoiseGenerator : MonoBehaviour
{
    [SerializeField] private ComputeShader computeShader;
    [SerializeField] private RenderTexture renderTexture;
    
    // Start is called before the first frame update
    void Start()
    {
        renderTexture = new RenderTexture(512, 512, 24);
        renderTexture.enableRandomWrite = true;
        renderTexture.Create();
        
        computeShader.SetTexture(0, "Result", renderTexture);
        computeShader.Dispatch(
            0, 
            renderTexture.width / 8, 
            renderTexture.height / 8, 
            1
        );
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
