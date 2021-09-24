using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

[CustomEditor(typeof(AISettingEditor))]

public class AISettingEditor : Editor
{
    [MenuItem("Assets/Open UI AISettingEditor")]
    public static void OpenInspector()
    {
        Selection.activeObject = AISetting.Instance;
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
