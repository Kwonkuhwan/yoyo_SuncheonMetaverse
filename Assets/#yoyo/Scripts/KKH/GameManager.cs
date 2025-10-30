using Suncheon.UI;
using Suncheon.WebData;
using UnityEngine;

namespace Suncheon
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager instance = null;
        public static GameManager Instance => instance;

        public DefaultData defaultData;

        [SerializeField] private UIInteractionManager uiInteractionManager;
        public UIInteractionManager UiInteractionManager => uiInteractionManager;

        #region 다음씬 이름
        public string nextSceneName = string.Empty;
        #endregion

        #region 게스트
        [SerializeField] private bool isGuest;
        public bool IsGuest
        {
            get { return isGuest; }
            set { isGuest = value; }
        }
        #endregion

        #region 캐릭터 커스텀
        [SerializeField] private string charCustom;
        public string CharCustom
        {
            get { return PlayerPrefs.GetString("KEKOS_SavedCharacter", ""); }
        }
        #endregion

        #region 방문하는곳 정보
        [SerializeField] private string visitPlayerName;
        public string VisitPlayerName
        {
            get => visitPlayerName;
            set { visitPlayerName = value; }
        }
        #endregion

        #region 로그인 데이터
        public Response_LoginResultData loginData;
        #endregion

        #region 초기 로그인 확인
        public bool IsInitSpawn = false;
        #endregion

        #region 볼륨 정보
        public float AllVolume = 1.0f;
        public float BGMVolume = 1.0f;
        public float EffectVolume = 1.0f;
        #endregion

        public bool isUIMouseOver = false;

        public bool TutorialCheck = false;

        [Header("Frame")]
        public bool is30Frame = true;

        private void Awake()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            SetFrame(is30Frame);
#elif !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
#endif
            if (instance == null || instance != this)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

            DefaultDataJsonLoad();
        }

        private void Start()
        {
            Application.lowMemory += OnLowMemory;
        }

        public void DefaultDataJsonLoad()
        {
            string jsonData = string.Empty;
            //ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            StartCoroutine(UTILS.Requset_HttpGetData($"https://metalibrary.suncheon.go.kr/jsondata/defaultdata.json", (jsonData) =>
            {
                DefaultData defaultData = JsonUtility.FromJson<DefaultData>(jsonData);
                SetDefultData(defaultData);
            }));
        }

        private void SetDefultData(DefaultData _defaultData)
        {
            defaultData = _defaultData;
        }

        public void OnDestroy()
        {
            instance = null;
        }

        public void SetFrame(bool is30frame)
        {
            if (is30frame)
            {
                is30Frame = true;
                Application.targetFrameRate = 30;
            }
            else
            {
                is30Frame = false;
                Application.targetFrameRate = 60;
            }
        }

        private void OnLowMemory()
        {
            //Debug.LogWarning("OnLowMemory");
        }
    }
}