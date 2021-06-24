using System.Collections;

using UnityEngine;

public class UI_Fixing : MonoBehaviour
{

    private void LateUpdate()
    {
        this.transform.rotation = Quaternion.Euler(90, 0, 0);

    }
}
