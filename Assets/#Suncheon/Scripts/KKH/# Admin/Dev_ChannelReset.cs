using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Suncheon.Admin
{
    public class Dev_ChannelReset : MonoBehaviour
    {
        [SerializeField] private Button btn_ChannelReset;

        private void Awake()
        {
            btn_ChannelReset.onClick.AddListener(() => ChannelReset());
        }

        private void ChannelReset()
        {
            StartCoroutine(UTILS.Requset_HttpGetData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.devChannelReset}", (jsonData) =>
            {
                if(string.Equals(jsonData, "1"))
                {
                    AdminUIManager.Instacne.ui_ResultPopUp.GetComponent<UI_ResultPopUp>().ShowPopUp($"채널 초기화 성공");
                }
                else
                {
                    AdminUIManager.Instacne.ui_ResultPopUp.GetComponent<UI_ResultPopUp>().ShowPopUp($"채널 초기화 실패");

                }
            }));
        }
    }
}