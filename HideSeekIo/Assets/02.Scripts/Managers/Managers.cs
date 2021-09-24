﻿using System;
using System.Collections;
using System.Collections.Generic;

using ExitGames.Client.Photon;

using UnityEngine;


public class Managers : MonoBehaviour
{
    static Managers s_instance; // 유일성이 보장된다
    static Managers Instance { get { Init(); return s_instance; } } // 유일한 매니저를 갖고온다

    #region Contents
    GameManager _game = new GameManager();
    SpawnManager _spawnManager = new SpawnManager();
    StatSelectManager _statSelectManager = new StatSelectManager();
    InputItemManager _inputItemManager = new InputItemManager();

    public static GameManager Game { get { return Instance._game; } }
    public static SpawnManager Spawn { get { return Instance._spawnManager; } }
    public static StatSelectManager StatSelectManager => Instance._statSelectManager;
    public static InputItemManager InputItemManager => Instance._inputItemManager;

    #endregion

    #region Core
    DataManager _data = new DataManager();
    PoolManager _pool = new PoolManager();
    ResourceManager _resource = new ResourceManager();
    SceneManagerEx _scene = new SceneManagerEx();
    SoundManager _sound = new SoundManager();
    UIManager _ui = new UIManager();

    public static DataManager Data { get { return Instance._data; } }
    public static InputManager Input { get { return InputManager.Instance; } }
    public static PoolManager Pool { get { return Instance._pool; } }
    public static ResourceManager Resource { get { return Instance._resource; } }
    public static SceneManagerEx Scene { get { return Instance._scene; } }
    public static SoundManager Sound { get { return Instance._sound; } }
    public static UIManager UI { get { return Instance._ui; } }

    #endregion

    public static event Action ClearEvent;
    void Start()
    {
#if UNITY_ANDROID
        Application.targetFrameRate = 30;
#endif
        Screen.SetResolution(1920, 1080, true);
        Init();
	}

    static void Init()
    {
        if (s_instance == null)
        {
			GameObject go = GameObject.Find("@Managers");
            if (go == null)
            {
                go = new GameObject { name = "@Managers" };
                go.AddComponent<Managers>();
            }

            DontDestroyOnLoad(go);
            s_instance = go.GetComponent<Managers>();
            
            s_instance._data.Init();
            s_instance._pool.Init();
            s_instance._sound.Init();
        }		
        
	}

    public static void Clear()
    {
        Input.Clear();
        Scene.Clear();
        UI.Clear();
        Pool.Clear();
        Game.Clear();
        Sound.Clear();
        ClearEvent?.Invoke();
    }
}
