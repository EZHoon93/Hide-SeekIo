using System.Collections;

using DG.Tweening;

using UnityEngine;

public class Door2 : MonoBehaviour
{
    public enum Type
    {
        LEFT,
        RIGHT
    }

    public Type type;
    bool _isOpen;
    Collider _collider;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        _collider.enabled = true;
    }
    private void Start()
    {
        _isOpen = false;
        Managers.Game.AddListenrOnGameState(Define.GameState.Gameing, Open);

        this.transform.localRotation = Quaternion.Euler(Vector3.zero);
    }
    
    void Open()
    {
        switch (type)
        {
            case Type.LEFT:
                this.transform.DOLocalRotate(new Vector3(0, -180, 0), 1.0f);
                break;
            case Type.RIGHT:
                this.transform.DOLocalRotate(new Vector3(0, 180, 0), 1.0f);
                break;
        }
        _collider.enabled = false;
    }

  
}
