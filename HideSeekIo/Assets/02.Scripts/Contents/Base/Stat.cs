using System.Collections;

using UnityEngine;
using Photon.Pun;

public class Stat : MonoBehaviourPun
{
    [SerializeField] StatScriptable _statScriptable;
    protected Animator _animator;

    public Animator animator => _animator;

    public float moveSpeed => _statScriptable.InitMoveSpeed;


    public int maxHp => _statScriptable.InitHp;
   


}
