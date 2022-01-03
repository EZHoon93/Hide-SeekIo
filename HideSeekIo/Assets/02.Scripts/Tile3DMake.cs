using System.Collections;
using System.Collections.Generic;

using Fabgrid;

using UnityEngine;

public class Tile3DMake : MonoBehaviour
{
    public Tilemap3D tilemap3D;
    public Tilemap3DEditor Tilemap3DEditor;
    public AddTile addTile;
    public string[] path;

    [ContextMenu("Setup")]
    public void Setup()
    {
        //tilemap3D.prefabsPath = path;
        //addTile.MyAddTile();
        print(tilemap3D.tileSet+"/" +tilemap3D.selectedTile);

        //foreach(var t in tilemap3D.tiles)
        //{
        //    print(t.)
        //}
    }
}
