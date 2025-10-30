using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Suncheon.WebData;

namespace Suncheon.Admin
{
    public class UI_Login : MonoBehaviour
    {
        [SerializeField] private GameObject ui_Mange;
        [SerializeField] private GameObject ui_Btns;

        [SerializeField] private TMP_InputField input_ID;
        [SerializeField] private TMP_InputField input_PW;
        [SerializeField] private Button btn_Login;

        private void Awake()
        {
            if (btn_Login != null) btn_Login.onClick.AddListener(() => BtnLoginClick());
            if(input_ID != null)
            {
                input_ID.onEndEdit.AddListener((str) =>
                {
                    BtnLoginClick();
                });
            }

            if(input_PW != null)
            {
                input_PW.onEndEdit.AddListener((str) =>
                {
                    BtnLoginClick();
                });
            }
        }

        private void Update()
        {
            if (input_ID != null)
            {
                if (input_ID.isFocused)
                {
                    if (Input.GetKeyDown(KeyCode.Tab))
                    {
                        input_PW.Select();
                    }
                }
            }
        }

        private void BtnLoginClick()
        {
            if (string.IsNullOrEmpty(input_ID.text) || string.IsNullOrEmpty(input_PW.text)) return;

            Resquest_AdminLogin resquest_AdminLogin = new Resquest_AdminLogin(input_ID.text, input_PW.text);
            StartCoroutine(UTILS.Requset_HttpPostData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.mngrLogin}", resquest_AdminLogin, (jsonData) =>
            {
                Response_AdminLogin response_AdminLogin = null;
                try
                {
                    response_AdminLogin = JsonUtility.FromJson<Response_AdminLogin>(jsonData);
                }
                catch
                {
                    response_AdminLogin = null;
                }
                if (response_AdminLogin == null) return;

                if (response_AdminLogin.rtnCode == "000")
                {
                    Response_AdminLoginResult response_AdminLoginResult = null;
                    try
                    {
                        response_AdminLoginResult = JsonUtility.FromJson<Response_AdminLoginResult>(response_AdminLogin.resultData);
                    }
                    catch
                    {
                        response_AdminLoginResult = null;
                    }

                    if (response_AdminLoginResult == null) 
                    { 
                        return;
                    }

                    LoginCheck(response_AdminLoginResult);
                }
                else
                {
                    input_PW.text = string.Empty;
                }
            }));
        }

        private void LoginCheck(Response_AdminLoginResult result)
        {
            if (result.manager == "Y")
            {
                ui_Mange.SetActive(true);
                ui_Btns.SetActive(true);
                gameObject.SetActive(false);

                AdminManager.Instance.AdminInfo = result;
                AdminChatManager.Instance.ChatServerConnect();
            }
        }
    }
}
