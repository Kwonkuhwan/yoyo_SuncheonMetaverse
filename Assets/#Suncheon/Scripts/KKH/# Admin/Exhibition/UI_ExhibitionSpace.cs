using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.IO;
using Suncheon.WebData;
using TMPro;
using Suncheon.UI;

namespace Suncheon.Admin
{
    public class UI_ExhibitionSpace : MonoBehaviour
    {
        [Header("전시공간 설정")]
        [SerializeField] private Button btn_ExhibitionImageUpLoad;
        [SerializeField] private Button btn_ExhibitionSave;
        [SerializeField] private TMP_Text text_UploadImageName;
        [SerializeField] private TMP_InputField input_Url;

        [SerializeField] private string imageName;
        [SerializeField] private byte[] base64Data;

        [SerializeField] private GameObject ui_ExhibitionYesNoPopUp;
        YesBtnDelegate yesBtnDelegate;
        NoBtnDelegate noBtnDelegate;

        private void Awake()
        {
            btn_ExhibitionImageUpLoad.onClick.AddListener(() => BtnExhibitionImageUpLoadClick());
            btn_ExhibitionSave.onClick.AddListener(() => BtnExhibitionSave());
        }

        private void Start()
        {
            StartCoroutine(UTILS.Requset_HttpGetData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.mngrExhibitionLoad}", (jsonData) =>
            {
                UTILS.Log($"PB_Interactive : {jsonData}");
                Response_ExhibitionLoad response_ExhibitionLoad = null;
                try
                {
                    response_ExhibitionLoad = JsonUtility.FromJson<Response_ExhibitionLoad>(jsonData);
                }
                catch
                {
                    response_ExhibitionLoad = null;
                }
                if (response_ExhibitionLoad == null) return;

                input_Url.text = response_ExhibitionLoad.exhibition_url;
            }));
        }

        public void BtnExhibitionImageUpLoadClick()
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
                Application.ExternalCall("OpenImageFile", "UI_ExhibitionSpace");
            }
            catch(System.Exception e)
            {
            }
#endif
        }

        public void Upload()
        {
            string url = input_Url.text;
            StartCoroutine(UTILS.Requset_HttpPostData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.mngrExhibitionSave}", "exhibition_url", url, "exhibition_img", imageName, base64Data, (jsonData) =>
            {
                Response_ReturnMsg response_ExhibitionImageUpLoad = JsonUtility.FromJson<Response_ReturnMsg>(jsonData);
                if (response_ExhibitionImageUpLoad == null) { return; }

                if (response_ExhibitionImageUpLoad.rtnCode == "000")
                {
                    AdminUIManager.Instacne.ui_ResultPopUp.GetComponent<UI_ResultPopUp>().ShowPopUp($"전시공간 설정이 정상 등록되었습니다.");
                }
                else
                {
                    AdminUIManager.Instacne.ui_ResultPopUp.GetComponent<UI_ResultPopUp>().ShowPopUp($"전시공간 설정에 등록되지 않았습니다.");
                    UTILS.Log(response_ExhibitionImageUpLoad.rtnMsg);
                }
            }));
        }

        public void BtnExhibitionSave()
        {
            if (string.IsNullOrEmpty(imageName) || base64Data == null || string.IsNullOrEmpty(input_Url.text)) return;

            yesBtnDelegate = new YesBtnDelegate(EventYesBtnClick);
            noBtnDelegate = new NoBtnDelegate(EventNoBtnClick);
            ui_ExhibitionYesNoPopUp.GetComponent<UI_YesNoPopUp>().SetYesBtn(yesBtnDelegate);
            ui_ExhibitionYesNoPopUp.GetComponent<UI_YesNoPopUp>().SetNoBtn(noBtnDelegate);
            ui_ExhibitionYesNoPopUp.GetComponent<UI_YesNoPopUp>().ShowPopUp();
        }

        public void EventYesBtnClick()
        {
            Upload();
            ui_ExhibitionYesNoPopUp.SetActive(false);
        }

        public void EventNoBtnClick()
        {
            ui_ExhibitionYesNoPopUp.SetActive(false);
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