using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewCloudTexture : MonoBehaviour
{
    CloudTextureComputer cloudComputer;

    public RenderTexture renderSlice;
    public ComputeShader texture3DSlicer;
    
    [Range(0.0f, 256.0f)]
    public float Slice;
    public bool InverseTexture;

    void Awake() {
        cloudComputer = gameObject.GetComponent<CloudTextureComputer>();
    }

    // Update is called once per frame
    void Update()
    {
        ComputeSlice();
    }

    private void CreateSlice() {
        renderSlice = new RenderTexture(cloudComputer.resolution, cloudComputer.resolution, 0, RenderTextureFormat.ARGB32);
        renderSlice.enableRandomWrite = true;
        renderSlice.Create();
    }

    private void ComputeSlice() {
        CreateSlice();
        texture3DSlicer.SetTexture(0, "MainTex", cloudComputer.cloudTexture);
        texture3DSlicer.SetTexture(0, "Result", renderSlice);
        texture3DSlicer.SetInt("Slice", (int)Slice % cloudComputer.resolution);
        texture3DSlicer.SetBool("Inverse", InverseTexture);
        texture3DSlicer.Dispatch(0, cloudComputer.resolution / 8, cloudComputer.resolution / 8, 1);
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest) {
        if (renderSlice == null) {
            return;
        }
        Graphics.Blit(renderSlice, dest);
    }
}
