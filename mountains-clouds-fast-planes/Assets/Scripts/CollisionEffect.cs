using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class CollisionEffect : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        print("Collision detected\n");
        SceneManager.LoadScene("Menu");
    }
}
