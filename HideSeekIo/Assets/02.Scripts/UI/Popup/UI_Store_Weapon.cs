using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI_Store_Weapon : UI_Popup
{
    [SerializeField] UI_Product_Weapon uI_ProductPrefab;   //상품프리팹
    [SerializeField] Transform _content;

    enum Buttons
    {
        Cancel,
    }

    private void Reset()
    {
        _content = this.transform.MyFindChild("content");
    }

    public override void Init()
    {
        base.Init();
        Bind<Button>(typeof(Buttons));

        GetButton((int)Buttons.Cancel).gameObject.BindEvent(Cancel);

    }
    private void Start()
    {
        uI_ProductPrefab = Managers.Resource.Load<UI_Product_Weapon>("Prefabs/UI/SubItem/UI_Product_Weapon");
        var sprites = Resources.LoadAll<Sprite>("Sprites/Weapon");
        foreach (var s in sprites)
        {
            var go = Instantiate(uI_ProductPrefab, _content);
            go.Setup(s.name, s);
        }
    }

    void Cancel(PointerEventData pointerEventData)
    {
        Managers.UI.ClosePopupUI();
    }

}
