
using Photon.Pun;

public class Item_Grenade : Item_Weapon
{
    public override void Use(PlayerController usePlayer)
    {
        if(_weapon_Throw == null)
        {
            _weapon_Throw =  Managers.Spawn.WeaponSpawn(Define.Weapon.Grenade, usePlayer.GetComponent<AttackBase>()).GetComponent<Weapon_Grenade>();
            
            _weapon_Throw.AttackSucessEvent += () => PhotonNetwork.Destroy(this.gameObject);

        }

        _weapon_Throw.UseToPlayerToServer();
    }
    [PunRPC]
    public void UseUseOnOtherClinets(int useViewID)
    {
        var usePlayer = Managers.Game.GetLivingEntity(useViewID);
        if (usePlayer == null) return;

        
    }

}
