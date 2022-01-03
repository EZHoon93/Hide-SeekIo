using System;
using Photon.Pun;
using UnityEngine;
using System.Collections.Generic;

public class LivingEntity : MonoBehaviourPun, IDamageable,  IPunObservable,IBuffable
{
    [SerializeField] FogOfWarController _fogOfWarController;
    [SerializeField] BuffController _buffController;


    int _maxHp;
    public int maxHp 
    {
        get => _maxHp;
        set
        {
            _maxHp = value;
            onChangeMaxHpEvent?.Invoke(value);
        }
    }

    int _currHp;

    public virtual  int currHp 
    {
        get => _currHp;
        set
        {
            _currHp = value;
            onChangeCurrHpEvent?.Invoke(value);
        }
    }
    Define.Team _team;
    public Define.Team Team
    {
        get => _team;
        set
        {
            _team = value;
            int layer = UtillLayer.GetLayerByTeam(_team);
            this.gameObject.layer = layer;
            Util.SetLayerRecursively(_fogOfWarController.gameObject, layer);
            onChangeTeamEvent?.Invoke(_team);
            if (this.IsMyCharacter())
            {

            }
        }
    }

    public virtual bool Dead { get; protected set; }
    public event Action onDeath;
    public event Action onRevive;
    public event Action<int> onDamageEventPoster;
    public event Action<int> onChangeMaxHpEvent;
    public event Action<int> onChangeCurrHpEvent;
    public event Action<Define.Team> onChangeTeamEvent;

    int _lastAttackViewID;
    public FogOfWarController fogController => _fogOfWarController;
    public BuffController buffController => _buffController;


  
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(currHp);
            stream.SendNext(Dead);
        }
        else
        {
            currHp = (int)stream.ReceiveNext();
            bool n_dead = (bool)stream.ReceiveNext();
            if(Dead != n_dead)
            {
                Dead = n_dead;
            }
        }
    }
 
   

    public virtual void OnPhotonInstantiate()
    {
        Managers.Game.RegisterLivingEntity(this.photonView.ViewID, this);
        currHp = maxHp;
        Dead = false;
        buffController.Init(this);

    }

    public virtual void OnPreNetDestroy(PhotonView rootView)
    {
        Managers.Game.UnRegisterLivingEntity(this.ViewID());
    }


  
    [PunRPC]
    public virtual void OnDamage(int damagerViewId, int damage, Vector3 hitPoint )
    {
        if (photonView.IsMine)
        {
            if (Dead) return;
            currHp = Mathf.Clamp(currHp -damage ,0 ,maxHp);
            _lastAttackViewID = damagerViewId;
            if (currHp <= 0 && !Dead)
            {
                //Die();
                photonView.RPC("Die", RpcTarget.All);
                return;
            }
            if (currHp > 0)
            {
                photonView.RPC("OnDamage", RpcTarget.Others , damagerViewId,damage,hitPoint);
            }
        }
        if (currHp > 0)
        {
            onDamageEventPoster?.Invoke(damage);
        }
    }

  

    [PunRPC]
    public virtual void Die()
    {
        if (photonView.IsMine)
        {
            Managers.photonGameManager.HiderDieOnLocal(this.ViewID(), _lastAttackViewID); 
        }
        if (onDeath != null)
        {
            onDeath();
        }
        Dead = true;
    }


    public void AddRenderer(RenderController renderController)
    {
        fogController.AddHideRender(renderController);
    }
    public void RemoveRenderer(RenderController renderController)
    {
        fogController.RemoveRenderer(renderController);
    }
   
}