using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TrackItem : MonoBehaviour
{
    Transform target;
    NavMeshAgent _agent;
    TrailRenderer _trailRenderer;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _trailRenderer = GetComponent<TrailRenderer>();
    }

    public void SetupTarget(Transform newTarget)
    {
        target = newTarget;
        _agent.SetDestination(target.position);
    }

    private void Update()
    {
        _trailRenderer.SetPosition(0, Managers.Game.myPlayer.transform.position);
        _agent.Move(_agent.nextPosition);
        print(_agent.transform.position);
    }
}
