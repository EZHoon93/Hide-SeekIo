using System;
using System.Collections;

using UnityEngine;
using UnityEngine.UI;

public class HiderCage : MonoBehaviour, IEnterTrigger , IExitTrigger
{
    [SerializeField] Slider _slider;
    [SerializeField] float _maxTime;
    [SerializeField] float _currentTime;
    [SerializeField] HiderHealth _parentHiderHealth;

    bool _isRevive;
    HiderController _revivePlayer;   //구해주고있는플레이어

    public event Action reviveEvent;

    private void OnEnable()
    {
        _slider.maxValue = _maxTime;
        _slider.value = 0;
        _currentTime = 0;
        _revivePlayer = null;
        _isRevive = false;
    }
    public void Enter(GameObject Gettingobject)
    {
        if (_isRevive) return;
        var hiderController = Gettingobject.GetComponent<HiderController>();
        if (hiderController == null) return;
        _revivePlayer = hiderController;
        _isRevive = true;

        print("Revieve Enter");
    }

    public void Exit(GameObject exitGameObject)
    {
        if (_isRevive == false) return;
        var hiderController = exitGameObject.GetComponent<HiderController>();
        if (hiderController == null) return;
        if (_revivePlayer == null) return;

        if(hiderController == _revivePlayer)
        {
            _isRevive = false;
            print("Revieve Exit");

        }
    }

    private void Update()
    {
        if(_isRevive && _revivePlayer != null)
        {
            if(_revivePlayer.hiderHealth.Dead == false)
            {
                _currentTime = Mathf.Clamp( _currentTime+Time.deltaTime, 0 , _maxTime);
                if(_currentTime >= _maxTime)
                {
                    reviveEvent?.Invoke();
                    this.gameObject.SetActive(false);
                }
            }
        }

        else
        {
            _currentTime = Mathf.Clamp(_currentTime - 3*Time.deltaTime, 0, _maxTime);
        }

        UpdateSlider();
    }

    void UpdateSlider()
    {
        _slider.value = _currentTime;
    }
}
