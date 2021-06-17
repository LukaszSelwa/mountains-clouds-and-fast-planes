using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour

{
    public GameObject arrow;
    public GameObject target;


    // Update is called once per frame
    void Update()
    {
       Transform t= target.transform;
       arrow.transform.LookAt(t);
       // obracamy jeszcze o 90 stopni bo strzałka domyślnie wskazuje w góre a nie prosto
       arrow.transform.Rotate(new Vector3(90, 0, 0));
    }
}
