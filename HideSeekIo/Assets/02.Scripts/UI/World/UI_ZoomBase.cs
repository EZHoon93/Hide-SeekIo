using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UI_ZoomBase : MonoBehaviour
{
    public abstract void UpdateZoom(Vector3 startPoint , Vector3 endPoint);

    public virtual void SetActiveZoom(bool active) => this.gameObject.SetActive(active);
    public virtual void ZoomOff()
    {

    }
    //public abstract void UpdateZoom(Vector2 inputVector2, float distance);
}
