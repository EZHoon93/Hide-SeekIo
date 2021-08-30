using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class PhotonMove : MonoBehaviourPun, IPunObservable
{
    Vector3 Pos;
    Vector3 MovedPos;
    Vector3 CPos;
    float lag;
   

    // Update is called once per frame
    protected virtual void Update()
    {

        Vector3 TmpPos = transform.position;
        if (!photonView.IsMine)
        {
            transform.position = Vector3.Lerp(transform.position, CPos + MovedPos * lag, Time.deltaTime * 10/*알아서 조정*/);
            return;
        }

        // 이동값 알아서 조정
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(-0.1f, 0, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(0.1f, 0, 0);
        }
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(0f, 0, 0.1f);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(0f, 0, -0.1f);
        }
        Pos = transform.position;
        MovedPos = Pos - TmpPos;

    }




    public virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(Pos);
            stream.Serialize(ref MovedPos);
        }
        else
        {
            stream.Serialize(ref CPos);
            stream.Serialize(ref MovedPos);


            lag = Mathf.Abs((float)(PhotonNetwork.Time - info.timestamp)) * 100/*알아서 조정*/;


        }

    }
}
