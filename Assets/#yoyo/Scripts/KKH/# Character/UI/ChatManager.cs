using Photon.Pun;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Suncheon.Player;
using UnityEngine.Events;
using Suncheon.WebData;

namespace Suncheon.UI
{
    public enum ChatMode
    {
        Normal = 0,
        Whispering,
        System
    }

    public class ReceiveMsgJson
    {
        public string type;
        public string cont;
    }

    public class ReceiveMsg
    {
        public string msg;
        public string nickname;
        public string type;
    }

    public class ChatManager : MonoBehaviour
    {
        private static ChatManager instance;
        public static ChatManager Instance => instance;

        [Header("Button")]
        [SerializeField] private Button btn_Normal;
        [SerializeField] private Button btn_Whispering;
        [SerializeField] private Button btn_System;
        [SerializeField] private Button btn_ChatMode;
        [SerializeField] private Button btn_Enter;
        [SerializeField] private TMP_Text text_ChatMode;

        [Space(10)]

        [Header("Chat")]
        [SerializeField] private TMP_InputField input_Chat;
        [SerializeField] private ScrollRect scrollRect_ChatBox;
        [SerializeField] private GameObject conenet_ChatBox;

        [SerializeField] private GameObject messageNormalObject;
        [SerializeField] private GameObject messageWhisperingObject;
        [SerializeField] private GameObject messageSystemObject;

        [SerializeField] private List<GameObject> _messageNormalObjPool = new List<GameObject>();
        [SerializeField] private List<GameObject> _messageWhisperingObjPool = new List<GameObject>();
        [SerializeField] private List<GameObject> _messageSystemObjPool = new List<GameObject>();

        [SerializeField] private string strReceiveType;
        [SerializeField] private string strReceiveMsg;

        public Image image_Normal_Notice;
        public Image image_Whisper_Notice;
        public Image image_System_Notice;

        [SerializeField] private Scrollbar chatbox_Scrollbar;

        [Space(10)]
        [Header("Photon")]
        [SerializeField] private PhotonView pv;

        PlayerChatManager playerChatManager;

        List<Dictionary<string, object>> data;
        List<string> fiterData;

        private ChatMode chatBoxMode = ChatMode.Normal;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }

            input_Chat.onEndEdit.AddListener((string str) => { EnterKeyOnTextField(); });

            btn_Normal.onClick.AddListener(() => BtnNormalClick());
            btn_Whispering.onClick.AddListener(() => BtnWhisperingClick());
            btn_System.onClick.AddListener(() => BtnSystemClick());
            btn_ChatMode.onClick.AddListener(() => BtnChatModeChange());
            btn_Enter.onClick.AddListener(() => Btn_Enter());

            if (!image_Normal_Notice) { UTILS.Log($"{PhotonNetwork.NickName} : image_Normal_Notice is null"); }
            if (!image_Whisper_Notice) { UTILS.Log($"{PhotonNetwork.NickName} : image_Whisper_Notice is null"); }
            if (!image_System_Notice) { UTILS.Log($"{PhotonNetwork.NickName} : image_System_Notice is null"); }
        }

        private void Start()
        {
            data = new List<Dictionary<string, object>>();
            fiterData = new List<string>();

            TextAsset textasset = Resources.Load<TextAsset>("Filter");
            string filterText = textasset.text;
            data = CSVReader.Read(filterText);

            for (int i = 0; i < data.Count; i++)
            {
                fiterData.Add(data[i]["Word"].ToString());
            }

            // 처음은 일반 채팅 모드로
            BtnNormalClick();
            
        }


        /// <summary>
        /// 메시지 스크롤뷰 오브젝트들 끄기
        /// </summary>
        private void ContentMessageClear()
        {
            TMP_Text[] texts = conenet_ChatBox.GetComponentsInChildren<TMP_Text>();
            foreach (TMP_Text text in texts)
            {
                text.gameObject.SetActive(false);
            }
        }

        #region 채팅 박스 모드 선택
        /// <summary>
        /// 일반 버튼 클릭
        /// </summary>
        private void BtnNormalClick()
        {
            ContentMessageClear();

            btn_Normal.GetComponent<ButtonControl>().OnChangeSprite(true);
            btn_Whispering.GetComponent<ButtonControl>().OnChangeSprite(false);
            btn_System.GetComponent<ButtonControl>().OnChangeSprite(false);

            foreach (var obj in _messageNormalObjPool)
            {
                obj.SetActive(true);
            }

            Color color = image_Normal_Notice.color;
            color.a = 0;
            image_Normal_Notice.color = color;

            chatBoxMode = ChatMode.Normal;
        }

        /// <summary>
        /// 귓속말 버튼 클릭
        /// </summary>
        private void BtnWhisperingClick()
        {
            ContentMessageClear();

            btn_Normal.GetComponent<ButtonControl>().OnChangeSprite(false);
            btn_Whispering.GetComponent<ButtonControl>().OnChangeSprite(true);
            btn_System.GetComponent<ButtonControl>().OnChangeSprite(false);

            foreach (var obj in _messageWhisperingObjPool)
            {
                obj.SetActive(true);
            }

            Color color = image_Whisper_Notice.color;
            color.a = 0;
            image_Whisper_Notice.color = color;

            chatBoxMode = ChatMode.Whispering;
        }

        /// <summary>
        /// 시스템 버튼 클릭
        /// </summary>
        private void BtnSystemClick()
        {
            ContentMessageClear();

            btn_Normal.GetComponent<ButtonControl>().OnChangeSprite(false);
            btn_Whispering.GetComponent<ButtonControl>().OnChangeSprite(false);
            btn_System.GetComponent<ButtonControl>().OnChangeSprite(true);

            foreach (var obj in _messageSystemObjPool)
            {
                obj.SetActive(true);
            }

            Color color = image_System_Notice.color;
            color.a = 0;
            image_System_Notice.color = color;

            chatBoxMode = ChatMode.System;
        }

        /// <summary>
        /// 귀속말모드 -> 일반모드 변경
        /// </summary>
        private void BtnChatModeChange()
        {
            UIInteractionManager.Instance.chatMode = ChatMode.Normal;
            UIInteractionManager.Instance.SendWhisperNickName = string.Empty;
            text_ChatMode.text = "모두에게";
        }
        #endregion

        #region 메시지 송수신
        /// <summary>
        /// Eenter입력시 메시지 송신
        /// </summary>
        private void EnterKeyOnTextField()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                Send_Chat();
            }
        }
        /// <summary>
        /// 엔터버튼 사용시
        /// </summary>
        void Btn_Enter()
        {
            Send_Chat();
        }

        /// <summary>
        /// 채팅 전송 기능
        /// </summary>
        void Send_Chat()
        {
            if (pv == null)
            {
                pv = NetworkManager.Instance.Go_Player.GetPhotonView();
            }

            if (pv.IsMine)
            {
                string msg = input_Chat.text.Trim();
                if (string.IsNullOrEmpty(msg) || msg == "\n" || msg == "\r")
                {
                    input_Chat.Select();
                    return;
                }
                SendMessage();

                input_Chat.ActivateInputField();
            }
        }

        /// <summary>
        /// 메시지 송신
        /// </summary>
        private void SendMessage()
        {
            if (string.IsNullOrEmpty(input_Chat.text)) return;

            if (playerChatManager == null)
            {
                playerChatManager = NetworkManager.Instance.Go_Player.GetComponent<PlayerChatManager>();
            }
            ReceivedMessage();

            //#if UNITY_EDITOR
            //#elif !UNITY_EDITOR && UNITY_WEBGL
            //            //playerChatManager.ChatSend(input_Chat.text.Trim());

            //            if (playerChatManager.isChatServerConnect)
            //            {
            //                if (UIInteractionManager.Instance.chatMode == ChatMode.Normal)
            //                {
            //                    Application.ExternalCall("chatSend", input_Chat.text);
            //                }
            //                else if (UIInteractionManager.Instance.chatMode == ChatMode.Whispering)
            //                {
            //                    InstertWhisperMessage(input_Chat.text);
            //                    Application.ExternalCall("whisper", input_Chat.text, UIInteractionManager.Instance.SendWhisperNickName);
            //                }
            //            }
            //            else
            //            {
            //                ReceivedMessage();
            //            }
            //#elif !UNITY_EDITOR && UNITY_ANDROID
            //            ReceivedMessage();
            //#endif
            input_Chat.text = string.Empty;
        }

        /// <summary>
        /// 에디터용 메시지 수신
        /// </summary>
        public void ReceivedMessage()
        {
            if (pv == null)
            {
                pv = NetworkManager.Instance.Go_Player.GetPhotonView();
            }

            string msg = input_Chat.text.Trim();

            foreach (string fiter in fiterData)
            {
                msg = msg.Replace(fiter, "**");
            }

            if (UIInteractionManager.Instance.chatMode == ChatMode.Normal)
            {
                pv.RPC("RPCShowTalkBox", RpcTarget.All, PhotonNetwork.NickName, msg);
                pv.RPC("RPCReceivedNormalMessage", RpcTarget.All, PhotonNetwork.NickName, msg);
            }
            else if (UIInteractionManager.Instance.chatMode == ChatMode.Whispering)
            {
                if (string.IsNullOrEmpty(UIInteractionManager.Instance.SendWhisperNickName)) return;
                //pv.RPC("RPCShowTalkBox", RpcTarget.All, PhotonNetwork.NickName, msg);
                pv.RPC("RPCReceivedWhisperingMessage", RpcTarget.All, PhotonNetwork.NickName, UIInteractionManager.Instance.SendWhisperNickName, msg);
            }
            else if (UIInteractionManager.Instance.chatMode == ChatMode.System)
            {
                pv.RPC("RPCReceivedSystemMessage", RpcTarget.All, msg);
            }
        }

        /// <summary>
        /// WebGL을 통한 메시지 수신
        /// </summary>
        /// <param name="msg"></param>
        public void WebGLReceivedMessage(string msg)
        {
            ReceiveMsgJson receiveMsgJson = null;
            try
            {
                receiveMsgJson = JsonUtility.FromJson<ReceiveMsgJson>(msg);
            }
            catch
            {
                receiveMsgJson = null;
            }

            if (receiveMsgJson == null) { return; }

            ReceiveMsg receiveMsg = null;

            try
            {
                receiveMsg = JsonUtility.FromJson<ReceiveMsg>(receiveMsgJson.cont);
            }
            catch
            {
                receiveMsg = null;
            }

            if (receiveMsg.type == "message")
            {
                pv.RPC("RPCShowTalkBox", RpcTarget.All, receiveMsg.nickname, receiveMsg.msg);
                pv.RPC("RPCReceivedNormalMessage", RpcTarget.All, PhotonNetwork.NickName, receiveMsg.msg);
                //ReceivedNormalMessage(receiveMsg.nickname, receiveMsg.msg);
            }
            else if (receiveMsg.type == "whisper")
            {
                //pv.RPC("RPCShowTalkBox", RpcTarget.All, receiveMsg.nickname, receiveMsg.msg);
                pv.RPC("RPCReceivedWhisperingMessage", RpcTarget.All, PhotonNetwork.NickName, UIInteractionManager.Instance.SendWhisperNickName, receiveMsg.msg);
                //ReceivedWhisperingMessage(receiveMsg.nickname, receiveMsg.msg);
            }
            else if (receiveMsg.type == "system")
            {
                //pv.RPC("RPCReceivedSystemMessage", RpcTarget.All, receiveMsg.msg);
                ReceivedSystemMessage(receiveMsg.msg);
            }
        }

        /// <summary>
        /// 일반 메시지 받기
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="msg"></param>
        public void ReceivedNormalMessage(string sender, string msg)
        {
            if (NetworkManager.Instance.Go_Player.GetPhotonView().IsMine)
            {
                bool isblock = false;
                foreach (Response_MyBlockListResultData list in NetworkManager.Instance.Go_Player.GetComponent<PlayerManager>().blockPlayerList.response_MyBlockListResults)
                {
                    if (sender == list.nickname)
                    {
                        isblock = true;
                        break;
                    }
                }
                if (isblock) { return; }

                if (_messageNormalObjPool.Count > 100)
                {
                    Destroy(_messageNormalObjPool[0]);
                }

                var newMessageObj = Instantiate(messageNormalObject, conenet_ChatBox.transform);
                _messageNormalObjPool.Add(newMessageObj);
                TMP_Text newMessageText = newMessageObj.GetComponent<TMP_Text>();

                newMessageText.alignment = TextAlignmentOptions.MidlineLeft;

                newMessageText.text = string.Format($"{sender} >> {msg}");

                if(chatBoxMode != ChatMode.Normal)
                {
                    newMessageObj.SetActive(false);
                }

                if (sender != PhotonNetwork.NickName)
                {
                    Color color = image_Normal_Notice.color;
                    color.a = 1;
                    image_Normal_Notice.color = color;
                }

                chatbox_Scrollbar.value = 0;
            }
        }

        /// <summary>
        /// 귀속말 메시지 받기
        /// </summary>
        /// <param name="receiver"></param>
        /// <param name="msg"></param>
        public void ReceivedWhisperingMessage(string sender, string receiver, string msg)
        {
            if (NetworkManager.Instance.Go_Player.GetPhotonView().IsMine)
            {
                bool isblock = false;
                foreach (Response_MyBlockListResultData list in NetworkManager.Instance.Go_Player.GetComponent<PlayerManager>().blockPlayerList.response_MyBlockListResults)
                {
                    if(sender == list.nickname)
                    {
                        isblock = true;
                        break;
                    }
                }
                if(isblock) { return; }

                if (sender == PhotonNetwork.NickName || receiver == PhotonNetwork.NickName)
                {
                    if (_messageWhisperingObjPool.Count > 100)
                    {
                        Destroy(_messageWhisperingObjPool[0]);
                    }

                    var newMessageObj = Instantiate(messageWhisperingObject, conenet_ChatBox.transform);
                    _messageWhisperingObjPool.Add(newMessageObj);
                    TMP_Text newMessageText = newMessageObj.GetComponent<TMP_Text>();

                    newMessageText.alignment = TextAlignmentOptions.MidlineLeft;

                    if (sender == PhotonNetwork.NickName)
                    {
                        newMessageText.text = string.Format($"{receiver} >> {msg}");
                    }
                    else
                    {
                        newMessageText.text = string.Format($"{receiver} << {msg}");
                    }

                    if (chatBoxMode != ChatMode.Whispering)
                    {
                        newMessageObj.SetActive(false);
                    }

                    if (receiver != PhotonNetwork.NickName)
                    {
                        Color color = image_Whisper_Notice.color;
                        color.a = 1;
                        image_Whisper_Notice.color = color;
                    }

                    chatbox_Scrollbar.value = 0;
                }
            }
        }

        /// <summary>
        /// 내가 보낸 귓속말 메시지 넣기
        /// </summary>
        /// <param name="msg"></param>
        private void InstertWhisperMessage(string msg)
        {
            if (NetworkManager.Instance.Go_Player.GetPhotonView().IsMine)
            {
                if (_messageWhisperingObjPool.Count > 100)
                {
                    Destroy(_messageWhisperingObjPool[0]);
                }

                var newMessageObj = Instantiate(messageWhisperingObject, conenet_ChatBox.transform);
                _messageWhisperingObjPool.Add(newMessageObj);
                TMP_Text newMessageText = newMessageObj.GetComponent<TMP_Text>();

                newMessageText.alignment = TextAlignmentOptions.MidlineLeft;

                newMessageText.text = string.Format($"{PhotonNetwork.NickName} >> {msg}");

                chatbox_Scrollbar.value = 0;
            }
        }

        /// <summary>
        /// 시스템 메시지 받기
        /// </summary>
        /// <param name="msg"></param>
        public void ReceivedSystemMessage(string msg)
        {
            UTILS.Log($"ReceivedSystemMessage");
            if (NetworkManager.Instance.Go_Player.GetPhotonView().IsMine)
            {
                UTILS.Log($"ReceivedSystemMessage : {msg}");
                if (_messageSystemObjPool.Count > 100)
                {
                    Destroy(_messageSystemObjPool[0]);
                }

                var newMessageObj = Instantiate(messageSystemObject, conenet_ChatBox.transform);
                _messageSystemObjPool.Add(newMessageObj);
                TMP_Text newMessageText = newMessageObj.GetComponent<TMP_Text>();

                newMessageText.alignment = TextAlignmentOptions.MidlineLeft;

                newMessageText.text = string.Format($"[공지] : {msg}");
                if (UIInteractionManager.Instance == null)
                {
                    UTILS.Log("UIInteractionManager is null");
                }
                else
                {
                    UIInteractionManager.Instance.ui_Announcement.GetComponent<UI_SystemPopUp>().ShowPopUp(msg);
                }

                if (chatBoxMode != ChatMode.System)
                {
                    newMessageObj.SetActive(false);
                }

                Color color = image_System_Notice.color;
                color.a = 1;
                image_System_Notice.color = color;

                chatbox_Scrollbar.value = 0;
            }
        }
        #endregion
    }

    public class NormalMessage
    {
        public string msg;
        public string user_id;
        public string nickname;
        public string type;
    }

    public class SystemMessage
    {
        public string msg;
        public string type;
    }
}