using UnityEngine;



[CreateAssetMenu(fileName = "Ch", menuName = "EZ/Create CharacterScriptable", order = 0)]
public class ProductScritable : ScriptableObject
{

    [SerializeField] public GameObject prefab;
    [SerializeField] public Sprite sprite;

    

}
