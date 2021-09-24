using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI_Select_Base : UI_Base
{
    [SerializeField] Define.StatType _statType;

    public Define.StatType statType => _statType;

    public event Action clickEvenetCallBack;
    Button _button;
    enum Buttons
    {
        Button
    }

    
    public override void Init()
    {
        //Bind<Button>(typeof(Buttons));
        //GetButton((int)Buttons.Button).gameObject.BindEvent(Click);
        _button = GetComponent<Button>();
        _button.onClick.AddListener(Click); 
    }

    public void SetActiveButton(bool active) => _button.interactable = active;

    void Click()
    {
        clickEvenetCallBack?.Invoke();  //UI 꺼줌,
        var playerController = Managers.Game.myPlayer;
        if (!playerController) return;
        Managers.StatSelectManager.PostEvent_StatDataToServer(playerController, statType);
    }
}
