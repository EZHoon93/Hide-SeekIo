using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

namespace AlmostEngine.Screenshot
{

    public class RemovePermissionNeeds
    {

        public static void ToggleiOSGalleryPermission(bool enable = true)
        {
            if (enable)
            {
                Debug.Log("Restoring Photo permission needs for iOS.");
            }
            else
            {
                Debug.Log("Removing Photo permission needs for iOS.");
            }

            // Plugin files
            ExcludePlugin("iOSUtils", !enable);

            // iOS photo dependency
            if (enable)
            {
                FrameworkDependency.AddiOSPhotosFrameworkDependency();
            }

            // Symbols
            ToggleDefineSymbol(BuildTargetGroup.iOS, "USC_EXCLUDE_IOS_GALLERY", !enable);
        }

        public static void ToggleShare(bool enable = true)
        {
            ToggleDefineSymbol(BuildTargetGroup.iOS, "USC_EXCLUDE_SHARE", !enable);
            ToggleDefineSymbol(BuildTargetGroup.Android, "USC_EXCLUDE_SHARE", !enable);
            ToggleDefineSymbol(BuildTargetGroup.WebGL, "USC_EXCLUDE_SHARE", !enable);
        }

        public static bool IsSymbolDefined(BuildTargetGroup targetGroup, string symbol)
        {
            return UnityEditor.PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup).Contains(symbol);
        }

        public static void ToggleDefineSymbol(BuildTargetGroup targetGroup, string symbol, bool enable)
        {
            if (enable)
            {
                if (!UnityEditor.PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup).Contains(symbol))
                {
                    UnityEditor.PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, UnityEditor.PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup) + ";" + symbol);
                }
            }
            else
            {
                UnityEditor.PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, UnityEditor.PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup).Replace(symbol, ""));
            }
            Debug.Log(targetGroup.ToString() + " define symbols: " + UnityEditor.PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup));
        }

        public static void AndroidLegacyExternalStorage(bool enable = true)
        {
            if (enable)
            {
                Debug.Log("Enabling legacy external storage for Android.");
            }
            else
            {
                Debug.Log("Disabling legacy external storage for Android.");
            }
            ToggleDefineSymbol(BuildTargetGroup.Android, "USC_ANDROID_LEGACY_EXTERNAL_STORAGE", enable);
        }

        public static void RemoveAndroidStoragePermission(bool exclude = true)
        {
            if (exclude)
            {
                Debug.Log("Removing storage permission needs for Android.");
            }
            else
            {
                Debug.Log("Restoring storage permission needs for Android.");
            }

            // External storage
            UnityEditor.PlayerSettings.Android.forceSDCardPermission = !exclude;
        }

        public static void ExcludePlugin(string pluginFileName, bool exclude = true)
        {
            string[] utils = AssetDatabase.FindAssets(pluginFileName);
            for (int i = 0; i < utils.Length; ++i)
            {
                string path = AssetDatabase.GUIDToAssetPath(utils[i]);
                if (!path.Contains(pluginFileName + ".m"))
                    continue;
                string newPath = "";
                if (exclude)
                {
                    newPath = path.Replace(".bk", "") + ".bk";
                }
                else
                {
                    newPath = path.Replace(".bk", "");
                }
                if (path != newPath)
                {
                    Debug.Log("Moving " + pluginFileName + " plugin: " + path + " to " + newPath);
                    AssetDatabase.MoveAsset(path, newPath);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            }
        }
    }
}