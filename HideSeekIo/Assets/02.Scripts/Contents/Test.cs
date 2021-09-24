using System.Collections;

using Photon.Pun;

using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] GameObject stone;
    // Use this for initialization
    [SerializeField] Transform test;
    [SerializeField] Transform end;
    //void Start()
    //{
    //    Managers.Spawn.WorldItemSpawn(Define.WorldItem.Box, Vector3.zero);
    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.A))
    //    {
    //        print("발사");
    //        var t =  Managers.Pool.Pop(stone).GetComponent<ThrowProjectileObject>();
    //        t.transform.position = test.position;
    //    }
    //}
}
