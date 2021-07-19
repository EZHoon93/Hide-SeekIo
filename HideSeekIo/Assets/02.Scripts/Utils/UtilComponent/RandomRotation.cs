using System.Collections;

using UnityEngine;

public class RandomRotation : MonoBehaviour
{
    private void Reset()
    {
        float ran = Random.Range(0, 360);
        this.transform.rotation = Quaternion.Euler(0, ran, 0);
    }
}
