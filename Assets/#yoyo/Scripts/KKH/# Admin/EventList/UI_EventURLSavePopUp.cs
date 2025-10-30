using Suncheon;
using Suncheon.WebData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_EventURLSavePopUp : MonoBehaviour
{
    [SerializeField] private TMP_Text text_EventName;
    [SerializeField] private TMP_InputField input_URL;
    [SerializeField] private Button btn_Set;
    [SerializeField] private int event_Seq;

    private void Awake()
    {
        btn_Set.onClick.AddListener(() => BtnSetClick());
    }

    public void Init(string eventName, string url, int seq)
    {
        text_EventName.text = eventName;
        input_URL.text = url;
        event_Seq = seq;
    }

    public void BtnSetClick()
    {
        Request_EventURLSave request_EventURLSave = new Request_EventURLSave(event_Seq, input_URL.text);
        StartCoroutine(UTILS.Requset_HttpPostData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.mngrEventUrlSave}", request_EventURLSave, (jsonData) =>
        {
            Response_ReturnMsg response_EventURLSave = JsonUtility.FromJson<Response_ReturnMsg>(jsonData);
            if (response_EventURLSave == null) return;

            if (response_EventURLSave.rtnCode == "000")
            {
                UTILS.Log("정상 동작");
                UTILS.Log(response_EventURLSave.rtnMsg);
            }
            else
            {
                UTILS.Log("비정상 동작");
                UTILS.Log(response_EventURLSave.rtnMsg);
            }
        }));
    }
}
