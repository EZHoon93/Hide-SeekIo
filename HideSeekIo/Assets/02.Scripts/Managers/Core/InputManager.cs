using System;
using System.Collections;

using UnityEngine;
using UnityEngine.UI;

public class InputManager : GenricSingleton<InputManager>
{
    [SerializeField] UI_Input_Run _runButton;
    [SerializeField] UI_ControllerJoystick _moveJoystick;
    [SerializeField] UI_ControllerJoystick _mainJoystick;
    [SerializeField] UI_ControllerJoystick _subJoystick;
    [SerializeField] UI_ControllerJoystick[] _itemControllerJoysticks;
    public Vector2 MoveVector { get; private set; }
    //public Vector2 AttackVector => _mainJoystick.InputVector2;

    public UI_ControllerJoystick moveJoystick => _moveJoystick;
    public UI_ControllerJoystick subJoystick => _subJoystick;
    public UI_ControllerJoystick mainJoystick => _mainJoystick;
    public UI_ControllerJoystick[] itemControllerJoysticks => _itemControllerJoysticks;

    public bool IsRun
    {
        get => _runButton.IsRun;
        set
        {
            _runButton.IsRun = value;
        }
    }

    public bool TestRun;

    [SerializeField] UI_Slider_CoolTime _attackCoolTimeUI;


    public void Clear()
    {
        _moveJoystick.gameObject.SetActive(false);
        _mainJoystick.gameObject.SetActive(false);
        _runButton.gameObject.SetActive(false);
        foreach (var itemButton in _itemControllerJoysticks)
        {
            itemButton.gameObject.SetActive(false);
        }
    }
    protected override  void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this.gameObject);
        _moveJoystick.gameObject.SetActive(false);
        _mainJoystick.gameObject.SetActive(false);
        _runButton.gameObject.SetActive(false);
        
    }




    private void Start()
    {
        InitSetup();
    }
    public void InitSetup()
    {
        _moveJoystick.gameObject.SetActive(false);
        _mainJoystick.gameObject.SetActive(false);
        _runButton.gameObject.SetActive(false);
        foreach (var itemButton in _itemControllerJoysticks)
        {
            itemButton.gameObject.SetActive(false);
        }
        PlayerInfo.LoadOptionData();
        GetComponent<UI_InputSetting>().Init();


    }

    private void Update()
    {
#if UNITY_EDITOR
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        MoveVector = new Vector2(h, v);
#endif
    }

    public void SetActiveSeekerController(bool isAcitve)
    {
        _moveJoystick.gameObject.SetActive(isAcitve);
        _mainJoystick.gameObject.SetActive(isAcitve);
        _runButton.gameObject.SetActive(false);
        foreach (var itemButton in _itemControllerJoysticks)
        {
            itemButton.gameObject.SetActive(false);
        }
    }
    public void SetActiveHiderController(bool isAcitve)
    {
        _moveJoystick.gameObject.SetActive(isAcitve);
        _mainJoystick.gameObject.SetActive(isAcitve);
        _runButton.gameObject.SetActive(isAcitve);
        foreach (var itemButton in _itemControllerJoysticks)
        {
            itemButton.gameObject.SetActive(false);
        }
    }
    public void OffAllController()
    {
        _moveJoystick.gameObject.SetActive(false);
        _mainJoystick.gameObject.SetActive(false);
        _runButton.gameObject.SetActive(false);
        foreach (var itemButton in _itemControllerJoysticks)
        {
            itemButton.gameObject.SetActive(false);
        }
    }

    public void AttackCoolTime(float maxCoolTime, float currentCoolTime)
    {
        _attackCoolTimeUI.UpdateCoolTime(maxCoolTime, currentCoolTime);
    }

    //public void AddItemByButton(int index, InGameItemController newItem)
    //{
    //    _itemControllerJoysticks[index].AddItem(newItem);
    //    //uI_ItemButtons[index].gameObject.SetActive(true);
    //    //uI_ItemButtons[index].AddItem(newItem);
    //}
    public void RemoveItemButton(int index)
    {
        //uI_ItemButtons[index].gameObject.SetActive(false);
        //uI_ItemButtons[index].RemoveItem();
    }


}
