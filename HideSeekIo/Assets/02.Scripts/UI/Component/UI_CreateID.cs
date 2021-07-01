using System.Collections;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class UI_CreateID : MonoBehaviour
{
    [SerializeField] Button _confirmButton;
    [SerializeField] TMP_InputField _idInputField;

    private void Awake()
    {
        _confirmButton.onClick.AddListener(() => CheckID());
    }


    void CheckID()
    {
        CreateID();
    }
    void CreateID()
    {
        PlayerInfo.CreateFirstID(_idInputField.text);
    }

}
