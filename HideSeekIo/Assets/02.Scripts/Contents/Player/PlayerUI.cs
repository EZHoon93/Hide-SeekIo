
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _playerNameText;
    [SerializeField] Slider _energySlider;
    [SerializeField] GameObject _hearRange;

    PlayerController _playerController;
    PlayerStat _playerStat;

    
    public void SetupPlayer(PlayerController playerController)
    {
        _playerController = playerController;
    }
    public void OnPhotonInstantiate()
    {
        _playerStat = _playerController.playerStat;
        _playerNameText.text = _playerController.NickName;
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
