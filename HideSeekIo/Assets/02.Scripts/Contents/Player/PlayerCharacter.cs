using System.Collections;

using UnityEngine;
using Photon.Pun;
public class PlayerCharacter : MonoBehaviourPun , IPunObservable
{
    [SerializeField] Transform _craeteTarget;


    CharacterAvater _characterAvater;
    LivingEntity _livingEntity;
    GameObject _changeObject;
    public CharacterAvater characterAvater => _characterAvater;
    public Animator animator => _characterAvater.animator;





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
            if(objectIndex == 0)
            {
                _changeObject?.gameObject.SetActive(false);
                characterAvater.gameObject.SetActive(true);
            }
            //0보다 작으면 안보여줌..
            else if(objectIndex < 0)
            {
                characterAvater.gameObject.SetActive(false);
                _changeObject?.gameObject.SetActive(false);
            }
            else
            {
                var mapController = Managers.Scene.currentGameScene.mapController.GetComponent<ObjectModeMapController>();
                var prefab = mapController.changeObjectList.objectList[objectIndex - 1];
                _changeObject = Instantiate(prefab);
                _changeObject.transform.ResetTransform(_craeteTarget.transform);
                _changeObject.gameObject.SetActive(true);
            }
        }
    }

    private void Awake()
    {
        _livingEntity = GetComponent<LivingEntity>();
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
        _characterAvater.OnPhotonInstantiate(playerController);
        _characterAvater.gameObject.SetActive(false);
        //Managers.Game.AddListenrOnGameState(Define.GameState.Gameing, GameStart);
        if(playerController.playerHealth.Team == Define.Team.Hide && Managers.Game.gameMode == Define.GameMode.Object)
        {
            //GetComponent<PlayerInput>().AddInputEvent()
        }
    }
    public virtual void OnPreNetDestroy(PhotonView rootView)
    {
        objectIndex = -2;

    }
    void GameStart()
    {
        //if (_livingEntity.Team == Define.Team.Seek)
        //{
        //    objectIndex = 1;
        //}
    }


    void CallBackMoveState(Vector2 vector2)
    {

    }
  
    public void ChangeOnwerShip(PlayerController _playerController)
    {
        if (photonView.IsMine && Managers.Scene.currentGameScene.gameMode == Define.GameMode.Object)
        {
            
            if(_playerController.Team == Define.Team.Seek)
            {
                objectIndex = -1;
            }
            else
            {
                var mapController = Managers.Scene.currentGameScene.mapController.GetComponent<ObjectModeMapController>();
                objectIndex = Random.Range(0, mapController.changeObjectList.objectList.Length)+1;
            }
        }
        objectIndex = 0;
    }
    public void CreateAvater(string avaterID)
    {
        if (_characterAvater)
        {
            Managers.Resource.Destroy(_characterAvater.gameObject);
        }
        string prefabID = $"Character/{avaterID}";
        _characterAvater = Managers.Resource.Instantiate(prefabID).GetComponent<CharacterAvater>();
        _characterAvater.transform.ResetTransform(_craeteTarget.transform);
        _characterAvater.gameObject.SetActive(false);
    }

    public void CreateAvaterByIndex(int index)
    {
        _characterAvater = Managers.Spawn.GetSkinByIndex(Define.ProductType.Character, index).GetComponent<CharacterAvater>();
        _characterAvater.transform.ResetTransform(_livingEntity.fogController.transform);

        //int layer = _livingEntity.Team == Define.Team.Hide ? (int)Define.Layer.Hider : (int)Define.Layer.Seeker;
        //Util.SetLayerRecursively(_characterAvater.gameObject, layer);
        //_characterAvater.gameObject.SetActive(false);
    }

    public RenderController GetRenderController()
    {
        return _characterAvater.renderController;
    }

   
}
