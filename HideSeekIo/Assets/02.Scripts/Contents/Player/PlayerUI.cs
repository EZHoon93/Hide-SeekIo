
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviourPun
{
    [SerializeField] TextMeshProUGUI _playerNameText;
    [SerializeField] TextMeshProUGUI _hpText;

    //[SerializeField] GameObject _hearRange;
    //[SerializeField] Image _warningImage;

    [SerializeField] Slider _hpSlider;
    [SerializeField] Slider _backHpSlider;
    [SerializeField] UI_ShootEnergy[] _uI_ShootEnergyArray;
    [SerializeField] Transform _moveUI;
    [SerializeField] SpriteRenderer _groundUI;
    [SerializeField] Sprite _sprite;

    PlayerController _playerController;
    PlayerInput _playerInput;

    bool _isBackHpHit;
    Define.Team _team;
    /// <summary>
    /// PlayerController Awake에서 발생
    /// </summary>
    public void SetupPlayer(PlayerController playerController)
    {
        _playerController = playerController;
        _playerController.playerShooter.onChangeMaxEnergyListeners += ChangeMaxEnergy;   //최대 에너지값 변경시 UI 변경
        _playerController.playerShooter.onChangeCurrEnergyListeners += ChangeCurrentEnergy;   //현재 에너지값 변경시 UI 변경
        _playerController.playerHealth.onChangeCurrHpEvent += ChangeCurrentHP;
        _playerController.playerHealth.onChangeMaxHpEvent+= ChangeMaxHP;

        CameraManager.Instance.cameraViewChangeEvent += ChangeCameraTarget;
    }
    public void OnPhotonInstantiate()
    {
        _playerInput = _playerController.playerInput;
        _playerNameText.text = _playerController.NickName;
        _isBackHpHit = false;
        foreach (var se in _uI_ShootEnergyArray)
            se.gameObject.SetActive(false);
        _moveUI.gameObject.SetActive(false);
    }

    public void ChangeStatEventCallBack(PlayerStat.StatChange statChange, object value )
    {

    }
    public void ChangeCameraTarget(PlayerController cameraViewPlayer)
    {
        //현재 보고 있는캐릭터랑 같은 오브젝트라면
        if(cameraViewPlayer == _playerController)
        {
            ChangeColor(Color.green);

        }
        //보고있는 캐릭터가 아닌 다른 오브젝트들이라면
        else
        {
            var targetViewteam = cameraViewPlayer.Team;
            //같은 팀이라면
            if(targetViewteam == _playerController.Team)
            {
                ChangeColor(Color.yellow);

            }
            //다른 팀이라면
            else
            {
                ChangeColor(Color.red);
            }
        }
    }


    public void ChangeTeam(Define.Team team)
    {
        if(team == Define.Team.Seek)
        {

        }
    }


    /// <summary>
    /// 자기자신 캐릭터는 초록색으로 바꿈
    /// </summary>
    public void ChangeOwnerShip()
    {
        bool active = _playerController.IsMyCharacter() ? true : false;

        if (active == false) return;
        _moveUI.gameObject.SetActive(active);
        ChangeColor(Color.green);
    }

    void ChangeColor(Color color)
    {
        _playerNameText.color = color;
        color.a = 0.33f;
        _groundUI.color = color;
    }

    public void ChangeWarining(bool active)
    {
        //_warningImage.enabled = active;
        Invoke("RestWarining", 3.0f);
    }
    void RestWarining()
    {
        //_warningImage.enabled = false;
    }

    void ChangeMaxEnergy(int newValue)
    {
        if (_playerController.IsMyCharacter() == false) return;
        for(int i = 0; i < newValue; i++)
        {
            _uI_ShootEnergyArray[i].gameObject.SetActive(true);
        }
        for(int j = newValue; j < 3; j++)
        {
            _uI_ShootEnergyArray[j].gameObject.SetActive(false);
        }
    }

    void ChangeCurrentEnergy(float newValue)
    {
        for(int i = 0; i < 3; i++)
        {
            float value = Mathf.Clamp( (newValue - i) , 0,1);
            _uI_ShootEnergyArray[i].UpdateUI(value);
        }
    }

    void ChangeCurrentHP(int newValue)
    {
        
        //대미지가 감소한 형태라면
        if (newValue < _hpSlider.value)
        {
            _isBackHpHit = true;
        }
        else
        {
            _backHpSlider.value = newValue;
        }
        _hpSlider.value = newValue;
        _hpText.text = newValue.ToString();
    }

    void ChangeMaxHP(int newValue)
    {
        _hpSlider.maxValue = newValue;
        _backHpSlider.maxValue = newValue;
    }


    void UpdateBackHealthSlider()
    {

        if (!_isBackHpHit) return;
        _backHpSlider.value = Mathf.Lerp(_backHpSlider.value, _hpSlider.value, Time.deltaTime * 2f);
        if (_hpSlider.value >= _backHpSlider.value - 2f)
        {
            _isBackHpHit = false;
            _backHpSlider.value = _hpSlider.value;
        }
        

    }

    private void Update()
    {
        
        if (_playerController.playerMove != null)
        {
            //_energySlider.value = _playerController.playerShooter.currentEnergy;
            //if (_energySlider.maxValue != _playerStat.MaxEnergy)
            //{
            //    _energySlider.maxValue = _playerStat.MaxEnergy;
            //}
        }

        UpdateBackHealthSlider();

        _moveUI.localPosition = _playerInput.controllerInputDic[InputType.Move].inputVector2*3;
    }
}
