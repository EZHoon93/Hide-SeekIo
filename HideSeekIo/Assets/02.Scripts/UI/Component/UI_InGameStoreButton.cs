using System.Collections;

using UnityEngine;
using UnityEngine.EventSystems;
public class UI_InGameStoreButton : MonoBehaviour, IPointerClickHandler
{

    [SerializeField]
    private float offset;

    [SerializeField]
    private RectTransform TaskListUITransform;

    private bool isOpen = true;

    private float timer;

    private void OnEnable()
    {
        //사이즈만큼 감소
        TaskListUITransform.anchoredPosition = new Vector2(-TaskListUITransform.sizeDelta.x, TaskListUITransform.anchoredPosition.y);
        isOpen = false;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(OpenAndHideUI());
    }

    private IEnumerator OpenAndHideUI()
    {
        isOpen = !isOpen;
        if (timer != 0f)
        {
            timer = 1f - timer;
        }

        while (timer <= 1f)
        {
            timer += Time.deltaTime * 2f;

            float start = isOpen ? -TaskListUITransform.sizeDelta.x : offset;
            float dest = isOpen ? offset : -TaskListUITransform.sizeDelta.x;
            TaskListUITransform.anchoredPosition = new Vector2(Mathf.Lerp(start, dest, timer), TaskListUITransform.anchoredPosition.y);
            yield return null;
        }
    }
    //[SerializeField] private float offset;
    //[SerializeField] RectTransform UITransform;

    //bool _isOpen = true;
    //float _timer;

    //public void Start()
    //{
    //    _isOpen = true;
    //}

    //public void OnPointerClick(PointerEventData eventData)
    //{
    //    StopAllCoroutines();
    //    StartCoroutine(OpenAndHideUI());
    //}

    //IEnumerator OpenAndHideUI()
    //{
    //    _isOpen = !_isOpen;
    //    if (_timer != 0f)
    //    {
    //        _timer = 1f - _timer;
    //    }
    //    while (_timer <= 1f)
    //    {
    //        _timer += Time.deltaTime * 2f;

    //        //float start = _isOpen ? -UITransform.sizeDelta.y : offset;
    //        //float dest = _isOpen ? offset : -UITransform.sizeDelta.y;
    //        float start = _isOpen ? -UITransform.sizeDelta.x : offset;
    //        float dest = _isOpen ? offset : -UITransform.sizeDelta.x;
    //        //UITransform.anchoredPosition = new Vector2(UITransform.anchoredPosition.x, Mathf.Lerp(start, dest, _timer));
    //        UITransform.anchoredPosition = new Vector2(Mathf.Lerp(start, dest, _timer), UITransform.anchoredPosition.y);
    //        yield return null;
    //    }
    //}
}
