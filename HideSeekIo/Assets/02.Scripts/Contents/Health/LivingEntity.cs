using System;
using Photon.Pun;
using UnityEngine;
using System.Collections.Generic;

public class LivingEntity : MonoBehaviourPun, IDamageable, IPunObservable
{
    [SerializeField] FogOfWarController _fogOfWarController;
    public int initHealth = 2; 
    public virtual int Health { get; set; }
    public virtual bool Dead { get; protected set; }
    public bool isShield { get; set; }
    public Define.Team Team; 
    public event Action onDeath;
    public event Action<int> onDamageEventPoster;
    int _lastAttackViewID;

    public List<BuffController> n_buffControllerList { get; set; } = new List<BuffController>();



    public FogOfWarController fogController => _fogOfWarController;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(Health);
            stream.SendNext(Dead);
            stream.SendNext(n_buffControllerList.Count);

        }
        else
        {
            Health = (int)stream.ReceiveNext();
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

    public virtual void InitSetup()
    {
        Dead = false;
        Health = initHealth;

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
    }


    // ?????? ????
    //???? ?????? ???? 
    [PunRPC]
    public virtual void OnDamage(int damagerViewId, int damage, Vector3 hitPoint)
    {
        if (photonView.IsMine)
        {
            if (isShield) return;
            Health -= damage;
            _lastAttackViewID = damagerViewId;  
            if (Health <= 0 && !Dead)
            {
                //Die();
                photonView.RPC("Die", RpcTarget.All);
                return;
            }
            if (Health > 0)
            {
                photonView.RPC("OnDamage", RpcTarget.Others , damagerViewId,damage,hitPoint);
            }
        }
        if (Health > 0)
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
}