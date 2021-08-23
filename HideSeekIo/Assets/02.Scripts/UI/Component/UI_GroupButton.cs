using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_GroupButton : MonoBehaviour
{
    Button _button;
    Image _onImage;

    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    public void SetActive(bool active)
    {
        _onImage.enabled = active;
        if (active)
        {

        }
    }
}
