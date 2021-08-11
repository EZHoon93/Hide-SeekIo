using System.Collections;

using BehaviorDesigner.Runtime;

using UnityEngine;
using UnityEngine.AI;

public class HiderInput_AI : HiderInput
{
    NavMeshAgent _agent;
    BehaviorTree _behaviorTree;
    SharedGameObject sharedGameObject;
    protected override void Awake()
    {
        _agent = this.gameObject.GetOrAddComponent<NavMeshAgent>();
        _behaviorTree = GetComponent<BehaviorTree>();
    }

    private void Start()
    {
        var initPoint = _behaviorTree.GetVariable("InitPoint");

        var mainScene =  Managers.Game.CurrentGameScene as GameMainScene;
        int ran = Random.Range(0, mainScene.itemSpawnManager.SeekerItemPoints.Length);
        var randomPoint = UtillGame.GetRandomPointOnNavMesh(mainScene.itemSpawnManager.SeekerItemPoints[ran].transform.position, 3);
        initPoint.SetValue( randomPoint);
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
        _agent.enabled = active;
        _behaviorTree.enabled = active;
    }
    private void Update()
    {
        OnUpdate();
    }

    public override void EnegyZero()
    {
        IsRun = false;
    }
    public void OnUpdate()
    {
        //if (IsStop)
        //{
        //    UpdateStopState();
        //    return;
        //}
        //if(_agent.remainingDistance <= 0.2f)
        //{
        //    MoveVector = Vector2.zero;
        //}
        //else
        //{
        //    MoveVector = new Vector2(_agent.velocity.x, _agent.velocity.z);
        //}
    }

}
