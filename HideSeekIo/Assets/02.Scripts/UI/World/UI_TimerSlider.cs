using System.Collections;

using UnityEngine;
using UnityEngine.UI;

public class UI_TimerSlider : MonoBehaviour
{
    Slider _slider;
    PunTimerObject _punTimerObject;

    private void Awake()
    {
        _slider = GetComponent<Slider>();
        _punTimerObject = this.transform.parent.GetComponentInParent<PunTimerObject>();
    }


    private void Update()
    {
        if (_slider.maxValue != _punTimerObject.InitRemainTime)
        {
            _slider.maxValue = _punTimerObject.InitRemainTime;
            _slider.value = _slider.maxValue;
        }

        _slider.value = Mathf.Lerp(_slider.value, _punTimerObject.RemainTime, Time.deltaTime * 3);
    }
}
