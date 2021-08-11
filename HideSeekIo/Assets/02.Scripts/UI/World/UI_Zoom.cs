using System.Collections;

using UnityEngine;

public class UI_Zoom : MonoBehaviour
{
    [SerializeField] Transform _meleeZoom;
    [SerializeField] Transform _throwZoom;
    [SerializeField] Transform _gunZoom;

    public Transform currentZoom { get; set; }
    Define.ZoomType _zoomType;
    Transform _taget;

    public void Setup(Define.ZoomType zoomType, Transform target)
    {
        _zoomType = zoomType;
        _taget = target;
        _meleeZoom.gameObject.SetActive(false);
        _throwZoom.gameObject.SetActive(false);
        _gunZoom.gameObject.SetActive(false);
        switch (_zoomType)
        {
            case Define.ZoomType.Gun:
                _gunZoom.gameObject.SetActive(true);
                currentZoom = _gunZoom;
                break;
            case Define.ZoomType.Melee:
                _meleeZoom.gameObject.SetActive(true);
                currentZoom = _meleeZoom;
                break;
            case Define.ZoomType.Throw:
                _throwZoom.gameObject.SetActive(true);
                currentZoom = _throwZoom;
                break;
        }
        this.gameObject.SetActive(false);
    }


    private void LateUpdate()
    {
        FixedUI();
    }

    public void FixedUI()
    {
        if (_taget)
        {
            this.transform.position = _taget.transform.position;
            this.transform.rotation = Quaternion.Euler(new Vector3(90, 0, 0));

        }
    }


}
