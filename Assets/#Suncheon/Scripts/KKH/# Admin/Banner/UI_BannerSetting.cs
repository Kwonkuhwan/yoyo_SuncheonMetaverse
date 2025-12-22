using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.IO;
using Suncheon.WebData;
using TMPro;
using Suncheon.UI;

namespace Suncheon.Admin
{
    public class UI_BannerSetting : MonoBehaviour
    {
        [Header("배너 설정")]
        [SerializeField] private Button btn_BannerImageUpLoad;
        [SerializeField] private Button btn_BannerSave;
        [SerializeField] private TMP_InputField input_Url;
        [SerializeField] private TMP_Text text_UploadImageName;

        [SerializeField] private string imageName;
        [SerializeField] private byte[] base64Data;

        [SerializeField] private GameObject ui_BannerYesNoPopUp;
        YesBtnDelegate yesBtnDelegate;
        NoBtnDelegate noBtnDelegate;

        private void Awake()
        {
            btn_BannerImageUpLoad.onClick.AddListener(() => BtnBannerImageUpLoadClick());
            btn_BannerSave.onClick.AddListener(() => BtnBannerSave());
        }

        private void Start()
        {
            StartCoroutine(UTILS.Requset_HttpGetData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.mngrBannerLoad}", (jsonData) =>
            {
                Response_BannerLoad response_BannerLoad = null;
                try
                {
                    response_BannerLoad = JsonUtility.FromJson<Response_BannerLoad>(jsonData);

                }
                catch
                {
                    response_BannerLoad = null;
                }

                if (response_BannerLoad == null) return;

                input_Url.text = response_BannerLoad.banner_url;
            }));
        }

        public void BtnBannerImageUpLoadClick()
        {
#if UNITY_EDITOR
            string path = EditorUtility.OpenFilePanel("Open File", "", "");
            if (!string.IsNullOrEmpty(path))
            {
                imageName = Path.GetFileName(path);
                text_UploadImageName.text = imageName;
                base64Data = File.ReadAllBytes(path);
                base64Data = UTILS.ByteTextureToResizeByte(base64Data);

            }
#elif !UNITY_EDITOR && UNITY_WEBGL
        try
        {
            // js 파일 호출
            Application.ExternalCall("OpenImageFile", "UI_BannerSetting");
        }
        catch(System.Exception e)
        {
        }
#endif
        }

        public void Upload()
        {
            string url = input_Url.text;
            StartCoroutine(UTILS.Requset_HttpPostData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.mngrBannerSave}", "banner_url", url, "banner_img", imageName, base64Data, (jsonData) =>
            {
                Response_ReturnMsg response_BannerImageUpLoad = JsonUtility.FromJson<Response_ReturnMsg>(jsonData);
                if (response_BannerImageUpLoad == null) { return; }

                if(response_BannerImageUpLoad.rtnCode == "000")
                {
                    AdminUIManager.Instacne.ui_ResultPopUp.GetComponent<UI_ResultPopUp>().ShowPopUp($"배너 설정이 정상 등록되었습니다.");
                }
                else
                {
                    AdminUIManager.Instacne.ui_ResultPopUp.GetComponent<UI_ResultPopUp>().ShowPopUp($"배너 설정에 등록되지 않았습니다.");
                }
            }));
        }

        public void BtnBannerSave()
        {
#if UNITY_EDITOR
            if (string.IsNullOrEmpty(imageName) || string.IsNullOrEmpty(input_Url.text)) return;
#elif !UNITY_EDITOR && UNITY_WEBGL
            if (string.IsNullOrEmpty(imageName) || base64Data == null || string.IsNullOrEmpty(input_Url.text)) return;
#endif
            yesBtnDelegate = new YesBtnDelegate(BannerYesBtnClick);
            noBtnDelegate = new NoBtnDelegate(BannerNoBtnClick);
            ui_BannerYesNoPopUp.GetComponent<UI_YesNoPopUp>().SetYesBtn(yesBtnDelegate);
            ui_BannerYesNoPopUp.GetComponent<UI_YesNoPopUp>().SetNoBtn(noBtnDelegate);
            ui_BannerYesNoPopUp.GetComponent<UI_YesNoPopUp>().ShowPopUp();
        }

        public void BannerYesBtnClick()
        {
            Upload();
            ui_BannerYesNoPopUp.SetActive(false);
        }

        public void BannerNoBtnClick()
        {
            ui_BannerYesNoPopUp.SetActive(false);
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