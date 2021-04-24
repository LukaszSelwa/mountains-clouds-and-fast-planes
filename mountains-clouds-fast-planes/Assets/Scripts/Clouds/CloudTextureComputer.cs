using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudTextureComputer : MonoBehaviour
{
    public ComputeShader computeShader;
    public ComputeShader texturesCombiner;
    
    [HideInInspector]
    public RenderTexture largeCloudTexture;
    
    [HideInInspector]
    public RenderTexture smallCloudTexture;

    [HideInInspector]
    public RenderTexture cloudTexture;

    public int resolution = 128;

    public int smallCloudResolution = 16;
    public int largeTexturePointsNumber = 4;
    public int smallTexturePointsNumber = 4;

    [Range(0.0f, 1.0f)]
    public float largeCloudThreshold;
    
    [Range(0.0f, 1.0f)]
    public float smallCloudThreshold;

    [Range(0.0f, 10.0f)]
    public float smallCloudScale;

    void Start() {
        largeCloudTexture = ComputeRenderTexture(largeTexturePointsNumber, resolution);
        smallCloudTexture = ComputeRenderTexture(smallTexturePointsNumber, smallCloudResolution);

        cloudTexture = CreateRenderTexture(resolution);
        CombineTextures(cloudTexture, largeCloudTexture, smallCloudTexture);
    }

    void Update() {
        CombineTextures(cloudTexture, largeCloudTexture, smallCloudTexture);
    }

    void CombineTextures(RenderTexture result, RenderTexture largeTexture, RenderTexture smallTexture) {
        int res = result.width;
        texturesCombiner.SetTexture(0, "Result", result);
        texturesCombiner.SetTexture(0, "LargeTexture", largeTexture);
        texturesCombiner.SetTexture(0, "SmallTexture", smallTexture);
        texturesCombiner.SetInt("SmallTextureResolution", smallCloudResolution);
        texturesCombiner.SetFloat("LargeThreshold", largeCloudThreshold);
        texturesCombiner.SetFloat("SmallThreshold", smallCloudThreshold);
        texturesCombiner.SetFloat("Scale", smallCloudScale);
        texturesCombiner.Dispatch(0, res / 8, res / 8, res / 8);
    }

    RenderTexture ComputeRenderTexture(int pointsNumber, int res) {
        var texture = CreateRenderTexture(res);
        var points = CreateRandomPoints(pointsNumber, res);

        var pointsBuffer = createPointsBuffer(points);

        computeShader.SetTexture(0, "Result", texture);
        computeShader.SetInt("PointsNr", pointsNumber);
        computeShader.SetInt("Resolution", res);
        computeShader.SetBuffer(0, "RPts", pointsBuffer);
        computeShader.Dispatch(0, res / 8, res / 8, res / 8);

        pointsBuffer.Release();
        return texture;
    }

    RenderTexture CreateRenderTexture(int res) {
        // set up 3d texture size x size x size
        var texture = new RenderTexture(res, res, 0, RenderTextureFormat.ARGB32);
        texture.enableRandomWrite = true;
        texture.dimension = UnityEngine.Rendering.TextureDimension.Tex3D;
        texture.volumeDepth = res;
        texture.Create(); 
        texture.wrapMode = TextureWrapMode.Repeat;
        return texture;
    }

    Vector3Int[] CreateRandomPoints(int number, int res) {
        int pointsNumberCube = number * number * number;
        int gridLen = res / number;
        var points = new Vector3Int[pointsNumberCube];
        for (int i = 0; i < pointsNumberCube; ++i) {
            points[i] = new Vector3Int(
                Random.Range(0, gridLen - 1),
                Random.Range(0, gridLen - 1),
                Random.Range(0, gridLen - 1)
            );
        }
        return points;
    }

    private ComputeBuffer createPointsBuffer(Vector3Int[] points) {
        int size = 3 * sizeof(int);
        ComputeBuffer pointsBuffer = new ComputeBuffer(points.Length, size);
        pointsBuffer.SetData(points);
        return pointsBuffer;
    }
}
