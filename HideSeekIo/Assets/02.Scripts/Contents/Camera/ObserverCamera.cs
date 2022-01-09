using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ObserverCamera : CameraBase
{
    [SerializeField] float speed = 5;
    InputBase _inputBase;
    private void Awake()
    {
        _inputBase = GetComponent<InputBase>();
        _inputBase.AddInputEvent(Define.AttackType.Joystick, ControllerInputType.Drag, InputType.Move, null);
        _inputBase.AddInputEvent(Define.AttackType.Joystick, ControllerInputType.Down, InputType.Move, FirstMove);
    }

    public override void Init()
    {
        _inputBase.SetActiveUserControllerJoystick(true);
    }
    private void OnDisable()
    {
        _inputBase.SetActiveUserControllerJoystick(false);
    }
    public void SetActive(bool active)
    {
        this.gameObject.SetActive(active);
        _inputBase.SetActiveUserControllerJoystick(active);
        //Managers.CameraManager.SetupFollowTarget(this.transform);
    }
    private void Update()
    {
        var inputVector3 = UtillGame.ConventToVector3( _inputBase.GetVector2(InputType.Move));
        Move(inputVector3);
    }

    void FirstMove(Vector2 vector2)
    {
        var cameraTargetPlayer = Managers.CameraManager.cameraTagerPlayer;
        if (cameraTargetPlayer)
        {
            this.transform.position = cameraTargetPlayer.transform.position;
        }

        Managers.CameraManager.cameraTagerPlayer = null;
        Managers.CameraManager.SetupFollowTarget(this.transform);
    }
    void Move(Vector3 inputVector)
    {
        
        this.transform.Translate(inputVector * Time.deltaTime  * speed);
    }
   
}
