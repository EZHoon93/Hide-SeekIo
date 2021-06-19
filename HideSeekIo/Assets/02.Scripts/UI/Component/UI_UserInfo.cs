using System.Collections;

using TMPro;

using UnityEngine;

public class UI_UserInfo : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _coinText;
    private void Start()
    {
        PlayerInfo.chnageInfoEvent += UpdateCoinText;
    }

    void UpdateCoinText()
    {
        _coinText.text = PlayerInfo.coin.ToString();
    }
}
