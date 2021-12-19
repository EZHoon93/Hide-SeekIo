using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;

public class UI_CountDown: MonoBehaviour
{
    //[SerializeField] string
    TextMeshProUGUI _countDownText;

    private void Awake()
    {
        _countDownText = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        Managers.Game.AddListenrOnGameEvent(Define.GameEvent.ChangeReadyTime,UpdateGameReadyTimeText);

    }

    void UpdateGameReadyTimeText(object newTime)
    {
        var gameMode = Managers.Scene.currentGameScene.gameMode;
        switch (gameMode)
        {
            case Define.GameMode.Object:
                ObjectModeCountText((int)newTime);
                break;
        }
    }

    void ObjectModeCountText(int newTime)
    {
        _countDownText.text = "잠시 s후 게임이 시작됩니다. ";
    }
}
