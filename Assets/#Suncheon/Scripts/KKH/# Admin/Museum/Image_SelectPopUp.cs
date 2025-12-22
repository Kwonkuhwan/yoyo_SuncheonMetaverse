using Suncheon.Admin;
using Suncheon.WebData;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Suncheon.Admin
{
    public class Image_SelectPopUp : MonoBehaviour
    {
        [SerializeField] private GameObject ui_SampleImagePopUp;

        [SerializeField] private TMP_Text text_FrameName;
        [SerializeField] private TMP_Text text_UploadImageName;

        [SerializeField] private Button btn_Upload;
        [SerializeField] private Button btn_SampleImage;
        [SerializeField] private Button btn_Set;

        public int nFrameCnt = 0;
        [SerializeField] private string imageName;
        [SerializeField] private byte[] base64Data;

        private void Awake()
        {
            btn_SampleImage.interactable = false;
            btn_Set.interactable = false;

            btn_Upload.onClick.AddListener(() => BtnUpLoadClick());
            btn_SampleImage.onClick.AddListener(() => BtnSampleImagePopUpClick());
            btn_Set.onClick.AddListener(() => BtnSetClick());
        }

        private void OnEnable()
        {
            imageName = string.Empty;
            base64Data = null;

            text_UploadImageName.text = string.Empty;
        }

        public void Init(int frameCnt)
        {
            nFrameCnt = frameCnt;
            text_FrameName.text = $"{nFrameCnt}번 그림 전시 설정";
        }

        private void BtnUpLoadClick()
        {
#if UNITY_EDITOR
            string path = EditorUtility.OpenFilePanel("Open File", "", "");
            if (!string.IsNullOrEmpty(path))
            {
                imageName = Path.GetFileName(path);
                text_UploadImageName.text = imageName;
                base64Data = File.ReadAllBytes(path);
                base64Data = UTILS.ByteTextureToResizeByte(base64Data);

                if (base64Data.Length > 0)
                {
                    btn_SampleImage.interactable = true;
                    btn_Set.interactable = true;
                }
            }
#elif !UNITY_EDITOR && UNITY_WEBGL
            try
            {
                // js 파일 호출
                Application.ExternalCall("OpenImageFile", gameObject.name);
            }
            catch(System.Exception e)
            {
            }
#endif
        }

        private void BtnSampleImagePopUpClick()
        {
            Sprite sprite = UTILS.ByteArrayToSprite(base64Data);
            ui_SampleImagePopUp.SetActive(true);
            ui_SampleImagePopUp.GetComponent<ImageSamplePopUp>().SetImage(sprite);
        }

        private void BtnSetClick()
        {            
            StartCoroutine(UTILS.Requset_HttpPostData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.mngrMuseumImageSave}",imageName, base64Data, nFrameCnt, (jsonData) =>
            {
                Response_ReturnMsg response_MuseumImageSave = null;
                try
                {
                    response_MuseumImageSave = JsonUtility.FromJson<Response_ReturnMsg>(jsonData);
                }
                catch
                {
                    response_MuseumImageSave = null;
                }

                if (response_MuseumImageSave == null) return;

                if(response_MuseumImageSave.rtnCode == "000")
                {
                    AdminUIManager.Instacne.ui_ResultPopUp.GetComponent<UI_ResultPopUp>().ShowPopUp($"해지면 열리는 박물관 {nFrameCnt}번 그림이 설정 되었습니다.");
                }
                else
                {
                    AdminUIManager.Instacne.ui_ResultPopUp.GetComponent<UI_ResultPopUp>().ShowPopUp($"해지면 열리는 박물관 {nFrameCnt}번 그림 설정 실패했습니다.");
                }
            }));
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
                btn_SampleImage.interactable = true;
                btn_Set.interactable = true;
            }
        }
        #endregion
    }
}