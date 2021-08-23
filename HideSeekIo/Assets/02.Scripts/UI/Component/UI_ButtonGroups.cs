using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_ButtonGroups : MonoBehaviour
{
    [SerializeField] UI_GroupButton[] _uI_GroupButtons;


    private void Start()
    {
        foreach (var u in _uI_GroupButtons)
            u.SetActive(false);

        _uI_GroupButtons[0].SetActive(true);
    }
}
