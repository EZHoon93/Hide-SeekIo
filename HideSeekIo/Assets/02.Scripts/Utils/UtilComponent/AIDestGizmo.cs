using System.Collections;

using UnityEngine;
using UnityEngine.AI;

public class AIDestGizmo : MonoBehaviour
{
    [SerializeField] NavMeshAgent _agent;
    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        this.transform.position = _agent.destination;
    }

   
}
