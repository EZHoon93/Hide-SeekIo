using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class UI_ItemButton : MonoBehaviour
{
    //Button _button;
    [SerializeField] InGameItemController _inGameItem;
    [SerializeField] Image _joystick;
    [SerializeField] Image _itemImage;
    //[SerializeField] Image _weaponUseImage; //수류탄같은 일회성 사용 아이템

    //public Image weaponUseImage => _weaponUseImage;
    public UltimateJoystick _ultimateJoystick;
    private void Awake()
    {
        //_button = GetComponent<Button>();
        _ultimateJoystick = GetComponent<UltimateJoystick>();
        _ultimateJoystick.OnTapCount += Click;
    }

  

    public void AddItem(InGameItemController newItem)
    {
        _joystick.enabled = newItem._item_Base.useType == Item_Base.UseType.Item ? false : true;
        _inGameItem = newItem;
        _itemImage.sprite = newItem._item_Base.ItemSprite;
        _itemImage.enabled = true;
        //_ultimateJoystick.OnDragCallback
    }

    //즉시아이템 사용
    void Click()
    {
        print("Click");
        var myPlayer = Managers.Game.myPlayer;
        if (myPlayer == null || _inGameItem ==  null) return;
        if(_inGameItem._item_Base.useType == Item_Base.UseType.Item)
        {
            _inGameItem.Use(myPlayer);
        }

    }

  
   
}
