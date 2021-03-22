using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplePlaneController : MonoBehaviour
{
    public float forwardAcceleration = 25f;
    public float rotateAcceleration = 90f;
    public float rollacceleration = 10f;

    private float currentForwardSpeed = 0.0f;
    private Vector2 screenCenter, mouseDistance;

    // Start is called before the first frame update
    void Start()
    {
        screenCenter.x = Screen.width * 0.5f;
        screenCenter.y = Screen.height * 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        updateRotate();
        updateForward();
    }

    void updateRotate() {
        mouseDistance.x = (Input.mousePosition.x - screenCenter.x) / screenCenter.y;
        mouseDistance.y = (Input.mousePosition.y - screenCenter.y) / screenCenter.y;

        mouseDistance = Vector2.ClampMagnitude(mouseDistance, 1f);
        transform.Rotate(
            xAngle: -mouseDistance.y * rotateAcceleration * Time.deltaTime,
            yAngle: mouseDistance.x * rotateAcceleration * Time.deltaTime,
            zAngle: 0f,
            relativeTo: Space.Self
        );
    }

    void updateForward() {
        currentForwardSpeed = Mathf.Lerp(currentForwardSpeed, Input.GetAxisRaw("Vertical") * forwardAcceleration, Time.deltaTime);
        transform.position += (transform.forward * currentForwardSpeed * Time.deltaTime);
    }
}
