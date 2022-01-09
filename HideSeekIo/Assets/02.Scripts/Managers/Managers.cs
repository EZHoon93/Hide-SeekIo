using System;
using UnityEngine;
using DG.Tweening.Core;
#if UNITY_EDITOR
using UnityEditor;
#endif


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
    public static GameManager Game { get => GetManager(Instance._game); set { Instance._game =value; } }
    public static PhotonGameManager photonGameManager { get => GetManager(Instance._photonGameManager); set { Instance._photonGameManager = value; } }

    public static EffectManager effectManager { get => GetManager(Instance._effectManager); set { Instance._effectManager = value; } }
    public static BuffManager buffManager { get => GetManager(Instance._buffManager); set { Instance._buffManager = value; } }
    public static CameraManager CameraManager { get => GetManager(Instance._cameraManager); set { Instance._cameraManager = value; } }
    public static AIManager aIManager{ get => GetManager(Instance._aIManager); set { Instance._aIManager = value; } }

    #endregion

    #region Core
    DataManager _data = new DataManager();
    PoolManager _pool = new PoolManager();
    ResourceManager _resource = new ResourceManager();
    SoundManager _sound = new SoundManager();
    UIManager _ui = new UIManager();
    SpawnManager _spawnManager = new SpawnManager();
    EventManager _eventManager = new EventManager();

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
    public static EventManager EventManager => Instance._eventManager;
    public static PhotonManager PhotonManager => Instance._photonManager;

    #endregion

    #region SingleTon ScriptableObject
    private const string SettingFileDirectory = "Assets/Resources";
    //private const string SettingFilePath = "Assets/Resources/Setting/GameSettings.asset";//정확한 파일 위치,만약파
    [SerializeField] ProductSetting _productSetting;
    [SerializeField] GameSetting _gameSetting;
    [SerializeField]  UISetting _uISetting;

    public static ProductSetting ProductSetting => GetSingleScriptableOjbectr(Instance._productSetting); 
    public static GameSetting GameSetting => GetSingleScriptableOjbectr(Instance._gameSetting);
    public static UISetting UISetting => GetSingleScriptableOjbectr(Instance._uISetting);

    #endregion

    void Start()
    {
#if UNITY_ANDROID
        //Application.targetFrameRate = 30;
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

    public static T GetSingleScriptableOjbectr<T>(T go) where T : ScriptableObject
    {
        if (go == null)
        {
            go = Resource.Load<T>(path: nameof(T) );
            //리소스폴더에도 없다면 생성.
            if(go == null)
            {
                print("nu...생성");
#if UNITY_EDITOR
                //존재하지않으면 폴더만듬
                if (!AssetDatabase.IsValidFolder(path: SettingFileDirectory))
                {
                    //폴더 만듬
                    AssetDatabase.CreateFolder(parentFolder: "Assets", newFolderName: "Resources");   //파일IO.도되지만 유니티 에디터에 즉각생성안되므로
                }

                var filePath = $"{SettingFileDirectory}/Setting/{typeof(T).Name}.asset";
                //어떠한 이유로 실패해서 안가져올수도있으므로 하드하게 강제적으로 직접 설정.
                go = AssetDatabase.LoadAssetAtPath<T>(filePath);

                //그래도 null이라면 만든다.
                if (go == null)
                {
                    go = ScriptableObject.CreateInstance<T>();   //아직 메모리에만 존재
                    AssetDatabase.CreateAsset(go, filePath); //생성 및 저장파일 경로
                }
#endif

            }

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

            if (Application.isEditor)
            {
                s_instance = go.GetComponent<Managers>();
                return;
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

        EventManager.Clear();
    }

    /// <summary>
    /// 사실상 리셋,
    /// </summary>
    public static void ContentsClear()
    {
        Game.Clear();
        //photonGameManager.C
        //effectManager.Clear();
        CameraManager.Clear();
        //aIManager.Clear();
    }
}
