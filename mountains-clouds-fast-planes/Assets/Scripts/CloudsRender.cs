using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ImageEffectAllowedInSceneView, RequireComponent(typeof(Camera))]
public class CloudsRender : MonoBehaviour
{
    public Shader shader;
    public Transform container;

    [Range(0.0f, 1.0f)]
    public float scaleLargeWorley, scaleSmallWorley, scaleTinyWorley;

    [Range(0.0f, 20.0f)]
    public float fogFactor, distanceFog;

    [Range(0.0f, 10.0f)]
    public float minLightScatter, maxLightScatter;

    [Range(0.0f, 10.0f)]
    public float lightFactor, distortOffset;

    [Range(0.0f, 1.0f)]
    public float cloudThreshold, smallNoiseFactor, tinyNoiseFactor;

    public Color distanceFogColor;
    public Color cloudDarkColor;
    public Texture2D fluidTexture;

    CloudTextureComputer cloudComputer;

    Material material;

    void Awake() {
        material = new Material(shader);
        cloudComputer = gameObject.GetComponent<CloudTextureComputer>();
    }
    void OnRenderImage(RenderTexture src, RenderTexture dest) {
        float topPlane = container.position.y + container.localScale.y / 2;
        float bottomPlane = container.position.y - container.localScale.y / 2;

        if (cloudComputer.cloudTexture != null) {
            material.SetTexture("_NoiseTex", cloudComputer.cloudTexture);
            material.SetTexture("_FluidTex", fluidTexture);
            
            material.SetFloat("_BottomPlane", bottomPlane);
            material.SetFloat("_TopPlane", topPlane);
            material.SetFloat("_BottomDeclinePlane", bottomPlane + container.localScale.y * 0.25f);
            material.SetFloat("_TopDeclinePlane", bottomPlane + container.localScale.y * 0.5f);

            material.SetFloat("_ScaleLargeWorley", scaleLargeWorley);
            material.SetFloat("_ScaleSmallWorley", scaleSmallWorley);
            material.SetFloat("_ScaleTinyWorley", scaleTinyWorley);

            material.SetFloat("_FogFactor", fogFactor);
            material.SetFloat("_MinLightScatter", minLightScatter);
            material.SetFloat("_MaxLightScatter", maxLightScatter);
            material.SetFloat("_LightFactor", lightFactor);
            material.SetFloat("_CloudThreshold", cloudThreshold);
            material.SetFloat("_SmallNoiseFactor", smallNoiseFactor);
            material.SetFloat("_TinyNoiseFactor", tinyNoiseFactor);
            material.SetFloat("_DistanceFogFactor", distanceFog);
            material.SetFloat("_DistortOffset", distortOffset);

            material.SetColor("_DistFogColor", distanceFogColor);
            material.SetColor("_CloudDarkColor", cloudDarkColor);
            Graphics.Blit(src, dest, material);
        }
    }
}
