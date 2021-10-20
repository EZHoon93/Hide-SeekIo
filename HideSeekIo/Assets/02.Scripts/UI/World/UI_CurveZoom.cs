using System.Collections;

using UnityEngine;

public class UI_CurveZoom : UI_ZoomBase
{
    [SerializeField] Transform _targetUI;
    [SerializeField] float amount;
    LineRenderer _lineRenderer;

    //Vector3 s
    private void Awake()
    {
        _lineRenderer = GetComponentInChildren<LineRenderer>();
        _lineRenderer.positionCount = 25;
        _lineRenderer.enabled = false;
        _targetUI.gameObject.SetActive(false);
    }

    public void UpdateRange(float newRange)
    {
        var rangeVector = new Vector3(newRange, newRange, newRange);
        _targetUI.transform.localScale = rangeVector;
    }
    public override void SetActiveZoom(bool active)
    {
        base.SetActiveZoom(active);
        _targetUI.gameObject.SetActive(active);
        if(active == false)
        {
            _targetUI.transform.localPosition = Vector3.zero;
        }
    }
    public override void ZoomOff()
    {
        _targetUI.gameObject.SetActive(false);
        _lineRenderer.enabled = false;
        _targetUI.transform.localPosition = Vector3.zero;
    }
    public override void UpdateZoom(Vector3 startPoint, Vector3 endPoint)
    {
        //_targetUI.position = endPoint;
        _targetUI.position = Vector3.Lerp(_targetUI.position, endPoint, 0.5f);
        _targetUI.gameObject.SetActive(true);
        var center = (startPoint + _targetUI.position) * 0.5f;
        center.y += amount;
        Vector3 RelCenter = startPoint - center;
        Vector3 aimRelCenter = _targetUI.position - center;
        Vector3 theArc;
        for (float index = 0.0f, interval = -0.0417f; interval < 1.0f;)
        {
            theArc = Vector3.Slerp(RelCenter, aimRelCenter, interval += 0.0417f);
            _lineRenderer.SetPosition((int)index++, theArc + center);
        }
        _lineRenderer.enabled = true;
        //_targetUI.position = endPoint;
    }

    //private void LateUpdate()
    //{
    //    var center = (this.transform.position + _targetUI.position) * 0.5f;
    //    center.y += amount;
    //    Vector3 RelCenter = this.transform.position - center;
    //    Vector3 aimRelCenter = _targetUI.position- center;
    //    Vector3 theArc;
    //    for (float index = 0.0f, interval = -0.0417f; interval < 1.0f;)
    //    {
    //        theArc = Vector3.Slerp(RelCenter, aimRelCenter, interval += 0.0417f);
    //        _lineRenderer.SetPosition((int)index++, theArc + center);
    //    }
    //    _lineRenderer.enabled = true;
    //    //_targetUI.position = endPoint;
    //}

    private void Update()
    {
        
    }

}
