using System.Collections;

using UnityEngine;

public class PlayerGrassDetect : MonoBehaviour 
{

    private void OnTriggerEnter(Collider other)
    {
        var grass = other.GetComponent<IGrassDetected>();
        grass.ChangeTransParent(true);
    }

    private void OnTriggerExit(Collider other)
    {
        var grass = other.GetComponent<IGrassDetected>();
        grass.ChangeTransParent(false);
    }
}
