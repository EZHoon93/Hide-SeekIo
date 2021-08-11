using System.Collections;

using UnityEngine;

public class UI_Fixing : MonoBehaviour
{
    private void LateUpdate()
    {
        var angle = this.transform.rotation.eulerAngles;

        //this.transform.rotation = Quaternion.Euler(0, angle.y, 0);
        //this.transform.rotation = Quaternion.Euler(90, 0, 0);
        this.transform.rotation = Quaternion.Euler(angle - this.transform.eulerAngles );
    }

    private void Update()
    {
        var angle = this.transform.rotation.eulerAngles;

        //this.transform.rotation = Quaternion.Euler(0, angle.y, 0);
        //this.transform.rotation = Quaternion.Euler(90, 0, 0);
        this.transform.rotation = Quaternion.Euler(angle - this.transform.eulerAngles);

    }
}
