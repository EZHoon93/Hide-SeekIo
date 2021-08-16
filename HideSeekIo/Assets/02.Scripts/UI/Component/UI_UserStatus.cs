using System.Collections;

using TMPro;

using UnityEngine;
using UnityEngine.UI;
public class UI_UserStatus : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _coinText;
    [SerializeField] TextMeshProUGUI _adCountText;

    [ContextMenu("Setup")]
    void Setup()
    {
        _coinText = Util.FindChild<TextMeshProUGUI>(this.gameObject, "CoinText" ,true);
        _adCountText = Util.FindChild<TextMeshProUGUI>(this.gameObject, "ADCountText",true);
    }

    private void Start()
    {
        PlayerInfo.chnageInfoEvent += UpdateCoinInfo;
        UpdateCoinInfo();
    }



    void UpdateCoinInfo()
    {
        _coinText.text = string.Format(PlayerInfo.coin.ToString(), "##.#");
        _adCountText.text = PlayerInfo.level.ToString();
        
    }

}
