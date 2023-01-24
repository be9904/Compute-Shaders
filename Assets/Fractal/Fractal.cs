using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class Fractal : MonoBehaviour
{
    // Mandelbrot set
    private float _width, _height;
    private float _rStart, _iStart;
    private int _maxIterations, _increment;
    private float _zoom;
    
    // Compute Shader
    [SerializeField] ComputeShader computeShader;
    [SerializeField] RawImage rawImage;
    private GraphicsBuffer _graphicsBuffer;
    private RenderTexture _renderTexture;

    private static int mainKernelID;
    private static int maxIterationsID;
    private static int resultTextureID;
    
    // Structured buffer
    public struct StructuredBuffer
    {
        public float w, h, r, i;
        public int screenWidth, screenHeight;
    }
    private StructuredBuffer[] data;

    // Start is called before the first frame update
    void Start()
    {
        // gpu property id
        mainKernelID = computeShader.FindKernel("CSMain");

        _width = 4.5f;
        _height = _width * Screen.height / Screen.width;
        _rStart = -2.0f;
        _iStart = 1.25f;
        _maxIterations = 500;
        _increment = 3;
        _zoom = 0.5f;

        data = new StructuredBuffer[1];

        data[0] = new StructuredBuffer
        {
            w = _width,
            h = _height,
            r = _rStart,
            i = _iStart,
            screenWidth = Screen.width,
            screenHeight = Screen.height
        };

        _graphicsBuffer = new GraphicsBuffer(
            GraphicsBuffer.Target.Structured, 
            data.Length,
            // Marshal.SizeOf(typeof(StructuredBuffer))
            4 * sizeof(float) + 2 * sizeof(int)
        );

        _renderTexture = new RenderTexture(Screen.width, Screen.height, 0);
        _renderTexture.enableRandomWrite = true;
        _renderTexture.Create();
        
        BuildFractal();
    }

    void BuildFractal()
    {
        _graphicsBuffer.SetData(data);
        computeShader.SetBuffer(mainKernelID, "buffer", _graphicsBuffer);
        
        computeShader.SetInt("maxIterations", _maxIterations);
        computeShader.SetTexture(mainKernelID, "Result", _renderTexture);
        
        computeShader.Dispatch(
            mainKernelID, 
            Screen.width / 8, 
            Screen.height / 8, 
            1
        );

        RenderTexture.active = _renderTexture;
        rawImage.material.mainTexture = _renderTexture;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        if (_graphicsBuffer != null)
        {
            _graphicsBuffer?.Dispose();
            _graphicsBuffer = null;
        }
    }
}
