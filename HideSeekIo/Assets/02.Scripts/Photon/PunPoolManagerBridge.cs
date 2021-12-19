
using Photon.Pun;
using UnityEngine;

public class PunPoolManagerBridge : MonoBehaviour, IPunPrefabPool
{
    [SerializeField] GameObject[] _spawnList;
    private void Start()
    {
        Setup();
    }
    public void Setup()
    {
        PhotonNetwork.PrefabPool = this;
        
        //foreach(var spawn in _spawnList)
        //{
        //    Managers.Pool.CreatePool(spawn, 8);
        //}


    }

    public GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation)
    {
        //var go = Managers.Resource.Instantiate($"Photon/{prefabId}");/
        var prefab = Managers.Resource.Load<GameObject>($"Prefabs/Photon/{prefabId}");
        var go = Managers.Pool.Pop(prefab).gameObject;

        if (go == null)
        {
            Debug.LogError("찾을수없음  " + prefabId);
        }

        go.transform.position = position;
        go.transform.rotation = rotation;
        go.gameObject.SetActive(false);
        return go;
    }

    public void Destroy(GameObject gameObject)
    {
        print("Destroy");
        var onPhotonViewPreNetDestroy = gameObject.GetComponents<IOnPhotonViewPreNetDestroy>();
        if(onPhotonViewPreNetDestroy != null)
        {
            foreach(var p in onPhotonViewPreNetDestroy)
            {
                p.OnPreNetDestroy(gameObject.GetPhotonView());
            }
            //onPhotonViewPreNetDestroy.OnPreNetDestroy(gameObject.GetPhotonView());
        }
        Managers.Resource.Destroy(gameObject);

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