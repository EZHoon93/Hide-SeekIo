using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class UI_Button : UI_Base
{
    public override void Init()
    {
        GetComponent<Button>().onClick.AddListener(OnClickEvent);
    }

    protected abstract void OnClickEvent();
    
}
