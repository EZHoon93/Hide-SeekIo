using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;   
public class PlayerObjectController : MonoBehaviourPun , IPunObservable
{
    //[SerializeField] PlayerCharacter _playerCharacter;
    //[SerializeField] PlayerMove _playerMove;
    //[SerializeField] PlayerHealth _playerHealth;
    //[SerializeField] PlayerUI _playerUI;
    [SerializeField] Sprite _lockSprite;
    [SerializeField] Sprite _unLockSprite;

    PlayerController _playerController;
    GameObject _changeObject;
    InputControllerObject _inputControllerObject;
  
    int _objectIndex = -2;
    public int objectIndex
    {
        get => _objectIndex;
        set
        {
            if (_objectIndex == value) return;
            if (_changeObject)
            {
                Managers.Resource.Destroy(_changeObject.gameObject);
            }
            _objectIndex = value;
            var mapController = Managers.Scene.currentGameScene.mapController.GetComponent<ObjectModeMapController>();
            _changeObject = Instantiate(mapController.changeObjectList.objectList[objectIndex].gameObject);
            _changeObject.transform.ResetTransform(_playerController.playerHealth.fogController.transform);
        }
    }

    private void Awake()
    {
        _inputControllerObject = GetComponent<InputControllerObject>();
    }

    void Use(Vector2 vector2)
    {

    }

    public void Setup(PlayerController playerController)
    {
        if(playerController.Team == Define.Team.Seek)
        {
            this.gameObject.SetActive(false);
            return;
        }

        _playerController = playerController;
        playerController.playerMove.onChangeMoveStateEvent += ChangeObject;
        this.gameObject.SetActive(true);


        if (photonView.IsMine == false) return;
        var mapController = Managers.Scene.currentGameScene.mapController.GetComponent<ObjectModeMapController>();
        objectIndex = Random.Range(0, mapController.changeObjectList.objectList.Length);
        ShowChangeObejct(true);
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
        }
    }

    public void OnPreNetDestroy(PlayerController playerController)
    {
        if (playerController.Team == Define.Team.Seek)
        {
            this.gameObject.SetActive(false);
            return;
        }

        //P _playerMove.onChangeMoveStateEvent -= ChangeObject;
    }

    //public void ChangeOwnerShipOnUser(bool isMyCharacter)
    //{
    //    if (Managers.Game.gameMode == Define.GameMode.Object && _playerHealth.Team == Define.Team.Hide)
    //    {
    //        if (isMyCharacter == false) return;
    //        var mapController = Managers.Scene.currentGameScene.mapController.GetComponent<ObjectModeMapController>();
    //        objectIndex = Random.Range(0, mapController.changeObjectList.objectList.Length);
    //        ShowChangeObejct(true);
    //    }
    //}

    void Die()
    {

    }


    void ShowChangeObejct(bool changeShow)
    {
        _changeObject.gameObject.SetActive(changeShow);
        //_playerController.playerCharacter.characterAvater.gameObject.SetActive(!changeShow);
        if(_playerController.playerHealth.IsMyCharacter() == false)
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
