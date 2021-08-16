
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using Photon.Pun;

public class UI_Info : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _roomStateText;
    [SerializeField] TextMeshProUGUI _levelText;
    [SerializeField] Slider _expSlider;

    private void Start()
    {
        PlayerInfo.chnageInfoEvent += UpdateCoinInfo;
        UpdateCoinInfo();
        StartCoroutine(UpdateServerInfo());

    }

    [ContextMenu("Setup")]
    void Setup()
    {
        _roomStateText = Util.FindChild<TextMeshProUGUI>(this.gameObject, "RoomState");
        _levelText = Util.FindChild<TextMeshProUGUI>(this.gameObject, "LevelText");
        _expSlider = Util.FindChild<Slider>(this.gameObject, "UserExpSlider");
    }

    void UpdateCoinInfo()
    {
        //_coinText.text = string.Format(PlayerInfo.coin.ToString(), "##.#");
        _levelText.text = PlayerInfo.level.ToString();
        _expSlider.maxValue = PlayerInfo.maxExp;
        _expSlider.value = PlayerInfo.exp;
    }


    IEnumerator UpdateServerInfo()
    {
        while (true)
        {
            string content = $"Ping : {PhotonNetwork.GetPing()}ms {PhotonNetwork.CloudRegion } ";
            _roomStateText.text = content;
            yield return new WaitForSeconds(1.0f);
        }
    }
}
