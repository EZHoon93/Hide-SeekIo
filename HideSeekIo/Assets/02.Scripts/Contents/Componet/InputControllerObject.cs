using UnityEngine;
using System.Collections;
using System;
using Photon.Pun;
using System.Collections.Generic;

public class InputControllerObject : MonoBehaviourPun
{
    [SerializeField] Sprite _sprite;
    public PlayerController playerController { get; private set; }
    public ConsumItem consumItem { get; private set; }
    public InputType inputType { get; set; }
    public Define.AttackType attackType { get; set; }
    public PlayerShooter.state shooterState{ get; set; }
    public float InitCoolTime { get; set; }
    public float RemainCoolTime { get; set; }
    public event Action<Vector2> useEventCallBack;
    public event Action<Vector2> zoomEventCallBack;
    public event Action useSucessStartCallBack;
    public event Action useSucessEndCallBack;
    public Sprite sprite => _sprite;
    public Vector2 lastInputSucessVector2 { get; set; }
    public Vector3 attackDirection { get; set; }

    IEnumerator coolTimeEnumerator;

    private void Awake()
    {
        consumItem = GetComponent<ConsumItem>();

    }

    public void OnPhotonInstantiate(PlayerController newPlayerController)
    {
        playerController = newPlayerController;
        RemainCoolTime = 0;
        Managers.InputItemManager.SetupControllerObject(playerController, this);
    }

    public void OnDestroyEvent()
    {
        if (playerController == null) return;
        playerController.playerInput.RemoveInputEvent(inputType);
        playerController = null;
    }
 
    public void AddUseEvent(Action<Vector2> action)
    {
        useEventCallBack = action;
    }

    public void AddZoomEvent(Action<Vector2> action)
    {
        zoomEventCallBack = action;
    }

    IEnumerator UpdateRemainCoolTime()
    {
        while(RemainCoolTime > 0)
        {
            yield return null;
            RemainCoolTime -= Time.deltaTime;
        }
    }

    public bool Use(Vector2 inputVector2)
    {
        if (RemainCoolTime > 0) return false;
        RemainCoolTime = InitCoolTime;
        lastInputSucessVector2 = inputVector2;
        //소비용아이템이면.
        if (consumItem)
        {
            consumItem.Use(playerController, inputType);
            playerController = null;
        }
        else
        {
            if (playerController.IsMyCharacter())
            {
                InputManager.Instance.GetControllerJoystick(inputType).StartCoolTime(RemainCoolTime);
            }
        }
        useEventCallBack?.Invoke(inputVector2);
        return true;
        
    }

    /// <summary>
    /// 사용 성공, 프로세스 시작
    /// </summary>
    public void Call_UseSucessStart()
    {
        useSucessStartCallBack?.Invoke();

    }
    public void Call_UseSucessEnd()
    {
        useSucessEndCallBack?.Invoke();
        if (consumItem)
        {
            Managers.Resource.PunDestroy(this);
        }
        
    }

    public void Zoom(Vector2 inputVector2)
    {
        zoomEventCallBack?.Invoke(inputVector2);
    }

    private void Update()
    {
        if(RemainCoolTime > 0)
        {
            RemainCoolTime -= Time.deltaTime;
        }
    }
}
