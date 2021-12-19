using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ObserverController : MonoBehaviourPun
{
    [SerializeField] float speed = 5;
    InputBase _inputBase;
    private void Awake()
    {
        _inputBase = GetComponent<InputBase>();
        _inputBase.AddInputEvent(Define.AttackType.Joystick, ControllerInputType.Drag, InputType.Move, null);
        _inputBase.AddInputEvent(Define.AttackType.Joystick, ControllerInputType.Down, InputType.Move, FirstMove);

    }
    public void SetActive(bool active)
    {
        this.gameObject.SetActive(active);
        _inputBase.SetActiveUserControllerJoystick(active);
        Managers.cameraManager.SetupcameraTagerPlayer(this.transform);
    }
    private void Update()
    {
        var inputVector3 = UtillGame.ConventToVector3( _inputBase.GetVector2(InputType.Move));
        Move(inputVector3);
    }

    void FirstMove(Vector2 vector2)
    {
        var cameraTargetPlayer = Managers.cameraManager.cameraTagerPlayer;
        if (cameraTargetPlayer)
        {
            this.transform.position = cameraTargetPlayer.transform.position;
        }

        Managers.cameraManager.SetupcameraTagerPlayer(this.transform);
    }
    void Move(Vector3 inputVector)
    {
        
        this.transform.Translate(inputVector * Time.deltaTime  * speed);
    }
   
}
