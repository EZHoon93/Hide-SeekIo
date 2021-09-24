using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderChildren : MonoBehaviour
{
    public event Action<Collider> triggerStayCallBack;
    private void OnTriggerStay(Collider other)
    {
        triggerStayCallBack?.Invoke(other);
    }
}
