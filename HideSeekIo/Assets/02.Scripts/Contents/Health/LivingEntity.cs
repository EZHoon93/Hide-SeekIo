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

    //public Transform HideTransform { get; private set; }


    public List<BuffController> BuffControllerList { get; set; } = new List<BuffController>();



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
                //BuffControllerList.Add(ref buffController);
                print(buffCount + "버프카운트수 ");
                //this.photonView.ObservedComponents.Add(buffController.photonView);
                BuffManager.Instance.RegisterBuffControllerOnLivingEntity(buffController, this);
            }

        }
    }

    private void Start()
    {
        InitSetup();
    }

    public virtual void InitSetup()
    {
        Dead = false;
        // 체력을 시작 체력으로 초기화
        Health = initHealth;
    }

    protected void OnEnable()
    {
        print("LivingEnetity OnEnable");
        GameManager.Instance.RegisterLivingEntity(this.photonView.ViewID, this);    //등록
    }

    public virtual void OnPhotonInstantiate()
    {
        print("LivingEnetity OnPhotonInstantiate");
        if (!GameManager.Instance) return;
    }

    // 데미지 처리
    // 호스트에서 먼저 단독 실행되고, 호스트를 통해 다른 클라이언트들에서 일괄 실행됨
    [PunRPC]
    public virtual void OnDamage(int damagerViewId, int damage, Vector3 hitPoint)
    {
        if (photonView.IsMine)
        {
            Health -= damage;
            print("대미지!!!!!!!!!!!!!!!!!" + damage + "/" + Health);

            // 다른 클라이언트들도 OnDamage를 실행하도록 함
            //photonView.RPC("OnDamage", RpcTarget.Others, damagerViewId, damage);

            // 체력이 0 이하 && 아직 죽지 않았다면 사망 처리 실행
            if (Health <= 0 && !Dead)
            {
                photonView.RPC("Die", RpcTarget.All);
            }
        }
        //else
        //{
        //    //대미지 이펙트
        //}

        //// 체력이 0 이하 && 아직 죽지 않았다면 사망 처리 실행
        //if (Health <= 0 && !Dead)
        //{
        //    Die();
        //}

    }

    [PunRPC]
    public virtual void Die()
    {
        // onDeath 이벤트에 등록된 메서드가 있다면 실행

        if (onDeath != null)
        {
            onDeath();
        }
        // 사망 상태를 참으로 변경
        Dead = true;
    }



    //public void OnPhotonInstantiate(PhotonMessageInfo info)
    //{
    //    if (info.photonView.InstantiationData == null) return;
    //    if (!GameManager.Instance) return;
    //    GameManager.Instance.RegisterLivingEntity(this.photonView.ViewID, this);    //등록
    //}

    public virtual void OnPreNetDestroy(PhotonView rootView)
    {
        if (!GameManager.Instance) return;
        //var poolableList =  GetComponent<PlayerSetup>()._createObjectList;
        //if (poolableList != null)
        //{
        //    foreach (var p in poolableList)
        //        p.Push();
        //}
        ////중복있으면제거
        //GameManager.instance.UnRegisterLivingEntity(this.photonView.ViewID, this);

        //if (!CameraManager.instance) return;
        //if (GameManager.instance.State != Define.GameState.End)
        //{
        //    if (CameraManager.instance.Target.GetLivingEntity() == this)    //현재보는캐릭이랑같다면
        //    {
        //        CameraManager.instance.ChangeNextPlayer();
        //    }
        //}


        ////내캐릭이라면 
        //if (this.IsMyCharacter())
        //{
        //    //UIManager.instance.GetButton_Etc(UI_Button_Etc.EzType.Camera).gameObject.SetActive(true);
        //    var inventory = UIManager.instance.GetSingleUI(UI_Single_Base.EzType.Inventory) as UI_Inventory;
        //    inventory.SetActive(false, Team);
        //}
    }


}