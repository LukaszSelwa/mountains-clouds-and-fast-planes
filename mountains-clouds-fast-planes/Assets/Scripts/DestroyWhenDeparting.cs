using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyWhenDeparting : MonoBehaviour, IDeparting
{
    public void Depart()
    {
        Destroy(gameObject);
    }
}
