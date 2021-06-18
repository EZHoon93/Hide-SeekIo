using System;
using System.Collections;
using System.Collections.Generic;
[Serializable]
public class UserData2: SaveGame
{
    
    public int hintKey;
    public Dictionary<Enum, Dictionary<int,int>> scoreDataDic;
    
    public UserData2()
    {
        hintKey = 0;
        scoreDataDic = new Dictionary<Enum, Dictionary<int, int>>();
    }
  
}