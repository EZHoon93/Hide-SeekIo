using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;   
public class PlayerObjectController : MonoBehaviourPun , IPunObservable
{
    [SerializeField] PlayerCharacter _playerCharacter;
    [SerializeField] PlayerMove _playerMove;
    [SerializeField] PlayerHealth _playerHealth;
    [SerializeField] PlayerUI _playerUI;
    GameObject _changeObject;
  
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
            _changeObject.transform.ResetTransform(_playerHealth.fogController.transform);
        }
    }


    private void Awake()
    {
        this.gameObject.SetActive(false);
        _playerHealth.onDeath += Die;
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
    public void OnPhotonInstantiate(PlayerController playerController)
    {
        //사물모드일 경우.
        if(Managers.Game.gameMode == Define.GameMode.Object && playerController.Team == Define.Team.Hide)
        {
            this.gameObject.SetActive(true);
            playerController.photonView.ObservedComponents.Add(this);
            _playerMove.onChangeMoveStateEvent += ChangeObject;
        }
        else
        {
            this.gameObject.SetActive(false);
        }
    }

    public void OnPreNetDestroy(PhotonView rootView)
    {

        if (Managers.Game.gameMode == Define.GameMode.Object)
        {
            _playerCharacter.photonView.ObservedComponents.Remove(this);
            _playerMove.onChangeMoveStateEvent -= ChangeObject;

        }
    }

    public void ChangeOwnerShipOnUser(bool isMyCharacter)
    {
        if (Managers.Game.gameMode == Define.GameMode.Object && _playerHealth.Team == Define.Team.Hide)
        {
            if (isMyCharacter == false) return;
            var mapController = Managers.Scene.currentGameScene.mapController.GetComponent<ObjectModeMapController>();
            objectIndex = Random.Range(0, mapController.changeObjectList.objectList.Length);
            ShowChangeObejct(true);
        }
    }

    void Die()
    {

    }


    void ShowChangeObejct(bool changeShow)
    {
        if (_changeObject == null)
            return;
        _changeObject.gameObject.SetActive(changeShow);
        _playerCharacter.characterAvater.gameObject.SetActive(!changeShow);
        _playerUI.SetActiveNameUI(!changeShow);
    }

    void ChangeObject(PlayerMove.MoveState moveState)
    {
        if (_changeObject == null)
            return;
        switch (moveState)
        {
            case PlayerMove.MoveState.Idle:
                ShowChangeObejct(true);
                break;
            default:
                ShowChangeObejct(false);
                break;
        }
    }
 
}
