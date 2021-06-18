using UnityEngine;
using Photon.Pun;

public abstract class InputBase : MonoBehaviourPun
{
    public abstract void Stop(float newTime);

    public Vector2 RandomVector2 { get; set; }
    public virtual  void OnPhotonInstantiate()
    {
        
    }
}
