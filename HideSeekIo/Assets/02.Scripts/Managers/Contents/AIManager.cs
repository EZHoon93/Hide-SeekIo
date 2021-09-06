using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AIManager : GenricSingleton<AIManager>
{
    [SerializeField] SpawnPoint[] _initAIDestPoint;  //초기 AI 도망갈 위치

    public SpawnPoint[] InitPoint => _initAIDestPoint;

    public Dictionary<int, LivingEntity> AIDic { get; set; } = new Dictionary<int, LivingEntity>();
    public int tt;

    private void Start()
    {
        StartCoroutine(CheckAI());
        PhotonGameManager.Instacne.AddListenr(Define.GameState.Wait, AI_Reset);
    }

    public void RegisterAI(LivingEntity newObject)
    {
        if (AIDic.ContainsKey(newObject.ViewID())) return;
        AIDic.Add(newObject.ViewID(), newObject);
    }

    public void UnRegisterAI(int viewID)
    {
        AIDic.Remove(viewID);
    }

    //혹시나 서버차이로 ai  가 안지워졌을때 대비, 지속적체크

    IEnumerator CheckAI()
    {
        //while (true)
        //{
        //    if(PhotonGameManager.Instacne.State == Define.GameState.CountDown ||
        //        PhotonGameManager.Instacne.State == Define.GameState.Wait)
        //    {
        //        AI_Reset();
        //    }
        //    yield return new WaitForSeconds(2.0f);

        //}
            yield return new WaitForSeconds(2.0f);

    }

    void AI_Reset()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            List<LivingEntity> removeList = new List<LivingEntity>();
            foreach(var ai in AIDic)
            {
                removeList.Add(ai.Value);
            }
            foreach(var r in removeList)
            {
                PhotonNetwork.Destroy(r.gameObject);
            }
            AIDic.Clear();

        }

        
    }
}
