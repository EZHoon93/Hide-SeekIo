using System.Collections;

using UnityEngine;

public class ScriptableController : MonoBehaviour
{
    public string path;
    

    [ContextMenu("Setup")]
    public void Setup()
    {
        var dataList = Resources.LoadAll<ProductScritable>("Data/"+path);
        print(dataList.Length);
        foreach(var d in dataList)
        {
            string prefabPath = $"Prefabs/{path}/{d.name}";
            string spritePath = $"Sprites/{path}/{d.name}";

            var prefab = Resources.Load<GameObject>(prefabPath);
            var sprite = Resources.Load<Sprite>(spritePath);

            d.prefab = prefab;
            d.sprite = sprite;

            print(prefabPath + "\n" + spritePath  + " / " + sprite.name);
        }

    }
}
