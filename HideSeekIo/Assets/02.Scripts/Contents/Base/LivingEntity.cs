using System;
using Photon.Pun;
using UnityEngine;
using System.Collections.Generic;

public class LivingEntity : MonoBehaviourPun, IDamageable, IBuffable, IPunObservable
{
    [SerializeField] FogOfWarController _fogOfWarController;
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
    public bool isEntityInGrass { get; set; }
    public virtual bool Dead { get; protected set; }
    public bool isShield { get; set; }
    public Define.Team Team; 
    public event Action onDeath;
    public event Action onRevive;

    public event Action<int> onDamageEventPoster;

    public event Action<int> onChangeMaxHpEvent;
    public event Action<int> onChangeCurrHpEvent;

    int _lastAttackViewID;

    public List<BuffController> n_buffControllerList { get; set; } = new List<BuffController>();
    public FogOfWarController fogController => _fogOfWarController;

  

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(currHp);
            stream.SendNext(Dead);
            stream.SendNext(n_buffControllerList.Count);

        }
        else
        {
            currHp = (int)stream.ReceiveNext();
            bool n_dead = (bool)stream.ReceiveNext();
            int buffCount = (int)stream.ReceiveNext();
            if (buffCount > n_buffControllerList.Count)
            {
                BuffManager.Instance.CheckBuffController(this);
            }
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

    protected virtual void FixedUpdate()
    {
        //isEntityInGrass = false;
        isEntityInGrass = false;
      
    }

    private void LateUpdate()
    {
        _fogOfWarController.hideInFog.isInGrass = isEntityInGrass;  // OnTriggerStay, FixedUpdate에서 판단후 최종판단, 값을넘김
    }

    public virtual void InitSetup()
    {
        //Dead = false;
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


    public virtual void OnPhotonInstantiate()
    {
        Managers.Game.RegisterLivingEntity(this.photonView.ViewID, this);    //????

        currHp = maxHp;
    }


    // ?????? ????
    //???? ?????? ???? 
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

    public virtual void OnApplyBuff(Define.BuffType buffType, float durationTime = -1)
    {
        if (Dead) return;
        BuffController buffController = n_buffControllerList.Find(s => s.BuffType == buffType);
        float createServerTime = photonView.IsMine ? (float)PhotonNetwork.Time : 0;
        if (buffController == null)
        {
            buffController = Managers.Resource.Instantiate($"Buff/BuffController",this.transform).GetComponent<BuffController>();
            n_buffControllerList.Add(buffController);
            this.photonView.ObservedComponents.Add(buffController);
            buffController.SetupLivingEntitiy(this);
            buffController.SetupInfo(buffType, createServerTime, durationTime);
        }
        else
        {
            buffController.Renew(createServerTime);
        }
    }

    [PunRPC]
    public virtual void Die()
    {
        if (photonView.IsMine)
        {

            PhotonGameManager.Instacne.HiderDieOnLocal(this.ViewID(), _lastAttackViewID); 
        }
        if (onDeath != null)
        {
            onDeath();
        }
        Dead = true;
    }

    #region Buff

    //public void AddBuffController(BuffController newBuff)
    //{
    //    var go = Managers.Resource.Instantiate($"Buff/BuffController", this.transform).GetComponent<BuffController>(); 
    //    go.transform.localPosition = Vector3.zero;
    //    go.gameObject.SetLayerRecursively(this.gameObject.layer);

    //    n_buffControllerList.Add(newBuff);

    //    this.photonView.ObservedComponents.Add(newBuff);

    //}

    //public void RemoveBuffController(BuffController removeBuff)
    //{
    //    n_buffControllerList.Remove(removeBuff);
    //    this.photonView.ObservedComponents.Remove(removeBuff);
    //}

    //public void BuffControllerCheckOnLocal(Define.BuffType buffType, LivingEntity livingEntity)
    //{
    //    if (livingEntity == null) return;
    //    if (livingEntity.Dead) return;
    //    var buffControllerList = livingEntity.n_buffControllerList;
    //    BuffController buffController = buffControllerList.Find(s => s.BuffType == buffType);
    //    //float durationTime = 10;
    //    float createServerTime = (float)PhotonNetwork.Time;

    //    if (buffController == null)
    //    {
    //        //buffController = n_buffControllerList(livingEntity.transform);
    //        livingEntity.AddBuffController(buffController);
    //    }
    //    buffController.Setup(buffType, livingEntity, createServerTime);
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