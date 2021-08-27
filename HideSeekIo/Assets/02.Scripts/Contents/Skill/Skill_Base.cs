using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public abstract class Skill_Base : MonoBehaviourPun 
{
    public Define.Skill skillType;
    public virtual Define.AttackType attakType { get; set; }
    public PlayerController playerController { get; set; }
    public InputControllerObject inputControllerObject { get; protected set; }


    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        SetupCallBack();
    }
    protected virtual void SetupCallBack()
    {
        inputControllerObject = this.gameObject.GetOrAddComponent<InputControllerObject>();
        inputControllerObject.inputType = InputType.Sub2;
        inputControllerObject.attackType = Define.AttackType.Button;
        inputControllerObject.AddUseEvent(Use);
        inputControllerObject.AddZoomEvent(Zoom);
    }

    private void Start()
    {
        playerController.playerShooter.SetupControllerObject(inputControllerObject);
        Setup();
    }

    protected virtual void Setup()
    {

    }

    //public void OnPho


    public virtual void Zoom(Vector2 inputVector2)
    {
        
    }

    public virtual void Use(Vector2 inputVector)
    {
        print("Skii Use ");
    }
}
