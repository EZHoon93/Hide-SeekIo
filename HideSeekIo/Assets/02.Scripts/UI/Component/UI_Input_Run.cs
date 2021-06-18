using UnityEngine;
using UnityEngine.EventSystems;

public class UI_Input_Run : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    bool _isRun;

    public bool IsRun => _isRun;

    public void OnPointerUp(PointerEventData eventData)
    {
        _isRun = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _isRun = true;
    }
}
