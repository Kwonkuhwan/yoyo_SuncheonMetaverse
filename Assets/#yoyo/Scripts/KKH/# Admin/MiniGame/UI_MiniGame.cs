using System.IO;
using UnityEditor;
using UnityEngine;
using Suncheon;
using Suncheon.WebData;
using UnityEngine.UI;
using Suncheon.UI;
using TMPro;
using Newtonsoft.Json.Linq;
using Suncheon.Admin;
using System;
using System.Text;

public class UI_MiniGame : MonoBehaviour
{
    [SerializeField] private Button btn_DownLoad;
    [SerializeField] private Button btn_UpLoad;
    [SerializeField] private Button btn_Save;

    [SerializeField] private TMP_Text text_UploadCSVName;

    [SerializeField] private string csvName;
    [SerializeField] private byte[] base64Data;

    [SerializeField] private GameObject ui_MiniGameYesNoPopUp;

    YesBtnDelegate yesBtnDelegate;
    NoBtnDelegate noBtnDelegate;

    private void Awake()
    {
        btn_UpLoad.onClick.AddListener(() => BtnMiniGameUpLoadClick());
        btn_DownLoad.onClick.AddListener(() => BtnMiniGameDownLoad());
        btn_Save.onClick.AddListener(() => BtnMiniGameSave());
    }

    public void BtnMiniGameUpLoadClick()
    {
        yesBtnDelegate = new YesBtnDelegate(MiniGameYesBtnClick);
        noBtnDelegate = new NoBtnDelegate(MiniGameNoBtnClick);
        ui_MiniGameYesNoPopUp.GetComponent<UI_YesNoPopUp>().SetYesBtn(yesBtnDelegate);
        ui_MiniGameYesNoPopUp.GetComponent<UI_YesNoPopUp>().SetNoBtn(noBtnDelegate);
        ui_MiniGameYesNoPopUp.GetComponent<UI_YesNoPopUp>().ShowPopUp();
    }

    public void MiniGameYesBtnClick()
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
        ui_MiniGameYesNoPopUp.SetActive(false);

    }

    public void MiniGameNoBtnClick()
    {
        ui_MiniGameYesNoPopUp.SetActive(false);
    }

    public void SetCSVName(string _csvName)
    {
        csvName = _csvName;
        text_UploadCSVName.text = csvName;
    }

    public void SetBase64Data(string _base64Data)
    {
        base64Data = Convert.FromBase64String(_base64Data);
        base64Data = Encoding.UTF8.GetBytes(Encoding.UTF8.GetString(base64Data));
    }

    public void Upload()
    {
        StartCoroutine(UTILS.Requset_HttpGetData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.mngrMiniGameLoad}", (jsonData) =>
        {
            JArray jArray = null;
            try
            {
                jArray = JArray.Parse(jsonData);
            }
            catch (System.Exception e)
            {
                jArray = null;
            }

            Response_MiniGameLoad response_MiniGameLoad = null;
            try
            {
                response_MiniGameLoad = JsonUtility.FromJson<Response_MiniGameLoad>(jArray.First.ToString());
            }
            catch
            {
                response_MiniGameLoad = null;
            }

            if (response_MiniGameLoad == null)
            {
                StartCoroutine(UTILS.Requset_HttpPostData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.mngrMiniGameSave}", "csv", "csv_length", csvName, base64Data, (jsonData) =>
                {
                    UTILS.Log(jsonData);
                    Response_ReturnMsg response_MiniGameSave = JsonUtility.FromJson<Response_ReturnMsg>(jsonData);
                    if (response_MiniGameSave != null) { return; }
                    if (response_MiniGameSave.rtnCode == "000")
                    {
                        AdminUIManager.Instacne.ui_ResultPopUp.GetComponent<UI_ResultPopUp>().ShowPopUp($"미니게임 설정이 정상 등록되었습니다.");
                    }
                    else
                    {
                        AdminUIManager.Instacne.ui_ResultPopUp.GetComponent<UI_ResultPopUp>().ShowPopUp($"미니게임 설정에 등록되지 않았습니다.");
                    }
                }));
                return;
            }

            StartCoroutine(UTILS.Requset_HttpPostData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.mngrMiniGameUpdate}", "csv", "csv_length", csvName, base64Data, (jsonData) =>
            {
                Response_ReturnMsg response_MiniGameSave = JsonUtility.FromJson<Response_ReturnMsg>(jsonData);
                if (response_MiniGameSave == null) { return; }
                if (response_MiniGameSave.rtnCode == "000")
                {
                    AdminUIManager.Instacne.ui_ResultPopUp.GetComponent<UI_ResultPopUp>().ShowPopUp($"미니게임 설정이 정상 업데이트되었습니다.");
                }
                else
                {
                    AdminUIManager.Instacne.ui_ResultPopUp.GetComponent<UI_ResultPopUp>().ShowPopUp($"미니게임 설정에 업데이트되지 않았습니다.");
                }
            }));
        }));
    }

    public void BtnMiniGameDownLoad()
    {
        StartCoroutine(UTILS.Requset_HttpGetData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.mngrMiniGameLoad}", (jsonData) =>
        {
            JArray jArray = null;
            try
            {
                jArray = JArray.Parse(jsonData);
            }
            catch (System.Exception e)
            {
                jArray = null;
            }
            if (jArray == null) return;

            Response_MiniGameLoad response_MiniGameLoad = null;
            try
            {
                response_MiniGameLoad = JsonUtility.FromJson<Response_MiniGameLoad>(jArray.First.ToString());
            }
            catch
            {
                response_MiniGameLoad = null;
            }
            if (response_MiniGameLoad == null) return;

            string fileUrl = "";
            if (string.IsNullOrEmpty(GameManager.Instance.defaultData.fileUrl.Trim()))
            {
                fileUrl = $"https://metalibrary.suncheon.go.kr/upload/";
            }
            else
            {
                fileUrl = GameManager.Instance.defaultData.fileUrl;
            }

            StartCoroutine(UTILS.Requset_HttpGetData($"{fileUrl}{response_MiniGameLoad.csv}", (jsonData) =>
            {
                string csvData = jsonData;
                //string base64String = Convert.ToBase64String(Encoding.UTF8.GetBytes(csvData));
#if UNITY_EDITOR
#elif !UNITY_EDITOR && UNITY_WEBGL
                // js 파일 호출
                Application.ExternalCall("DownLoadCSV", csvData, response_MiniGameLoad.csv);
#endif
                UTILS.Log(response_MiniGameLoad.csv);
            }));
        }));
    }

    public void BtnMiniGameSave()
    {
        if (string.IsNullOrEmpty(csvName) || base64Data == null) return;
        Upload();
    }
}
