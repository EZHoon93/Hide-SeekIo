using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace BeautifyForPPS {

    public class Demo1 : MonoBehaviour
    {

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.J)) {

                BeautifySettings.settings.bloomIntensity.value += 0.1f;
            }
            if (Input.GetKeyDown(KeyCode.T) || Input.GetMouseButtonDown(0))
            {
                BeautifySettings.volume.enabled = !BeautifySettings.volume.enabled;
                UpdateText();
            }
            if (Input.GetKeyDown(KeyCode.B)) BeautifySettings.instance.Blink(0.2f);

            if (Input.GetKeyDown(KeyCode.C)) {
                BeautifySettings.settings.compareMode.value = !BeautifySettings.settings.compareMode;
            }
        }

        void UpdateText()
        {
            if (BeautifySettings.volume.enabled)
            {
                GameObject.Find("Beautify").GetComponent<Text>().text = "Beautify ON";
            }
            else
            {
                GameObject.Find("Beautify").GetComponent<Text>().text = "Beautify OFF";
            }
        }


    }
}
