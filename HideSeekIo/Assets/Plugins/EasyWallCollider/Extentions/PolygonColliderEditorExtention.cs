using System;
#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;
#endif
using UnityEngine;

namespace PepijnWillekens.EasyWallColliderUnity {
    public static class PolygonColliderEditorExtention {
        public static void DrawIcon(GameObject gameObject, int idx) {
            #if UNITY_EDITOR
            var largeIcons = GetTextures("sv_label_", string.Empty, 0, 8);
            var icon = largeIcons[idx];
            var egu = typeof(EditorGUIUtility);
            var flags = BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.NonPublic;
            var args = new object[] {gameObject, icon.image};
            var setIcon = egu.GetMethod("SetIconForObject", flags, null,
                new Type[] {typeof(UnityEngine.Object), typeof(Texture2D)}, null);
            setIcon.Invoke(null, args);
            #endif
        }

        #if UNITY_EDITOR
        private static GUIContent[] GetTextures(string baseName, string postFix, int startIndex, int count) {
            GUIContent[] array = new GUIContent[count];
            for (int i = 0; i < count; i++) {
                array[i] = EditorGUIUtility.IconContent(baseName + (startIndex + i) + postFix);
            }

            return array;
        }
        #endif
    }
}