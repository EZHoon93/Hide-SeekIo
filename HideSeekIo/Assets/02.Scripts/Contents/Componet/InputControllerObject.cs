using UnityEngine;
using System.Collections;
using System;
using Photon.Pun;
using System.Collections.Generic;

public class InputControllerObject : MonoBehaviourPun
{
    [SerializeField] Sprite _sprite;
    ConsumItem _consumItem;
    public InputType inputType { get; set; }
    public Define.AttackType attackType { get; set; }
    public PlayerShooter.state shooterState{ get; set; }
    public float InitCoolTime { get; set; }
    public float RemainCoolTime { get; set; }

    public event Action<Vector2> useEventCallBack;
    public event Action<Vector2> zoomEventCallBack;
    public Sprite sprite => _sprite;


    private void Awake()
    {
        _consumItem = GetComponent<ConsumItem>();
    }

    public void OnPhotonInstantiate(PlayerController newPlayerController)
    {
        RemainCoolTime = 0;
        newPlayerController.playerShooter.SetupInputControllerObject(this);
    }

  
    public void OnDestroyEvent()
    {
    }
 
    public void AddUseEvent(Action<Vector2> action)
    {
        useEventCallBack = action;
    }

    public void AddZoomEvent(Action<Vector2> action)
    {
        zoomEventCallBack = action;
    }

  

    public bool Use(Vector2 inputVector2)
    {
        print(RemainCoolTime);
        if (RemainCoolTime > 0) return false;
        RemainCoolTime = InitCoolTime;
        //소비용아이템이면.
        if (_consumItem)
        {
            _consumItem.Use();
        }
        else
        {
            //controllerInput.Use();
        }
        useEventCallBack?.Invoke(inputVector2);
        return true;
        
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
