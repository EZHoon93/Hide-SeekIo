using UnityEngine;
using UnityEngine.EventSystems;

public class UI_Input_Run : MonoBehaviour, IPointerDownHandler
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


    private void OnEnable()
    {
        IsRun = true;
    }

    

    public void OnPointerDown(PointerEventData eventData)
    {
        IsRun = !_isRun;
    }
}
