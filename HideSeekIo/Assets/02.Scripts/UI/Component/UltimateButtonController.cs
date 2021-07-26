using System.Collections;

using UnityEngine;

public class UltimateButtonController : MonoBehaviour
{
    GameObject p;
    public void RunChange()
    {
        var highlightImage = UltimateButton.GetUltimateButton("Run").buttonHighlight;

        highlightImage.enabled = !highlightImage.enabled;
    }
}
