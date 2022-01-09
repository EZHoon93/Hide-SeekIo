/*
Copyright (c) 2020 Omar Duarte
Unauthorized copying of this file, via any medium is strictly prohibited.
Writen by Omar Duarte, 2020.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PluginMaster
{
    public static class DropUtils
    {
        public struct DroppedItem
        {
            public GameObject obj;
            public string subfolder;
            public DroppedItem(GameObject obj, string subfolder = null) => (this.obj, this.subfolder) = (obj, subfolder);
        }

        public static DroppedItem[] GetDirPrefabs(string dirPath)
        {
            var filePaths = System.IO.Directory.GetFiles(dirPath, "*.prefab");
            var subItemList = new List<DroppedItem>();
            var dirName = dirPath.Substring(Mathf.Max(dirPath.LastIndexOf('/'), dirPath.LastIndexOf('\\')) + 1);
            foreach (var filePath in filePaths)
            {
                DroppedItem item;
                item.obj = AssetDatabase.LoadAssetAtPath<GameObject>(filePath);
                var prefabType = PrefabUtility.GetPrefabAssetType(item.obj);
                if (prefabType != PrefabAssetType.Regular && prefabType != PrefabAssetType.Variant) continue;
                item.subfolder = dirName;
                subItemList.Add(item);
            }
            if (PaletteManager.selectedPalette.brushCreationSettings.includeSubfolders)
            {
                var subdirPaths = System.IO.Directory.GetDirectories(dirPath);
                foreach (var subdirPath in subdirPaths) subItemList.AddRange(GetDirPrefabs(subdirPath));
            }
            return subItemList.ToArray();
        }

        public static DroppedItem[] GetDroppedPrefabs()
        {
            var itemList = new List<DroppedItem>();
            for (int i = 0; i < DragAndDrop.objectReferences.Length; ++i)
            {
                var objRef = DragAndDrop.objectReferences[i];
                if (objRef is GameObject)
                {
                    if (objRef == null) continue;
                    if (PrefabUtility.GetPrefabAssetType(objRef) == PrefabAssetType.NotAPrefab) continue;
                    var path = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(objRef);
                    if (path == string.Empty) continue;
                    var prefab = objRef as GameObject;
                    var prefabInstance = PrefabUtility.GetNearestPrefabInstanceRoot(objRef) as GameObject;
                    if (prefabInstance != null)
                    {
                        var assetType = PrefabUtility.GetPrefabAssetType(prefabInstance);
                        if (assetType == PrefabAssetType.NotAPrefab|| assetType == PrefabAssetType.NotAPrefab) continue;
                        if (assetType == PrefabAssetType.Variant) prefab = prefabInstance;
                        else prefab = PrefabUtility.GetCorrespondingObjectFromSource(prefabInstance);
                    }
                    itemList.Add(new DroppedItem(prefab, path));
                }
                else
                {
                    var path = DragAndDrop.paths[i];
                    if (objRef is DefaultAsset && AssetDatabase.IsValidFolder(path)) itemList.AddRange(GetDirPrefabs(path));
                }
            }
            return itemList.ToArray();
        }

        public static DroppedItem[] GetFolderItems()
        {
            DroppedItem[] items = null;
            var folder = EditorUtility.OpenFolderPanel("Add Prefabs in folder:", Application.dataPath, "Assets");
            if (folder.Contains(Application.dataPath))
            {
                folder = folder.Replace(Application.dataPath, "Assets");
                items = GetDirPrefabs(folder);
                if (items.Length == 0) EditorUtility.DisplayDialog("No Prefabs found", "No prefabs found in folder", "Ok");
            }
            else if (folder != string.Empty) EditorUtility.DisplayDialog("Folder Error", "Folder must be under Assets folder", "Ok");
            return items;
        }
    }
}
