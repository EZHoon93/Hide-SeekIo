using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_CharacterCamera : MonoBehaviour, IDragHandler
{
    [SerializeField] Vector3 _rotateAmount;
    [SerializeField] Transform _rotatePanel;
    Vector2 _prevPos = Vector3.zero;
    public void OnDrag(PointerEventData eventData)
    {
        var gapPos = eventData.position - _prevPos;
        _prevPos = eventData.position;
        if(gapPos.x > 0)
        {
            _rotatePanel.transform.Rotate(_rotateAmount);
        }
        else
        {
            _rotatePanel.transform.Rotate(-_rotateAmount);

        }


    }
}
