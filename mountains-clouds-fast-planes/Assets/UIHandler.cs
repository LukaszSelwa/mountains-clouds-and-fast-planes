using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity​Engine.UIElements;

public class UIHandler : MonoBehaviour
{
    public UIDocument UIDocument;

    private GameObject obiektGry;
    private PlaneController pc;

    private Label label;

    void Start()
    {
        var root = UIDocument.rootVisualElement;
        // get ui elements by name
        label = root.Q<Label>("Speed");
        obiektGry = GameObject.Find("PlayerPlane");
        pc = obiektGry.GetComponent<PlaneController>();
       
    }


    private void Update()
    {
        int speed = (int)pc.rb.velocity.magnitude*3;
        label.text = speed.ToString()+" mph";
    }

}