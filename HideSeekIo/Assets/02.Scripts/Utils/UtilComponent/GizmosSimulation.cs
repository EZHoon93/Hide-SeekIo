using System.Collections;

using UnityEngine;

public class GizmosSimulation : MonoBehaviour
{
    public float radius;
    public Color color = Color.green;
   

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = color;
        Gizmos.DrawWireSphere(this.transform.position, radius);
    }
}
