using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPosition : MonoBehaviour
{
    [Range(0.0f, 2000.0f)]
    public float positionY;
    
    [Range(0.0f, 360)]
    public float rotationX;

    [Range(0.0f, 360)]
    public float rotationY;

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(350.0f, positionY, -20.0f);
        transform.eulerAngles = new Vector3(rotationX + 180, rotationY, 0);
    }
}
