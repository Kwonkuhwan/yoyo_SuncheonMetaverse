using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdminChatManager : MonoBehaviour
{
    private static AdminChatManager instace = null;
    public static AdminChatManager Instance => instace;

    void Start()
    {
        if (instace == null || instace != this)
        {
            instace = this;
        }
    }

    public void ChatServerConnect()
    {
#if !UNITY_EDITOR && UNITY_WEBGL
            Application.ExternalCall("setRoomName","CH01");
            Application.ExternalCall("setNickName","Admin001");
            Application.ExternalCall("InitChat");
#endif
    }
}
