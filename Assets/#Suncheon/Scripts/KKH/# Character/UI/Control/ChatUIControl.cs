using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Suncheon.UI
{
    public class ChatUIControl : MonoBehaviour
    {
        [Header("Unity UI")]
        [SerializeField] private Button btn_MessageSend;
        [SerializeField] private Button btn_Normal;
        [SerializeField] private Button btn_Whispering;
        [SerializeField] private Button btn_System;
        [SerializeField] private GameObject ChatContentObj;
        [SerializeField] private GameObject MessageObject;
        [SerializeField] private TMP_InputField input_ChatBox;
        [SerializeField] private ScrollRect _textChatScrollRect;

        [SerializeField] List<GameObject> _messageObjPool = new List<GameObject>();

        private void Awake()
        {
            btn_Normal.onClick.AddListener(() => NormalBtnClick());
            btn_Whispering.onClick.AddListener(() => WhisperingBtnClick());
            btn_System.onClick.AddListener(() => SystemBtnClick());

            //btn_MessageSend.onClick.AddListener(() => SendMessage());

            _textChatScrollRect = GetComponent<ScrollRect>();

            input_ChatBox.onEndEdit.AddListener((string text) => { EnterKeyOnTextField(); });
        }

        private void NormalBtnClick()
        {
            SelectChatBtn(btn_Normal);
        }

        private void WhisperingBtnClick()
        {
            SelectChatBtn(btn_Whispering);
        }

        private void SystemBtnClick()
        {
            SelectChatBtn(btn_System);
        }

        private void SelectChatBtn(Button selectBtn)
        {
            btn_Normal.GetComponent<ButtonControl>().OnChangeSprite(false);
            btn_Whispering.GetComponent<ButtonControl>().OnChangeSprite(false);
            btn_System.GetComponent<ButtonControl>().OnChangeSprite(false);

            selectBtn.GetComponent<ButtonControl>().OnChangeSprite(true);
        }

        private void SendScrollRectToBottom()
        {
            //yield return new WaitForEndOfFrame();

            // We need to wait for the end of the frame for this to be updated, otherwise it happens too quickly.
            _textChatScrollRect.normalizedPosition = new Vector2(0, 0);

            //yield return null;
        }

        private void EnterKeyOnTextField()
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                return;
            }

            if (!Input.GetKeyDown(KeyCode.Return))
            {
                return;
            }
        }
    }
}