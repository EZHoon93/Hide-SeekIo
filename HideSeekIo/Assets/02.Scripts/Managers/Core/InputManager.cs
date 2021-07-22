using System;
using System.Collections;

using UnityEngine;

public class InputManager : MonoBehaviour
{


    public static InputManager Instacne
    {
        get
        {
            if (_instance == null)
            {
                // 씬에서 GameManager 오브젝트를 찾아 할당
                _instance = FindObjectOfType<InputManager>();
            }

            // 싱글톤 오브젝트를 반환
            return _instance;
        }
    }
    private static InputManager _instance; // 싱글톤이 할당될 static 변수

    [SerializeField] UI_Input_Run _runButton;
    [SerializeField] UltimateJoystick _moveJoystick;
    [SerializeField] UltimateJoystick _attackJoystick;
    [SerializeField] UltimateJoystick _skillJoystick;
    [SerializeField] UI_ItemButton[] uI_ItemButtons;




    public Vector2 MoveVector { get; private set; }
    public Vector2 AttackVector { get; private set; }
    public Vector2 SkillVector { get; set; }
    public bool AttackTouch { get; private set; }
    public bool SkillTouch { get; private set; }
    public bool IsRun
    {
        get => _runButton.IsRun;
        set
        {
            _runButton.IsRun = value;
        }
    }
    
    [SerializeField] UI_Slider_CoolTime _attackCoolTimeUI;


    public void Clear()
    {
        _moveJoystick.gameObject.SetActive(false);
        _attackJoystick.gameObject.SetActive(false);
        _skillJoystick.gameObject.SetActive(false);
        _runButton.gameObject.SetActive(false);
        foreach (var itemButton in uI_ItemButtons)
        {
            itemButton.gameObject.SetActive(false);
        }
    }
    private void Awake()
    {
        // 씬에 싱글톤 오브젝트가 된 다른 GameManager 오브젝트가 있다면
        if (Instacne != this)
        {
            // 자신을 파괴
            Destroy(gameObject);
        }
        DontDestroyOnLoad(this.gameObject);

        _moveJoystick.gameObject.SetActive(false);
        _attackJoystick.gameObject.SetActive(false);
        _skillJoystick.gameObject.SetActive(false);
        _runButton.gameObject.SetActive(false);

    }
    private void Start()
    {
        InitSetup();

        _moveJoystick.OnDragCallback += UpdateMoveJoystick;
        _attackJoystick.OnDragCallback += UpdateAttackoystick;
        _skillJoystick.OnDragCallback += UpdateSkillJoystick;

        _moveJoystick.OnPointerUpCallback += () =>
        {
            MoveVector = Vector2.zero;
        };
        _attackJoystick.OnPointerUpCallback += () => {
            AttackVector = Vector2.zero;
            AttackTouch = false;
        };

        _skillJoystick.OnPointerUpCallback += () =>
        {
            SkillVector = Vector2.zero;
            SkillTouch = false;
        };


        _attackJoystick.OnPointerDownCallback += () => AttackTouch = true;
        _skillJoystick.OnPointerDownCallback += () => SkillTouch = true;
    }
    public void InitSetup()
    {
        _moveJoystick.gameObject.SetActive(false);
        _attackJoystick.gameObject.SetActive(false);
        _skillJoystick.gameObject.SetActive(false);
        //uI_Button_Run.gameObject.SetActive(false);
    }

    void UpdateMoveJoystick()
    {
#if UNITY_ANDROID
        MoveVector = new Vector2(UltimateJoystick.GetHorizontalAxis("Move"), UltimateJoystick.GetVerticalAxis("Move"));
#endif
#if UNITY_EDITOR

#endif

    }

    void UpdateAttackoystick()
    {
#if UNITY_ANDROID
        AttackVector = new Vector2(UltimateJoystick.GetHorizontalAxis("Attack"), UltimateJoystick.GetVerticalAxis("Attack"));
        AttackTouch = UltimateJoystick.GetJoystickState("Attack");

#endif
    }

    void UpdateSkillJoystick()
    {
        SkillVector = new Vector2(UltimateJoystick.GetHorizontalAxis("Skill"), UltimateJoystick.GetVerticalAxis("Skill"));
    }
    private void Update()
    {
#if UNITY_EDITOR
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        MoveVector = new Vector2(h, v);
        //MoveVector = new Vector2(UltimateJoystick.GetHorizontalAxis("Move"), UltimateJoystick.GetVerticalAxis("Move"));

#endif

        //UpdateAttackoystick();
    }

    public void SetActiveSeekerController(bool isAcitve)
    {
        _moveJoystick.gameObject.SetActive(isAcitve);
        _attackJoystick.gameObject.SetActive(isAcitve);
        _skillJoystick.gameObject.SetActive(isAcitve);
        _runButton.gameObject.SetActive(false);
        foreach(var itemButton in uI_ItemButtons)
        {
            itemButton.gameObject.SetActive(false);
        }
    }
    public void SetActiveHiderController(bool isAcitve)
    {
        _moveJoystick.gameObject.SetActive(isAcitve);
        _attackJoystick.gameObject.SetActive(isAcitve);
        _skillJoystick.gameObject.SetActive(false);
        _runButton.gameObject.SetActive(isAcitve);
        foreach (var itemButton in uI_ItemButtons)
        {
            itemButton.gameObject.SetActive(false);
        }
    }
    public void OffAllController()
    {
        _moveJoystick.gameObject.SetActive(false);
        _attackJoystick.gameObject.SetActive(false);
        _skillJoystick.gameObject.SetActive(false);
        _runButton.gameObject.SetActive(false);
        foreach (var itemButton in uI_ItemButtons)
        {
            itemButton.gameObject.SetActive(false);
        }
    }

    public void AttackCoolTime(float maxCoolTime, float currentCoolTime)
    {
        _attackCoolTimeUI.UpdateCoolTime(maxCoolTime, currentCoolTime);
    }

    public void AddItemByButton(int index , InGameItemController newItem)
    {
        uI_ItemButtons[index].gameObject.SetActive(true);
        uI_ItemButtons[index].AddItem(newItem);
    }
    public void RemoveItemButton(int index)
    {
        uI_ItemButtons[index].gameObject.SetActive(false);
        uI_ItemButtons[index].RemoveItem();
    }

    
}
