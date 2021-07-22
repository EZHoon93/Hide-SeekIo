using System.Collections;

using UnityEngine;
using UnityEngine.UI;

public class UI_ItemButton : MonoBehaviour
{
    Button _button;
    [SerializeField] InGameItemController _inGameItem;
    [SerializeField] Image _clickImage;
    [SerializeField] Image _itemImage;
    [SerializeField] Image _weaponUseImage; //수류탄같은 일회성 사용 아이템

    int touchCount;
    private void Awake()
    {
        _button = GetComponent<Button>();
    }
    
    public void AddItem(InGameItemController newItem)
    {
        _inGameItem = newItem;
        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(() => Click());
        _clickImage.enabled = false;
        _weaponUseImage.enabled = false;
        _itemImage.sprite = newItem._item_Base.ItemSprite;
    }

    public void RemoveItem()
    {
        _inGameItem = null;
        _clickImage.enabled = false;
        _weaponUseImage.enabled = false;
        
        touchCount = 0;
    }

    void Click()
    {
        if(touchCount >= 1)
        {
            
            if(_inGameItem._item_Base.useType ==  Item_Base.UseType.Item)
            {
                _inGameItem.Use(Managers.Game.myPlayer);
                _inGameItem = null;
            }
            //무기 아이템
            else
            {
                //이미 사용중이라면 => 취소로전환
                if (_weaponUseImage.enabled)
                {
                    Managers.Game.myPlayer.GetAttackBase().UsePermanent();   //기본무기로전환
                    _clickImage.enabled = false;
                    _weaponUseImage.enabled = false;
                }
                //사용중이아니라면 => 사용으로 전환
                else
                {
                    _inGameItem.Use(Managers.Game.myPlayer);
                    CancelInvoke("Click");
                    _clickImage.enabled = false;
                    _weaponUseImage.enabled = true;
                }
                
            }
        }
        else
        {
            _clickImage.enabled = true;
            touchCount = 1;
            CancelInvoke("Click");
            Invoke("ClickOff", 1.5f);
        }
    }

    //void RemoveWeaponType()
    //{
    //    //아이템 사용시 제거 이벤트
    //    _inGameItem.GetComponent<Item_Weapon>().RegisterRemove(
    //        () =>
    //        {
    //            _inGameItem.RemoveItem(Managers.Game.myPlayer);
    //            print("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@제거하라고");
    //        }
    //     ); ;
    //}
    void ClickOff()
    {
        _clickImage.enabled = false;
        touchCount = 0;
    }
   
}
