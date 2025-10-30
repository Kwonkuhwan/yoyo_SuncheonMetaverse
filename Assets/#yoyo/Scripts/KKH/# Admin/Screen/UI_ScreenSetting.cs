using Suncheon.UI;
using Suncheon.WebData;
using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Suncheon.Admin
{
    public class UI_ScreenSetting : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown dropdown_Location;
        [SerializeField] private TMP_InputField input_Url;
        [SerializeField] private TMP_Text text_UploadImageName;

        [SerializeField] private Button btn_ScreenSave;
        [SerializeField] private Button btn_ScreenImageUpload;
        [SerializeField] private List<string> drop_Options = new List<string>();

        [SerializeField] private string imageName;
        [SerializeField] private byte[] base64Data;

        string url;
        string location;

        [SerializeField] private GameObject ui_ScreenYesNoPopUp;
        YesBtnDelegate yesBtnDelegate;
        NoBtnDelegate noBtnDelegate;

        bool isSet = false;

        private void Awake()
        {
            btn_ScreenSave.onClick.AddListener(() => BtnScreenSaveClick());
            btn_ScreenImageUpload.onClick.AddListener(() => BtnScreenImageUpLoadClick());
            dropdown_Location.ClearOptions();
            foreach (string location in Enum.GetNames(typeof(EventLoacation)))
            {
                drop_Options.Add(location);
            }

            dropdown_Location.AddOptions(drop_Options);

            dropdown_Location.onValueChanged.AddListener((index) => OnValueChanged(index));
        }

        private void Start()
        {
            string location = Enum.GetName(typeof(EventLoacation), 0);
            StartCoroutine(UTILS.Requset_HttpGetData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.mngrScreenLoad}", $"screen_location={location}", (jsonData) =>
            {
                Response_ScreenLoad response_ScreenLoad = null;
                try
                {
                    response_ScreenLoad = JsonUtility.FromJson<Response_ScreenLoad>(jsonData);

                }
                catch
                {
                    response_ScreenLoad = null;
                }

                if (response_ScreenLoad == null) return;
                SetInit(response_ScreenLoad);
            }));
        }

        // 드롭박스 값 변경
        private void OnValueChanged(int index)
        {
            string location = Enum.GetName(typeof(EventLoacation), index);
            StartCoroutine(UTILS.Requset_HttpGetData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.mngrScreenLoad}", $"screen_location={location}", (jsonData) =>
            {
                Response_ScreenLoad response_ScreenLoad = null;
                try
                {
                    response_ScreenLoad = JsonUtility.FromJson<Response_ScreenLoad>(jsonData);

                }
                catch
                {
                    response_ScreenLoad = null;
                }

                if (response_ScreenLoad == null) return;
                SetInit(response_ScreenLoad);
            }));
        }

        // 정보 설정
        private void SetInit(Response_ScreenLoad result)
        {
            input_Url.text = result.screen_url;
            imageName = result.screen_img;

            isSet = true;
        }

        public void BtnScreenImageUpLoadClick()
        {
#if UNITY_EDITOR
            string path = EditorUtility.OpenFilePanel("Open File", "", "");
            if (!string.IsNullOrEmpty(path))
            {
                imageName = Path.GetFileName(path);
                base64Data = File.ReadAllBytes(path);
                base64Data = UTILS.ByteTextureToResizeByte(base64Data);
                text_UploadImageName.text = imageName;
            }
#elif !UNITY_EDITOR && UNITY_WEBGL
        try
        {
            // js 파일 호출
            Application.ExternalCall("OpenImageFile", "UI_ScreenSetting");
        }
        catch(System.Exception e)
        {
        }
#endif
        }

        // 서버에 업로드
        private void Upload()
        {
            if (!isSet)
            {
                StartCoroutine(UTILS.Requset_HttpPostData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.mngrScreenSave}", "screen_url", url, "screen_img", imageName, base64Data, "screen_location", location, (jsonData) =>
                {
                    Response_ReturnMsg response_BannerImageUpLoad = JsonUtility.FromJson<Response_ReturnMsg>(jsonData);
                    if (response_BannerImageUpLoad == null) { return; }

                    if (response_BannerImageUpLoad.rtnCode == "000")
                    {
                        AdminUIManager.Instacne.ui_ResultPopUp.GetComponent<UI_ResultPopUp>().ShowPopUp($"스크린 설정이 정상 등록되었습니다.");
                    }
                    else
                    {
                        AdminUIManager.Instacne.ui_ResultPopUp.GetComponent<UI_ResultPopUp>().ShowPopUp($"스크린 설정에 등록되지 않았습니다.");
                    }
                }));

            }
            else
            {
                StartCoroutine(UTILS.Requset_HttpPostData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.mngrScreenUpdate}", "screen_url", url, "screen_img", imageName, base64Data, "screen_location", location, (jsonData) =>
                {
                    Response_ReturnMsg response_BannerImageUpLoad = JsonUtility.FromJson<Response_ReturnMsg>(jsonData);
                    if (response_BannerImageUpLoad == null) { return; }

                    if (response_BannerImageUpLoad.rtnCode == "000")
                    {
                        AdminUIManager.Instacne.ui_ResultPopUp.GetComponent<UI_ResultPopUp>().ShowPopUp($"스크린 설정이 정상 등록되었습니다.");
                    }
                    else
                    {
                        AdminUIManager.Instacne.ui_ResultPopUp.GetComponent<UI_ResultPopUp>().ShowPopUp($"스크린 설정에 등록되지 않았습니다.");
                    }
                }));
            }
        }

        // 저장 버튼 클릭 함수
        private void BtnScreenSaveClick()
        {
            url = input_Url.text;
            location = dropdown_Location.options[dropdown_Location.value].text;

            if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(location) || string.IsNullOrEmpty(imageName) || base64Data == null || string.IsNullOrEmpty(input_Url.text)) return;

            yesBtnDelegate = new YesBtnDelegate(ScreenYesBtnClick);
            noBtnDelegate = new NoBtnDelegate(ScreenNoBtnClick);
            ui_ScreenYesNoPopUp.GetComponent<UI_YesNoPopUp>().SetYesBtn(yesBtnDelegate);
            ui_ScreenYesNoPopUp.GetComponent<UI_YesNoPopUp>().SetNoBtn(noBtnDelegate);
            ui_ScreenYesNoPopUp.GetComponent<UI_YesNoPopUp>().ShowPopUp($"장소 : {location}\nURL : {url}");
        }

        public void ScreenYesBtnClick()
        {
            Upload();
        }

        public void ScreenNoBtnClick()
        {
            ui_ScreenYesNoPopUp.SetActive(false);
        }

        #region WebGL 이미지 데이터
        string base64String = string.Empty;
        bool isEnd = false;
        public void SetImageName(string _imageName)
        {
            imageName = _imageName;
            text_UploadImageName.text = imageName;

            base64String = string.Empty;
        }

        public void SetBase64Data(string _base64Data)
        {
            isEnd = false;

            base64String += _base64Data;
        }

        public void SetIsEnd(string isend)
        {
            isEnd = bool.Parse(isend);

            if (isEnd)
            {
                base64Data = System.Convert.FromBase64String(base64String);
                base64Data = UTILS.ByteTextureToResizeByte(base64Data);
            }
        }
        #endregion
    }
}
