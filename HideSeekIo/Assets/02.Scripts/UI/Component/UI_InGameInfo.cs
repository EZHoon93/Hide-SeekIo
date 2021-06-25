using TMPro;
using UnityEngine;


public class UI_InGameInfo : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _seekerCountText;
    [SerializeField] TextMeshProUGUI _hiderCountText;
    [SerializeField] TextMeshProUGUI _gameTimeText;


    public void ResetTextes()
    {
        UpdateSeekerText(0);
        UpdateHiderText(0);
        UpdateGameTimeText(0);
    }

    public void UpdateSeekerText(int count)
    {
        _seekerCountText.text = count.ToString();
    }

    public void UpdateHiderText(int count)
    {
        _hiderCountText.text = count.ToString();
    }

    public void UpdateGameTimeText(int newTime)
    {
        _gameTimeText.text = Util.GetTimeFormat(newTime);
    }

}
