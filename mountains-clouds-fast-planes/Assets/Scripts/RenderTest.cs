using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ImageEffectAllowedInSceneView, RequireComponent(typeof(Camera))]
public class RenderTest : MonoBehaviour
{
    public Shader shader;
    public Transform container;
    Material material;

    void Awake() {
        material = new Material(shader);
    }
    void OnRenderImage(RenderTexture src, RenderTexture dest) {
        material.SetFloat("BottomPlane", container.position.y - container.localScale.y / 2);
        material.SetFloat("TopPlane", container.position.y + container.localScale.y / 2);
        Graphics.Blit(src, dest, material);
    }
}
