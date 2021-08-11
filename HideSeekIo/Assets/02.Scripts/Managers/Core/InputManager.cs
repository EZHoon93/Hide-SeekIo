using System;
using System.Collections;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

public class InputManager : GenricSingleton<InputManager>
{
    //[SerializeField] UltimateJoystick _moveJoystick;
    //public Vector2 MoveVector { get; private set; }

    [SerializeField] UI_ControllerJoystick[] _controllerJoysticks;


    //public UltimateJoystick moveJoystick => _moveJoystick;



    public UI_ControllerJoystick GetControllerJoystick(InputType inputType)
    {
        foreach(var joystick in _controllerJoysticks)
        {
            if(joystick.inputType == inputType)
            {
                return joystick;
            }
        }

        return null;
       //return _controllerJoysticks.Single(s => s.inputType == inputType);
    }

    public void Clear()
    {
        SetActiveController(false);
    }
    protected override  void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this.gameObject);
        Clear();
    }




    private void Start()
    {
        InitSetup();
    }
    public void InitSetup()
    {
        PlayerInfo.LoadOptionData();
        GetComponent<UI_InputSetting>().Init();
    }

    private void Update()
    {

#if UNITY_ANDROID
        //MoveVector = new Vector2(_moveJoystick.GetHorizontalAxis(), _moveJoystick.GetVerticalAxis());
#endif

#if UNITY_EDITOR
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        //MoveVector = new Vector2(h, v);
#endif
    }
    public void SetActiveController(bool active)
    {
        foreach (var joystick in _controllerJoysticks)
            joystick.gameObject.SetActive(active);

        GetControllerJoystick(InputType.Item1).gameObject.SetActive(false); //아이템조이스틱은 Off로시작
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
