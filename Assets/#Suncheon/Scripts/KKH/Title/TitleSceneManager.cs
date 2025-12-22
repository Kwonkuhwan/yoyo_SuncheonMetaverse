using Photon.Pun;
using Suncheon.WebData;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Suncheon
{
    public class TitleSceneManager : MonoBehaviour
    {
        #region 로딩
        [Header("로딩 관련")]
        [SerializeField] GameObject panel_Loading;
        [SerializeField] TMP_Text text_loading;
        [SerializeField] Image image_loading;
        [SerializeField] float lodingSpeed = 500f;
        [SerializeField] private float cooltime = 0.0f;
        [SerializeField] float maxRandomCount = 3.0f;
        [SerializeField] float maxLoadingCount = 3.0f;
        [SerializeField] bool isLoading = false;
        [Range(0.0f, 1.0f)]
        [SerializeField] float blink_Loading_Time;
        #endregion

        [Space(10)]

        #region 시작
        [Header("시작 관련")]
        [SerializeField] GameObject panel_Title;
        [SerializeField] GameObject panel_TitleBackWindow;
        [SerializeField] GameObject panel_StartClick;
        [SerializeField] Image image_Start;
        [SerializeField] Button btn_Start;
        [Range(0.0f, 1.0f)]
        [SerializeField] float blink_Start_Time;
        [SerializeField] Animation anim_Title;
        #endregion

        [Space(10)]

        #region 로그인(순천, 게스트)
        // 시작타이틀의 로그인 버튼 관련
        [SerializeField] Animation anim_Login;              // Click to Start 클릭 후 나오는 로그인버튼 애니메이션
        [SerializeField] GameObject panel_Login;            //
        [SerializeField] Button btn_Login;
        [SerializeField] Animation anim_SelectLogin;

        [Header("로그인(순천, 게스트)")]
        [SerializeField] GameObject panel_SelectLogin;      // 로그인 종류 선택
        [SerializeField] Button btn_SunCheon;               // 로그인 창
        [SerializeField] Button btn_Guest;                  // 게스트 로그인
        #endregion

        [SerializeField] GameObject panel_InfoPanel;           // 개인정보 처리 방침
        [SerializeField] Button btn_Info;

        [Space(10)]
        [Header("순천 로그인")]
        [SerializeField] GameObject panel_SunCheonLogin;
        [SerializeField] GameObject panel_SuncheonLogin_01;
        [SerializeField] GameObject panel_SunCheonLogin_Error;
        [SerializeField] TMP_InputField input_SunCheon_ID;
        [SerializeField] TMP_InputField input_SunCheon_PW;

        [SerializeField] Button btn_SunCheonLogin;
        [SerializeField] GameObject panel_SuncheonLogin_02;
        [SerializeField] GameObject panel_SunCheonNickNameLogin_Error;
        [SerializeField] TMP_InputField input_SunCheon_NickName;
        [SerializeField] Button btn_SunCheonNickNameLogin;

        [Space(10)]
        [Header("게스트 로그인")]
        [SerializeField] GameObject panel_GuestLogin;
        [SerializeField] GameObject panel_GuestLogin_Error;
        [SerializeField] Button btn_GuestLogin;

        [Space(10)]

        #region 뒤로가기
        [Header("뒤로가기")]
        [SerializeField] Button btn_Back;
        #endregion

        private void Awake()
        {
            maxLoadingCount = Random.Range(1.0f, maxRandomCount);

            PlayerPrefs.DeleteKey("KEKOS_SavedCharacter");

            btn_Start.onClick.AddListener(() => StartBtnClick());

            btn_Login.onClick.AddListener(() => LoginBtnClick());

            btn_SunCheon.onClick.AddListener(() => SunCheonBtnClick());

            btn_Guest.onClick.AddListener(() => GuestBtnClick());

            btn_Info.onClick.AddListener(() => OnInfoBtnClick());

            btn_SunCheonLogin.onClick.AddListener(() => SunCheonLoginClick());
            btn_SunCheonNickNameLogin.onClick.AddListener(() => SunCheonNickNameLoginClick());
            input_SunCheon_ID.onEndEdit.AddListener((msg) =>
            {
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    SunCheonLoginClick();
                }
            });
            input_SunCheon_PW.onEndEdit.AddListener((msg) =>
            {
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    SunCheonLoginClick();
                }
            });

            btn_GuestLogin.onClick.AddListener(() => InfoPanelOn());

            btn_Back.onClick.AddListener(() => BackBtnClick());

            isLoading = false;
            panel_Title.SetActive(true);
            panel_Loading.SetActive(true);
            panel_TitleBackWindow.SetActive(false);
            panel_StartClick.SetActive(false);
            panel_Login.SetActive(false);
        }

        private void Start()
        {
            StartCoroutine(LoadingWait());
        }

        private void Update()
        {
            if (!isLoading)
            {
                image_loading.gameObject.transform.Rotate(new Vector3(0, 0, -lodingSpeed * Time.deltaTime));
                cooltime += Time.deltaTime;
            }

            if (input_SunCheon_ID.isFocused)
            {
                if(Input.GetKeyDown(KeyCode.Tab))
                {
                    input_SunCheon_PW.Select();
                }
            }
        }

        IEnumerator LoadingWait()
        {
            int dotCnt = 0;
            string loadingText = "Loading";
            string dotText = string.Empty;
            while (cooltime < maxLoadingCount)
            {
                yield return new WaitForSeconds(blink_Loading_Time);

                if (dotCnt > 2)
                {
                    dotText = string.Empty;
                    dotCnt = 0;
                }
                else
                {
                    dotText += ".";
                    dotCnt++;
                }

                text_loading.text = loadingText + dotText;
            }

            StartCoroutine(StartClickWait());
            isLoading = true;
        }

        private IEnumerator StartClickWait()
        {
            panel_Loading.SetActive(false);
            panel_TitleBackWindow.SetActive(true);
            panel_StartClick.SetActive(true);

            while (true)
            {
                yield return new WaitForSeconds(blink_Start_Time);

                image_Start.gameObject.SetActive(!image_Start.gameObject.activeInHierarchy);
            }
        }

        /// <summary>
        /// 시작 버튼 클릭
        /// </summary>
        private void StartBtnClick()
        {
            if (isLoading)
            {
                StartCoroutine(StartBtnClickAnim());
            }
        }

        IEnumerator StartBtnClickAnim()
        {
            //NetworkManager.Instacne.ConnectPhoton();

            panel_StartClick.SetActive(false);
            panel_Login.SetActive(true);
            yield return new WaitForSeconds(0.1f);
            anim_Login.Play();
        }

        #region Login
        /// <summary>
        /// 로그인 버튼 클릭
        /// </summary>
        private void LoginBtnClick()
        {
            StartCoroutine(LoginBtnClickAnim());
        }

        IEnumerator LoginBtnClickAnim()
        {
            btn_Login.gameObject.SetActive(false);
            btn_Back.gameObject.SetActive(true);

            anim_Title.clip = anim_Title["Anim_TilteMove"].clip;
            anim_Title.Play();

            yield return new WaitForSeconds(1.0f);

            panel_SelectLogin.SetActive(true);
            anim_SelectLogin.Play();
        }
        /// <summary>
        /// 순천 계정 로그인 버튼 클릭
        /// </summary>
        private void SunCheonBtnClick()
        {
            GameManager.Instance.IsGuest = false;

            panel_SelectLogin.SetActive(false);
            panel_SunCheonLogin.SetActive(true);

            //UTILS.OpenWebView("http://naver.com");
            //Application.OpenURL("http://naver.com");
        }

        /// <summary>
        /// 게스트 로그인 버튼 클릭
        /// </summary>
        private void GuestBtnClick()
        {
            GameManager.Instance.IsGuest = true;

            panel_SelectLogin.SetActive(false);
            panel_GuestLogin.SetActive(true);
        }
        #endregion

        #region 순천 로그인 관련
        private void SunCheonLoginClick()
        {
            if (string.IsNullOrEmpty(input_SunCheon_ID.text.Trim()))
            {
                StartCoroutine(ErrorPanelOn(panel_GuestLogin_Error));
                return;
            }
            if (string.IsNullOrEmpty(input_SunCheon_PW.text.Trim()))
            {
                StartCoroutine(ErrorPanelOn(panel_GuestLogin_Error));
                return;
            }

            // 여기에 https 통신 추가
            Request_Post_Login post_Login = new Request_Post_Login(input_SunCheon_ID.text, input_SunCheon_PW.text);
            StartCoroutine(UTILS.Requset_HttpPostData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.loginUrl}", post_Login, (jsonData) =>
            {
                Response_Post_Login response_Post_Login = JsonUtility.FromJson<Response_Post_Login>(jsonData);
                if (response_Post_Login == null) return;

                // 에러코드 처리
                if (response_Post_Login.rtnCode == "999")
                {
                    StartCoroutine(ErrorPanelOn(panel_SunCheonLogin_Error));
                    return;
                }
                if (response_Post_Login.rtnCode != "000")
                {
                    return;
                }

                Response_LoginResultData response_LoginResultData = JsonUtility.FromJson<Response_LoginResultData>(response_Post_Login.resultData);
                SunCheonLogin(response_LoginResultData);
            }));
        }

        private void SunCheonLogin(Response_LoginResultData resultData)
        {
            // 닉네임이 비어있지 않으면
            if (resultData.nickname != string.Empty)
            {
                SuccessLogin(resultData.nickname);
            }
            else
            {
                panel_SuncheonLogin_01.SetActive(false);
                panel_InfoPanel.SetActive(true);
            }

            GameManager.Instance.loginData = resultData;
        }

        private void SunCheonNickNameLoginClick()
        {
            // 닉네임이 부적절하거나 공백일때 Error 문구 띄우고 리턴
            if (!UTILS.NickNameCheck(input_SunCheon_NickName.text.Trim()))
            {               
                StartCoroutine(ErrorPanelOn(panel_SunCheonNickNameLogin_Error));
                return;
            }
            // 닉네임이 정상적이면 다음 씬으로 넘긴다.
            else
            {
                Request_NickNameCheck request_NickNameCheck = new Request_NickNameCheck(input_SunCheon_NickName.text.Trim());
                StartCoroutine(UTILS.Requset_HttpPostData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.nickNameCheckUrl}", request_NickNameCheck, (jsonData) =>
                {
                    UTILS.Log($"SunCheonNickNameLoginClick : {jsonData}");
                    Response_NickNameCheck response_NickNameCheck = JsonUtility.FromJson<Response_NickNameCheck>(jsonData);
                    if(!NickNameCheck(response_NickNameCheck)) return;
                }));
            }
        }

        private bool NickNameCheck(Response_NickNameCheck response_NickNameCheck)
        {
            if (response_NickNameCheck.rtnCode == "000")
            {
                SuccessLogin(input_SunCheon_NickName.text.Trim());
                return true;
            }
            else
            {
                // 닉네임 중복 에러
                StartCoroutine(ErrorPanelOn(panel_SunCheonNickNameLogin_Error));
                return false;
            }
        }
        #endregion

        #region 게스트 로그인 관련

        private void InfoPanelOn()
        {
            panel_InfoPanel.SetActive(true);            
        }

        private void OnInfoBtnClick()
        {
            if (GameManager.Instance.IsGuest)
            {
                GuestLoginBtnClick();
            }
            else
            {
               panel_InfoPanel.SetActive(false); 
               panel_SuncheonLogin_02.SetActive(true);
            }
        }

        private void GuestLoginBtnClick()
        {
            StartCoroutine(UTILS.Requset_HttpGetData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.nonLoginUrl}", (jsonData) =>
            {
                Response_NonMemberLogin response_NonMemberLogin = JsonUtility.FromJson<Response_NonMemberLogin>(jsonData);
                Response_NonMemberLoginResultData response_NonMemberLoginResultData = JsonUtility.FromJson<Response_NonMemberLoginResultData>(response_NonMemberLogin.resultData);
                if (response_NonMemberLogin == null) return;
                if (response_NonMemberLogin.rtnCode == "999")
                {
                    StartCoroutine(ErrorPanelOn(panel_GuestLogin_Error));
                    return;
                }

                UTILS.Log(response_NonMemberLoginResultData.name);
                SuccessLogin(response_NonMemberLoginResultData.name);
            }));
        }

        private IEnumerator LoadingFadeOut(string sceneName)
        {
            anim_Title.clip = anim_Title["Anim_TilteFadeOut"].clip;
            anim_Title.Play();

            yield return new WaitForSeconds(2.0f);
            UTILS.LoadingSceneLoad(sceneName);
        }

        IEnumerator ErrorPanelOn(GameObject panel)
        {
            panel.SetActive(true);
            input_SunCheon_PW.text = string.Empty;

            yield return new WaitForSeconds(3.0f);

            panel.SetActive(false);
        }
        #endregion
        public void BackBtnClick()
        {
            if (panel_SunCheonLogin.activeInHierarchy)
            {
                panel_InfoPanel.SetActive(false);

                if (panel_SuncheonLogin_01.activeInHierarchy)
                {
                    btn_SunCheonLogin.interactable = true;
                    input_SunCheon_ID.text = string.Empty;
                    input_SunCheon_PW.text = string.Empty;
                    panel_SunCheonLogin.SetActive(false);
                    panel_SelectLogin.SetActive(true);
                    anim_SelectLogin.Play();
                }
                else
                {
                    input_SunCheon_NickName.text = string.Empty;
                    panel_SuncheonLogin_02.SetActive(false);
                    panel_SuncheonLogin_01.SetActive(true);
                }
            }
            else if (panel_GuestLogin.activeInHierarchy)
            {
                panel_InfoPanel.SetActive(false);

                panel_GuestLogin.SetActive(false);
                panel_SelectLogin.SetActive(true);
                anim_SelectLogin.Play();
            }
            else
            {
                panel_SelectLogin.SetActive(true);
                anim_SelectLogin.Play();
            }
        }

        private void SuccessLogin(string strNickName)
        {
            if (string.IsNullOrEmpty(GameManager.Instance.loginData.nickname))
            {
                GameManager.Instance.loginData.nickname = strNickName;
            }

            PhotonNetwork.NickName = strNickName;
            panel_Login.SetActive(false);
            StartCoroutine(LoadingFadeOut("02_Garden_Scene"));
        }
    }
}