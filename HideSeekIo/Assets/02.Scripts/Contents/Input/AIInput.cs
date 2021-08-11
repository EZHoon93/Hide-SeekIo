using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using UnityEngine.AI;

public class AIInput : InputBase
{
    BehaviorTree _behaviorTree;
    NavMeshAgent _agent;
    public override void OnPhotonInstantiate()
    {
        base.OnPhotonInstantiate();

        //Resources.Load<BehaviorTree>
        _agent = this.gameObject.GetOrAddComponent<NavMeshAgent>();
        _behaviorTree = this.gameObject.GetOrAddComponent<BehaviorTree>();
        _behaviorTree.ExternalBehavior = GameSetting.Instance.externalBehavior;
        _agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;

    }

    private void Update()
    {
        if (photonView.IsMine == false) return;
        Vector2 inputVector2 = Vector2.zero;
        if( _agent.remainingDistance >= 1)
        {
            inputVector2 = UtillGame.ConventToVector2(_agent.velocity);
        }
        controllerInputDic[InputType.Move].Call(ControllerInputType.Drag, inputVector2.normalized);
        //MoveVector = InputManager.Instance.MoveVector;
    }
}
