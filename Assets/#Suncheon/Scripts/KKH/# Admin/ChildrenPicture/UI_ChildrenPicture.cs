using Newtonsoft.Json.Linq;
using Suncheon.WebData;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Suncheon.Admin
{
    public class UI_ChildrenPicture : MonoBehaviour
    {
        [SerializeField] private Button btn_Set;
        [SerializeField] private Transform tr_ChildrenPictureObjectList;

        List<string> imageNames = new List<string>();
        List<string> imageFileNames = new List<string>();
        List<byte[]> imageBytes = new List<byte[]>();

        private void Awake()
        {
            btn_Set.onClick.AddListener(()=>BtnSetClick());
        }

        private void Upload()
        {
            imageNames.Clear();
            imageFileNames.Clear();
            imageBytes.Clear();

            foreach (var cpo in tr_ChildrenPictureObjectList.GetComponentsInChildren<ChildrenPictureObject>())
            {
                imageNames.Add(cpo.input_FreamName.text);
                imageFileNames.Add(cpo.ImageName);
                imageBytes.Add(cpo.ImageByte);
            }

            StartCoroutine(UTILS.Requset_HttpPostData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.mngrChildrenArtSave}", imageNames, imageFileNames, imageBytes, (jsonData) =>
            {
                Response_ReturnMsg response_returnMsg = null;
                try
                {
                    response_returnMsg = JsonUtility.FromJson<Response_ReturnMsg>(jsonData);
                }
                catch
                {
                    response_returnMsg = null;
                }

                if (response_returnMsg == null) return;

                if(response_returnMsg.rtnCode == "000")
                {
                    AdminUIManager.Instacne.ui_ResultPopUp.GetComponent<UI_ResultPopUp>().ShowPopUp($"어린이 그림 전시의 그림들 저장이 성공으로 진행되었습니다.");
                }
                else
                {
                    AdminUIManager.Instacne.ui_ResultPopUp.GetComponent<UI_ResultPopUp>().ShowPopUp($"어린이 그림 전시의 그림들 저장이 실패하였습니단.");
                }
            }));
        }

        private void BtnSetClick()
        {
            Upload();
        }
    }
}