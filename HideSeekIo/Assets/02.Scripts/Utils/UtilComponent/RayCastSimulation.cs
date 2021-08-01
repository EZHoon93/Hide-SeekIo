using UnityEngine;
using System.Collections;

public class RayCastSimulation : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    public LayerMask layerMask;
   
    
    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        var myPos = this.transform.position + offset;
        var targetPos = target.transform.position + offset;
        var direction = targetPos - myPos;
        direction.y = 0;
        if (Physics.Raycast(myPos, direction.normalized, out hit, 11, layerMask))
        {
            Debug.DrawRay(myPos, direction, Color.red);
        }
        else
        {
            Debug.DrawRay(myPos, direction, Color.red);
        }
    }
}
