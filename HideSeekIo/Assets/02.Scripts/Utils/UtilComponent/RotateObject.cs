using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    public Vector3 rotation;

    // Update is called once per frame
    void Update()
    {
        this.transform.Rotate(rotation*Time.deltaTime);
    }
}
