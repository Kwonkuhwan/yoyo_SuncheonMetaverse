using Newtonsoft.Json.Linq;
using Suncheon.WebData;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Suncheon.Admin {
    public class AdminGuestBookInfo : MonoBehaviour
    {
        private int seq;

        [SerializeField] private TMP_Text text_Writer;
        [SerializeField] private TMP_Text text_Complainant;
        [SerializeField] private TMP_Text text_MsgInfo;

        [SerializeField] private Button btn_Restoration;
        [SerializeField] private Button btn_Delete;

        private void Awake()
        {
            btn_Restoration.onClick.AddListener(() => BtnRestorationClick());
            btn_Delete.onClick.AddListener(() => BtnDeleteClick());
        }

        public void Init(int _seq, string _writer, string _complainant, string _msg)            
        {
            seq = _seq;
            text_Writer.text = _writer;
            text_Complainant.text = _complainant;
            text_MsgInfo.text = _msg;
        }

        private void BtnRestorationClick()
        {
            Request_GBorRBRestoration request_GBorRbRestoration = new Request_GBorRBRestoration();
            request_GBorRbRestoration.report_seq = seq;
            StartCoroutine(UTILS.Requset_HttpPostData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.mngrGBookOrRBookRestoration}", request_GBorRbRestoration, (jsonData) =>
            {
                Response_ReturnMsg response_GBorRbRestoration = null;
                try
                {
                    response_GBorRbRestoration = JsonUtility.FromJson<Response_ReturnMsg>(jsonData);
                }
                catch
                {
                    response_GBorRbRestoration = null;
                }

                if (request_GBorRbRestoration == null) return;

                if (response_GBorRbRestoration.rtnCode == "000")
                {
                    AdminUIManager.Instacne.ui_ResultPopUp.GetComponent<UI_ResultPopUp>().ShowPopUp($"{text_Writer.text}님의 방명록 복구을(를) 성공으로 진행되었습니다.");
                    Destroy(gameObject);
                }
                else
                {
                    AdminUIManager.Instacne.ui_ResultPopUp.GetComponent<UI_ResultPopUp>().ShowPopUp($"{text_Writer.text}님의 복구을(를) 실패했습니다.");
                }
            }));
        }

        private void BtnDeleteClick()
        {
            Request_GBorRBDelete request_GBorRBDelete = new Request_GBorRBDelete();
            request_GBorRBDelete.report_seq = seq;
            StartCoroutine(UTILS.Requset_HttpPostData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.mngrGBookOrRBookDelete}", request_GBorRBDelete, (jsonData) =>
            {
                Response_ReturnMsg response_GBorRBDelete = null;
                try
                {
                    response_GBorRBDelete = JsonUtility.FromJson<Response_ReturnMsg>(jsonData);
                }
                catch 
                { 
                    response_GBorRBDelete = null;
                }

                if(response_GBorRBDelete == null) return;

                if (response_GBorRBDelete.rtnCode == "000")
                {
                    AdminUIManager.Instacne.ui_ResultPopUp.GetComponent<UI_ResultPopUp>().ShowPopUp($"{text_Writer.text}님의 방명록 삭제을(를) 성공으로 진행되었습니다.");
                    Destroy(gameObject);
                }
                else
                {
                    AdminUIManager.Instacne.ui_ResultPopUp.GetComponent<UI_ResultPopUp>().ShowPopUp($"{text_Writer.text}님의 삭제을(를) 실패했습니다.");
                }
            }));
        }
    }
}
