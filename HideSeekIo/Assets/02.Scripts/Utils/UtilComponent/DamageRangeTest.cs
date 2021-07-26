using System.Collections;

using UnityEngine;

public class DamageRangeTest : MonoBehaviour
{
    public LayerMask attackLayer;
    public float distance;
    public float damage = 1;
    public int viewID = 0;
    [Range(0,360)]
    public float angle;

    private void OnEnable()
    {
        UtillGame.DamageInRange(this.transform, distance, 1, viewID, attackLayer, angle);

    }
}
