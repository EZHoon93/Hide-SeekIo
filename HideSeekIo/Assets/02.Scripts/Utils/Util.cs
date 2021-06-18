using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Util
{
    public static T GetOrAddComponent<T>(GameObject go) where T : UnityEngine.Component
    {
        T component = go.GetComponent<T>();
        if (component == null)
            component = go.AddComponent<T>();
        return component;
    }

    public static GameObject FindChild(GameObject go, string name = null, bool recursive = false)
    {
        Transform transform = FindChild<Transform>(go, name, recursive);
        if (transform == null)
            return null;

        return transform.gameObject;
    }

    public static T FindChild<T>(GameObject go, string name = null, bool recursive = false) where T : UnityEngine.Object
    {
        if (go == null)
            return null;

        if (recursive == false)
        {
            for (int i = 0; i < go.transform.childCount; i++)
            {
                Transform transform = go.transform.GetChild(i);
                if (string.IsNullOrEmpty(name) || transform.name == name)
                {
                    T component = transform.GetComponent<T>();
                    if (component != null)
                        return component;
                }
            }
        }
        else
        {

            foreach (T component in go.GetComponentsInChildren<T>())
            {
                if (string.IsNullOrEmpty(name) || component.name == name)
                    return component;
            }
        }

        return null;
    }

    public static void StartCoroutine(MonoBehaviour monoBehaviour, ref IEnumerator enumerator, IEnumerator nextEnumerator)
    {
        if (enumerator != null)
        {
            monoBehaviour.StopCoroutine(enumerator);
        }
        enumerator = nextEnumerator;
        monoBehaviour.StartCoroutine(enumerator);
    }

    //콜백 함수, time초뒤 해당 함수실행
    public static void CallBackFunction(MonoBehaviour monoBehaviour, float time, System.Action action)
    {
        monoBehaviour.StartCoroutine(CallBack(time, action));
    }

    static IEnumerator CallBack(float time, System.Action action)
    {
        yield return new WaitForSeconds(time);
        action?.Invoke();
    }

    public static void ImageFillAmount(UnityEngine.UI.Image image, ref IEnumerator enumerator, float coolTime)
    {
        StartCoroutine(image, ref enumerator, ProcessCoolTime(image, coolTime));
    }


    static IEnumerator ProcessCoolTime(UnityEngine.UI.Image image, float coolTime)
    {
        //image.StopCoroutine()
        //GetComponent<IEnumerator>()
        image.enabled = true;
        var processCoolTime = coolTime;
        while (processCoolTime > 0)
        {
            processCoolTime -= Time.deltaTime;
            image.fillAmount = processCoolTime / coolTime;
            yield return null;
        }
        image.fillAmount = 0;
        image.enabled = false;
    }

    public static void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }

    #region Color Text
    static string GetColorToCode(Color color)
    {
        //var result = "<"+_color.r
        string r = ((int)(color.r * 255)).ToString("X2");
        string g = ((int)(color.g * 255)).ToString("X2");
        string b = ((int)(color.b * 255)).ToString("X2");
        string a = ((int)(color.a * 255)).ToString("X2");
        string result = string.Format("{0}{1}{2}{3}", r, g, b, a);

        return result;
    }

    public static string GetColorContent(Color _color, string _content)
    {
        var colorHexCode = GetColorToCode(_color);

        var result = "<color=#" + colorHexCode + ">" + _content + "</color>";


        return result;

    }

    /// <summary>
    /// 초 => 00:00 타입 으로 전환
    /// </summary>
    /// <param name="sec"></param>
    /// <returns></returns>
    public static string GetTimeFormat(int newSec)
    {
        if (newSec < 0)
        {
            return "00:00";
        }
        int min = newSec / 60;
        int sec = newSec % 60;
        var timeStr = string.Format("{0:D1}:{1:D2}", min, (int)sec);
        return timeStr;

    }
    #endregion


}
