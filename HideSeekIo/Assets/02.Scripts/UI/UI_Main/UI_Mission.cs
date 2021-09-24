using System.Collections;
using System.Collections.Generic;

using DG.Tweening;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class UI_Mission : UI_Base
{
    [SerializeField] TextMeshProUGUI _titleText;
    [SerializeField] TextMeshProUGUI _timeText;
    [SerializeField] TextMeshProUGUI _sucessText;
    [SerializeField] Image _missionImage;
    [SerializeField] Transform _moveTarget;

    public override void Init()
    {
        
    }

    private void Start()
    {
        //_missionImage.gameObject.SetActive(false);
        _moveTarget.localPosition = Vector3.zero;
    }

    public void Setup(MissionInfo missionInfo)
    {
        _moveTarget.DOLocalMoveX(310, 1);
    }

    public void End()
    {
        _moveTarget.DOLocalMoveX(0, 1);

    }

    public void UpdateRemainTime(string content) => _timeText.text = content;
    public void UpdateSueessText(string content) => _sucessText.text = content;
}
