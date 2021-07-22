using System.Collections;

using DG.Tweening;

using TMPro;

using UnityEngine;

public class UI_Text : MonoBehaviour
{
    TextMeshProUGUI _text;

    private void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
    }

    public void UpdateText(string content   )
    {

        _text.text = content;
    }

    public void UpdateColorText(string content, Color color)
    {
        UpdateText(content);

    }

    public void UpdateFadeText(string content, float time)
    {
        //uiMain.killText.DOFade(0.0f, 2.0f);

    }
}
