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
            updatePos();
            label.text = "Score: " + points.ToString();
        }
    }

    void updatePos()
    {
        Vector3 pos = target.transform.position;
        pos.x += Random.Range(-100f, 200f);
        pos.z += Random.Range(-100f, 200f);

        float hight = TerrainHeightChecker.getHeight(pos);

        pos.y = hight + Random.Range(50f, 100f);

        target.transform.position = pos;
    }

}
