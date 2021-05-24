using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class PointsOrchestrator : MonoBehaviour
{
    public GameObject target;
    public TMP_Text scoreText;

    int points = 0;

    private void Start()
    {
        scoreText.text = "Your score: 0";
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            points++;
            target.transform.Translate(new Vector3(Random.Range(-100f,100f),0, Random.Range(-100f, 100f)));
            scoreText.text = "Your score: " + points.ToString();
        }
    }

}
