using Newtonsoft.Json;
using Photon.Pun;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Suncheon.Player
{
    class ChatMessage
    {
        [JsonProperty("type")]
        public string type;
        [JsonProperty("img")]
        public string img;
        [JsonProperty("chatSessionId")]
        public string chatSessionId;
        [JsonProperty("userNick")]
        public string userNick;
        //[JsonProperty("sendUserNick")]
        //public string sendUserNick;
        [JsonProperty("sendMsg")]
        public string sendMsg;

        public ChatMessage(string _type, string _img, string _chatSessionId, string _userNick, string _sendMsg)
        {
            type = _type;
            img = _img;
            chatSessionId = _chatSessionId;
            userNick = _userNick;
            sendMsg = _sendMsg;
        }

        //public ChatMessage(string _type, string _img, string _chatSessionId, string _userNick, string _sendUserNick, string _sendMsg)
        //{
        //    type = _type;
        //    img = _img;
        //    chatSessionId = _chatSessionId;
        //    userNick = _userNick;
        //    sendUserNick = _sendUserNick;
        //    sendMsg = _sendMsg;
        //}
    }

    public class PlayerChatManager : MonoBehaviour
    {
        public PhotonView pv;

        private WebSocketSharp.WebSocket ws;

        private string chatServerName = string.Empty;
        public bool isChatServerConnect = false;

        //public void ChatInit()
        //{
        //    string chatServerURL = $"wss://suncheonlib.yoyointeractive.co.kr/suncheonlib/chat-ws.ct?roomNumber={chatServerName}";

        //    try
        //    {
        //        ws = new WebSocketSharp.WebSocket(chatServerURL);
        //        Debug.Log($"ws Init!!! : {chatServerURL}");
        //    }
        //    catch
        //    {
        //        Debug.LogError("ws Init Failed");
        //    }

        //    if (ws == null) { Debug.LogError("ws is null"); return; }

        //    try
        //    {
        //        ws.OnOpen += OnOpen;
        //        ws.OnMessage += OnMessage;
        //        ws.OnError += OnError;
        //        ws.OnClose += OnClose;

        //        ws.Connect();

        //        if (ws.ReadyState == WebSocketSharp.WebSocketState.Closed)
        //        {
        //            Debug.Log("서버가 닫혀있습니다.");
        //        }
        //        else if (ws.ReadyState == WebSocketSharp.WebSocketState.Connecting)
        //        {
        //            Debug.Log("서버가 연결 중입니다.");
        //        }
        //        else if (ws.ReadyState == WebSocketSharp.WebSocketState.Open)
        //        {
        //            Debug.Log("서버에 정상 연결 되었습니다.");
        //            isChatServerConnect = true;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Debug.Log(e);
        //    }
        //}

        //private void OnDestroy()
        //{
        //    try
        //    {
        //        ws.Close();
        //    }
        //    catch
        //    {
        //        ws = null;
        //    }
        //}

        //private void OnClose(object sender, WebSocketSharp.CloseEventArgs e)
        //{
        //    Debug.Log($"WebSocket Close : {e.Code}, {e.Reason}, {e.WasClean}");
        //}

        //private void OnOpen(object sender, EventArgs e)
        //{
        //    if (ws.ReadyState == WebSocketSharp.WebSocketState.Open)
        //    {
        //        // 여기에서 메시지를 보내거나 다른 작업을 수행할 수 있습니다.
        //        //ChatMessage chatMessage = new ChatMessage("message", "", "", PhotonNetwork.NickName, "ㅎㅇ");
        //        //string json = JsonUtility.ToJson(chatMessage);
        //    }
        //    else
        //    {
        //        Debug.Log("WebSocket connection is Close");
        //    }
        //}

        //private void OnError(object sender, WebSocketSharp.ErrorEventArgs e)
        //{
        //    Debug.Log($"Error message {e.Message}");
        //}

        //void OnMessage(object sender, WebSocketSharp.MessageEventArgs e)
        //{
        //    Debug.Log("Received message: " + e.Data);

        //    ChatMessage chatMessage = JsonUtility.FromJson<ChatMessage>(e.Data);
        //    if (chatMessage == null) return;

        //    if (chatMessage.type == "error")
        //    {
        //        ws.Close();
        //        ws = null;
        //    }
        //    else if (chatMessage.type == "sendMsg")
        //    {
        //        Debug.Log("send message: " + e.ToString());
        //    }
        //    else
        //    {
        //        Debug.LogWarning("unknown type!");
        //    }

        //}

        //public void ChatSend(string chatContent)
        //{
        //    if (ws.ReadyState == WebSocketSharp.WebSocketState.Open)
        //    {
        //        ChatMessage chatMessage = new ChatMessage("message", "", "", PhotonNetwork.NickName, chatContent);
        //        string json = JsonUtility.ToJson(chatMessage);

        //        if (ws.IsAlive)
        //        {
        //            Debug.Log(json);
        //            ws.SendAsync(json, (comp) =>
        //            {
        //                Debug.Log(comp);
        //            });
        //        }
        //        else
        //        {

        //        }
        //    }
        //    else
        //    {
        //        Debug.Log("WebSocket connection is Close");
        //    }
        //}

        //public void NoticeSend(string chatContent)
        //{
        //    if (ws.ReadyState == WebSocketSharp.WebSocketState.Open)
        //    {
        //        ChatMessage chatMessage = new ChatMessage("system", "", "", PhotonNetwork.NickName, chatContent);
        //        string json = JsonUtility.ToJson(chatMessage);

        //        ws.Send(json);
        //    }
        //    else
        //    {
        //        Debug.Log("WebSocket connection is Close");
        //    }
        //}

        //public void WhisperSend(string sendUserNick, string chatContent)
        //{
        //    if (ws.ReadyState == WebSocketSharp.WebSocketState.Open)
        //    {
        //        ChatMessage chatMessage = new ChatMessage("system", "", "", PhotonNetwork.NickName, sendUserNick, chatContent);
        //        string json = JsonUtility.ToJson(chatMessage);

        //        ws.Send(json);
        //    }
        //    else
        //    {
        //        Debug.Log("WebSocket connection is Close");
        //    }
        //}

        public void ChatServerConnect()
        {
            SetChatServerName(ref chatServerName);
            UTILS.Log($"ChatServerConnect : {chatServerName}");
            if (pv.IsMine)
            {
                try
                {
#if UNITY_EDITOR
                    isChatServerConnect = true;

#elif !UNITY_EDITOR && UNITY_WEBGL
                    Application.ExternalCall("setRoomName", chatServerName);
                    Application.ExternalCall("setNickName", PhotonNetwork.NickName);
                    Application.ExternalCall("InitChat");
                    isChatServerConnect = true;
#elif !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
                    isChatServerConnect = true;
                    //ChatInit();
#endif
                }
                catch
                {
                    return;
                }
            }
        }

        private void SetChatServerName(ref string chatServerName)
        {
            if (SceneManager.GetActiveScene().name.Equals("02_Garden_Scene")) { chatServerName = $"10"; }
            else if (SceneManager.GetActiveScene().name.Equals("03_Inside"))
            {
                if (NetworkManager.Instance.LibPos == LibName.삼산도서관) { chatServerName = $"2"; }
                else if (NetworkManager.Instance.LibPos == LibName.그림책도서관) { chatServerName = $"3"; }
                else if (NetworkManager.Instance.LibPos == LibName.연향도서관) { chatServerName = $"4"; }
                else if (NetworkManager.Instance.LibPos == LibName.기적의도서관) { chatServerName = $"5"; }
                else if (NetworkManager.Instance.LibPos == LibName.신대도서관) { chatServerName = $"6"; }

                if (NetworkManager.Instance.SpPos == SpawnerPos.자료실) { chatServerName += $"1"; }
                else if (NetworkManager.Instance.SpPos == SpawnerPos.어린이실) { chatServerName += $"2"; }
                else if (NetworkManager.Instance.SpPos == SpawnerPos.시청각실) { chatServerName += $"3"; }
            }
            else if (NetworkManager.Instance.SpPos == SpawnerPos.소그룹동아리 || NetworkManager.Instance.SpPos == SpawnerPos.중그룹동아리)
            {
                chatServerName = $"{GetComponent<PlayerManager>().joinedClubData.groupSeq}";
            }
            else if (NetworkManager.Instance.SpPos == SpawnerPos.개인서재)
            {
                chatServerName = $"{NetworkManager.Instance.user_no}";
            }

            chatServerName += $"{NetworkManager.Instance.channel.ToString("D2")}";
        }
    }
}