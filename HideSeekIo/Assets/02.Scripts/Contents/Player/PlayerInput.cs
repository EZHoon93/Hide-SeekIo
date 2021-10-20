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
    PlayerInput  _playerInput;

    public Dictionary<InputType, ControllerInput> controllerInputDic { get; set; } =
    new Dictionary<InputType, ControllerInput>()
    {
        {InputType.Move ,  new ControllerInput(InputType.Move ) },
        {InputType.Main ,  new ControllerInput(InputType.Main) },
        {InputType.Sub1 ,   new ControllerInput(InputType.Sub1) },
        {InputType.Sub2 , new ControllerInput(InputType.Sub2) },
        {InputType.Sub3 , new ControllerInput(InputType.Sub3) },
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
                InputManager.Instance.GetControllerJoystick(input.Key).controllerInput = input.Value;
            }
            InputManager.Instance.SetActiveController(true);
            behaviorTree.enabled = false;
            navMeshAgent.enabled = false;
            isAI = false;

        }
        if (PhotonNetwork.IsMasterClient && this.gameObject.IsValidAI())
        {
            behaviorTree.ExternalBehavior = GameSetting.Instance.hiderTree;
            behaviorTree.enabled = true;
            navMeshAgent.enabled = true;

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

    public virtual void AddInputEvent(Define.AttackType attackType, ControllerInputType controllerInputType, InputType inputType , Action<Vector2> action, Sprite sprite= null)
    {
        ControllerInput addInput = null;
        bool isCache = controllerInputDic.TryGetValue(inputType, out addInput);
        if (isCache == false)
        {
            controllerInputDic.Add(inputType, addInput);
        }
        addInput.controllerDic[controllerInputType] = action;
        if (this.IsMyCharacter())
        {
            InputManager.Instance.GetControllerJoystick(inputType).SetActiveControllerType(attackType, null);
            InputManager.Instance.GetControllerJoystick(inputType).ResetUIController(); 
        }
    }

    public ControllerInput GetControllerInput(InputType inputType)
    {
        return controllerInputDic[inputType];
    }
    //public ControllerInput  SetupControllerInput(InputControllerObject inputControllerObject,  Sprite sprite = null)
    //{
    //    inputControllerObject.controllerInput = controllerInputDic[inputControllerObject.inputType];

    //    if (this.IsMyCharacter())
    //    {
    //        InputManager.Instance.GetControllerJoystick(inputControllerObject.inputType).SetActiveControllerType(inputControllerObject.attackType, sprite);
    //        InputManager.Instance.GetControllerJoystick(inputControllerObject.inputType).ResetUIController();
    //    }

    //    return controllerInputDic[inputControllerObject.inputType];
    //}



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

    public void SetupControllerInputUI(Define.AttackType attackType, InputType inputType , Sprite sprite)
    {
        if (this.IsMyCharacter())
        {
            controllerInputDic[inputType].uI_ControllerJoystick = InputManager.Instance.GetControllerJoystick(inputType);
            InputManager.Instance.GetControllerJoystick(inputType).SetActiveControllerType(attackType, null);
            InputManager.Instance.GetControllerJoystick(inputType).ResetUIController();
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
