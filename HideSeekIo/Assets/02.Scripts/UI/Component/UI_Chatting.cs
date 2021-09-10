using System.Collections;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;
public class UI_Chatting : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] TextMeshProUGUI _chattingText;
    [SerializeField] Scrollbar _scrollbar;
    [SerializeField] TMP_InputField _inputField;
    [SerializeField] ScrollRect _scrollRect;
    [SerializeField] Button _sendButton;
    [SerializeField] Button _openButton;
    [SerializeField] Button _closeButton;
    [SerializeField] Transform _panel;


    bool isTouch;

    //int verticalSizeIndex = 1;

    private void Awake()
    {
        //_sendButton.onClick.AddListener(() => SendChattingMessage());
        _openButton.onClick.AddListener(() => ChangeChattingActive(true));
        _closeButton.onClick.AddListener(() => ChangeChattingActive(false));
        _inputField.text = null;
        PhotonGameManager.Instacne.reciveChattingEvent += UpdateChatting;
        PhotonGameManager.Instacne.enterUserList += OnPlayerEnteredRoom;
        PhotonGameManager.Instacne.leftUserList += OnPlayerLeftRoom;


    }
    private void Start()
    {
        Invoke("ChangeBarSize", 1.0f);
        //UpdateChatting(Define.ChattingColor.System, content);

    }

    void ChangeChattingActive(bool active)
    {
        _panel.gameObject.SetActive(active);
        _closeButton.gameObject.SetActive(active);
        _openButton.gameObject.SetActive(!active);
        ChangeBarSize();
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
        if (isTouch == false) //채팅 스크롤 터지웅이 아니라면
        {
            _scrollbar.value = 0;
        }
        _scrollbar.value = 0f;
    }
    public void OnValueChanged()
    {
        _scrollbar.size = 0.1f;
    }
    public void SendChattingMessage(string content)
    {
        if (content.Length > 0)
        {
            PhotonGameManager.Instacne.photonView.RPC("SendChattingMessageOnServer", Photon.Pun.RpcTarget.All, Define.ChattingColor.Message, content);
            _inputField.text = null;
        }
    }
    public void UpdateInputField()
    {
        var content = _inputField.text;
        if (string.IsNullOrEmpty(content)) return;
        SendChattingMessage(content);
    }
    //public void OnPointerClick(PointerEventData eventData)
    //{
    //    print("드래그 클릭");
    //}

    //public void OnPointerDown(PointerEventData eventData)
    //{
    //    print("드래그시작");
    //}
    //public void OnPointerUp(PointerEventData eventData)
    //{
    //    print("드래그 업");
    //    ChangeBarSize();
    //}

    public  void OnPlayerEnteredRoom(Player newPlayer)
    {
        var content = newPlayer.NickName + "님이 참가 하였습니다.";
        UpdateChatting(Define.ChattingColor.System, content);
    }



    public  void OnPlayerLeftRoom(Player otherPlayer)
    {
        var content = otherPlayer.NickName + "님이 참가 하였습니다.";
        UpdateChatting(Define.ChattingColor.System, content);
    }

    public void ClickChattingButton()
    {
        //_chattingPanel.gameObject.SetActive(!_chattingPanel.activeSelf);
    }

 
}