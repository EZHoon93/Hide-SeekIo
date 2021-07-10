using System.Collections;

using UnityEngine;
using Photon.Pun;
using System;

public class WeaponController : MonoBehaviour,  IPunInstantiateMagicCallback, IOnPhotonViewPreNetDestroy
{

    public enum WeaponType
    {
        Melee,
        Throw,
        Gun
    }
    public enum State
    {
        Delay,
        End
    }
    public enum Type
    {
        Permanent,//영구적
        Disposable  //일회용
    }

    //[SerializeField]
    //protected GameObject _uICanvas;

    //protected string _attackAnimationName;
    //protected float _attackDelayTime;   //대미지 주기까지 시간
    //protected float _afterAttackDelayTime;  //다음움직임 까지 시간
    //protected float _distance;
    //protected float _initCoolTime;
    //protected float _remianCoolTime;

    public string AttackAnim { get; set; }
    public float AttackDelay { get; set; }
    public float AfaterAttackDelay { get; set; }
    public float AttackDistance { get; set; }
    public float InitCoolTime { get; set; }
    public float ReaminCoolTime { get; set; }

    public Vector2 LastAttackInput { get; protected set; }     //공격 박향. 캐릭터 바라보는방향으로맞추기위해 
    public WeaponType weaponType { get; protected set; }
    public State state { get; set; }

    public Type type { get; set; }
    public GameObject UICanvas { get; set; }
    public AttackBase newAttacker { get; set; }

    public Action AttackSucessEvent;
    public Action AttackEndEvent;




    protected virtual void Awake()
    {
        UICanvas = GetComponentInChildren<Canvas>().gameObject;
        //UICanvas.SetActive(false);
    }


    public virtual void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        if (info.photonView.InstantiationData == null) return;
        UICanvas.SetActive(false);
        AttackSucessEvent = null;
        AttackEndEvent = null;
        var playerViewID = (int)info.photonView.InstantiationData[0];
        newAttacker = Managers.Game.GetLivingEntity(playerViewID).GetComponent<AttackBase>();
        //newAttacker.SetupWeapon(this);
        newAttacker.GetComponentInChildren<FogOfWarController>().AddHideRender(this.GetComponentInChildren<Renderer>());
        ReaminCoolTime = 0;
    }
    public void OnPreNetDestroy(PhotonView rootView)
    {
        if (Managers.Resource == null) return;
        UICanvas.transform.SetParent(this.transform);
    }

    public bool AttackCheck(Vector2 inputVector)
    {
        if (ReaminCoolTime > 0)
        {
            return false;
        }
        ReaminCoolTime = InitCoolTime;
        return true;
    }

    private void Update()
    {
        if (ReaminCoolTime >= 0)
        {
            ReaminCoolTime -= Time.deltaTime;
        }
    }
}
