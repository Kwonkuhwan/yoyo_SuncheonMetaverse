using Suncheon.UI;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Suncheon.Admin
{
    public class Admin_Chat_Empty : MonoBehaviour
    {
        public void WebGLReceivedMessage(string msg) 
        { 
            //Debug.Log($"[공지] {msg} 전달"); 
        }

        // 서버와 통산을 위해 빈 함수 작성
        public void SetReceiveType(string type)
        {
        }

        public void SetReceiveMsg(string msg)
        {
        }

        public void Reponse_Chat()
        {

        }
    }
}