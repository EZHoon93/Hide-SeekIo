using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Test_Collider2D : MonoBehaviour , IPointerUpHandler , IPointerDownHandler , IDragHandler
{
    static Test_Collider2D curr;
    public Text text;
    public int score;
    public Vector3 originPos;

    private void Start()
    {
        text.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        curr.transform.position = eventData.position;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        var clickObject = eventData.pointerCurrentRaycast.gameObject.GetComponent<Test_Collider2D>();
        originPos = clickObject.transform.position;
        curr = clickObject;
        curr.GetComponent<Image>().raycastTarget = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        var clickObject = eventData.pointerCurrentRaycast.gameObject.GetComponent<Test_Collider2D>();
        clickObject.score = curr.score;
        clickObject.text.text = curr.score.ToString();
        curr.GetComponent<Image>().raycastTarget = true;
        curr.transform.position = originPos;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
    }
}
