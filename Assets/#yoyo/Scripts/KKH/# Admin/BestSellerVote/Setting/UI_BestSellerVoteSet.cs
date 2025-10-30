using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

using Suncheon.UI;
using System;
using System.Globalization;
using Suncheon.WebData;

namespace Suncheon.Admin
{
    public class UI_BestSellerVoteSet : MonoBehaviour
    {
        [SerializeField] private GameObject bestSellerVoteSettingObjects;
        [SerializeField] private GameObject calendarController;

        [Header("투표 제목")]
        [SerializeField] private TMP_InputField input_VoteName;

        [Header("날짜 입력")]
        [SerializeField] private TMP_Text text_Date;
        [SerializeField] private TMP_InputField input_H;
        [SerializeField] private TMP_InputField input_Min;
        [SerializeField] private TMP_InputField input_Date;     // 얼마나 진행하는지(Date) 일수

        [Header("업로드 데이터")]
        [SerializeField] private string VoteName;
        [SerializeField] private string startDate;
        [SerializeField] private string endDate;
        [SerializeField] private string date;

        [SerializeField] private List<string> list_BestSellerVoteSetBookName = new List<string>();
        [SerializeField] private List<string> list_BestSellerVoteSetName = new List<string>();
        [SerializeField] private List<byte[]> list_BestSellerVoteSetByte = new List<byte[]>();

        [Header("버튼")]
        [SerializeField] private Button btn_Calendar;
        [SerializeField] private Button btn_Set;

        [Header("YesNo 팝업")]
        [SerializeField] private GameObject ui_BestSellerVoteSetYesNoPopUp;
        YesBtnDelegate yesBtnDelegate;
        NoBtnDelegate noBtnDelegate;

        private void Awake()
        {
            btn_Calendar.onClick.AddListener(() => BtnCalendarOpen());
            btn_Set.onClick.AddListener(() => BtnBestSellerVoteSetSaveClick());
        }

        private void OnEnable()
        {
            text_Date.text = DateTime.Now.ToString("yyyy-MM-dd");
        }

        public void BtnBestSellerVoteSetSaveClick()
        {
            VoteName = input_VoteName.text;
            date = input_Date.text;
            startDate = $"{text_Date.text} {input_H.text}:{input_Min.text}";
            DateTime dateTime = DateTime.ParseExact(startDate, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
            endDate = dateTime.AddDays(double.Parse(date)).ToString("yyyy-MM-dd HH:mm");

            SetList();

            if (string.IsNullOrEmpty(VoteName) || string.IsNullOrEmpty(startDate) || string.IsNullOrEmpty(endDate) || string.IsNullOrEmpty(date) ||
                list_BestSellerVoteSetBookName.Count != 4 || list_BestSellerVoteSetByte.Count != 4 || list_BestSellerVoteSetName.Count != 4) return;

            yesBtnDelegate = new YesBtnDelegate(EventYesBtnClick);
            noBtnDelegate = new NoBtnDelegate(EventNoBtnClick);
            ui_BestSellerVoteSetYesNoPopUp.GetComponent<UI_YesNoPopUp>().SetYesBtn(yesBtnDelegate);
            ui_BestSellerVoteSetYesNoPopUp.GetComponent<UI_YesNoPopUp>().SetNoBtn(noBtnDelegate);
            ui_BestSellerVoteSetYesNoPopUp.GetComponent<UI_YesNoPopUp>().ShowPopUp($"이벤트 : {VoteName}\n날짜 : {startDate}\n진행일수 : {date}");
        }

        public void EventYesBtnClick()
        {
            Upload();
            ui_BestSellerVoteSetYesNoPopUp.SetActive(false);
        }

        public void EventNoBtnClick()
        {
            ui_BestSellerVoteSetYesNoPopUp.SetActive(false);
        }

        private void Upload()
        {
            StartCoroutine(UTILS.Requset_HttpPostData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.mngrVoteInsert}", input_VoteName.text, startDate, endDate,
                list_BestSellerVoteSetBookName, list_BestSellerVoteSetName, list_BestSellerVoteSetByte, (jsonData) =>
            {
                Response_ReturnMsg returnMsg = null;
                try
                {
                    returnMsg = JsonUtility.FromJson<Response_ReturnMsg>(jsonData);
                }
                catch
                {
                    returnMsg = null;
                }

                if (returnMsg == null || returnMsg.rtnCode != "000")
                {
                    AdminUIManager.Instacne.ui_ResultPopUp.GetComponent<UI_ResultPopUp>().ShowPopUp("투표 설정에 실패하였습니다.");
                    return;
                }
                else
                {
                    AdminUIManager.Instacne.ui_ResultPopUp.GetComponent<UI_ResultPopUp>().ShowPopUp($"투표 설정이 성공적으로 진행되었습니다.");
                }
            }));
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

        private void SetList()
        {
            list_BestSellerVoteSetBookName.Clear();
            list_BestSellerVoteSetName.Clear();
            list_BestSellerVoteSetByte.Clear();

            foreach (var obj in bestSellerVoteSettingObjects.transform.GetComponentsInChildren<BestSellerVoteSettingObject>())
            {
                if (string.IsNullOrEmpty(obj.input_BookName.text) || string.IsNullOrEmpty(obj.ImageName) || obj.ImageByte.Length <= 0)
                {
                    continue;
                }

                list_BestSellerVoteSetBookName.Add(obj.input_BookName.text);
                list_BestSellerVoteSetName.Add(obj.ImageName);
                list_BestSellerVoteSetByte.Add(obj.ImageByte);
            }
        }
    }
}
