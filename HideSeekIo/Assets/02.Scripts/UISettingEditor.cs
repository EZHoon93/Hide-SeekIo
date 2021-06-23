using UnityEditor;

using UnityEngine;
//using UnityEditor;

[CustomEditor(typeof(UISetting))]
public class UISettingEditor : Editor
{
    [MenuItem("Assets/Open UI Setting")]
    

    public static void OpenInspector()
    {
        Selection.activeObject = UISetting.Instance;
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
