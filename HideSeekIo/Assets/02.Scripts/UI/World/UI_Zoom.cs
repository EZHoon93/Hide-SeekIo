using System.Collections;

using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class UI_Zoom : MonoBehaviour
{
    [SerializeField] Transform _meleeZoom;
    [SerializeField] Transform _throwZoom;
    [SerializeField] Transform _gunZoom;

    [SerializeField] Image image;
    public Transform currentZoom { get; set; }
    Weapon.WeaponType _weaponType;
    Transform _taget;

    [SerializeField] LineRenderer lineRenderer;

   
    public void Setup(Weapon.WeaponType weaponType, Weapon weapon, Transform target)
    {
        _weaponType = weaponType;
        _taget = target;
        _meleeZoom.gameObject.SetActive(false);
        _throwZoom.gameObject.SetActive(false);
        _gunZoom.gameObject.SetActive(false);
        switch (_weaponType)
        {
            case Weapon.WeaponType.Gun:
                _gunZoom.gameObject.SetActive(true);
                currentZoom = _gunZoom;
                break;
            case Weapon.WeaponType.Melee:
                _meleeZoom.gameObject.SetActive(true);
                currentZoom = _meleeZoom;
                break;
            case Weapon.WeaponType.Throw:
                _throwZoom.gameObject.SetActive(true);
                currentZoom = _throwZoom;
                break;
        }
        this.gameObject.SetActive(false);
        //weapon.destroyEventCallBack += () => weapon.inputControllerObject.RemoveEvent(ControllerInputType.Down, Zoom);
        //weapon.inputControllerObject.AddEvent(ControllerInputType.Drag, Zoom);
    }

    public void ZoomLine(float distance)
    {

    }

    public void Zoom(Vector2 vector2)
    {

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
            //var s = _throwZoom.transform.position;
            //s.y = _taget.transform.position.y;
            //var isx = UtillGame.IsPointOnNavMesh(s);
            //if (isx)
            //{
            //    image.color = Color.red;
            //}
            //else
            //{
            //    image.color = Color.white;

            //}

            //_throwZoom.
        }
    }


}
