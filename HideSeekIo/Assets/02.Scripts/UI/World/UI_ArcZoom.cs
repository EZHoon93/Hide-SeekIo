using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_ArcZoom : UI_ZoomBase
{
    [SerializeField] float _angle;
    [SerializeField] Image _arcImage;
    [SerializeField] Transform _parentTr;
    private void Start()
    {
        _arcImage.fillAmount = _angle / 360;
        //_arcParent.localRotation = Quaternion.Euler(0, 0, _angle);
    }

    [ContextMenu("Setup")]
    void Setup()
    {
        _arcImage.fillAmount = _angle / 360;
        _arcImage.transform.localRotation = Quaternion.Euler(0, 0, _angle * 0.5f);
    }
    public void SetupAngle(float distance, float angle)
    {
        _angle = angle;
        _arcImage.fillAmount = _angle / 360;
        _arcImage.transform.localRotation = Quaternion.Euler(0, 0, _angle * 0.5f);
        _parentTr.localScale = Vector3.one * distance;
    }
    public override void UpdateZoom(Vector3 startPoint, Vector3 endPoint)
    {
        var angle = Mathf.Atan2(endPoint.y, endPoint.x) * Mathf.Rad2Deg;
        _parentTr.localRotation = Quaternion.Euler(0, 0, (90 - angle));

    }

  
}
