
using DG.Tweening;
using UnityEngine;

public class Door3 : MonoBehaviour
{
    public enum Type
    {
        LEFT,
        RIGHT
    }

    public Type type;
    bool _isOpen;

 
    private void Start()
    {
        _isOpen = false;
        Managers.Game.AddListenrOnGameState(Define.GameState.Gameing, Open);
        this.transform.rotation = Quaternion.Euler(Vector3.zero);
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
        Invoke("Close", 2.0f);
    }

    void Close()
    {
        this.transform.DOLocalRotate(new Vector3(0, 0, 0), 1.0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        Open();
    }
}
