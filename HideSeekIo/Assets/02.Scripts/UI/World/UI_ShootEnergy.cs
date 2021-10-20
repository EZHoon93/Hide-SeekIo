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
    public void UpdateUI(float value)
    {
        isFull = value == 1 ? true : false;
        if (!isFull)
            _slider.value = value;
    }
}