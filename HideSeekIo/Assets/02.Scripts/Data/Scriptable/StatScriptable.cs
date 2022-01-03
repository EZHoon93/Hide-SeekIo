using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "StatData", menuName = "EZ/Create StatData", order = 0)]


public class StatScriptable : ScriptableObject
{
    [SerializeField] int _initHp;
    [SerializeField] float _initMoveSpeed;


    public int InitHp => _initHp;
    public float InitMoveSpeed => _initMoveSpeed;

}
