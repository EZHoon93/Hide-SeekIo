using System.Collections;

using UnityEngine;

public class UI_WorldCamerView : MonoBehaviour
{


    protected virtual void LateUpdate()
    {
        transform.rotation = Camera.main.transform.rotation;
    }
}
