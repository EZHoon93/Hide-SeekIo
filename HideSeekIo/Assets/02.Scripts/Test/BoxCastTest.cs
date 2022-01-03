using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxCastTest : MonoBehaviour
{
    [SerializeField] MeshRenderer[] _meshRenderers;

    [SerializeField] Transform _target;
    void Update()
    {
        Clear();

        var rot = Quaternion.LookRotation(_target.position);


        var raycastHits = Physics.BoxCastAll(this.transform.position, new Vector3(0.1f, 1, 0.5f), this.transform.forward, 
          _target.rotation
            , 5);
        foreach (var raycastHit in raycastHits)
        {
            var ma = raycastHit.collider.GetComponent<MeshRenderer>();
            if (ma)
            {
                ma.material.color = Color.red;
            }
        }
    }

    void Clear()
    {
        foreach(var s in _meshRenderers)
        {
            s.material.color = Color.white;
        }
    }
}
