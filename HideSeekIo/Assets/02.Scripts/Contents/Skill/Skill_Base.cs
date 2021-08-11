using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public abstract class Skill_Base : MonoBehaviourPun , IAttack
{
    public Define.Skill skillType;
    public virtual Define.ControllerType controllerType { get; set; }
    public Action<IAttack> AttackSucessEvent { get; set; }
    public Action AttackEndEvent { get; set; }
    public string AttackAnim { get; set; }
    public PlayerController playerController { get; set; }

    public float InitCoolTime { get; set; }
    public float RemainCoolTime { get; set; }

    public virtual void Zoom(Vector2 inputVector2)
    {
        
    }

    public virtual void Use(PlayerController usePlayerController)
    {
        
    }

    public bool AttackCheck(Vector2 inputVector)
    {
        Use(playerController);
        RemainCoolTime = InitCoolTime;
        return true;
    }

    private void Update()
    {
        if (playerController == null) return;
        if (RemainCoolTime >= 0)
        {
            RemainCoolTime -= Time.deltaTime;
            if (playerController.IsMyCharacter())
            {
                InputManager.Instance.GetControllerJoystick(InputType.Skill)._UI_Slider_CoolTime.UpdateCoolTime(InitCoolTime, RemainCoolTime);
            }
        }
    }
}
