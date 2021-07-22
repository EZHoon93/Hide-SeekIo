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
        // ü���� ���� ü������ �ʱ�ȭ
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