using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poolable : MonoBehaviour
{
	public bool IsUsing;

	public virtual void Push()
    {
        Managers.Pool.Push(this);
    }
}