using System;
using Photon.Pun;
using UnityEngine;
using System.Collections.Generic;

// ����ü�μ� ������ ���� ������Ʈ���� ���� ���븦 ����
// ü��, ������ �޾Ƶ��̱�, ��� ���, ��� �̺�Ʈ�� ����
public class LivingEntity : MonoBehaviourPun, IDamageable, IPunObservable
    
{
    public int initHealth = 2; // ���� ü��

    public virtual int Health { get; set; }

    public bool Dead { get; protected set; }
    public Define.Team Team;

    public event Action onDeath; // ����� �ߵ��� �̺�Ʈ

    int _lastAttackViewID;  //�ֱٿ� �������÷��̾� ����̵�

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
                print(buffCount + "����ī��Ʈ�� ");
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
        // ü���� ���� ü������ �ʱ�ȭ
        Health = initHealth;
    }

    protected void OnEnable()
    {
        print("LivingEnetity OnEnable");
        GameManager.Instance.RegisterLivingEntity(this.photonView.ViewID, this);    //���
    }

    public virtual void OnPhotonInstantiate()
    {
        print("LivingEnetity OnPhotonInstantiate");
        if (!GameManager.Instance) return;
    }

    // ������ ó��
    // ȣ��Ʈ���� ���� �ܵ� ����ǰ�, ȣ��Ʈ�� ���� �ٸ� Ŭ���̾�Ʈ�鿡�� �ϰ� �����
    [PunRPC]
    public virtual void OnDamage(int damagerViewId, int damage, Vector3 hitPoint)
    {
        if (photonView.IsMine)
        {
            Health -= damage;
            print("�����!!!!!!!!!!!!!!!!!" + damage + "/" + Health);

            // �ٸ� Ŭ���̾�Ʈ�鵵 OnDamage�� �����ϵ��� ��
            //photonView.RPC("OnDamage", RpcTarget.Others, damagerViewId, damage);

            // ü���� 0 ���� && ���� ���� �ʾҴٸ� ��� ó�� ����
            if (Health <= 0 && !Dead)
            {
                photonView.RPC("Die", RpcTarget.All);
            }
        }
        //else
        //{
        //    //����� ����Ʈ
        //}

        //// ü���� 0 ���� && ���� ���� �ʾҴٸ� ��� ó�� ����
        //if (Health <= 0 && !Dead)
        //{
        //    Die();
        //}

    }

    [PunRPC]
    public virtual void Die()
    {
        // onDeath �̺�Ʈ�� ��ϵ� �޼��尡 �ִٸ� ����

        if (onDeath != null)
        {
            onDeath();
        }
        // ��� ���¸� ������ ����
        Dead = true;
    }



    //public void OnPhotonInstantiate(PhotonMessageInfo info)
    //{
    //    if (info.photonView.InstantiationData == null) return;
    //    if (!GameManager.Instance) return;
    //    GameManager.Instance.RegisterLivingEntity(this.photonView.ViewID, this);    //���
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
        ////�ߺ�����������
        //GameManager.instance.UnRegisterLivingEntity(this.photonView.ViewID, this);

        //if (!CameraManager.instance) return;
        //if (GameManager.instance.State != Define.GameState.End)
        //{
        //    if (CameraManager.instance.Target.GetLivingEntity() == this)    //���纸��ĳ���̶����ٸ�
        //    {
        //        CameraManager.instance.ChangeNextPlayer();
        //    }
        //}


        ////��ĳ���̶�� 
        //if (this.IsMyCharacter())
        //{
        //    //UIManager.instance.GetButton_Etc(UI_Button_Etc.EzType.Camera).gameObject.SetActive(true);
        //    var inventory = UIManager.instance.GetSingleUI(UI_Single_Base.EzType.Inventory) as UI_Inventory;
        //    inventory.SetActive(false, Team);
        //}
    }


}