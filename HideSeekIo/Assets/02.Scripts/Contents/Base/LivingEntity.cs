using System;
using Photon.Pun;
using UnityEngine;
using System.Collections.Generic;

public class LivingEntity : MonoBehaviourPun, IDamageable,  IPunObservable,IBuffable
{
    [SerializeField] FogOfWarController _fogOfWarController;
    [SerializeField] BuffController _buffController;
    [SerializeField] protected int initcurrHp = 2;


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
            int layer = UtillLayer.SetupLayerByTeam(_team);
            this.gameObject.layer = layer;
            Util.SetLayerRecursively(_fogOfWarController.gameObject, layer);
        }
    }

    public bool isEntityInGrass { get; set; }
    public virtual bool Dead { get; protected set; }
    public bool isShield { get; set; }

    public event Action onDeath;
    public event Action onRevive;
    public event Action<int> onDamageEventPoster;
    public event Action<int> onChangeMaxHpEvent;
    public event Action<int> onChangeCurrHpEvent;
    int _lastAttackViewID;


    public List<BuffController> n_buffControllerList { get; set; } = new List<BuffController>();
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
 
    private void OnEnable()
    {
        InitSetup();
    }

    public virtual void OnPhotonInstantiate()
    {
        Managers.Game.RegisterLivingEntity(this.photonView.ViewID, this);    //????

        currHp = maxHp;
    }

    public virtual void OnPreNetDestroy(PhotonView rootView)
    {
        foreach (var buff in n_buffControllerList.ToArray())
            Managers.Resource.Destroy(buff.gameObject);

        n_buffControllerList.Clear();
    }

    public virtual void ChangeOwnerShipOnUser(bool isMyCharacter)
    {

    }


    protected virtual void FixedUpdate()
    {
        //isEntityInGrass = false;
        isEntityInGrass = false;
      
    }

    private void LateUpdate()
    {
        _fogOfWarController.hideInFog.isInGrass = isEntityInGrass;  // OnTriggerStay, FixedUpdate???? ?????? ????????, ????????
    }

    public virtual void InitSetup()
    {
        Dead = false;
        buffController.Init(this);
        //currHp = initcurrHp;
        switch (Team)
        {
            //case Define.Team.Hide:
            //    fogController._fogOfWarUnit.shapeType = FoW.FogOfWarShapeType.Circle;
            //    fogController._fogOfWarUnit.offset = Vector2.zero;
            //    fogController.ChangeSight(5);
            //    break;
            //case Define.Team.Seek:
            //    fogController._fogOfWarUnit.shapeType = FoW.FogOfWarShapeType.Box;
            //    fogController._fogOfWarUnit.boxSize = new Vector2(5f, 5f);
            //    //fogController.ChangeSight(2);
            //    //fogController._fogOfWarUnit.angle = 360;
            //    //fogController._fogOfWarUnit.innerRadius = 0.3f;
            //    //fogController._fogOfWarUnit.circleRadius = 2.5f;
            //    fogController._fogOfWarUnit.offset = new Vector2(0, 2.0f);
            //    break;
        }

    }


  
    [PunRPC]
    public virtual void OnDamage(int damagerViewId, int damage, Vector3 hitPoint )
    {
        if (photonView.IsMine)
        {
            if (Dead) return;
            if (isShield) return;
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
            if (isShield) return;
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

    #region Buff
    //public virtual void OnApplyBuff(Define.BuffType buffType, float durationTime = -1)
    //{
    //    if (Dead) return;
    //    BuffController buffController = n_buffControllerList.Find(s => s.BuffType == buffType);
    //    float createServerTime = photonView.IsMine ? (float)PhotonNetwork.Time : 0;
    //    if (buffController == null)
    //    {
    //        buffController = Managers.Resource.Instantiate($"Buff/BuffController", this.transform).GetComponent<BuffController>();
    //        AddBuffController(buffController);
    //        buffController.SetupInfo(buffType, createServerTime, durationTime);
    //    }
    //    else
    //    {
    //        buffController.Renew(createServerTime);
    //    }
    //}
    //public void AddBuffController(BuffController newBuff)
    //{
    //    newBuff.transform.ResetTransform(this.transform);
    //    newBuff.SetupLivingEntitiy(this);
    //    n_buffControllerList.Add(newBuff);
    //    this.photonView.ObservedComponents.Add(newBuff);
    //}

    //public void RemoveBuffController(BuffController removeBuff)
    //{
    //    removeBuff.transform.SetParent(null);
    //    n_buffControllerList.Remove(removeBuff);
    //    this.photonView.ObservedComponents.Remove(removeBuff);
    //}

    //public void BuffControllerCheckOnLocal(Define.BuffType buffType ,float durationTime)
    //{
    //    BuffController buffController = n_buffControllerList.Find(s => s.BuffType == buffType);
    //    float createServerTime = (float)PhotonNetwork.Time;
    //    if (buffController == null)
    //    {
    //        n_buffControllerList.Add(buffController);
    //    }
    //    buffController.SetupInfo(buffType, createServerTime, durationTime);
    //}

    #endregion

    public void AddRenderer(RenderController renderController)
    {
        fogController.AddHideRender(renderController);
    }
    public void RemoveRenderer(RenderController renderController)
    {
        fogController.RemoveRenderer(renderController);
    }


    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Grass"))
        {
            isEntityInGrass = true;
        }
    }
}