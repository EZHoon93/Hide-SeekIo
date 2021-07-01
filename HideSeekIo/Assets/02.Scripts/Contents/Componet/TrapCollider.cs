using System.Collections;

using UnityEngine;

public class TrapCollider : MonoBehaviour
{
    Trap _trap;
    private void Awake()
    {
        _trap =  this.transform.parent.GetComponent<Trap>();
    }

    private void OnTriggerEnter(Collider other)
    {
        print(other.gameObject.name + "갈ㄻ");
        if( other.gameObject.layer == (int)Define.Layer.Seeker)
        {
            _trap.TrapCollider(other.gameObject);
        }
    }

}
