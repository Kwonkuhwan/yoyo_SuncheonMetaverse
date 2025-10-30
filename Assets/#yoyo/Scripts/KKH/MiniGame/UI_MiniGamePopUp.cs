using Suncheon.Player;
using Suncheon.UI;
using Suncheon.WebData;
using UnityEngine;
using UnityEngine.UI;

namespace Suncheon
{
    public class UI_MiniGamePopUp : MonoBehaviour
    {
        [SerializeField] private GameObject go_OxGame;
        [SerializeField] private GameObject go_CardGame;

        [SerializeField] private Button btn_CardGameStart;
        [SerializeField] private Button btn_OXGameStart;
        [SerializeField] private Button btn_FeedingGameStart;

        [SerializeField] private Transform transform_OXGame;
        [SerializeField] private Mgr_Card mgr_Card;

        protected YesBtnDelegate yesBtnDelegate;
        protected NoBtnDelegate noBtnDelegate;

        private void Awake()
        {
            if (btn_CardGameStart != null) btn_CardGameStart.onClick.AddListener(() => BtnCardGameClick());
            if (btn_OXGameStart != null) btn_OXGameStart.onClick.AddListener(() => BtnOXGameClick());
            if (btn_FeedingGameStart != null) btn_FeedingGameStart.onClick.AddListener(() => BtnFeedingGameClick());
        }

        virtual public void CardGameYesButtonClick()
        {
            Request_CardGameCnt request_CardGameCnt = new Request_CardGameCnt(1);
            StartCoroutine(UTILS.Requset_HttpPostData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.userMiniGameCntSet}", request_CardGameCnt, (jsonData) =>
            {
                Response_ReturnMsg returnMsg = null;
                try
                {
                    returnMsg = JsonUtility.FromJson<Response_ReturnMsg>(jsonData);
                }
                catch
                {
                    returnMsg = null;
                }
                if (returnMsg == null) return;

                if(returnMsg.rtnCode == "000")
                {
                    UIInteractionManager.Instance.CloseAllOpenPopUp();
                    UIInteractionManager.Instance.OpenPopUp(go_CardGame);
                    mgr_Card.GameReset();

                    NetworkManager.Instance.Go_Player.GetComponent<PlayerManager>().achievementInfo.card += 1;
                }                
            }));
        }

        // 확인 취소 팝업 On
        virtual public void CardGameNoButtonClick()
        {
            UIInteractionManager.Instance.ClosePopUp(UIInteractionManager.Instance.ui_YesNoPopUp);
        }

        public void BtnCardGameClick()
        {
            yesBtnDelegate = new YesBtnDelegate(CardGameYesButtonClick);
            noBtnDelegate = new NoBtnDelegate(CardGameNoButtonClick);

            UIInteractionManager.Instance.ui_YesNoPopUp.GetComponent<UI_YesNoPopUp>().SetYesBtn(yesBtnDelegate);
            UIInteractionManager.Instance.ui_YesNoPopUp.GetComponent<UI_YesNoPopUp>().SetNoBtn(noBtnDelegate);
            UIInteractionManager.Instance.ui_YesNoPopUp.GetComponent<UI_YesNoPopUp>().ShowPopUp("미니게임 카드뒤집기(을)를\n시작하시겠습니까?");

            UTILS.Log("BtnCardGameClick");
        }

        virtual public void OXGameYesButtonClick()
        {     
            Request_OxGameCnt request_OxGameCnt = new Request_OxGameCnt(1);
            StartCoroutine(UTILS.Requset_HttpPostData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.userMiniGameCntSet}", request_OxGameCnt, (jsonData) =>
            {
                Response_ReturnMsg returnMsg = null;
                try
                {
                    returnMsg = JsonUtility.FromJson<Response_ReturnMsg>(jsonData);
                }
                catch
                {
                    returnMsg = null;
                }
                if (returnMsg == null) return;

                if (returnMsg.rtnCode == "000")
                {
                    UIInteractionManager.Instance.CloseAllOpenPopUp();
                    go_OxGame.SetActive(true);
                    QuizManager.GameStart();

                    if (QuizManager.instance != null)
                    {
                        NetworkManager.Instance.Go_Player.GetComponent<PlayerManager>().OXGameStart();
                    }

                    NetworkManager.Instance.Go_Player.GetComponent<PlayerManager>().achievementInfo.ox += 1;
                }
            }));
        }

        // 확인 취소 팝업 On
        virtual public void OXGameNoButtonClick()
        {
            UIInteractionManager.Instance.ClosePopUp(UIInteractionManager.Instance.ui_YesNoPopUp);
        }

        public void BtnOXGameClick()
        {
            yesBtnDelegate = new YesBtnDelegate(OXGameYesButtonClick);
            noBtnDelegate = new NoBtnDelegate(OXGameNoButtonClick);

            UIInteractionManager.Instance.ui_YesNoPopUp.GetComponent<UI_YesNoPopUp>().SetYesBtn(yesBtnDelegate);
            UIInteractionManager.Instance.ui_YesNoPopUp.GetComponent<UI_YesNoPopUp>().SetNoBtn(noBtnDelegate);
            UIInteractionManager.Instance.ui_YesNoPopUp.GetComponent<UI_YesNoPopUp>().ShowPopUp("미니게임 OX퀴즈(을)를\n시작하시겠습니까?\n오천그린광장으로 강제이동됩니다.");
        }

        virtual public void FeedingGameYesButtonClick()
        {
            

            Request_FeedingGameCnt request_FeedingGameCnt = new Request_FeedingGameCnt(1);
            StartCoroutine(UTILS.Requset_HttpPostData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.userMiniGameCntSet}", request_FeedingGameCnt, (jsonData) =>
            {
                Response_ReturnMsg returnMsg = null;
                try
                {
                    returnMsg = JsonUtility.FromJson<Response_ReturnMsg>(jsonData);
                }
                catch
                {
                    returnMsg = null;
                }
                if (returnMsg == null) return;

                if (returnMsg.rtnCode == "000")
                {
                    UIInteractionManager.Instance.CloseAllOpenPopUp();
                    WhaleFeedingGame.GameStart();
                    NetworkManager.Instance.Go_Player.GetComponent<PlayerManager>().achievementInfo.feeding += 1;
                }
            }));
        }

        // 확인 취소 팝업 On
        virtual public void FeedingGameNoButtonClick()
        {
            UIInteractionManager.Instance.ClosePopUp(UIInteractionManager.Instance.ui_YesNoPopUp);
        }

        public void BtnFeedingGameClick()
        {
            yesBtnDelegate = new YesBtnDelegate(FeedingGameYesButtonClick);
            noBtnDelegate = new NoBtnDelegate(FeedingGameNoButtonClick);

            UIInteractionManager.Instance.ui_YesNoPopUp.GetComponent<UI_YesNoPopUp>().SetYesBtn(yesBtnDelegate);
            UIInteractionManager.Instance.ui_YesNoPopUp.GetComponent<UI_YesNoPopUp>().SetNoBtn(noBtnDelegate);
            UIInteractionManager.Instance.ui_YesNoPopUp.GetComponent<UI_YesNoPopUp>().ShowPopUp("미니게임 뚱이의 모험(을)를\n시작하시겠습니까?");
        }
    }
}