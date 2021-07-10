
using TMPro;

using UnityEngine;

public class UI_KillNotice : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _textKillPlayer;
    [SerializeField] TextMeshProUGUI _deathKillPlayer;

    public void Setup(string killPlayer, string deathPlayer)
    {
        _textKillPlayer.text = killPlayer;
        _deathKillPlayer.text = deathPlayer;
        Invoke("AfeterDestroy", 3.0f);
    }

    void AfeterDestroy()
    {
        Managers.Resource.Destroy(this.gameObject);
    }
  
}
