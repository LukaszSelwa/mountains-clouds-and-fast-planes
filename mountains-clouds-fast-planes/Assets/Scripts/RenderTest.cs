using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ImageEffectAllowedInSceneView, RequireComponent(typeof(Camera))]
public class RenderTest : MonoBehaviour
{
    public Shader shader;
    public Transform container;

    [Range(0.0f, 10.0f)]
    public float scaleCloudTex;

    [Range(0.0f, 20.0f)]
    public float fogFactor;
    CloudTextureComputer cloudComputer;
    Material material;

    void Awake() {
        material = new Material(shader);
        cloudComputer = gameObject.GetComponent<CloudTextureComputer>();
    }
    void OnRenderImage(RenderTexture src, RenderTexture dest) {
        if (cloudComputer.cloudTexture != null) {
            material.SetTexture("NoiseTex", cloudComputer.cloudTexture);
            material.SetFloat("BottomPlane", container.position.y - container.localScale.y / 2);
            material.SetFloat("TopPlane", container.position.y + container.localScale.y / 2);
            material.SetFloat("ScaleCloudTex", scaleCloudTex);
            material.SetFloat("FogFactor", fogFactor);
            Graphics.Blit(src, dest, material);
        }
    }
}
