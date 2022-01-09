using BehaviorDesigner.Runtime;

using Photon.Pun;

using UnityEngine;
using UnityEngine.AI;



public class PlayerInput : InputBase
{
    NavMeshAgent navMeshAgent;
    BehaviorTree behaviorTree;
    public Vector2 RandomVector2 { get; set; }
    public float stopTime { get; set; }
    public bool isAI { get; set; }
    public bool _isEditTest;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        behaviorTree = GetComponent<BehaviorTree>();
        navMeshAgent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        navMeshAgent.enabled = false;
        behaviorTree.enabled = false;
        GetComponent<PlayerHealth>().onDeath += HandleDeath;
    }

    void ResetVariable()
    {
        stopTime = 0;
        RandomVector2 = Vector2.one;
    }
    /// <summary>
    /// 최초 생성시 , 방장은 모든 플레이어를 AI로 생성 ,
    /// </summary>
    public virtual void OnPhotonInstantiate()
    {
        ResetVariable();
        if (this.IsMyCharacter())
        {
            ChangePlayerType(Define.PlayerType.User);
            SetActiveUserControllerJoystick(true);
        }

        if(this.gameObject.IsValidAI())
        {
            ChangePlayerType(Define.PlayerType.AI);

        }
    }

    public void OnPreNetDestroy(PhotonView rootView)
    {
        RemoveInputEvent(InputType.Main);
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
                if (_isEditTest)
                {
                    float h = Input.GetAxis("Horizontal");
                    float v = Input.GetAxis("Vertical");
                    Vector2 move = UtillGame.GetInputVector2_ByCamera(new Vector2(h, v));

                    controllerInputDic[InputType.Move].inputVector2 = move * RandomVector2;
                }
                else
                {

                }
#if UNITY_EDITOR

                if (Input.GetKeyDown(KeyCode.I))
                {
                    _isEditTest  = !_isEditTest;
                }
#endif

            }

        }


    }


    public override void SetupControllerInputUI(Define.AttackType attackType, InputType inputType, Sprite sprite)
    {
        if(this.IsMyCharacter() == false)
        {
            return;
        }
        base.SetupControllerInputUI(attackType, inputType, sprite);
    }

    public void HandleDeath()
    {
        navMeshAgent.enabled = false;
        behaviorTree.enabled = false;
    }
    public void ChangePlayerType(Define.PlayerType playerType)
    {
        if (playerType == Define.PlayerType.User)
        {
            behaviorTree.enabled = false;
            navMeshAgent.enabled = false;
            isAI = false;
        }
        else
        {
            if (PhotonNetwork.IsMasterClient == false)
            {
                return;
            }
            var extBehaviorTree = this.GetComponent<PlayerController>().Team == Define.Team.Hide ? Managers.GameSetting.hiderTree : Managers.GameSetting.seekerTree;
            behaviorTree.ExternalBehavior = extBehaviorTree;
            behaviorTree.enabled = true;
            navMeshAgent.enabled = true;
            isAI = true;
        }
    }



    //public void ChangeTeam(Define.Team team)
    //{
    //    if (PhotonNetwork.IsMasterClient && this.gameObject.IsValidAI())
    //    {
    //        var extBehaviorTree = team == Define.Team.Hide ? GameSetting.Instance.hiderTree : GameSetting.Instance.seekerTree;
    //        behaviorTree.ExternalBehavior = extBehaviorTree;
    //        navMeshAgent.enabled = true;
    //        behaviorTree.enabled = true;
    //        //navMeshAgent.enabled = false;
    //        //behaviorTree.enabled = false;
    //        //var move = GetComponent<PlayerStat>();
    //        //behaviorTree.SetVariable("CurrentEnergy", move.CurrentEnergy);
    //    }
    //}

    public virtual void Stop(float newTime)
    {
        stopTime = newTime;
    }

    public void RemoveStop()
    {
        stopTime = 0;
    }

}
