using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_StraightZoom : UI_ZoomBase
{
    LineRenderer _lineRenderer;
    Vector3 _endPoint;
    Vector3 _startPoint;

    private void Awake()
    {
        _lineRenderer = GetComponentInChildren<LineRenderer>();
        _lineRenderer.positionCount = 2;
        _lineRenderer.enabled = false;
    }
  
    public override void UpdateZoom(Vector3 startPoint,Vector3 endPoint)
    {
        _startPoint = startPoint;
        _endPoint = endPoint;
    }

    private void LateUpdate()
    {
        _lineRenderer.SetPosition(0, _startPoint);
        _lineRenderer.SetPosition(1, _endPoint);
        _lineRenderer.enabled = true;
    }
    public override void ZoomOff()
    {
        base.ZoomOff();
    }
    
   
}
