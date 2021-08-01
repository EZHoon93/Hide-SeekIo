using System.Collections;
using UnityEngine;
using Photon.Pun;
using System;
using System.Linq;
using FoW;

public abstract class PlayerController : MonoBehaviourPun
{
    public string NickName { get;  set; }
    public Define.Team Team => GetLivingEntity().Team;
    public abstract LivingEntity GetLivingEntity();
    public abstract AttackBase GetAttackBase();
    public abstract InputBase GetInputBase();
    
    public InGameItemController[] itemInventory { get; protected set; } = new InGameItemController[3];


    public virtual void OnPhotonInstantiate(PhotonView photonView)
    {
        if (this.IsMyCharacter())    //?????? ??????
        {
            var mainSceneUI = Managers.UI.SceneUI as UI_Main;
            //mainSceneUI.InGameStore.Setup(Team);
        }
        for(int i =0; i < itemInventory.Length; i++)
        {
            itemInventory[i] = null;
        }

        //StartCoroutine(UpdateFog());??

    }

    private void Awake()
    {
        
    }

    protected virtual void HandleDeath()
    {
    }

    IEnumerator UpdateFog()
    {
        GetComponentInChildren<FogOfWarTeam>().team = this.ViewID();
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            FoW.FogOfWarTeam.GetTeam(this.ViewID()).ManualUpdate(1);
        }
        
    }

    ////?????? ?????? ???? ?????? ???? ????
    //protected virtual void FixedUpdate()
    //{
    //    n_sync = false;
    //}

    ////???????? ???????? true => 
    //[PunRPC]
    //public void CallSync()
    //{
    //    n_sync = true;
    //}





}

