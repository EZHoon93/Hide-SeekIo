using Photon.Pun;

public abstract class GameState_Base : MonoBehaviourPun 
{
    protected UI_Main uI_Main;
    protected bool isNextScene;
 
    protected virtual void OnEnable()
    {
        uI_Main = Managers.UI.SceneUI as UI_Main;
    }

    public abstract float remainTime { get; }
    public abstract void OnPhotonInstantiate(PhotonMessageInfo info, float createServerTime);
    public abstract void OnUpdate(int remainTime);
    public abstract void OnTimeEnd();

    public virtual void OnDestroy()
    {

    }


    public void NextScene(Define.GameState gameState , object whoCanWin = null)
    {
        if (isNextScene) return;
        isNextScene = true;
        Managers.Spawn.GameStateSpawn(gameState, whoCanWin);

    }
}
