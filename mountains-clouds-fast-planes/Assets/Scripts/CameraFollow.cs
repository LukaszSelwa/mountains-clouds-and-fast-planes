using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public int framesBehind;

    private (Vector3, Vector3) _initial;
    private Queue<(Vector3, Vector3)> _queue;
    
    void Start()
    {
        _queue = new Queue<(Vector3, Vector3)>();
        _initial = (transform.localPosition, transform.localEulerAngles);
    }
    
    void FixedUpdate()
    {
        var currentTransform = transform;
        (currentTransform.localPosition, currentTransform.localEulerAngles) = _initial;

        var pos = currentTransform.position;
        var angles = currentTransform.eulerAngles;
        
        _queue.Enqueue((pos, angles));

        (transform.position, transform.eulerAngles) = _queue.Peek();

        while (_queue.Count > framesBehind) _queue.Dequeue();
    }
}
