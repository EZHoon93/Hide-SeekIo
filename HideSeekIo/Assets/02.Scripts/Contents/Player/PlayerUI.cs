
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using System.Runtime.InteropServices;

public class PlayerUI : MonoBehaviourPun
{
    [SerializeField] TextMeshProUGUI _playerNameText;
    [SerializeField] TextMeshProUGUI _hpText;

    //[SerializeField] GameObject _hearRange;
    //[SerializeField] Image _warningImage;

    [SerializeField] Slider _hpSlider;
    [SerializeField] Slider _backHpSlider;
    [SerializeField] Slider _energySlider;
    [SerializeField] UI_ShootEnergy[] _uI_ShootEnergyArray;
    [SerializeField] Transform _moveUI;
    [SerializeField] SpriteRenderer _groundUI;
    [SerializeField] Sprite _sprite;

    PlayerController _playerController;
    PlayerInput _playerInput;

    bool _isBackHpHit;
    Define.Team _team;
    

    public void SetupPlayer(PlayerController playerController)
    {
        _playerController = playerController;
        //_playerController.playerShooter.onChangeMaxEnergyListeners += ChangeMaxEnergy;   //최대 에너지값 변경시 UI 변경
        //_playerController.playerShooter.onChangeCurrEnergyListeners += ChangeCurrentEnergy;   //현재 에너지값 변경시 UI 변경
        _playerController.playerHealth.onChangeCurrHpEvent += ChangeCurrentHP;
        _playerController.playerHealth.onChangeMaxHpEvent+= ChangeMaxHP;
        //_playerController.playerMove.onChangeMoveEnergy += ChangeCurrentMoveEnergy;
        //_playerController.playerMove.onChangeMoveMaxEnergy += ChangeMaxMoveEnergy;

        Managers.cameraManager.cameraViewChangeEvent += ChangeCameraTarget;
    }

    private void Awake()
    {
        SetActiveUI(false);
        foreach (var se in _uI_ShootEnergyArray)
            se.gameObject.SetActive(false);
    }
    public void OnPhotonInstantiate()
    {
        _playerInput = _playerController.playerInput;
        _playerNameText.text = _playerController.NickName;
        _isBackHpHit = false;
      
    }

    public void OnPreNetDestroy(PhotonView rootView)
    {
        Managers.cameraManager.cameraViewChangeEvent -= ChangeCameraTarget;
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
                if(cameraViewPlayer.Team == Define.Team.Hide)
                {
                    ChangeColor(Color.red);
                }
                //술래라면.. 숨는오브젝트들 UI가 안보이게
                else
                {
                    _playerNameText.gameObject.SetActive(false);
                }
            }
        }
    }


    public void ChangeTeam(Define.Team team)
    {
        var viewPlayer = Managers.cameraManager.cameraTagerPlayer;
        if (viewPlayer == null) return;
        if(_playerController.IsMyCharacter() == false)
        {
            if(viewPlayer.Team == team)
            {
                ChangeColor(Color.yellow);
            }
            else
            {
                ChangeColor(Color.red);
            }
        }
      
    }


    /// <summary>
    /// 자기자신 캐릭터는 초록색으로 바꿈
    /// </summary>
    public void ChangeOwnerShip()
    {
        bool active = _playerController.IsMyCharacter() ? true : false;
        if (active)
        {
            ChangeColor(Color.green);
        }
    }

    void SetActiveUI(bool active)
    {
        
        _energySlider.gameObject.SetActive(active);
        _moveUI.gameObject.SetActive(active);
        _energySlider.gameObject.SetActive(active);
        _hpSlider.gameObject.SetActive(active);
        _backHpSlider.gameObject.SetActive(active);
        _hpText.gameObject.SetActive(active);
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

    #region ShootEnergy
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
    #endregion

    #region Hp
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
    #endregion

    #region MoveEnergy
    public void ChangeCurrentMoveEnergy(float newValue)
    {
        _energySlider.value = newValue;
    }

    public void ChangeMaxMoveEnergy(float newValue)
    {
        _energySlider.maxValue = newValue;
    }

    public void SetActiveMoveEnergyUI(bool active) => _energySlider.gameObject.SetActive(active);
    


    #endregion
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

        _moveUI.localPosition = _playerInput.GetVector2(InputType.Move)*3;
    }
}
