
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
    //Character_Base _character_Base;

    private void Awake()
    {
        this.transform.parent.GetComponentInParent<IOnPhotonInstantiate>().onPhotonInstantiateEvent += OnPhotonInstantiate;
    }
    void OnPhotonInstantiate(PhotonView photonView)
    {
        _playerController = this.transform.parent.GetComponentInParent<PlayerController>();
        //_character_Base = _playerController.playerCharacter.character_Base;
        _playerNameText.text = _playerController.NickName;

        bool active = _playerController.IsMyCharacter() ? true : false;
        _energySlider.enabled = active;

        //switch (_playerController.Team)
        //{
        //    case Define.Team.Hide:
        //        _hiderMove = _playerController.GetComponent<HiderMove>();
        //        _energySlider.maxValue = _hiderMove.MaxEnergy;
        //        _energySlider.value = _energySlider.maxValue;

        //        _energySlider.gameObject.SetActive(_hiderMove.IsMyCharacter());
        //        _hearRange.SetActive(false);
        //        break;
        //    case Define.Team.Seek:
        //        _energySlider.gameObject.SetActive(false);
        //        _hearRange.SetActive(true);

        //        break;
        //}
        
    }

    private void Update()
    {
        //if (_playerController.playerMove != null)
        //{
        //    _energySlider.value = _character_Base.CurrentEnergy;
        //    if(_energySlider.maxValue != _character_Base.MaxEnergy)
        //    {
        //        _energySlider.maxValue = _character_Base.MaxEnergy;
        //    }

        //}
    }
}
