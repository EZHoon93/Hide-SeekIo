using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class SeekrRader : MonoBehaviour
{
    LivingEntity _myLivingEntity;
    [SerializeField] SpriteRenderer _spriteRenderer;
    HashSet<LivingEntity> _detectedHash = new HashSet<LivingEntity>();  //탐지된 객체
    [SerializeField] bool isDectedColor;
    IEnumerator _enumerator;
    [SerializeField] int count;

    private void Awake()
    {
        _myLivingEntity = GetComponentInParent<LivingEntity>();

        this.transform.parent.GetComponent<IOnPhotonInstantiate>().OnPhotonInstantiateEvent += OnPhotonInstantiate;
    }
    private void OnPhotonInstantiate()
    {
        Clear();
        Util.StartCoroutine(this, ref  _enumerator, RaderUpdate());
        Color color2 = Color.gray;
        color2.a = 0.5f;
        _spriteRenderer.color = color2;
        isDectedColor = false;
    }
    void Clear()
    {
        _spriteRenderer.enabled = false;
        foreach (var d in _detectedHash)
        {
            d.onDeath -= () => enemyDie(d);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        var livingEntity =  other.GetComponent<LivingEntity>();
        if (_myLivingEntity == livingEntity) return;
        _detectedHash.Add(livingEntity);
        livingEntity.onDeath += () => enemyDie(livingEntity) ;
        UpdateSprte();
    }
    private void OnTriggerExit(Collider other)
    {
        var livingEntity = other.GetComponent<LivingEntity>();
        if (_myLivingEntity == livingEntity) return;
        livingEntity.onDeath -= () => enemyDie(livingEntity);
        _detectedHash.Remove(livingEntity);
        UpdateSprte();
        
    }

    void enemyDie(LivingEntity livingEntity)
    {
        livingEntity.onDeath -= UpdateSprte;
        _detectedHash.Remove(livingEntity);
        UpdateSprte();
    }

    void UpdateSprte()
    {
        count = _detectedHash.Count;
        if (_detectedHash.Count > 0)
        {
            if (isDectedColor) return;
            Color color = Color.red;
            color.a = 0.5f;
            _spriteRenderer.color = color;
            isDectedColor = true;

        }
        else
        {
            if (!isDectedColor) return;
            Color color2 = Color.gray;
            color2.a = 0.5f;
            _spriteRenderer.color = color2;
            isDectedColor = false;
        }
        
    }
    IEnumerator RaderUpdate()
    {
        yield return new WaitForSeconds(5.0f);
        _spriteRenderer.enabled = true;

        while (true)
        {
            yield return new WaitForSeconds(0.3f);
            UpdateSprte();
        }
    }
    
}
