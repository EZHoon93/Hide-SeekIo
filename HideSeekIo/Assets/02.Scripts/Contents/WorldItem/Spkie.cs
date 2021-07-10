using System.Collections;

using DG.Tweening;

using PepijnWillekens.EasyWallColliderUnity;

using UnityEngine;

public class Spkie : MonoBehaviour
{
    [SerializeField] Transform _spkies;    //가시오브젝트들
    [SerializeField] EasyWallCollider _collider;
    private void Start()
    {
        PhotonGameManager.Instacne.GameStartEvent += Down;
    }

    void Down()
    {
        _spkies.DOLocalMoveY(-1, 1 ).OnComplete( () => _collider.gameObject.SetActive(false));
        
    }

    
}
