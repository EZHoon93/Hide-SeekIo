using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;
using UnityEngine.AI;
using BehaviorDesigner.Runtime;

public enum ControllerInputType
{
    Down,
    Drag,
    Up,
    Tap
}
public enum InputType
{
    Move,
    Main,
    Sub1,
    Sub2,
    Sub3
}

public class ControllerInput
{
    public UI_ControllerJoystick uI_ControllerJoystick { get; set; }
    public InputType _inputType;
    public Define.AttackType attackType { get; set; }
    public Dictionary<ControllerInputType, Action<Vector2>> controllerDic { get; set; }
    = new Dictionary<ControllerInputType, Action<Vector2>>()
    {
        {ControllerInputType.Down,null },
        {ControllerInputType.Drag,null },
        {ControllerInputType.Up,null },
        {ControllerInputType.Tap,null },
    };

    public ControllerInput(InputType inputType)
    {
        _inputType = inputType;
    }
    public void Call(ControllerInputType controllerInputType, Vector2 vector2)
    {
        controllerDic[controllerInputType]?.Invoke(vector2);
        inputVector2 = vector2;
        if(controllerInputType == ControllerInputType.Up)
        {
            inputVector2 = Vector2.zero;
        }
    }

    public void Reset() 
    {
        inputVector2 = Vector2.zero; ;
        remainCoolTime = 0;
        controllerDic[ControllerInputType.Down] = null;
        controllerDic[ControllerInputType.Drag] = null;
        controllerDic[ControllerInputType.Up] = null;
        controllerDic[ControllerInputType.Tap] = null;
    }   

    public void Use()
    {

    }
    public void removeEvent()
    {

    }

    public Vector2 inputVector2 { get;  set; }
    public float coolTime { get; set; }
    public float remainCoolTime { get; set; }
}


public class PlayerInput : InputBase
{
    NavMeshAgent navMeshAgent;
    BehaviorTree behaviorTree;
    public Vector2 RandomVector2 { get; set; }
    public float stopTime { get; set; }
    public bool isAI { get; set; }
    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        behaviorTree = GetComponent<BehaviorTree>();
        navMeshAgent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        navMeshAgent.enabled = false;
        behaviorTree.enabled = false;
        GetComponent<PlayerHealth>().onDeath += HandleDeath;
    }

    
    /// <summary>
    /// 최초 생성시 , 방장은 모든 플레이어를 AI로 생성 ,
    /// </summary>
    public virtual void OnPhotonInstantiate()
    {
        stopTime = 0;
        RandomVector2 = Vector2.one;
        isAI = true;
        if (PhotonNetwork.IsMasterClient)
        {
            behaviorTree.ExternalBehavior = GameSetting.Instance.hiderTree;
            behaviorTree.enabled = true;
            navMeshAgent.enabled = true;
        }
        else
        {
            behaviorTree.enabled = false;
            navMeshAgent.enabled = false;
        }
    }

    public void OnPreNetDestroy(PhotonView rootView)
    {
        RemoveInputEvent(InputType.Main);
    }


    public void HandleDeath()
    {
        navMeshAgent.enabled = false;
        behaviorTree.enabled = false;
    }

    /// <summary>
    /// 각 플레이어가 조종
    /// </summary>

    public void ChangeOwnerShip()
    {
        if (this.IsMyCharacter())
        {
            foreach (var input in controllerInputDic)
            {
                Managers.Input.GetControllerJoystick(input.Key).controllerInput = input.Value;
                Managers.Input.GetControllerJoystick(input.Key).SetActiveControllerType(input.Value.attackType);
            }
            //Managers.Input.SetActiveController(true);
            behaviorTree.enabled = false;
            navMeshAgent.enabled = false;
            isAI = false;

        }
        if (PhotonNetwork.IsMasterClient && this.gameObject.IsValidAI())
        {
            var extBehaviorTree = this.GetComponent<PlayerController>().Team == Define.Team.Hide ? GameSetting.Instance.hiderTree : GameSetting.Instance.seekerTree;
            behaviorTree.ExternalBehavior = extBehaviorTree;
            navMeshAgent.enabled = true;
            behaviorTree.enabled = true;

        }
    }

    public void ChangeAI()
    {
        behaviorTree.ExternalBehavior = GameSetting.Instance.hiderTree;
        behaviorTree.enabled = true;
        navMeshAgent.enabled = true;
        isAI = true;
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            if(stopTime > 0)
            {
                stopTime -= Time.deltaTime;
                controllerInputDic[InputType.Move].inputVector2 = Vector2.zero;
                return;
            }

            if (isAI)
            {
                if(navMeshAgent.remainingDistance <= 0.2f)
                {
                    controllerInputDic[InputType.Move].inputVector2 = Vector2.zero;
                }
                else
                {
                    float h = navMeshAgent.velocity.x;
                    float v = navMeshAgent.velocity.z;
                    Vector2 move = new Vector2(h, v);
                    controllerInputDic[InputType.Move].inputVector2 = move.normalized * RandomVector2;
                }
           
            }
            else
            {
#if UNITY_EDITOR
                float h = Input.GetAxis("Horizontal");
                float v = Input.GetAxis("Vertical");
                Vector2 move = new Vector2(h, v);
                controllerInputDic[InputType.Move].inputVector2 = move * RandomVector2;
#endif

            }

        }


    }

    
    public void ChangeTeam(Define.Team team)
    {
        if (PhotonNetwork.IsMasterClient && this.gameObject.IsValidAI())
        {
            var extBehaviorTree = team == Define.Team.Hide ? GameSetting.Instance.hiderTree : GameSetting.Instance.seekerTree;
            behaviorTree.ExternalBehavior = extBehaviorTree;
            navMeshAgent.enabled = true;
            behaviorTree.enabled = true;
            //navMeshAgent.enabled = false;
            //behaviorTree.enabled = false;
            //var move = GetComponent<PlayerStat>();
            //behaviorTree.SetVariable("CurrentEnergy", move.CurrentEnergy);
        }
    }

    public virtual void Stop(float newTime)
    {
        stopTime = newTime;
    }

    public void RemoveStop()
    {
        stopTime = 0;
    }

}
