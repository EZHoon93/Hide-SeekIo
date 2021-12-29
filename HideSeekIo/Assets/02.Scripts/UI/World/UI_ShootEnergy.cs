using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
public class UI_ShootEnergy : MonoBehaviour
{
    Slider _slider;
    [SerializeField] Image _fillImage;

    bool _isFull;
    bool isFull
    {
        get => _isFull;
        set
        {
            if (_isFull == value) return;
            _isFull = value;
            _fillImage.enabled = value;
        }
    }

    private void Awake()
    {
        _slider = GetComponent<Slider>();
    }

    private void OnEnable()
    {
        _isFull = false;
        _fillImage.enabled = false;
    }
    public void SetupMaxValue(float maxValue) => _slider.maxValue = maxValue;
    public void UpdateUI(float value)
    {
        isFull = value == _slider.maxValue ? true : false;
        if (!isFull)
            _slider.value = value;
    }
}