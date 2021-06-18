using System.Collections;

using BehaviorDesigner.Runtime;

using UnityEngine;
using UnityEngine.AI;

public class HiderInput_AI : HiderInput
{
    NavMeshAgent _agent;
    BehaviorTree _behaviorTree;
    protected override void Awake()
    {
        _agent = this.gameObject.GetOrAddComponent<NavMeshAgent>();
        _behaviorTree = GetComponent<BehaviorTree>();
    }
    public override void OnPhotonInstantiate()
    {
        base.OnPhotonInstantiate();
        SetActiveComponent(true);
    }
    protected override void HandleDeath()
    {
        SetActiveComponent(false);
    }
    void SetActiveComponent(bool active)
    {
        print(active + "??");
        _agent.enabled = active;
        _behaviorTree.enabled = active;
    }
    private void Update()
    {
        OnUpdate();
    }

    public void OnUpdate()
    {
        //if (IsStop)
        //{
        //    UpdateStopState();
        //    return;
        //}
        if(_agent.remainingDistance < 0.2f)
        {
            MoveVector = Vector2.zero;
        }
        else
        {
            MoveVector = new Vector2(_agent.velocity.x, _agent.velocity.z);
        }
    }

}
