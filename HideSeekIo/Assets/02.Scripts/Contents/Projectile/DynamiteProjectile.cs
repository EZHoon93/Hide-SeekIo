using System.Collections;

using UnityEngine;
using UnityEngine.UI;

public class DynamiteProjectile : ThrowProjectileObject
{
    [SerializeField] Slider _slider;
    [SerializeField] Image _rangeImage;
    [SerializeField] float _durationTime;
    [SerializeField] float _range;
    [SerializeField] int _damage = 1;
    float _remainTime;

    public override void Play(int useViewID, Vector3 startPoint, Vector3 endPoint)
    {
        base.Play(useViewID,startPoint, endPoint);
        _modelObject.SetActive(true);
        _slider.gameObject.SetActive(false);
        _rangeImage.gameObject.SetActive(false);
    }
    protected override void Explosion()
    {
        _slider.maxValue = _durationTime;
        _slider.value = _slider.maxValue;
        _remainTime = _durationTime;

        _slider.gameObject.SetActive(true);
        _rangeImage.gameObject.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(WaitExplosion());
    }

    IEnumerator WaitExplosion()
    {
        while(_remainTime > 0)
        {
            _slider.value = _remainTime;
            print(_remainTime);
            _remainTime -= Time.deltaTime;
            yield return null;
        }

        //깜박임..
        yield return new WaitForSeconds(0.3f);
        EffectManager.Instance.EffectOnLocal(Define.EffectType.GrenadeEffect, this.transform.position, 0);
        UtillGame.DamageInRange(this.transform, _range, _damage, _useViewID, UtillLayer.seekerToHiderAttack);
        _modelObject.SetActive(false);
        _slider.gameObject.SetActive(false);
        _rangeImage.gameObject.SetActive(false);
        _fogOfWarUnit.enabled = true;
        yield return new WaitForSeconds(1.0f);
        Push();
    }
}
