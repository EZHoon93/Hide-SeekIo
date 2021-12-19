using UnityEngine;
using UnityEngine.EventSystems;

public class UI_Input_Run : MonoBehaviour
{
    [SerializeField] GameObject _runEffectImage;
    bool _isRun;


    public bool IsRun
    {
        get => _isRun;

        set
        {
            _isRun = value;
            _runEffectImage.gameObject.SetActive(_isRun);
        }

    }

    private void Awake()
    {
        GetComponent<UltimateButton>().OnGetButtonDown += OnPointerDown;
        print("@@@@@@@@@@@@@셋업");
    }

    private void OnEnable()
    {
        IsRun = true;
    }

    

    public void OnPointerDown()
    {
        IsRun = !_isRun;
        print("클릭 런 !!" + IsRun);
    }
}
