using Newtonsoft.Json.Linq;
using Suncheon.UI;
using Suncheon.WebData;
using System.IO;
using System.Text;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Suncheon.Admin
{
    public class UI_NPC : MonoBehaviour
    {
        [SerializeField] private Button btn_DownLoad;
        [SerializeField] private Button btn_UpLoad;
        [SerializeField] private Button btn_Save;

        [SerializeField] private TMP_Text text_UploadCSVName;

        [SerializeField] private string csvName;
        [SerializeField] private byte[] base64Data;

        [SerializeField] private GameObject ui_NPCYesNoPopUp;

        YesBtnDelegate yesBtnDelegate;
        NoBtnDelegate noBtnDelegate;

        private void Awake()
        {
            btn_UpLoad.onClick.AddListener(() => BtnNPCUpLoadClick());
            btn_DownLoad.onClick.AddListener(() => BtnNPCDownLoad());
            btn_Save.onClick.AddListener(() => BtnNPCSave());
        }

        public void BtnNPCUpLoadClick()
        {
            yesBtnDelegate = new YesBtnDelegate(NPCYesBtnClick);
            noBtnDelegate = new NoBtnDelegate(NPCNoBtnClick);
            ui_NPCYesNoPopUp.GetComponent<UI_YesNoPopUp>().SetYesBtn(yesBtnDelegate);
            ui_NPCYesNoPopUp.GetComponent<UI_YesNoPopUp>().SetNoBtn(noBtnDelegate);
            ui_NPCYesNoPopUp.GetComponent<UI_YesNoPopUp>().ShowPopUp();
        }

        public void NPCYesBtnClick()
        {
#if UNITY_EDITOR
            string path = EditorUtility.OpenFilePanel("Open File", "", "");
            if (!string.IsNullOrEmpty(path))
            {
                csvName = Path.GetFileName(path);
                text_UploadCSVName.text = csvName;
                base64Data = File.ReadAllBytes(path);
            }
#elif !UNITY_EDITOR && UNITY_WEBGL
            try
            {
                // js 파일 호출
                Application.ExternalCall("OpenCSVFile", gameObject.name);
            }
            catch (System.Exception e)
            {
            }
#endif
            ui_NPCYesNoPopUp.SetActive(false);
        }

        public void NPCNoBtnClick()
        {
            ui_NPCYesNoPopUp.SetActive(false);
        }

        public void SetCSVName(string _csvName)
        {
            csvName = _csvName;
            text_UploadCSVName.text = csvName;
        }

        public void SetBase64Data(string _base64Data)
        {
            base64Data = System.Convert.FromBase64String(_base64Data);
            base64Data = Encoding.UTF8.GetBytes(Encoding.UTF8.GetString(base64Data));
        }

        public void Upload()
        {
            StartCoroutine(UTILS.Requset_HttpPostData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.mngrNpcFAQSave}", "faq_file", "file_length", csvName, base64Data, (jsonData) =>
            {
                UTILS.Log(jsonData);
                Response_ReturnMsg response_NPCSave = null;
                try
                {
                    response_NPCSave = JsonUtility.FromJson<Response_ReturnMsg>(jsonData);
                }
                catch
                {
                    response_NPCSave = null;
                }
                if (response_NPCSave == null) { return; }

                if (response_NPCSave.rtnCode == "000")
                {
                    AdminUIManager.Instacne.ui_ResultPopUp.GetComponent<UI_ResultPopUp>().ShowPopUp($"NPC(FAQ) 설정이 정상 등록되었습니다.");
                }
                else
                {
                    AdminUIManager.Instacne.ui_ResultPopUp.GetComponent<UI_ResultPopUp>().ShowPopUp($"NPC(FAQ) 설정에 등록되지 않았습니다.");
                }
            }));
        }

        public void BtnNPCDownLoad()
        {
            StartCoroutine(UTILS.Requset_HttpGetData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.mngrNpcFAQLoad}", (jsonData) =>
            {
                Response_NPCLoad response_NPCLoad = JsonUtility.FromJson<Response_NPCLoad>(jsonData);
                if (response_NPCLoad == null) return;

                string fileUrl = "";
                if (string.IsNullOrEmpty(GameManager.Instance.defaultData.fileUrl.Trim()))
                {
                    fileUrl = $"https://metalibrary.suncheon.go.kr/upload/";
                }
                else
                {
                    fileUrl = GameManager.Instance.defaultData.fileUrl;
                }

                StartCoroutine(UTILS.Requset_HttpGetData($"{fileUrl}{response_NPCLoad.faq_file}", (jsonData) =>
                {
                    string csvData = jsonData;

#if UNITY_EDITOR
#elif !UNITY_EDITOR && UNITY_WEBGL
                // js 파일 호출
                Application.ExternalCall("DownLoadCSV", csvData, response_NPCLoad.faq_file);
#endif
                }));
            }));
        }

        public void BtnNPCSave()
        {
            if (string.IsNullOrEmpty(csvName) || base64Data == null) return;
            Upload();
        }
    }
}