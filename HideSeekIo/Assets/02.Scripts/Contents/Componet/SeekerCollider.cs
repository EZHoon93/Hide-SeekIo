using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekerCollider : MonoBehaviour
{
    Collider _collider;
    IParentCollider _parentCollider;
    private void Awake()
    {
        _collider = GetComponent<Collider>();
        _parentCollider = this.transform.parent.GetComponentInParent<IParentCollider>();
    }

    public void SetActiveCollider(bool active)
    {
        _collider.enabled = active;
    }

    private void OnTriggerEnter(Collider other)
    {
        _parentCollider.ParentOnTriggerEnter(other);
    }
}
