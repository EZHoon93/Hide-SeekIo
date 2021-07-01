using System.Collections;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class UI_Hider : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _playerNameText;
    [SerializeField] Slider _energySlider;

    HiderController _hiderController;
    private void OnEnable()
    {
        SetUp();
    }
    void SetUp()
    {
        _hiderController = this.transform.parent.GetComponentInParent<HiderController>();
        _playerNameText.text = _hiderController.NickName;
        //_energySlider.maxValue = _hiderController.hiderMove.MaxEnergy;
        //_energySlider.value = _energySlider.maxValue;
    }

    private void Update()
    {
        //_energySlider.value = _hiderController.hiderMove.CurrentEnergy;
    }
}
