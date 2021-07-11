using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poolable : MonoBehaviour
{
    [HideInInspector]
	public bool IsUsing;

	public virtual void Push()
    {
        Managers.Pool.Push(this);
    }
}