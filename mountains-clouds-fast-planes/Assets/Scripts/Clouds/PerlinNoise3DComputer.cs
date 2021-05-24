using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinNoise3DComputer : MonoBehaviour
{
    public static Texture3D ComputePerlinNoise(int resolution, float scale) {
        var texture = new Texture3D(resolution, resolution, resolution, TextureFormat.ARGB32, false);
        texture.wrapMode = TextureWrapMode.Repeat;
        Color[] colors = new Color[resolution * resolution * resolution];

        for (int z = 0; z < resolution; z++) {
            int zOffset = z * resolution * resolution;
            for (int y = 0; y < resolution; y++) {
                int yOffset = y * resolution;
                for (int x = 0; x < resolution; x++) {
                    float sample = SampleSymmetricPerlinNoise3D(x, y, z, resolution, scale);
                    colors[x + yOffset + zOffset] = new Color(sample, sample, sample);
                }
            }
        }
        texture.SetPixels(colors);
        texture.Apply();
        return texture;
    }

    static int mirror(int x, int pos) {
        return (x < pos) ? x : 2 * pos - x;
    }

    static float SampleSymmetricPerlinNoise3D(int x, int y, int z, int res, float scale) {
        float step = scale / (float)res;
        x = mirror(x, res/2);
        y = mirror(y, res/2); 
        z = mirror(z, res/2);
        return  SamplePerlinNoise3D((x*0.7071f - y*0.7071f)*step , (x*0.7071f + y*0.7071f)*step, z*step);
    }

    static float SamplePerlinNoise3D(float x, float y, float z)
    {
        float xy = Mathf.PerlinNoise(x, y);
        float xz = Mathf.PerlinNoise(x, z);
        float yz = Mathf.PerlinNoise(y, z);
        float yx = Mathf.PerlinNoise(y, x);
        float zx = Mathf.PerlinNoise(z, x);
        float zy = Mathf.PerlinNoise(z, y);

        return (xy + xz + yz + yx + zx + zy) / 6;
    }
}
