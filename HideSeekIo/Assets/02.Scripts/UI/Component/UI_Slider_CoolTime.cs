using System.Collections;

using UnityEngine;
using UnityEngine.UI;

public class UI_Slider_CoolTime : MonoBehaviour
{
    Slider _slider;
    private void Awake()
    {
        _slider = _slider ?? GetComponent<Slider>();
        
    }

    public void UpdateCoolTime(float initValue , float currentValue)
    {
        if (_slider.maxValue != initValue)
            _slider.maxValue = initValue;

        //print  (  initValue +  "쿨타임" + currentValue);
        _slider.value = initValue -currentValue;
    }
}
