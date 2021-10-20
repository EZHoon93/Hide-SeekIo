using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(InputControllerObject))]
public abstract class Skill_Base : MonoBehaviourPun 
{
    [SerializeField] protected UI_ZoomBase _uI_ZoomBase;
    public virtual Define.Skill skillType { get; set; }
    public virtual Define.AttackType attakType { get; set; }
    public InputControllerObject inputControllerObject { get; protected set; }

    //public PlayerController playerController => inputControllerObject.playerController;
    public PlayerController playerController;
    private void Awake()
    {
        inputControllerObject = GetComponent<InputControllerObject>();
        SetupCallBack();
    }
    public void ChangeOnwerShip()
    {
    }
    protected virtual void SetupCallBack()
    {
        inputControllerObject = this.gameObject.GetOrAddComponent<InputControllerObject>();
        inputControllerObject.inputType = InputType.Sub1;
        inputControllerObject.attackType = attakType;
        inputControllerObject.AddUseEvent(Use);
        inputControllerObject.AddZoomEvent(Zoom);
    }

    public virtual void OnPhotonInstantiate(PlayerController newPlayerController)
    {
        playerController = newPlayerController;
        inputControllerObject.inputType = playerController.Team == Define.Team.Hide ? InputType.Sub2 : InputType.Sub2;
         //Managers.InputItemManager.SetupSkill(playerController, this);
        inputControllerObject.OnPhotonInstantiate(playerController);
    }

    private void Start()
    {
        SetupData();
    }


    protected virtual void SetupData()
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
