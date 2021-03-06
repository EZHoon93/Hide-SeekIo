using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;

public class ChangeObjectController : MonoBehaviourPun , IPunObservable , IPunInstantiateMagicCallback, IOnPhotonViewPreNetDestroy
{
    InputControllerObject _inputControllerObject;
    [SerializeField] PlayerController _playerController;
    GameObject _changeObject;
    ObjectModeController objectModeMapControllers;
    int _objectIndex = -2;
    public int objectIndex
    {
        get => _objectIndex;
        set
        {
            if (_objectIndex == value ) return;
            if (_changeObject)
            {
                Managers.Resource.Destroy(_changeObject.gameObject);
            }
            _objectIndex = value;
            var mapController = Managers.Scene.currentGameScene.mapController.GetComponent<ObjectModeMapController>();
            _changeObject = Instantiate(mapController.changeObjectList.objectList[objectIndex].gameObject);
            _changeObject.transform.ResetTransform(_playerController.playerHealth.fogController.transform);
            var isObjectShow = _playerController.playerMove.State == PlayerMove.MoveState.Idle ? true : false;
            ShowChangeObejct(isObjectShow);
        }
    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(objectIndex);
        }
        else
        {
            objectIndex = (int)stream.ReceiveNext();
            print(objectIndex);

        }
    }


    private void Awake()
    {
        _inputControllerObject = GetComponent<InputControllerObject>();
        _inputControllerObject.AddZoomEvent(CallBack_Zoom);
    }


    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        var infoData = info.photonView.InstantiationData;
        if (infoData == null)
        {
            return;
        }
        var actorNumber = (int)infoData[0];
        var playerViewID = (int)infoData[1];
        _objectIndex = -1;
        objectModeMapControllers = Managers.Scene.currentGameScene.mapController.GetComponent<ObjectModeMapController>().changeObjectList;

        //var userController = Managers.Game.GetUserController(actorNumber);
        _playerController = Managers.Game.GetPlayerController(playerViewID);

        if (_playerController == null)
        {

        }
        else
        {
            this.transform.ResetTransform(_playerController.playerHealth.fogController.transform);
            _playerController.playerShooter.SetupInputControllerObject(_inputControllerObject);
            _playerController.playerMove.onChangeMoveStateEvent += ChangeObject;
            _playerController.playerHealth.onDeath += HandleDeath;
        }

        if (photonView.IsMine)
        {
            objectIndex = Random.Range(0, objectModeMapControllers.objectList.Length);
        }
    }

    public void OnPreNetDestroy(PhotonView rootView)
    {
        _playerController.playerMove.onChangeMoveStateEvent -= ChangeObject;
        _playerController.playerHealth.onDeath -= HandleDeath;

    }

    void HandleDeath()
    {
        _playerController.playerMove.onChangeMoveStateEvent -= ChangeObject;
        ShowChangeObejct(false);
    }



    void CallBack_Zoom(Vector2 vector2)
    {
        var playerMove = _playerController.playerMove;
        var isonlyRotation = playerMove.isOnlyRotation;
        var inputType = _inputControllerObject.inputType;
        var itemUI = _playerController.playerInput.GetControllerJoystickUI(inputType).itemImage;
        playerMove.isOnlyRotation = !isonlyRotation;
        if (isonlyRotation)
        {
            itemUI.color = Color.white;
        }
        else
        {
            itemUI.color = Color.black;
        }
    }

    /// <summary>
    /// true => 사물보게, 캐릭안보이게
    /// </summary>
    /// <param name="changeShow"></param>
    void ShowChangeObejct(bool changeShow)
    {
        if (_changeObject == null)
        {
            return;
        }
        _changeObject.gameObject.SetActive(changeShow);
        _playerController.characterAvater.gameObject.SetActive(!changeShow);
        if (_playerController.playerHealth.IsMyCharacter() == false)
        {
            _playerController.playerUI.SetActiveNameUI(!changeShow);
        }
    }

    void ChangeObject(PlayerMove.MoveState moveState)
    {
        if (_changeObject == null)
            return;
        switch (moveState)
        {
            case PlayerMove.MoveState.Idle:
                //_playerController.playerInput.SetupControllerInputUI
                ShowChangeObejct(true);
                break;
            default:
                ShowChangeObejct(false);
                break;
        }
    }

    
}
