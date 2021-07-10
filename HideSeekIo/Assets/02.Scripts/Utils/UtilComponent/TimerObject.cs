using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���ÿ��� ����ϴ� Ÿ�̸Ӿ�����
/// </summary>
public class TimerObject : MonoBehaviour
{
    [SerializeField] float _time;

    Poolable _poolable;
    private void Awake()
    {
        _poolable =  GetComponent<Poolable>();
    }

    private void OnEnable()
    {
        Invoke("AfaterDestroy", _time);
    }
    void AfaterDestroy()
    {
        if (_poolable != null)
        {
            Managers.Pool.Push(_poolable);
        }
        else
        {
            Managers.Resource.Destroy(this.gameObject);
        }
    }
}
