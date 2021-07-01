using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FT_Fallow : MonoBehaviour
{
    public Transform target;


    void Start()
    {
        
    }


    void Update()
    {
        transform.position = target.transform.position;
    }
}
