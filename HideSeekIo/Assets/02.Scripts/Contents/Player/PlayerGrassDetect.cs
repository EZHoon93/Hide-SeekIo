using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlayerGrassDetect : MonoBehaviour 
{
    List<Grass2> _grassList = new List<Grass2>(32);

    private void Start()
    {
        //Managers.Game.AddListenrOnGameState(Define.GameState.Wait, Clear);
    }

    public void Clear()
    {
        foreach(var g in _grassList)
        {
            g.SetActiveByGrassDetected(false);
        }
        _grassList.Clear();
    }
    private void OnTriggerEnter(Collider other)
    {
        var grass = other.GetComponent<Grass2>();
        if (grass != null)
        {
            grass.SetActiveByGrassDetected(true);
            if(_grassList.Contains(grass) == false)
            {
                _grassList.Add(grass);
            }
        }

        var livingEntity = other.GetComponent<LivingEntity>();
        if (livingEntity)
        {
            livingEntity.fogController.hideInFog.isGrassDetected = true;
            return;
        }

    }

    private void OnTriggerExit(Collider other)
    {
        var grass = other.GetComponent<Grass2>();
        if (grass != null)
        {
            grass.SetActiveByGrassDetected(false);
            if (_grassList.Contains(grass))
            {
                _grassList.Remove(grass);
            }
            return;
        }


        var livingEntity = other.GetComponent<LivingEntity>();
        if (livingEntity)
        {
            livingEntity.fogController.hideInFog.isGrassDetected = false;
            return;
        }
    }
}
