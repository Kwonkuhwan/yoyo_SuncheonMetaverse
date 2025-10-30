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
    public class UI_SSMuseum : MonoBehaviour
    {
        [SerializeField] private GameObject SSMuseumSettingObjects;

        [Header("업로드 데이터")]
        [SerializeField] private List<string> list_SSMuseumSetLocation = new List<string>();
        [SerializeField] private List<string> list_SSMuseumSetName = new List<string>();
        [SerializeField] private List<byte[]> list_SSMuseumSetByte = new List<byte[]>();

        [Header("버튼")]
        [SerializeField] private Button btn_Set;

        [Header("YesNo 팝업")]
        [SerializeField] private GameObject ui_SSMuseumSetYesNoPopUp;
        YesBtnDelegate yesBtnDelegate;
        NoBtnDelegate noBtnDelegate;

        private void Awake()
        {
            btn_Set.onClick.AddListener(() => BtnSSMuseumSetSaveClick());
        }

        public void BtnSSMuseumSetSaveClick()
        {
            SetList();

            if (list_SSMuseumSetByte.Count <= 0 || list_SSMuseumSetName.Count <= 0) return;

            yesBtnDelegate = new YesBtnDelegate(EventYesBtnClick);
            noBtnDelegate = new NoBtnDelegate(EventNoBtnClick);
            ui_SSMuseumSetYesNoPopUp.GetComponent<UI_YesNoPopUp>().SetYesBtn(yesBtnDelegate);
            ui_SSMuseumSetYesNoPopUp.GetComponent<UI_YesNoPopUp>().SetNoBtn(noBtnDelegate);
            ui_SSMuseumSetYesNoPopUp.GetComponent<UI_YesNoPopUp>().ShowPopUp($"삼산도서관 한점 미술관을\n설정하시겠습니까?");
        }

        public void EventYesBtnClick()
        {
            Upload();
            ui_SSMuseumSetYesNoPopUp.SetActive(false);
        }

        public void EventNoBtnClick()
        {
            ui_SSMuseumSetYesNoPopUp.SetActive(false);
        }

        private void Upload()
        {
            StartCoroutine(UTILS.Requset_HttpPostDataMuseum($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.ssMuseumSet}", list_SSMuseumSetLocation, list_SSMuseumSetName, list_SSMuseumSetByte, (jsonData) =>
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
                    AdminUIManager.Instacne.ui_ResultPopUp.GetComponent<UI_ResultPopUp>().ShowPopUp("한점 미술관 설정에 실패하였습니다.");
                    return;
                }
                else
                {
                    AdminUIManager.Instacne.ui_ResultPopUp.GetComponent<UI_ResultPopUp>().ShowPopUp($"한점 미술관 설정이 성공적으로 진행되었습니다.");
                }
            }));
        }

        private void SetList()
        {
            list_SSMuseumSetLocation.Clear();
            list_SSMuseumSetName.Clear();
            list_SSMuseumSetByte.Clear();

            foreach (var obj in SSMuseumSettingObjects.transform.GetComponentsInChildren<SSMuseumSettingObject>())
            {
                if (string.IsNullOrEmpty(obj.ImageName) || obj.ImageByte.Length <= 0)
                {
                    continue;
                }

                list_SSMuseumSetLocation.Add(obj.ImageLoaction);
                list_SSMuseumSetName.Add(obj.ImageName);
                list_SSMuseumSetByte.Add(obj.ImageByte);
            }
        }
    }
}
