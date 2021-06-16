using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity​Engine.UIElements;



public class PointsOrchestrator : MonoBehaviour
{
    public GameObject target;
    public UIDocument UIDocument;
    private Label label;

    int points = 0;

    private void Start()
    {
        var root = UIDocument.rootVisualElement;
        // get ui elements by name
        label = root.Q<Label>("Score");
        label.text = "Score: 0";

    }

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            points++;
            target.transform.Translate(new Vector3(Random.Range(-100f,100f),0, Random.Range(-100f, 100f)));
            label.text = "Score: " + points.ToString();
        }
    }

}
