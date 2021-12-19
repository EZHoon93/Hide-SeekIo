using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;

public abstract class BaseScene : MonoBehaviourPun
{
    public Define.Scene SceneType { get; protected set; } = Define.Scene.Unknown;

	void Awake()
	{
		Init();
	}

	protected virtual void Init()
    {
        Object obj = GameObject.FindObjectOfType(typeof(EventSystem));
        if (obj == null)
            Managers.Resource.Instantiate("UI/EventSystem").name = "@EventSystem";

        Managers.Scene.currentScene = this; 
    }

    public abstract void Clear();
}
