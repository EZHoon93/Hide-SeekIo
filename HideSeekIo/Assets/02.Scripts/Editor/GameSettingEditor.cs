using UnityEditor;

using UnityEngine;

#if UNITY_EDITOR

[CustomEditor(typeof(GameSettingEditor))]

public class GameSettingEditor : Editor
{
    [MenuItem("Assets/Open UI GameSettingEditor")]
    public static void OpenInspector()
    {
        Selection.activeObject = Managers.GameSetting;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
            AssetDatabase.SaveAssets();
        }
    }


}
#endif
