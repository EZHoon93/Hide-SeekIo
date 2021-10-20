using System.Collections;

using UnityEngine;
using Photon.Pun;

public class Stat : MonoBehaviourPun
{

    [SerializeField] protected float _moveSpeed;

    public float moveSpeed
    {
        get => _moveSpeed;
        set
        {
            _moveSpeed = value;
        }
    }

    [SerializeField] protected int _maxHp;

    public int maxHp 
    {
        get => _maxHp;
        set
        {
            _maxHp = value;
        }
    }


}
