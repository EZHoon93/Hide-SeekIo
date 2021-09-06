using System.Collections;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class UI_CoolTime : MonoBehaviour
{
     Image _coolTimeImage;
    TextMeshProUGUI _coolTimeText;
    public float coolTime { get; set; }

    float _remainCoolTime;

    IEnumerator enumerator;

    private void Awake()
    {
        _coolTimeImage = GetComponent<Image>();
        _coolTimeText = this.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        _coolTimeImage.enabled = false;
    }
    private void OnEnable()
    {
        _coolTimeText.text = null;
    }
    public void StartCoolTime(float coolTime)
    {
        _remainCoolTime = coolTime;
        Util.StartCoroutine(this, ref enumerator, UpdateCoolTime());
    }
    IEnumerator UpdateCoolTime()
    {
        _coolTimeImage.enabled = true;
        _coolTimeText.text = _remainCoolTime.ToString();
        while (_remainCoolTime > 0)
        {
            yield return new WaitForSeconds(1.0f);
            _remainCoolTime -= 1;
            _coolTimeText.text = _remainCoolTime.ToString();
        }
        _coolTimeImage.enabled = false;
        _coolTimeText.text = null;

    }
}
