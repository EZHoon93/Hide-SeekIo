using System.Collections;
using System.Collections.Generic;

using UnityEditor;

using UnityEngine;

enum TileType
{
    Empty = 0 ,
    Wall,
    ChangeObject
}
public class MakMaker : MonoBehaviour
{
    public GameObject wallObject;
    public int xSize;
    public int ySize;
    [Range(0, 10)]
    public int maxWallSize;
    [Range(0,10)]
    public int minWallSize;

    [Range(0, 1)]
    public float wallPercet;    //벽만들확률

    TileType[,] _tiles;

    public int tx, ty;

    public List<GameObject> gameObjects = new List<GameObject>();
    
    [ContextMenu("Make")]
    public void Make()
    {
        Init();

        for (int y = 1; y < ySize -1; y++)
        {
            for (int x = 1; x < xSize -1; x++)
            {
             
                //벽이있는 칸이면 넘어감.
                if (_tiles[x, y] == TileType.Wall)
                {
                    continue;
                }
                //벽을 만들수없으면 넘어감.. (자신을 제외한 주변이 다막히면)
                if (CanNotWall(x, y) )
                {
                    continue;
                }

                //길을 만들수있는 칸이라면.. => array[x,y] == 0 이면
                var sucessWall = Random.Range(0.0f, 1.0f);
                if (sucessWall <= wallPercet)
                {
                    MakeWall(x, y);
                }

            }
        }
    }
    [ContextMenu("Clear")]

    public void Clear()
    {
        foreach (var g in gameObjects.ToArray())
        {
            DestroyImmediate(g.gameObject);
        }
        gameObjects.Clear();
    }

    void MakeWall(int x, int y)
    {
        _tiles[x, y] = TileType.Wall;

        var newObject = Instantiate(wallObject, new Vector3(x, 1, y), Quaternion.identity);
        gameObjects.Add(newObject);
        newObject.transform.SetParent(this.transform);
    }

    bool CanNotWall(int currX, int currY)
    {
        int wallCount = 0;

        if( _tiles[currX -1 , currY] == TileType.Wall)
        {
            wallCount++;
        }
        //if (_tiles[currX + 1, currY] == TileType.Wall)
        //{
        //    wallCount++;
        //}
        if (_tiles[currX  ,currY - 1] == TileType.Wall)
        {
            wallCount++;
        }
        if(currX == 1 && currY == 1)
        {
            print(wallCount  +"/"+  _tiles[currX-1, currY] +"/" + _tiles[currX , currY - 1]);
        }
        //if (_tiles[currX , currY+1] == TileType.Wall)
        //{
        //    wallCount++;
        //}


        return wallCount == 2;
    }
    void Init()
    {
      
        _tiles = new TileType[xSize, ySize] ;

        for (int y = 0; y < ySize; y++)
        {
            for (int x = 0; x < xSize; x++)
            {
                if(x == 0 || x == xSize - 1  || y == 0 || y== ySize - 1)
                {
                    MakeWall(x, y);
                }
                else
                {
                    _tiles[x, y] = TileType.Empty;
                }
            }
        }
    }
}
