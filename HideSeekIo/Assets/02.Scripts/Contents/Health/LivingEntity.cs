using System;
using Photon.Pun;
using UnityEngine;
using System.Collections.Generic;

// 생명체로서 동작할 게임 오브젝트들을 위한 뼈대를 제공
// 체력, 데미지 받아들이기, 사망 기능, 사망 이벤트를 제공
public class LivingEntity : MonoBehaviourPun, IDamageable, IPunObservable
{
    public int initHealth = 2; // 시작 체력

    public virtual int Health { get; set; }

    public bool Dead { get; protected set; }
    public Define.Team Team;

    public event Action onDeath; // 사망시 발동할 이벤트

    int _lastAttackViewID;  //최근에 공격한플레이어 뷰아이디


    public List<BuffController> BuffControllerList { get; private set; } = new List<BuffController>();
    public FogOfWarController fogController { get; private set; }
    



    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(BuffControllerList.Count);
            stream.SendNext(Health);

        }
        else
        {
            int buffCount = (int)stream.ReceiveNext();
            Health = (int)stream.ReceiveNext();
            if (buffCount > BuffControllerList.Count)
            {
                var buffController = BuffManager.Instance.MakeBuffController(this.transform);
                AddBuffController(buffController);
            }

        }
    }

    protected virtual void Awake()
    {
        fogController = Managers.Resource.Instantiate("Contents/FogOfWar",this.transform).GetComponent<FogOfWarController>();
        fogController.transform.localPosition = new Vector3(0, 0.5f, 0);
        
    }
    
    private void OnEnable()
    {
        InitSetup();
    }

    public virtual void InitSetup()
    {
        Dead = false;
        // 체력을 시작 체력으로 초기화
        Health = initHealth;

        switch (Team)
        {
            case Define.Team.Hide:
                fogController._fogOfWarUnit.shapeType = FoW.FogOfWarShapeType.Circle;
                fogController._fogOfWarUnit.offset = Vector2.zero;
                fogController.ChangeSight(5);
                break;
            case Define.Team.Seek:
                //fogController._fogOfWarUnit.shapeType = FoW.FogOfWarShapeType.Box;
                //fogController._fogOfWarUnit.boxSize = new Vector2(2.5f, 2.5f);
                fogController.ChangeSight(2);
                fogController._fogOfWarUnit.angle = 30;
                fogController._fogOfWarUnit.innerRadius = 0.1f;
                fogController._fogOfWarUnit.circleRadius = 2.5f;
                fogController._fogOfWarUnit.offset = new Vector2(0, 1.0f);
                break;
        }
    }


    public virtual void OnPhotonInstantiate()
    {
        Managers.Game.RegisterLivingEntity(this.photonView.ViewID, this);    //등록
    }


    // 데미지 처리
    //로컬 유저가 처리 
    [PunRPC]
    public virtual void OnDamage(int damagerViewId, int damage, Vector3 hitPoint)
    {
        if (photonView.IsMine)
        {
            Health -= damage;
            _lastAttackViewID = damagerViewId;  //공격을 가한 플레이어 뷰아이디 저장

            // 체력이 0 이하 && 아직 죽지 않았다면 사망 처리 실행
            if (Health <= 0 && !Dead)
            {
                Die();
            }
        }
    }


    public virtual void Die()
    {
        // onDeath 이벤트에 등록된 메서드가 있다면 실행
        if (photonView.IsMine)
        {

            PhotonGameManager.Instacne.HiderDieOnLocal(this.ViewID(), _lastAttackViewID);  //다른 유저에게 알림 =>viewGroup을 위해매니저가 동작
        }
        if (onDeath != null)
        {
            onDeath();
        }
        // 사망 상태를 참으로 변경
        Dead = true;

        //var uiMain = Managers.UI.SceneUI as UI_Main;
        //uiMain.KillNotice

    }

    public void AddBuffController(BuffController newBuff)
    {
        BuffControllerList.Add(newBuff);
        this.photonView.ObservedComponents.Add(newBuff);

    }

    public void RemoveBuffController(BuffController removeBuff)
    {
        BuffControllerList.Remove(removeBuff);
        this.photonView.ObservedComponents.Remove(removeBuff);
    }

}