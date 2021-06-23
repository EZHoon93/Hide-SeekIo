﻿using System.Collections;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class UI_UserInfo : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _coinText;
    [SerializeField] TextMeshProUGUI _levelText;
    [SerializeField] Slider _expSlider;
    private void Start()
    {
        PlayerInfo.chnageInfoEvent += UpdateCoinInfo;
        UpdateCoinInfo();
    }

    void UpdateCoinInfo()
    {
        _coinText.text = string.Format(PlayerInfo.coin.ToString(), "##.#");
        _levelText.text = PlayerInfo.level.ToString();
        _expSlider.maxValue = PlayerInfo.maxExp;
        _expSlider.value = PlayerInfo.exp;
    }
}
