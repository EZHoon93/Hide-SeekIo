using System.Collections;

using UnityEngine;

public class PlayerGrassDetect : MonoBehaviour 
{

    private void OnTriggerEnter(Collider other)
    {
        var grass = other.GetComponent<IGrassDetected>();
        grass.SetActiveByGrassDetected(true);
    }

    private void OnTriggerExit(Collider other)
    {
        var grass = other.GetComponent<IGrassDetected>();
        grass.SetActiveByGrassDetected(false);
    }
}
