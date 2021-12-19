using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;   
public class PlayerObjectController : MonoBehaviourPun , IPunObservable
{
    [SerializeField] PlayerCharacter _playerCharacter;
    [SerializeField] PlayerMove _playerMove;
    GameObject _changeObject;

    int _seletObjectIndex; //선택된 인덱스
    /// <summary>
    /// 0이면 캐릭터, -1이면 안보여줌, 그이상은 해당맵의 오브젝트
    /// </summary>
    int _objectIndex = -2;
    public int objectIndex
    {
        get => _objectIndex;
        set
        {
            if (_objectIndex == value) return;
            _objectIndex = value;
            if (objectIndex == -1)
            {
                _changeObject?.gameObject.SetActive(false);
                _playerCharacter.characterAvater.gameObject.SetActive(true);
            }
            else
            {
                var mapController = Managers.Scene.currentGameScene.mapController.GetComponent<ObjectModeMapController>();
                var prefab = mapController.changeObjectList.objectList[objectIndex];
                if (_changeObject)
                {
                    Managers.Resource.Destroy(_changeObject);
                }
                _changeObject = Instantiate(prefab);
                _changeObject.transform.ResetTransform(this.transform);
                _changeObject.gameObject.SetActive(true);
            }
        }
    }


    private void Awake()
    {
        this.gameObject.SetActive(false);

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

    void ChangeObject(PlayerMove.MoveState moveState)
    {
        print("ChangeObj");
    }
 
}
