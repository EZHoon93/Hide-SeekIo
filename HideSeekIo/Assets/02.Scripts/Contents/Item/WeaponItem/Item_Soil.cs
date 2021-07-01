
using Photon.Pun;

public class Item_Soil : Item_Weapon
{

    public override void Use(PlayerController usePlayer)
    {
        if (_weapon_Throw == null)
        {
            _weapon_Throw = Managers.Spawn.WeaponSpawn(Define.Weapon.Soil, usePlayer.GetComponent<AttackBase>()).GetComponent<Weapon_Throw>();

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