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
                BuffManager.Instance.RegisterBuffControllerOnLivingEntity(buffController, this);
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
        // ü���� ���� ü������ �ʱ�ȭ
        Health = initHealth;
    }


    public virtual void OnPhotonInstantiate()
    {
        Managers.Game.RegisterLivingEntity(this.photonView.ViewID, this);    //���
    }


    // ������ ó��
    //���� ������ ó�� 
    [PunRPC]
    public virtual void OnDamage(int damagerViewId, int damage, Vector3 hitPoint)
    {
        if (photonView.IsMine)
        {
            Health -= damage;
            _lastAttackViewID = damagerViewId;  //������ ���� �÷��̾� ����̵� ����

            // ü���� 0 ���� && ���� ���� �ʾҴٸ� ��� ó�� ����
            if (Health <= 0 && !Dead)
            {
                Die();
            }
        }
    }


    public virtual void Die()
    {
        // onDeath �̺�Ʈ�� ��ϵ� �޼��尡 �ִٸ� ����
        if (photonView.IsMine)
        {

            PhotonGameManager.Instacne.HiderDieOnLocal(this.ViewID(), _lastAttackViewID);  //�ٸ� �������� �˸� =>viewGroup�� ���ظŴ����� ����
        }
        if (onDeath != null)
        {
            onDeath();
        }
        // ��� ���¸� ������ ����
        Dead = true;

        //var uiMain = Managers.UI.SceneUI as UI_Main;
        //uiMain.KillNotice

    }



}