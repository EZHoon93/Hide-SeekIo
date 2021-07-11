using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    public Vector3 rotation;
    [SerializeField] Transform _target;
    // Update is called once per frame
    void Update()
    {
        if (_target)
        {
            _target.Rotate(rotation * Time.deltaTime);
        }
        else
        {
            this.transform.Rotate(rotation * Time.deltaTime);
        }
    }
}
