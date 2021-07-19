using System.Collections;

using UnityEngine;

public class SeekerBlock : MonoBehaviour
{

    private void Awake()
    {
        PhotonGameManager.Instacne.AddListenr(Define.GameState.Gameing, Explosion);
    }

    void Explosion()
    {
        EffectManager.Instance.EffectOnLocal(Define.EffectType.CloudBurst, this.transform.position, 0);
        Managers.Resource.Destroy(this.gameObject);
    }
}
