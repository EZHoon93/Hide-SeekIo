using System.Collections;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
public class UI_Chatting : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IPointerUpHandler, IPointerDownHandler
{
    [SerializeField] TextMeshProUGUI _chattingText;
    [SerializeField] Scrollbar _scrollbar;
    [SerializeField] TMP_InputField _inputField;
    [SerializeField] ScrollRect _scrollRect;
    [SerializeField] Button _sendButton;


    bool isTouch;

    //int verticalSizeIndex = 1;

    private void Awake()
    {
        _sendButton.onClick.AddListener(() => SendChattingMessage());
        PhotonGameManager.Instacne.reciveChattingEvent += UpdateChatting;

    }
    private void Start()
    {
        print("이벤트추가");
        Invoke("ChangeBarSize", 1.0f);
    }
    void ChangeBarSize()
    {
        _scrollbar.value = 0f;
        _scrollbar.size = 0.1f;
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        isTouch = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isTouch = false;
    }
 
    public void UpdateChatting(Define.ChattingColor chattingColor,  string _text)
    {
        Color textColor = UISetting.Instance.ChattingColorDic[chattingColor];
        _chattingText.text += Util.GetColorContent(textColor, _text) + "\n";
        print("ㅋㅋ");

        if (isTouch == false) //채팅 스크롤 터지웅이 아니라면
        {
            _scrollbar.value = 0;
        }
        _scrollbar.value = 0f;
    }

    public void EChangeValue()
    {
        if (isTouch) //터치중이라면 ..
        {
            print("Change Value 1111");
        }
        else
        {
            print("Change Value 00");
            _scrollbar.value = 0;

            //scrollbar.value = 0;
        }
    }

    public void SendChattingMessage()
    {
        var content = _inputField.text;
        _inputField.text = null;
        if (content.Length > 0)
        {
            PhotonGameManager.Instacne.photonView.RPC("SendChattingMessageOnServer", Photon.Pun.RpcTarget.All, Define.ChattingColor.Message, content);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        print("드래그 클릭");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        print("드래그시작");
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        print("드래그 업");

    }

    public void OnValueChanged()
    {
        _scrollbar.size = 0.1f;
    }

    public void ClickChattingButton()
    {
        //_chattingPanel.gameObject.SetActive(!_chattingPanel.activeSelf);
    }

    public void ClearChatting()
    {
        _chattingText.text = null;
    }
}