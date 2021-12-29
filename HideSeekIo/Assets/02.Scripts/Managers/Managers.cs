using System;
using UnityEngine;


public class Managers : MonoBehaviour
{
    static Managers s_instance; // 유일성이 보장된다
    static Managers Instance { get { Init(); return s_instance; } } // 유일한 매니저를 갖고온다

    #region Contents
    GameManager _game;
    PhotonGameManager _photonGameManager;
    EffectManager _effectManager;
    BuffManager _buffManager;
    CameraManager _cameraManager;
    AIManager _aIManager;
    EventManager _eventManager;
    public static GameManager Game { get => GetManager(Instance._game); set { Instance._game =value; } }
    public static PhotonGameManager photonGameManager { get => GetManager(Instance._photonGameManager); set { Instance._photonGameManager = value; } }

    public static EffectManager effectManager { get => GetManager(Instance._effectManager); set { Instance._effectManager = value; } }
    public static BuffManager buffManager { get => GetManager(Instance._buffManager); set { Instance._buffManager = value; } }
    public static CameraManager cameraManager { get => GetManager(Instance._cameraManager); set { Instance._cameraManager = value; } }
    public static AIManager aIManager{ get => GetManager(Instance._aIManager); set { Instance._aIManager = value; } }

    public static EventManager eventManager { get => GetManager(Instance._eventManager); set { Instance._eventManager = value; } }
    #endregion

    #region Core
    DataManager _data = new DataManager();
    PoolManager _pool = new PoolManager();
    ResourceManager _resource = new ResourceManager();
    SoundManager _sound = new SoundManager();
    UIManager _ui = new UIManager();
    SpawnManager _spawnManager = new SpawnManager();

    [SerializeField] InputManager _input;
    [SerializeField] SceneManagerEx _scene;
    [SerializeField] PhotonManager _photonManager;

    public static DataManager Data => Instance._data;
    public static InputManager Input => Instance._input;
    public static PoolManager Pool => Instance._pool;
    public static ResourceManager Resource => Instance._resource;
    public static SceneManagerEx Scene => Instance._scene;
    public static SoundManager Sound { get { return Instance._sound; } }
    public static UIManager UI { get { return Instance._ui; } }
    public static SpawnManager Spawn { get { return Instance._spawnManager; } }

    public static PhotonManager photonManager => Instance._photonManager;

    #endregion

    #region Setting

    [SerializeField] ProductSetting _productSetting;

    public static ProductSetting productSetting => Instance._productSetting;
    #endregion

    void Start()
    {
#if UNITY_ANDROID
        Application.targetFrameRate = 30;
#endif
        Screen.SetResolution(1920, 1080, true);
        Init();
	}
    static T GetManager<T>(T go) where T : Component
    {
        if(go == null)
        {
            go = FindObjectOfType<T>();
        }

        return go;
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
            s_instance._input.Init();
            s_instance._sound.Init();
        }		
        
	}


    //코어 클리어
    public static void Clear()
    {
        Input.Clear();
        Scene.Clear();
        UI.Clear();
        Pool.Clear();
        Sound.Clear();
    }

    /// <summary>
    /// 사실상 리셋,
    /// </summary>
    public static void ContentsClear()
    {
        Game.Clear();
        //photonGameManager.C
        //effectManager.Clear();
        cameraManager.Clear();
        //aIManager.Clear();
    }
}
