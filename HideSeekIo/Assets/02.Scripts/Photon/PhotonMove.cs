
using UnityEngine;
using Photon.Pun;

public class PhotonMove : MonoBehaviourPun  , IPunObservable
{
    public enum DataState
    {
        SerializeView,
        ServerView
    }
    public DataState dataState;
    protected virtual Transform target { get; set; }
    protected float rotationSpeed = 8;
    private Vector3 networkPosition = Vector3.zero; //We lerp towards this
    private Vector3 oldPosition;
    private Vector3 movement;
    Vector3 n_direction;
    float m_distance;
    bool m_firstTake;
    public float SmoothingDelay = 5;
    protected virtual void OnEnable()
    {
        m_firstTake = true;
        dataState = DataState.SerializeView;
    }

    protected virtual void Update()
    {
        if (this.photonView.IsMine == false)
        {
            //transform.position = Vector3.Lerp(transform.position, networkPosition, Time.deltaTime * this.SmoothingDelay);
            transform.position = Vector3.Lerp(transform.position, networkPosition, m_distance * (2.0f / PhotonNetwork.SerializationRate));
            UpdateSmoothRotate(n_direction);
            UpdateRemote();
        }
        else
        {
      
            oldPosition = this.transform.position;
            UpdateLocal();
            networkPosition = this.transform.position;
            movement = networkPosition - oldPosition;
        }
    }
  
    public virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            WriteData(stream, info);
        }
        else
        {
            ReadData(stream, info);
        }
    }

    protected virtual void WriteData(PhotonStream stream, PhotonMessageInfo info)
    {
        stream.SendNext(n_direction);
        stream.SendNext(transform.position);
        stream.SendNext(movement);
    }

    protected virtual void ReadData(PhotonStream stream, PhotonMessageInfo info)
    {
        n_direction = (Vector3)stream.ReceiveNext();
        networkPosition = (Vector3)stream.ReceiveNext();
        movement = (Vector3)stream.ReceiveNext();
        if (m_firstTake)
        {
            this.transform.position = networkPosition;
            m_firstTake = false;
        }
        else
        {
            float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
            networkPosition += movement * lag;
            m_distance = Vector3.Distance(this.transform.position, networkPosition);
        }
    }

    protected virtual void UpdateLocal()
    {

    }

    protected virtual void UpdateRemote()
    {
       
    }
 
    protected void UpdateSmoothRotate(Vector3 direction)
    {
        if (direction.normalized.sqrMagnitude == 0)
        {
            n_direction = this.target.forward;
            return;
        }
        n_direction = direction;
        Quaternion quaternion = Quaternion.Euler(0, 0, 0);
        var newDirection = quaternion * direction;
        Quaternion newRotation = Quaternion.LookRotation(newDirection);
        this.target.rotation = Quaternion.Slerp(this.target.rotation, newRotation, rotationSpeed * Time.deltaTime);
    }

    protected void UpdateImmediateRotate(Vector3 inputVector3)
    {
        inputVector3.y = target.transform.position.y;
        this.target.LookAt(inputVector3);
        n_direction = this.target.forward;

    }




}
