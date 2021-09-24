
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _playerNameText;
    [SerializeField] TextMeshProUGUI _levelText;
    [SerializeField] Slider _energySlider;
    [SerializeField] GameObject _hearRange;
    [SerializeField] Image _warningImage;

    PlayerController _playerController;
    PlayerStat _playerStat;

    
    public void SetupPlayer(PlayerController playerController)
    {
        _playerController = playerController;
        _energySlider.gameObject.SetActive(false);
    }
    public void OnPhotonInstantiate()
    {
        _playerStat = _playerController.playerStat;
        _playerNameText.text = _playerController.NickName;
        _levelText.text = _playerStat.level.ToString();
        _warningImage.enabled = false;
    }

    public void ChangeStatEventCallBack(PlayerStat.StatChange statChange, object value )
    {

    }

    public void ChangeOwnerShip()
    {
        bool active = _playerController.IsMyCharacter() ? true : false;
        _energySlider.gameObject.SetActive(active);
    }

    public void ChangeWarining(bool active)
    {
        print(active + "!!Warinignimaga");
        _warningImage.enabled = active;
        Invoke("RestWarining", 3.0f);
    }
    void RestWarining()
    {
        _warningImage.enabled = false;

    }
    private void Update()
    {
        
        if (_playerController.playerMove != null)
        {
            _energySlider.value = _playerStat.CurrentEnergy;
            if (_energySlider.maxValue != _playerStat.MaxEnergy)
            {
                _energySlider.maxValue = _playerStat.MaxEnergy;
            }

        }
    }
}
