using Newtonsoft.Json.Linq;
using Suncheon;
using Suncheon.Admin;
using Suncheon.WebData;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_RecommBookInfo : MonoBehaviour
{
    [SerializeField] private TMP_Text text_BookName;
    [SerializeField] private TMP_Text text_Writer;
    [SerializeField] private TMP_Text text_BookInfo;

    [SerializeField] private Button btn_Restoration;
    [SerializeField] private Button btn_Delete;
    [SerializeField] private UI_RecommBook ui_RecommBook;

    int book_seq;
    int report_seq;

    private void Awake()
    {
        btn_Restoration.onClick.AddListener(() => BtnRestorationClick());
        btn_Delete.onClick.AddListener(() => BtnDeleteClick());
    }

    private void OnDisable()
    {
        text_BookName.text = string.Empty;
        text_Writer.text = string.Empty;
        text_BookInfo.text = string.Empty;

        ui_RecommBook.LoadRecommBookList();
    }

    public void Init(int _book_seq, int _report_seq)
    {
        book_seq = _book_seq;
        report_seq = _report_seq;

        Request_SelectRecmBook request_SelectRecmBook = new Request_SelectRecmBook();
        request_SelectRecmBook.recm_seq = book_seq.ToString();
        StartCoroutine(UTILS.Requset_HttpPostData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.SelectRecmBook}", request_SelectRecmBook, (jsonData) =>
        {
            JArray jArray = JArray.Parse(jsonData);
            Response_SelectRecmBook response_SelectRecmBook = null;
            try
            {
                response_SelectRecmBook = JsonUtility.FromJson<Response_SelectRecmBook>(jArray.First.ToString());
            }
            catch
            {
                response_SelectRecmBook = null;
            }

            if (response_SelectRecmBook == null) return;

            text_BookName.text = response_SelectRecmBook.title;
            text_Writer.text = response_SelectRecmBook.nickname;
            text_BookInfo.text = response_SelectRecmBook.content;
        }));
    }

    private void BtnRestorationClick()
    {
        Request_GBorRBRestoration request_GBorRbRestoration = new Request_GBorRBRestoration();
        request_GBorRbRestoration.report_seq = book_seq;
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

            if (response_GBorRbRestoration == null) return;

            if(response_GBorRbRestoration.rtnCode == "000")
            {
                AdminUIManager.Instacne.ui_ResultPopUp.GetComponent<UI_ResultPopUp>().ShowPopUp($"{text_BookName.text}의 책 추천 복구을(를) 성공으로 진행되었습니다.");
                gameObject.SetActive(false);
            }
            else
            {
                AdminUIManager.Instacne.ui_ResultPopUp.GetComponent<UI_ResultPopUp>().ShowPopUp($"{text_BookName.text}의 책 추천 복구을(를) 실패했습니다.");
            }
        }));
    }

    private void BtnDeleteClick()
    {
        Request_GBorRBDelete request_GBorRBDelete = new Request_GBorRBDelete();
        request_GBorRBDelete.report_seq = book_seq;
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

            if (response_GBorRBDelete == null) return;

            if (response_GBorRBDelete.rtnCode == "000")
            {
                AdminUIManager.Instacne.ui_ResultPopUp.GetComponent<UI_ResultPopUp>().ShowPopUp($"{text_BookName.text}님의 책 추천 삭제을(를) 성공으로 진행되었습니다.");
                gameObject.SetActive(false);
            }
            else
            {
                AdminUIManager.Instacne.ui_ResultPopUp.GetComponent<UI_ResultPopUp>().ShowPopUp($"{text_BookName.text}님의 책 추천 삭제을(를) 실패했습니다.");
            }
        }));
    }
}
