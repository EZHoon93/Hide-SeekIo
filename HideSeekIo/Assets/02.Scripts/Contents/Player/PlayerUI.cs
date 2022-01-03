
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using System.Runtime.InteropServices;

public class PlayerUI : MonoBehaviourPun
{
    [SerializeField] TextMeshProUGUI _playerNameText;


    [SerializeField] Transform _moveUI;
    [SerializeField] SpriteRenderer _groundUI;
    //[SerializeField] Sprite _sprite;

    [SerializeField] Slider[] _hpSliderArray;   //

    PlayerController _playerController;

    public void SetupPlayer(PlayerController playerController)
    {
        _playerController = playerController;
        _playerController.playerHealth.onChangeCurrHpEvent += ChangeCurrentHP;
        _playerController.playerHealth.onChangeMaxHpEvent += ChangeMaxHP;
        _playerController.playerHealth.onChangeTeamEvent += OnCallBack_ChangeTeam;

        Managers.cameraManager.cameraViewChangeEvent += OnCallBack_ChangeCameraTarget;

       
    }

 
    public void OnPhotonInstantiate()
    {
        //_playerInput = _playerController.playerInput;
        _playerNameText.text = _playerController.NickName;
        ChangeUI(_playerController.Team);

    }

    public void OnPreNetDestroy(PhotonView rootView)
    {
        Managers.cameraManager.cameraViewChangeEvent -= OnCallBack_ChangeCameraTarget;
    }

    void ChangeUI(Define.Team team)
    {
        var uiLayerIndex =  UtillLayer.GetUILayerByTeam(team);
        Util.SetLayerRecursively(this.gameObject, uiLayerIndex);
    }
   


    void ChangeColor(Color color)
    {
        _playerNameText.color = color;
        color.a = 0.33f;
        _groundUI.color = color;
    }


    public void SetActiveNameUI(bool active) => _playerNameText.gameObject.SetActive(active);

 

    #region Hp

    public void SetActiveHealthUI(bool active)
    {
        foreach (var slider in _hpSliderArray)
            slider.gameObject.SetActive(active);
    }

    void ChangeCurrentHP(int newValue)
    {
        var sliderMaxValue = _hpSliderArray[0].maxValue;
        for (int i = 0; i < _hpSliderArray.Length; i++)
        {
            float value = Mathf.Clamp((newValue - i * sliderMaxValue), 0, sliderMaxValue);
            _hpSliderArray[i].value = value;
        }
    }

    void ChangeMaxHP(int newValue)
    {
    
    }

    #endregion

    

    #region CallBack Event
    //public void ChangeStatEventCallBack(PlayerStat.StatChange statChange, object value)
    //{

    //}

    public void OnCallBack_ChangeCameraTarget(PlayerController cameraViewPlayer)
    {
        print("OnCallBack_ChangeCameraTarget");
        //현재 보고 있는캐릭터랑 같은 오브젝트라면
        if (cameraViewPlayer == _playerController)
        {
            ChangeColor(Color.green);

        }
        //보고있는 캐릭터가 아닌 다른 오브젝트들이라면
        else
        {
            var targetViewteam = cameraViewPlayer.Team;
            //같은 팀이라면
            if (targetViewteam == _playerController.Team)
            {
                ChangeColor(Color.yellow);

            }
            //다른 팀이라면
            else
            {
                //숨는팀
                if (cameraViewPlayer.Team == Define.Team.Hide)
                {
                    ChangeColor(Color.red);
                }
                //술래팀  => 숨는오브젝트들 UI가 안보이게
                else
                {
                    //SetActiveUI(false);
                }
            }
        }
    }




    public void OnCallBack_ChangeTeam(Define.Team team)
    {
        ChangeUI(team);

    }


    #endregion
}
