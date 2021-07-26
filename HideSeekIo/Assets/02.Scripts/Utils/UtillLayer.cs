using System.Collections;

using UnityEngine;

public class UtillLayer
{
    public static int seekerToHiderAttack = (1 << (int)Define.Layer.Hider) | (1 << (int)Define.Layer.SeekerItem);
    public static int hiderToSeekerAttack = (1 << (int)Define.Layer.Seeker) | (1 << (int)Define.Layer.HiderItem);

}
