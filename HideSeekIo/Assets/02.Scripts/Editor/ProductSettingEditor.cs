using UnityEditor;

using UnityEngine;

#if UNITY_EDITOR

[CustomEditor(typeof(ProductSetting))]

public class ProductSettingEditor : Editor
{
    [MenuItem("Assets/Open ProductSettingEditor")]
    public static void OpenInspector()
    {
        Selection.activeObject = ProductSetting.Instance;
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
