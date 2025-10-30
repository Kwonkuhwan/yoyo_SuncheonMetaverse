using Suncheon;
using Suncheon.WebData;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EventObject : MonoBehaviour
{
    [SerializeField] private TMP_Text text_EventName;
    [SerializeField] private TMP_Text text_EventDate;
    [SerializeField] private TMP_Text text_EventPosition;
    [SerializeField] private TMP_Text text_EventTime;
    [SerializeField] private Button btn_EventShare;
    [SerializeField] private Button btn_EventDelete;

    [SerializeField] private Response_EventListData eventListData;

    [SerializeField] private GameObject ui_UrlSavePopUp;

    private void Awake()
    {
        btn_EventShare.onClick.AddListener(() => BtnEventShareClick());
        btn_EventDelete.onClick.AddListener(() => BtnEventDeleteClick());
    }

    public void Init(Response_EventListData resultData, GameObject urlSavePopUp)
    {
        eventListData = resultData;
        text_EventName.text = resultData.eventName;
        text_EventDate.text = resultData.eventStartTime;
        text_EventPosition.text = resultData.eventLocation;
        text_EventTime.text = $"{resultData.eventTime}m";

        ui_UrlSavePopUp = urlSavePopUp;
    }

    private void BtnEventShareClick()
    {
        ui_UrlSavePopUp.SetActive(true);
        ui_UrlSavePopUp.GetComponent<UI_EventURLSavePopUp>().Init(eventListData.eventName, eventListData.eventUrl, eventListData.eventSeq);
    }

    private void BtnEventDeleteClick()
    {
        Request_EventDelete request_EventDelete = new Request_EventDelete(eventListData.eventSeq);
        StartCoroutine(UTILS.Requset_HttpPostData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.mngrEventUrlDelete}", request_EventDelete, (jsonData) =>
        {
            Response_ReturnMsg response_EventDelete = JsonUtility.FromJson<Response_ReturnMsg>(jsonData);
            if (response_EventDelete == null) return;

            if (response_EventDelete.rtnCode == "000")
            {
                UTILS.Log("정상 동작");
                UTILS.Log(response_EventDelete.rtnMsg);
                Destroy(gameObject);
            }
            else
            {
                UTILS.Log("비정상 동작");
                UTILS.Log(response_EventDelete.rtnMsg);
            }
        }));
    }


}
