using UnityEngine;
using UnityEngine.UI;
using BehaviorDesigner.Runtime;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameSetting : ScriptableObject
{
    private const string SettingFileDirectory = "Assets/Resources";
    private const string SettingFilePath = "Assets/Resources/GameSettings.asset";//정확한 파일 위치,만약파

    private static GameSetting _instacne;

    public static GameSetting Instance
    {
        get
        {

            if (_instacne != null)
            {
                return _instacne;
            }

            //만약 없으면 파일경로에 해당 파일이 있는지.
            _instacne = Resources.Load<GameSetting>(path: "GameSettings");
            //에디터에서만 작용하므로 전처리기.
            //런타임이 아니라 에디터에서 사용 => usingUenityEditor
            //해당파일도 없으면
            if (_instacne == null)
            {
#if UNITY_EDITOR
                //존재하지않으면 폴더만듬
                if (!AssetDatabase.IsValidFolder(path: SettingFileDirectory))
                {
                    //폴더 만듬
                    AssetDatabase.CreateFolder(parentFolder: "Assets", newFolderName: "Resources");   //파일IO.도되지만 유니티 에디터에 즉각생성안되므로
                }

                //어떠한 이유로 실패해서 안가져올수도있으므로 하드하게 강제적으로 직접 설정.
                _instacne = AssetDatabase.LoadAssetAtPath<GameSetting>(SettingFilePath);

                //그래도 null이라면 만든다.
                if (_instacne == null)
                {
                    _instacne = CreateInstance<GameSetting>();   //아직 메모리에만 존재
                    AssetDatabase.CreateAsset(_instacne, SettingFilePath); //생성 및 저장파일 경로
                }
#endif


            }
            return _instacne;
        }

    }




    [SerializeField] RuntimeAnimatorController _hiderAnimatiorController;
    [SerializeField] RuntimeAnimatorController _seekrAnimatorController;
    //[SerializeField] ExternalBehaviorTree
    [SerializeField] RuntimeAnimatorController _playerAnimator;

    public ExternalBehaviorTree _seekerTree;

    public ExternalBehaviorTree externalBehavior;
    public RuntimeAnimatorController playerAnimator => _playerAnimator;
    public RuntimeAnimatorController GetRuntimeAnimatorController(Define.Team team)
    {
        switch (team)
        {
            case Define.Team.Hide:
                return _hiderAnimatiorController;
            case Define.Team.Seek:
                return _seekrAnimatorController;
        }

        return null;
    }
}
