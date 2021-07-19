using System.Collections;

using UnityEngine;

public class UI_Store_Etc : UI_Popup
{
    private void Start()
    {
        PhotonGameManager.Instacne.gameJoin += Destroy;
    }

    void Destroy()
    {
        PhotonGameManager.Instacne.gameJoin -= Destroy;
        Managers.Resource.Destroy(this.gameObject);
    }
}
