using System.Collections;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class UI_KillNotice : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _textKillPlayer;
    [SerializeField] TextMeshProUGUI _deathKillPlayer;

    public void Setup(string killPlayer, string deathPlayer)
    {
        _textKillPlayer.text = killPlayer;
        _deathKillPlayer.text = deathPlayer;
    }
  
}
