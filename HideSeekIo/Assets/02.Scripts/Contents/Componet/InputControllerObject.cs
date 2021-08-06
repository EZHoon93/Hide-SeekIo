using UnityEngine;
using System.Collections;
using System;
using Photon.Pun;


public class InputControllerObject : MonoBehaviour, IPunInstantiateMagicCallback
{
    public Define.ControllerType controllerType;

    public string AttackAnim { get; set; }
    public float AttackDelay { get; set; }
    public float AfaterAttackDelay { get; set; }
    public float AttackDistance { get; set; }
    public float InitCoolTime { get; set; }
    public float ReaminCoolTime { get; set; }
    public Vector2 LastAttackInput { get; protected set; }     //공격 박향. 캐릭터 바라보는방향으로맞추기위해 

    [SerializeField] protected Transform _zoomUI;
    public PlayerController hasPlayerController { get; set; }

    public Action<Weapon> UseSucessEvent;
    public Action UseEndEvent;
    public GameObject UICanvas { get; set; }


    protected virtual void Awake()
    {
        UICanvas = GetComponentInChildren<Canvas>().gameObject;
    }


    public virtual void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        if (info.photonView.InstantiationData == null) return;
        UICanvas.SetActive(true);
        _zoomUI.gameObject.SetActive(false);
        var playerViewID = (int)info.photonView.InstantiationData[0];
        //hasPlayerController.GetAttackBase().SetupWeapon(this, isBaseWeapon);

    }
}
