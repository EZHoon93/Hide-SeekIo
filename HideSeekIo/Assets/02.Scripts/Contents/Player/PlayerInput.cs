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

public class MyInput
{
    public Dictionary<ControllerInputType, Action<Vector2>> controllerDic { get; set; }
    = new Dictionary<ControllerInputType, Action<Vector2>>()
    {
        {ControllerInputType.Down,null },
        {ControllerInputType.Drag,null },
        {ControllerInputType.Up,null },
        {ControllerInputType.Tap,null },
    };

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

    public Vector2 inputVector2 { get;  set; }
    public float coolTime { get; set; }
    public float remainCoolTime { get; set; }
}


public class PlayerInput : MonoBehaviourPun
{
    NavMeshAgent navMeshAgent;
    BehaviorTree behaviorTree;

    //protected float _stopTime;
    //protected bool _isAttack;
    public Vector2 RandomVector2 { get; set; }
    //public bool IsStop { get; protected set; }
    public float stopTime { get; set; }

    public bool isAI { get; set; }

    public Dictionary<InputType, MyInput> controllerInputDic { get; set; } =
    new Dictionary<InputType, MyInput>()
    {
        {InputType.Move ,  new MyInput() },
        {InputType.Main ,  new MyInput() },
        {InputType.Sub1 ,   new MyInput() },
        {InputType.Sub2 , new MyInput() },
        {InputType.Sub3 , new MyInput() },
    };

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        behaviorTree = GetComponent<BehaviorTree>();
        navMeshAgent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        navMeshAgent.enabled = false;
        behaviorTree.enabled = false;
        GetComponent<PlayerHealth>().onDeath += HandleDeath;
        //behaviorTree.GetVariable("")
    }

    
    public virtual void OnPhotonInstantiate()
    {
        stopTime = 0;
        RandomVector2 = Vector2.one;

      
    }


    public void HandleDeath()
    {
        navMeshAgent.enabled = false;
        behaviorTree.enabled = false;
    }
    public void ChangeOwnerShip()
    {
        if (this.IsMyCharacter())
        {
            foreach (var input in controllerInputDic)
            {
                InputManager.Instance.GetControllerJoystick(input.Key).myInput = input.Value;
            }
            InputManager.Instance.SetActiveController(true);
        }
        if (PhotonNetwork.IsMasterClient && this.gameObject.IsValidAI())
        {
            behaviorTree.ExternalBehavior = GameSetting.Instance.hiderTree;
            behaviorTree.enabled = true;
            navMeshAgent.enabled = true;

        }
        isAI = this.gameObject.IsValidAI();

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
                float h = navMeshAgent.velocity.x;
                float v = navMeshAgent.velocity.z;
                Vector2 move = new Vector2(h, v);
                controllerInputDic[InputType.Move].inputVector2 = move.normalized * RandomVector2;
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
            behaviorTree.ExternalBehavior = GameSetting.Instance.seekerTree;
            navMeshAgent.enabled = true;
            behaviorTree.enabled = true;
            //navMeshAgent.enabled = false;
            //behaviorTree.enabled = false;
            //var move = GetComponent<PlayerStat>();
            //behaviorTree.SetVariable("CurrentEnergy", move.CurrentEnergy);
        }
    }

    public virtual void AddInputEvent(Define.AttackType attackType, ControllerInputType controllerInputType, InputType inputType , System.Action<Vector2> action, Sprite sprite= null)
    {
        MyInput addInput = null;
        bool isCache = controllerInputDic.TryGetValue(inputType, out addInput);
        if (isCache == false)
        {
            controllerInputDic.Add(inputType, addInput);
        }
        addInput.controllerDic[controllerInputType] = action;
        if (this.IsMyCharacter())
        {
            InputManager.Instance.GetControllerJoystick(inputType).SetActiveControllerType(attackType, sprite);
            InputManager.Instance.GetControllerJoystick(inputType).ResetUIController();
        }
    }



    public virtual void RemoveInputEvent(InputType inputType)
    {
        if (controllerInputDic.ContainsKey(inputType))
        {
            controllerInputDic[inputType].Reset();
        }
        if (this.IsMyCharacter())
        {
           InputManager.Instance.GetControllerJoystick(inputType).gameObject.SetActive(false);
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
