using System.Collections;
using System.Collections.Generic;

using ExitGames.Client.Photon;

using UnityEngine;

namespace Data
{
    public class PhotonCustomData
    {
        public static readonly byte[] memVector2 = new byte[2 * 4];

        //public static short SerializeVector2(StreamBuffer outStream, object customobject)
        //{
        //    SendAllSkinInfo vo = (SendAllSkinInfo)customobject;
        //    lock (memVector2)
        //    {
        //        byte[] bytes = memVector2;
        //        int index = 0;
        //        Protocol.Serialize(vo.chacterTypeNum, bytes, ref index);
        //        Protocol.Serialize(vo.avaterSkinID);
        //        outStream.Write(bytes, 0, 2 * 4);
        //    }

        //    return 2 * 4;
        //}

        //private static object DeserializeVector2(StreamBuffer inStream, short length)
        //{
        //    Vector2 vo = new Vector2();
        //    lock (memVector2)
        //    {
        //        inStream.Read(memVector2, 0, 2 * 4);
        //        int index = 0;
        //        Protocol.Deserialize(out vo.x, memVector2, ref index);
        //        Protocol.Deserialize(out vo.y, memVector2, ref index);
        //    }

        //    return vo;
        //}
    }

    //public static short SerializeVector2(StreamBuffer outStream, object customobject)
    //{
    //    Vector2 vo = (Vector2)customobject;
    //    //lock (memVector2)
    //    //{
    //    //    byte[] bytes = memVector2;
    //    //    int index = 0;
    //    //    Protocol.Serialize(vo.x, bytes, ref index);
    //    //    Protocol.Serialize(vo.y, bytes, ref index);
    //    //    outStream.Write(bytes, 0, 2 * 4);
    //    //}

    //    return 2 * 4;
    //}
    //public static object DeserializeVector2(StreamBuffer inStream, short length)
    //{
    //    Vector2 vo = new Vector2();
    //    lock (memVector2)
    //    {
    //        inStream.Read(memVector2, 0, 2 * 4);
    //        int index = 0;
    //        Protocol.Deserialize(out vo.x, memVector2, ref index);
    //        Protocol.Deserialize(out vo.y, memVector2, ref index);
    //    }

    //    return vo;
    //}

}