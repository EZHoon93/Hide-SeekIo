using System.Collections;

using UnityEngine;
using UnityEngine.AI;

public class AIDestGizmo : MonoBehaviour
{
    [SerializeField] NavMeshAgent _agent;

    GameObject cube;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if(cube  == null)
        {
            cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.SetParent(this.transform);
        }
        cube.transform.position = _agent.destination;
    }

   
}
