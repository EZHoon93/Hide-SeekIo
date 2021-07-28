using System.Collections;

using UnityEngine;
using Photon.Pun;
using System;

public class InGameItemController : MonoBehaviourPun , IPunInstantiateMagicCallback , IOnPhotonViewPreNetDestroy
{
    

    PlayerController _hasPlayer; //아이템을 가지고있는 플레이어 
    public Item_Base _item_Base { get; private set; }
    public Define.InGameItem itemType { get; private set; }

    public int Index { get;  set; }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        var data = info.photonView.InstantiationData;
        if (data == null) return;
        int buyPlayerViewID = (int)data[0];
        itemType = (Define.InGameItem)data[1];

        //얻은 플레이어찾음
        _hasPlayer = Managers.Game.GetLivingEntity(buyPlayerViewID).GetComponent<PlayerController>();
        if (_hasPlayer == null)
        {
            return;
        }
        //기존 설정되어있더 아이템 제거
        if(_item_Base != null)
        {
            Managers.Resource.Destroy(_item_Base.gameObject);
        }
        //새로운 아이템 으로 설정
        _item_Base = Managers.Resource.Instantiate($"InGameItem/{itemType.ToString()}",this.transform).GetComponent<Item_Base>();
        if (_item_Base)
        {
            _item_Base.OnPhotonInstantiate(_hasPlayer);
            _item_Base.DestroyEvent += () => PhotonNetwork.Destroy(this.gameObject);    //사용후 파괴
        }
        //플레이어에게 아이템 등록
        _hasPlayer.AddItem(this);    //플레이어에게 할당
    }

    public void OnPreNetDestroy(PhotonView rootView)
    {
        _hasPlayer.RemoveItem(this);
    }

    //일회용 아이템이면 삭제
    public void Use(PlayerController usePlayer)
    {
        if (_item_Base.State == Item_Base.UseState.Server)
        {
            photonView.RPC("UseOnServer", RpcTarget.All);
        }
        else
        {
            _item_Base.Use(_hasPlayer);
        }
    }

    [PunRPC]
    public void UseOnServer()
    {
        _item_Base.Use(_hasPlayer);
    }


}
