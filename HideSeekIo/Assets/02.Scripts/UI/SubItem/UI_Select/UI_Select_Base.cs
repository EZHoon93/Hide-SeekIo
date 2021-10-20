using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public   class UI_Select_Base : UI_Base
{
    public event Action clickEvenetCallBack;
    Button _button;
    enum Buttons
    {
        Button
    }

    
    public override void Init()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(Click); 
    }

    public void SetActiveButton(bool active) => _button.interactable = active;

    protected virtual void Click()
    {
        clickEvenetCallBack?.Invoke();  //UI 꺼줌,
    }
}
