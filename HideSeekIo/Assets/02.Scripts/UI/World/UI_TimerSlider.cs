using System.Collections;

using UnityEngine;
using UnityEngine.UI;

public class UI_TimerSlider : MonoBehaviour
{
    Slider _slider;
    TimerItemController _timerItemController;

    private void Awake()
    {
        _slider = GetComponentInChildren<Slider>();
        _timerItemController = this.transform.parent.GetComponentInParent<TimerItemController>();
    }


    private void Update()
    {
        if (_slider.maxValue != _timerItemController.DurationTime)
        {
            _slider.maxValue = _timerItemController.DurationTime;
            _slider.value = _slider.maxValue;
        }

        _slider.value = Mathf.Lerp(_slider.value, _timerItemController.RemainTime, Time.deltaTime * 3);
    }
}
