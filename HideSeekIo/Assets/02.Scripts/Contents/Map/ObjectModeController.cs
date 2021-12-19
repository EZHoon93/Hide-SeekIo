using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectModeController : MonoBehaviour
{
    [SerializeField] GameObject[] _ojbectList;

    public GameObject[] objectList => _ojbectList;
    public int index { get; set; }

    [ContextMenu("EditorSetup")]
    public void EditorSetup()
    {
        Setup(MakeRandom());

    }
    [ContextMenu("EditorRandomRotation")]
    public void EditorRandomRotation()
    {
        float ran = Random.Range(0, 360);
        this.transform.rotation = Quaternion.Euler(0, ran, 0);
    }
    public int MakeRandom()
    {
        //foreach(var o in _ojbectList)
        //{
        //    o.SetActive(false);
        //}

        index = Random.Range(0, _ojbectList.Length);

        //_ojbectList[index].SetActive(true);

        return index;
    }


    public void Setup(int newIndex)
    {
        foreach (var o in _ojbectList)
        {
            o.SetActive(false);
        }

        _ojbectList[newIndex].gameObject.SetActive(true);
    }
}
