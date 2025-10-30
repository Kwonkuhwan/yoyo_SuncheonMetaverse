using Suncheon;
using Suncheon.UI;
using Suncheon.WebData;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Suncheon.Admin
{
    public class UI_Notice : MonoBehaviour
    {
        [Header("공지 관련")]
        [SerializeField] private TMP_InputField input_Notice;
        [SerializeField] private Button btn_Notice;

        [SerializeField] private GameObject ui_NoticeYesNoPopUp;
        YesBtnDelegate yesBtnDelegate;
        NoBtnDelegate noBtnDelegate;

        private void Awake()
        {
            btn_Notice.onClick.AddListener(() => BtnNoticeClick());
        }

        #region 공지사항 관련
        public void BtnNoticeClick()
        {
            if (string.IsNullOrEmpty(input_Notice.text.Trim())) return;

            yesBtnDelegate = new YesBtnDelegate(NoticeYesBtnClick);
            noBtnDelegate = new NoBtnDelegate(NoticeNoBtnClick);
            ui_NoticeYesNoPopUp.GetComponent<UI_YesNoPopUp>().SetYesBtn(yesBtnDelegate);
            ui_NoticeYesNoPopUp.GetComponent<UI_YesNoPopUp>().SetNoBtn(noBtnDelegate);
            ui_NoticeYesNoPopUp.GetComponent<UI_YesNoPopUp>().ShowPopUp();           
        }

        public void NoticeYesBtnClick()
        {
            Request_Notice request_Notice = new Request_Notice(input_Notice.text);
            StartCoroutine(UTILS.Requset_HttpPostData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.mngrNotice}", request_Notice, (jsonData) =>
            {
                Response_ReturnMsg response_Notice = null;
                try
                {
                    response_Notice = JsonUtility.FromJson<Response_ReturnMsg>(jsonData);
                }
                catch
                {
                    response_Notice = null;
                }
                if (response_Notice == null) return;

                if (response_Notice.rtnCode == "000")
                {
                    UTILS.Log($"{response_Notice.rtnMsg}");
#if UNITY_EDITOR
#elif !UNITY_EDITOR && UNITY_WEBGL
                    Application.ExternalCall("noticeSend", input_Notice.text);
#endif
                    AdminUIManager.Instacne.ui_ResultPopUp.GetComponent<UI_ResultPopUp>().ShowPopUp($"{response_Notice.rtnMsg}\n{input_Notice.text}");
                }
                else
                {
                    UTILS.Log($"{response_Notice.rtnMsg}");
                    AdminUIManager.Instacne.ui_ResultPopUp.GetComponent<UI_ResultPopUp>().ShowPopUp($"{response_Notice.rtnMsg}");
                }
            }));

            ui_NoticeYesNoPopUp.SetActive(false);
            input_Notice.text = string.Empty;
        }

        public void NoticeNoBtnClick()
        {
            ui_NoticeYesNoPopUp.SetActive(false);
        }
        #endregion
    }
}