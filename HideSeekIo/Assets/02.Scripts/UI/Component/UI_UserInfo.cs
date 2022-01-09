using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
public class UI_UserInfo : UI_Independent 
{
    [SerializeField] TextMeshProUGUI _coinText;
    [SerializeField] TextMeshProUGUI _gemText;

    [SerializeField] TextMeshProUGUI _levelText;
    [SerializeField] Slider _expSlider;

  

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        PlayerInfo.chnageInfoEvent += UpdateCoinInfo;
        UpdateCoinInfo();
    }

    void UpdateCoinInfo()
    {
        _coinText.text = string.Format(PlayerInfo.coin.ToString(), "##.#");
        _gemText.text = string.Format(PlayerInfo.gem.ToString(), "##.#");
        _levelText.text = $"LV.{PlayerInfo.level.ToString()}";
        _expSlider.maxValue = PlayerInfo.maxExp;
        _expSlider.value = PlayerInfo.exp;
    }
 

   
}
