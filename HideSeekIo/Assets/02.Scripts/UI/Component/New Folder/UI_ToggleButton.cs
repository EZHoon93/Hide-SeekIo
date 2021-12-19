using System.Collections;

using UnityEngine;
using UnityEngine.UI;

public class UI_ToggleButton : MonoBehaviour
{
    [SerializeField] Button _onButton;
    [SerializeField] Button _offButton;



    public void SetActive(bool active)
    {
        _onButton.gameObject.SetActive(active);
        _offButton.gameObject.SetActive(!active);
    }


}
