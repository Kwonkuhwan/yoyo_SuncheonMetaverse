using Suncheon.WebData;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Suncheon.Admin
{
    public class UI_OperationalltyBoard : MonoBehaviour
    {
        [SerializeField] private Button btn_Date;
        [SerializeField] private Button btn_Serch;
        [SerializeField] private TMP_Text text_Date;
        [SerializeField] private GameObject calendarController;

        [SerializeField] private TMP_Text text_CumulativeVisitorsValue;
        [SerializeField] private TMP_Text text_OneDayVisitorsValue;
        [SerializeField] private TMP_Text text_NewVisitorsValue;
        [SerializeField] private TMP_Text text_AverageTimeValuee;

        [SerializeField] private TMP_Text text_WeekDayHourTime;
        [SerializeField] private TMP_Text text_WeekDayHourDate;

        [SerializeField] private TMP_Text text_MonthHourTime;
        [SerializeField] private TMP_Text text_MonthHourDate;

        [SerializeField] private GameObject go_ProgressBar;
        [SerializeField] private Transform transform_Chart;
        [SerializeField] private Transform transform_X;
        [SerializeField] private TMP_Text text_MaxValue;

        private void Awake()
        {
            btn_Date.onClick.AddListener(() => BtnCalendarOpen());
            btn_Serch.onClick.AddListener(() => BtnSerch());
        }

        private void Start()
        {
            btn_Date.GetComponentInChildren<TMP_Text>().text = DateTime.Now.ToString("yyyy-MM-dd");
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

        public void BtnSerch()
        {
            Request_DashBoardDataCntLoad request_DashBoardDataLoad = new Request_DashBoardDataCntLoad(text_Date.text);
            StartCoroutine(UTILS.Requset_HttpPostData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.mngrDashBoardCntData}", request_DashBoardDataLoad, (jsonData) =>
            {
                 Response_DashBoardCntResult response_DashBoardData = null;
                try
                {
                    response_DashBoardData = JsonUtility.FromJson<Response_DashBoardCntResult>(jsonData);
                }
                catch
                {
                    response_DashBoardData = null;
                }

                if (response_DashBoardData == null) return;
                SetDashBoardCnt(response_DashBoardData);
            }));

            Request_DashBoardChartLoad request_DashBoardChartLoad = new Request_DashBoardChartLoad(text_Date.text);
            StartCoroutine(UTILS.Requset_HttpPostData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.mngrDashBoardChartData}", request_DashBoardChartLoad, (jsonData) =>
            {
                Response_DashBoardChartData response_DashBoardChartData = null;
                try
                {
                    response_DashBoardChartData = new Response_DashBoardChartData(jsonData);
                }
                catch
                {
                    response_DashBoardChartData = null;
                }

                if (response_DashBoardChartData == null) return;
                SetDashBoardChart(response_DashBoardChartData);
            }));
        }

        private void SetDashBoardCnt(Response_DashBoardCntResult result)
        {
            text_CumulativeVisitorsValue.text = result.allUserCnt.ToString();
            text_OneDayVisitorsValue.text = result.userCnt.ToString();
            text_NewVisitorsValue.text = result.newUserCnt.ToString();
            text_AverageTimeValuee.text = TimeSpan.FromHours(result.avaHour).ToString(@"hh\:mm\:ss");

            text_WeekDayHourDate.text = result.weekDate;
            text_WeekDayHourTime.text = TimeSpan.FromHours(result.weekHour).ToString(@"hh\:mm\:ss");

            text_MonthHourDate.text = result.monthData;
            text_MonthHourTime.text = TimeSpan.FromHours(result.monthHour).ToString(@"hh\:mm\:ss");
        }

        private void SetDashBoardChart(Response_DashBoardChartData result)
        {
            foreach(var obj in transform_Chart.GetComponentsInChildren<ProgressBar>())
            {
                Destroy(obj.gameObject);
            }

            List<float> list = new List<float>();
            foreach (var data in result.response_DashBoardResultDatas)
            {
                list.Add(float.Parse(data.cnt));
            }

            float maxValue = list.Max();
            text_MaxValue.text = maxValue.ToString();
            foreach (var data in result.response_DashBoardResultDatas)
            {
                GameObject bar = Instantiate(go_ProgressBar, transform_Chart);
                bar.GetComponent<ProgressBar>().SetProgressBar(float.Parse(data.cnt), (float.Parse(data.cnt) / maxValue), data.time);
            }
        }
    }
}
