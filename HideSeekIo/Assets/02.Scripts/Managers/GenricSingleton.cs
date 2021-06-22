using System.Collections;

using UnityEngine;
using Photon.Pun;
public class GenricSingleton<T> : MonoBehaviourPun where T : MonoBehaviour
{

    public static T Instance  
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<T>();
            }

            return instance;
        }
    }

    private static T instance;


    protected virtual void Awake()
    {
        if (Instance != this)
        {
            // 자신을 파괴
            Destroy(gameObject);
        }

        print("셋업싱글톤" + gameObject.name);
    }

}
