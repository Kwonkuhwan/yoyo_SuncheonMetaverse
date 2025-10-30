using Suncheon;
using Suncheon.UI;
using Suncheon.WebData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Suncheon.Admin
{
    public enum EventLoacation
    {
        광장중앙무대 = 0,
        삼산도서관시청각실,
        그림책도서관시청각실,
        연향도서관시청각실,
        기적의도서관시청각실,
        조례호수도서관시청각실,
        신대도서관시청각실,
    }

    public class UI_EventUpLoad : MonoBehaviour
    {
        [SerializeField] private TMP_InputField input_EventName;
        [SerializeField] private TMP_Dropdown dropdown_Location;

        [SerializeField] private Button btn_Date;
        [SerializeField] private TMP_Text text_Date;
        [SerializeField] private GameObject calendarController;
        [SerializeField] private TMP_InputField input_H;
        [SerializeField] private TMP_InputField input_Min;
        [SerializeField] private TMP_InputField input_Time;     // 얼마나 진행하는지(min)

        [SerializeField] private TMP_InputField input_URL;
        [SerializeField] private Button btn_EventSave;

        [SerializeField] private List<string> drop_Options = new List<string>();

        [SerializeField] private GameObject ui_EventYesNoPopUp;
        YesBtnDelegate yesBtnDelegate;
        NoBtnDelegate noBtnDelegate;

        string eventName;
        string location;
        string url;
        string strDate;
        string time;

        private void Awake()
        {
            btn_EventSave.onClick.AddListener(() => BtnEventSaveClick());
            btn_Date.onClick.AddListener(() => BtnCalendarOpen());

            dropdown_Location.ClearOptions();
            foreach (string location in Enum.GetNames(typeof(EventLoacation)))
            {
                drop_Options.Add(location);
            }

            dropdown_Location.AddOptions(drop_Options);

        }

        public void BtnCalendarOpen()
        {
            if (calendarController.GetComponent<CalendarController>().ShowHideCheck())
            {
                calendarController.GetComponent<CalendarController>().HideCalender();
            }
            else
            {
                calendarController.GetComponent<CalendarController>().ShowCalendar(text_Date);
            }
        }

        public void BtnEventSaveClick()
        {
            eventName = input_EventName.text;
            location = dropdown_Location.options[dropdown_Location.value].text;
            url = input_URL.text;
            strDate = $"{text_Date.text} {input_H.text}:{input_Min.text}";
            time = input_Time.text;

            if (string.IsNullOrEmpty(eventName) || string.IsNullOrEmpty(location) || string.IsNullOrEmpty(url) || string.IsNullOrEmpty(strDate) || string.IsNullOrEmpty(time)) return;

            yesBtnDelegate = new YesBtnDelegate(EventYesBtnClick);
            noBtnDelegate = new NoBtnDelegate(EventNoBtnClick);
            ui_EventYesNoPopUp.GetComponent<UI_YesNoPopUp>().SetYesBtn(yesBtnDelegate);
            ui_EventYesNoPopUp.GetComponent<UI_YesNoPopUp>().SetNoBtn(noBtnDelegate);     
            ui_EventYesNoPopUp.GetComponent<UI_YesNoPopUp>().ShowPopUp($"이벤트 : {eventName}\n장소 : {location}\nURL : {url}\n날짜 : {strDate}\n진행시간 : {time}");
        }

        public void Upload()
        {           
            Request_EvenetSave request_EvenetSave = new Request_EvenetSave(eventName, location, url, strDate, time);
            StartCoroutine(UTILS.Requset_HttpPostData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.mngrEventSet}", request_EvenetSave, (jsonData) =>
            {
                Response_ReturnMsg response_EventSave = null;
                try
                {
                    response_EventSave = JsonUtility.FromJson<Response_ReturnMsg>(jsonData);
                }
                catch
                {
                    response_EventSave = null;
                }

                if (response_EventSave == null) return;

                if (response_EventSave.rtnCode == "000")
                {
                    UTILS.Log("정상 동작");
                    UTILS.Log(response_EventSave.rtnMsg);
                    AdminUIManager.Instacne.ui_ResultPopUp.GetComponent<UI_ResultPopUp>().ShowPopUp($"이벤트 설정이 정상 등록되었습니다.");

                }
                else
                {
                    UTILS.Log("비정상 동작");
                    UTILS.Log(response_EventSave.rtnMsg);
                    AdminUIManager.Instacne.ui_ResultPopUp.GetComponent<UI_ResultPopUp>().ShowPopUp($"이벤트 설정이 등록되지 않았습니다.");
                }
            }));
        }

        public void EventYesBtnClick()
        {       
            Upload();
            ui_EventYesNoPopUp.SetActive(false);
        }

        public void EventNoBtnClick()
        {
            ui_EventYesNoPopUp.SetActive(false);
        }
    }
}