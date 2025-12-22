using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Suncheon.Admin
{
    public class BestSellerVoteSettingObject : MonoBehaviour
    {
        [SerializeField] private GameObject ui_SampleImagePopUp;

        public TMP_InputField input_BookName;
        [SerializeField] private TMP_Text text_UploadImageName;

        [SerializeField] private Button btn_VoteBookImageUpload;
        [SerializeField] private Button btn_SampleImage;

        [SerializeField] private string imageName;
        public string ImageName => imageName;
        [SerializeField] private byte[] base64Data;
        public byte[] ImageByte => base64Data;

        private void Awake()
        {
            btn_SampleImage.interactable = false;

            btn_VoteBookImageUpload.onClick.AddListener(() => BtnUpLoadClick());
            btn_SampleImage.onClick.AddListener(() => BtnSampleImagePopUpClick());
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
            }
        }
        #endregion
    }
}
