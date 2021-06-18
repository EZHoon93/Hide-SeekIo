using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class SeekrRader : MonoBehaviour
{
    [SerializeField] SpriteRenderer _spriteRenderer;
    HashSet<GameObject> _detectedHash = new HashSet<GameObject>();  //탐지된 객체

    bool isDectedColor;
    IEnumerator _enumerator;
    private void OnEnable()
    {
        _spriteRenderer.enabled = false;
        Util.StartCoroutine(this, ref  _enumerator, RaderUpdate());
    }
    private void OnTriggerEnter(Collider other)
    {
        if (_detectedHash.Contains(other.gameObject) == false)
        {
            _detectedHash.Add(other.gameObject);
            UpdateSprte();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (_detectedHash.Contains(other.gameObject))
        {
            _detectedHash.Remove(other.gameObject);
            UpdateSprte();
        }
    }

    void UpdateSprte()
    {
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
            Color color2 = Color.white;
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
