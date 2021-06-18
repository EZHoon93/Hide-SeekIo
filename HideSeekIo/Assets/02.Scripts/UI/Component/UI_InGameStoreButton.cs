using System.Collections;

using UnityEngine;
using UnityEngine.EventSystems;
public class UI_InGameStoreButton : MonoBehaviour, IPointerClickHandler
{

    [SerializeField] private float offset;
    [SerializeField] RectTransform UITransform;

    bool _isOpen = true;
    float _timer;

    public void Start()
    {
        _isOpen = true;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(OpenAndHideUI());
    }

    IEnumerator OpenAndHideUI()
    {
        _isOpen = !_isOpen;
        if (_timer != 0f)
        {
            _timer = 1f - _timer;
        }
        while (_timer <= 1f)
        {
            _timer += Time.deltaTime * 2f;

            float start = _isOpen ? -UITransform.sizeDelta.y : offset;
            float dest = _isOpen ? offset : -UITransform.sizeDelta.y;
            UITransform.anchoredPosition = new Vector2(UITransform.anchoredPosition.x, Mathf.Lerp(start, dest, _timer));
            yield return null;
        }
    }
}
