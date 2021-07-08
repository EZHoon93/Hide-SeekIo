#if UNITY_ANDROID && USC_ANDROID_LEGACY_EXTERNAL_STORAGE

using System.IO;

using UnityEngine;
using UnityEditor.Android;

namespace AlmostEngine.Screenshot
{
#if UNITY_2018_4_OR_NEWER
    public class AndroidPostProcessBuild : IPostGenerateGradleAndroidProject
    {
        public int callbackOrder { get { return 1; } }
        public void OnPostGenerateGradleAndroidProject(string basePath)
        {
            // Get the manifest
            string manifestPath = basePath + "/src/main/AndroidManifest.xml";
            string manifest = File.ReadAllText(manifestPath);

            // Insert storage legacy entry
            int lastEntryIndex = manifest.IndexOf("<application") + 12;
            manifest = manifest.Insert(lastEntryIndex, " android:requestLegacyExternalStorage=\"true\" ");
            Debug.Log("Inserting requestLegacyExternalStorage to Android manifest.\n" + manifest);
            
            // Save manifest
            File.WriteAllText(manifestPath, manifest);
        }
    }
#endif
}

#endif