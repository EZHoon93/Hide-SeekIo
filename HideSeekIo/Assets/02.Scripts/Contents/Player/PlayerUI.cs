
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
    }

    public void ChangeStatEventCallBack(PlayerStat.StatChange statChange, object value )
    {

    }

    public void ChangeOwnerShip()
    {
        bool active = _playerController.IsMyCharacter() ? true : false;
        _energySlider.gameObject.SetActive(active);
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
