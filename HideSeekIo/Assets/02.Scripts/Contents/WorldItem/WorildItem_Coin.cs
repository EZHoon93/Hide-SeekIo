using DG.Tweening;

using UnityEngine;

public class WorildItem_Coin : MonoBehaviour , IEnterTrigger
{
    [SerializeField] private float _shakeScaleDuration = 1;
    [SerializeField] private float _hideScaleDuration = .25f;
    [SerializeField] public int Value = 1;

    bool _isCollect;

    private void OnEnable()
    {
        _isCollect = false;
        //CoinSpawnManager.coinCount ++;
    }
    private void OnDisable()
    {
        //CoinSpawnManager.coinCount --;
    }
    public void CollectEffect()
    {
        _isCollect = true;
        enabled = false;
        transform.DOShakeScale(_shakeScaleDuration);
        transform.DOScale(Vector3.zero, _hideScaleDuration).SetDelay(_shakeScaleDuration);
        EffectManager.Instance.EffectOnLocal(Define.EffectType.Coin, this.transform.position, 0 );


    }

    private void Update()
    {
        transform.Rotate(Vector3.one);
    }

    public void Enter(GameObject Gettingobject)
    {
        if (_isCollect) return;
        var hiderPlayer = Gettingobject.GetComponent<PlayerController>();
        if (hiderPlayer == null) return;
        CollectEffect();
    }
}
