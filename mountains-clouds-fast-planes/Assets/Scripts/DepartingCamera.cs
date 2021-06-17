using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepartingCamera : MonoBehaviour, IDeparting
{
    public Shader shader;

    bool departed;
    Material material;

    void Awake() {
        material = new Material(shader);
        departed = false;
    }
    public void Depart() {
        departed = true;
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest) {
        material.SetInt("_Departed", departed ? 1 : 0);
        Graphics.Blit(src, dest, material);
    }
}
