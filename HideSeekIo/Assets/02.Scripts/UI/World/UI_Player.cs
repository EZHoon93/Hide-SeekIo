
using Photon.Pun;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class UI_Player : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _playerNameText;
    [SerializeField] Slider _energySlider;
    [SerializeField] GameObject _hearRange;

    PlayerController _playerController;
    HiderMove _hiderMove;

    private void Awake()
    {
        this.transform.parent.GetComponentInParent<IOnPhotonInstantiate>().OnPhotonInstantiateEvent += OnPhotonInstantiate;
    }
    void OnPhotonInstantiate(PhotonView photonView)
    {
        _playerController = this.transform.parent.GetComponentInParent<PlayerController>();
        _playerNameText.text = _playerController.NickName;
        switch (_playerController.Team)
        {
            case Define.Team.Hide:
                _hiderMove = _playerController.GetComponent<HiderMove>();
                _energySlider.maxValue = _hiderMove.MaxEnergy;
                _energySlider.value = _energySlider.maxValue;
                _energySlider.gameObject.SetActive(true);
                _hearRange.SetActive(false);
                break;
            case Define.Team.Seek:
                _energySlider.gameObject.SetActive(false);
                _hearRange.SetActive(true);

                break;
        }
        
    }

    private void Update()
    {
        if(_hiderMove != null)
        {
            _energySlider.value = _hiderMove.CurrentEnergy;

        }
    }
}
