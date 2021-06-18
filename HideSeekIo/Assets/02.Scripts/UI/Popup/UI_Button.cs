using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Button : UI_Popup
{
    enum Buttons
    {
        PointButton,
        TextButton

    }

    enum Texts
    {
        PointText,
        ScoreText,
    }

    enum GameObjects
    {
        TestObject,
    }

    enum Images
    {
        ItemIcon,
    }

    public override void Init()
    {
        base.Init();

		Bind<Button>(typeof(Buttons));
		Bind<Text>(typeof(Texts));
		Bind<GameObject>(typeof(GameObjects));
		Bind<Image>(typeof(Images));

		GetButton((int)Buttons.PointButton).gameObject.BindEvent(OnButtonClicked);
        GetButton((int)Buttons.TextButton).gameObject.BindEvent(OnButtonClicked);

        GameObject go = GetImage((int)Images.ItemIcon).gameObject;
		BindEvent(go, (PointerEventData data) => { go.transform.position = data.position; }, Define.UIEvent.Drag);


       

    }

    int _score = 0;

    public void OnButtonClicked(PointerEventData data)
    {
        _score++;
        GetText((int)Texts.ScoreText).text = $"점수 : {_score}";
    }
    public void OnButtonClicked2(PointerEventData data)
    {
        print("테스트");
    }

}
