using System.Collections;

using UnityEngine;

public class HiderInput : InputBase
{
    public Vector2 MoveVector { get; set; } //감지된 움직인 벡터 값
    public bool IsRun { get; set; }
    public bool IsStop { get; private set; }



    float stopTime;
    bool _isAI;

    private void Awake()
    {
        _isAI = this.gameObject.IsValidAI();
    }
    //생성시 실행
    
    public void InitSetup()
    {
     
    }

    public override void OnPhotonInstantiate()
    {
        IsStop = false;
        if (this.IsMyCharacter())
        {
            InputManager.Instacne.SetActiveHiderController(true);
        }
        RandomVector2 = Vector2.one;
    }
    void Update()
    {
        UpdateIsUser();

        if (!photonView.IsMine) return; //로컬 유저만 실행
        if (_isAI)
        {
            UpdateIsAI();
        }
        else
        {
            UpdateIsUser();
        }


        if (IsStop)
        {
            UpdateStopState();
        }
        MoveVector *= RandomVector2;
    }

    void UpdateIsUser()
    {
        MoveVector = InputManager.Instacne.MoveVector;
        IsRun = InputManager.Instacne.IsRun;
    }

    void UpdateIsAI()
    {

    }

    //스탑 상태에서 발생
    void UpdateStopState()
    {
        if (stopTime > 0)
        {
            stopTime -= Time.deltaTime;
            MoveVector = Vector2.zero;
        }
        else
        {
            IsStop = false;
        }
    }



    public override void Stop(float newTime)
    {

    }


}
