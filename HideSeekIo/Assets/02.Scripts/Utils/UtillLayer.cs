using System.Collections;

using UnityEngine;

public class UtillLayer
{
    public static int seekerToHiderAttack = (1 << (int)Define.Layer.Hider) | (1 << (int)Define.Layer.SeekerItem);
    public static int hiderToSeekerAttack = (1 << (int)Define.Layer.Seeker) | (1 << (int)Define.Layer.HiderItem);


    public static int GetLayerByTeam(Define.Team team)
    {
        int layer = team == Define.Team.Hide ? (int)Define.Layer.Hider : (int)Define.Layer.Seeker;

        return layer;
    }


    public static int GetUILayerByTeam(Define.Team team)
    {
        int layer = team == Define.Team.Hide ? (int)Define.Layer.UIHider : (int)Define.Layer.UISeeker;

        return layer;
    }
}
