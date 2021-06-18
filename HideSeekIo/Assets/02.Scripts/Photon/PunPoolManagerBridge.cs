
using Photon.Pun;
using UnityEngine;

public class PunPoolManagerBridge : MonoBehaviour, IPunPrefabPool
{

    private void Start()
    {
        Setup();
    }
    public void Setup()
    {
        PhotonNetwork.PrefabPool = this;
        


    }

    public GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation)
    {
        //var go = ObjectPoolManager.instance.Pop_Server(prefabId);
        var go = Managers.Resource.Instantiate($"Photon/{prefabId}");
        go.transform.position = position;
        go.transform.rotation = rotation;
        go.gameObject.SetActive(false);
        return go;
    }

    public void Destroy(GameObject gameObject)
    {
        //var poolable = gameObject.GetComponent<PoolableObject>();
        //if (poolable)
        //{
        //    poolable.Push();
        //}

        //var photonDestroyList = gameObject.GetComponentsInChildren<IOnPhotonViewPreNetDestroy>();
        //if (photonDestroyList.Length > 0)
        //{
        //    foreach(var destroy in photonDestroyList)
        //    {
        //        destroy.OnPreNetDestroy(gameObject.GetPhotonView());
        //    }
        //}
    }


}