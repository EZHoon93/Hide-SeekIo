
using System;

using UnityEngine;
using UnityEngine.UI;

public class UI_SelectButtonOutLine : MonoBehaviour
{
    [SerializeField] Image _image;
    public static Action<int> changeOueLineEvent;
    bool _active;
    private void Awake()
    {
        _image = GetComponent<Image>();
        _image.enabled = false;
        changeOueLineEvent -= ChangeOutLine;
        changeOueLineEvent += ChangeOutLine;
        //Managers.photonGameManager.gameJoin += Destroy;
    }

    private void OnDestroy()
    {
        changeOueLineEvent -= ChangeOutLine;
        //Managers.photonGameManager.gameJoin -= Destroy;
    }

    void Destroy()
    {
        _image.enabled = false;
    }
    public void Click()
    {
        _image.enabled = !_image.enabled;
        changeOueLineEvent(this.GetInstanceID());
    }

    void ChangeOutLine(int instasnceID)
    {
        
        if (this.GetInstanceID() != instasnceID)
        {
            _image.enabled = false;
        }
    }
}
