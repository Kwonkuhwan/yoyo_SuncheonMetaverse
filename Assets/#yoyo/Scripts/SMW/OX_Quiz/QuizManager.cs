using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Suncheon.WebData;
using Newtonsoft.Json.Linq;
using System.Collections;
using Photon.Pun;
using Suncheon.UI;

namespace Suncheon
{
    public enum Quiz
    {
        QuizData,

    }

    public enum QuizType
    {
        QUIZ,
        DESCRIPTION,
        SCORE
    }

    [Serializable]
    public struct UI_Quiz
    {
        public GameObject ui_obj;
    }

    [Serializable]
    public struct Display
    {
        public TextMeshProUGUI CountText;
        public TextMeshProUGUI QuizText;
        public Button PopupButton;
    }

    [Serializable]
    public struct Quiz_Popup
    {
        public TextMeshProUGUI CountText;
        public TextMeshProUGUI QuizText;
        public TextMeshProUGUI DescriptionText;
        public TextMeshProUGUI UserAnswerText;
        public TextMeshProUGUI CorrectAnswerText;
        public Text Quiz_ScoreText;
        public TextMeshProUGUI Quiz_EndText;

        public Button CloseButton;
        public Button NextButton;
        public Button EndButton;

        public GameObject Quiz_DescriptionTextobj;
        public GameObject UserAnswerobj;
        public GameObject Quiz_Scoreobj;

        public GameObject[] StarObjs;
    }

    public class QuizManager : MonoBehaviour
    {
        public static Action GameStart;
        public static Action GameEnd;
        [SerializeField] private GameObject canvas_Screen;
        [SerializeField] private GameObject canvas_Player;

        public static QuizManager instance;
        List<Dictionary<string, object>> data;

        //----------------- UI
        [SerializeField] Display display;
        [SerializeField] Quiz_Popup popup;
        [SerializeField] UI_Quiz ui_quiz;

        //----------------- List
        List<string> quizList;
        List<bool> answerList;
        List<string> descriptionList;

        //----------------- Action
        public Action<bool> GradingEvent;
        public Action ResetQuizEvent;

        //----------------- Color
        Color oColor;
        Color xColor;

        int index = 0;
        int score = 0;
        bool isStart = false;

        Transform OXspawnPos;
        CameraManager CameraManager;

        bool isSetQuizData = false;

        public bool GamePlaying = false;

        [Header("나가기 UI")]
        [SerializeField] Button btn_exit;
        [SerializeField] Button btn_y;
        [SerializeField] Button btn_n;
        [SerializeField] GameObject exit_UI;


        private void Awake()
        {
            instance = this;
            init();
            /* TODO
             * DB에 저장되어 있는 값 불러오는걸로 변경예정
             */
            SetQuizData();

            btn_exit.onClick.AddListener(() => Btn_Exit());
            btn_y.onClick.AddListener(() => Btn_Y());
            btn_n.onClick.AddListener(() => Btn_N());
        }

        private void init()
        {
            //------------ OX color
            ColorUtility.TryParseHtmlString("#0095F4", out oColor);
            ColorUtility.TryParseHtmlString("#F4464A", out xColor);

            //------------ Display
            display.PopupButton.onClick.AddListener(OnClickDisplayPopup);

            //------------ Popup
            popup.CloseButton.onClick.AddListener(OnClickCloseQuiz);
            popup.NextButton.onClick.AddListener(OnClickNextQuiz);
            popup.EndButton.onClick.AddListener(OnClickEndButton);

            //------------ CameraManager
            if (CameraManager == null)
            {
                CameraManager = FindAnyObjectByType<CameraManager>();
            }
        }

        private void OnEnable()
        {
            GameStart += StartGame;
            GameEnd += EndGame;
            Reset();
        }

        private void OnDisable()
        {
            GameStart -= StartGame;
            GameEnd -= EndGame;
        }

        public void StartGame()
        {
            if (canvas_Screen)
            {
                canvas_Screen.SetActive(false);
                GamePlaying = true;
                //NetworkManager.Instance.Go_Player.GetComponentInChildren<CameraManager>().Start_QuizGame();
                //canvas_Player.SetActive(false);
                Off_otherPlayer();
            }
        }

        public void EndGame()
        {
            if (canvas_Screen)
            {
                canvas_Screen.SetActive(true);
                GamePlaying = false;
                //NetworkManager.Instance.Go_Player.GetComponentInChildren<CameraManager>().End_QuizGame();
                //canvas_Player.SetActive(true);
                On_otherPlayer();
            }
        }

        #region OX게임 나가기
        void Btn_Exit()
        {
            UIInteractionManager.Instance.OpenPopUp(exit_UI);
        }

        void Btn_Y()
        {
            EndQuiz();
            UIInteractionManager.Instance.ClosePopUp(exit_UI);
        }

        void Btn_N()
        {
            UIInteractionManager.Instance.ClosePopUp(exit_UI);
        }
        #endregion

        public void SetPlayerQuiz(Transform spawn)
        {
            if (transform.gameObject.activeSelf)
            {
                Reset();
            }
            else
            {
                transform.gameObject.SetActive(true);
            }
            OXspawnPos = spawn;     // spawn위치설정
            isStart = true;         // 게임시작
            SetPlayerLoockQuiz();
        }

        void SetPlayerLoockQuiz()
        {
            if (OXspawnPos == null) return;
            NetworkManager.Instance.Go_Player.transform.position = OXspawnPos.position;
            CameraManager.LookTarget(display.QuizText.gameObject.transform);
        }

        /// <summary>
        /// 문제, 설명 두가지 세팅 함수
        /// </summary>
        void Quiz_Set(bool type = true, bool userAnswer = false)
        {
            // 완료처리 해야됨
            if (index > 9)
            {
                EndQuiz();
                return;
            }

            if (type)
            {
                TypeTextChange(QuizType.QUIZ);
                TypeButtonChange(QuizType.QUIZ);

                popup.CountText.text = (index + 1) + "/10";
                display.CountText.text = (index + 1) + "/10";
                popup.QuizText.text = quizList[index].ToString();
                display.QuizText.text = quizList[index].ToString();

            }
            else
            {
                TypeTextChange(QuizType.DESCRIPTION);
                TypeButtonChange(QuizType.DESCRIPTION);

                popup.DescriptionText.text = descriptionList[index].ToString();
                popup.UserAnswerText.text = userAnswer == true ? "O" : "X";
                popup.UserAnswerText.color = userAnswer == true ? oColor : xColor;
                popup.CorrectAnswerText.text = answerList[index] ? "O" : "X";
                popup.CorrectAnswerText.color = answerList[index] ? oColor : xColor;
            }
        }

        public void Reset()
        {
            isStart = false;
            index = 0;
            score = 0;

            // 퀴즈 리스트 바꾸기
            //SetQuizData();
            StartCoroutine(WaiteQuizSet());
        }

        IEnumerator WaiteQuizSet()
        {
            yield return null;
            int cnt = 0;
            int max = 5;
            while (true)
            {
                if (isSetQuizData) break;

                if (cnt < max)
                {
                    yield return new WaitForSeconds(1.0f);
                }
                else
                {
                    break;
                }
            }

            SetQuizList();
            Quiz_Set();
            if (ResetQuizEvent != null) ResetQuizEvent();
        }

        // CSV파일 읽어오기
        void SetQuizData()
        {
            StartCoroutine(UTILS.Requset_HttpGetData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.mngrMiniGameLoad}", (jsonData) =>
            //StartCoroutine(UTILS.Requset_HttpGetData($"https://metalibrary.suncheon.go.kr/suncheonlib/frnt/mngr/game.ax", (jsonData) =>
            {
                UTILS.Log(jsonData);
                Response_MiniGameLoad response_MiniGameLoad = JsonUtility.FromJson<Response_MiniGameLoad>(JArray.Parse(jsonData).First.ToString());//JsonUtility.FromJson<Response_MiniGameLoad>(jsonData);
                if (response_MiniGameLoad == null) return;

                StartCoroutine(UTILS.Requset_HttpGetData($"{GameManager.Instance.defaultData.fileUrl}{response_MiniGameLoad.csv}", (jsonData) =>
                //StartCoroutine(UTILS.Requset_HttpGetData($"https://metalibrary.suncheon.go.kr/upload/{response_MiniGameLoad.csv}", (jsonData) =>
                {
                    SetQuizData(jsonData);
                }));
                //#endif

            }));
        }

        void SetQuizData(string jsonData)
        {
            data = new List<Dictionary<string, object>>();
            data = CSVReader.Read(jsonData);

            isSetQuizData = true;
        }

        // List 값 대입하기
        void SetQuizList()
        {
            if (data == null) return;

            quizList = new List<string>();
            descriptionList = new List<string>();
            answerList = new List<bool>();

            for (int i = 0; i < data.Count; i++)
            {
                object value;

                if (data[i].TryGetValue("Quiz", out value))
                {
                    quizList.Add(value.ToString());
                }
                // 키가 존재하지 않는 경우 처리
                else continue;

                if(data[i].TryGetValue("Description", out value))
                {
                    descriptionList.Add(value.ToString());
                }
                else continue;

                if (data[i].TryGetValue("Answer", out value))
                {
                    if (value.ToString() == "0")
                    {
                        answerList.Add(false);
                    }
                    else answerList.Add(true);
                }
                else continue;
            }
        }

        void OnClickCloseQuiz()
        {
            ui_quiz.ui_obj.gameObject.SetActive(false);
        }


        void OnClickDisplayPopup()
        {
            ui_quiz.ui_obj.gameObject.SetActive(true);
        }

        void OnClickNextQuiz()
        {
            index++;
            Quiz_Set(true);
        }

        void OnClickEndButton()
        {
            ui_quiz.ui_obj.SetActive(false);
        }

        void EndQuiz()
        {
            if (score >= 100)
            {
                popup.Quiz_EndText.text = "축하합니다 전부 맞혔어요!!";
                ShowStars(3);
            }
            else if (score >= 50)
            {
                popup.Quiz_EndText.text = "아쉽네요 좀 더 노력해 보세요!!";
                ShowStars(2);
            }
            else
            {
                popup.Quiz_EndText.text = "안타깝네요 실력을 더 키워보세요!!";
                ShowStars(1);
            }

            popup.Quiz_ScoreText.text = score.ToString() + "점";
            display.QuizText.text = popup.Quiz_EndText.text;

            TypeTextChange(QuizType.SCORE);
            TypeButtonChange(QuizType.SCORE);

            GameEnd();

            transform.gameObject.SetActive(false);
        }

        public void ChooseUserAnswer(bool answer)
        {
            if (index > 9 || !isStart)
            {
                return;
            }

            if (popup.Quiz_DescriptionTextobj.activeSelf) return;

            if (answerList[index] == answer)
            {
                score += 10;
                GradingEvent(true);
            }
            else
            {
                GradingEvent(false);
            }

            SetPlayerLoockQuiz();
            ui_quiz.ui_obj.gameObject.SetActive(true);
            Quiz_Set(false, answer);
        }

        void TypeButtonChange(QuizType type)
        {
            switch (type)
            {
                case QuizType.QUIZ:
                    {
                        popup.CloseButton.gameObject.SetActive(true);
                        popup.NextButton.gameObject.SetActive(false);
                        popup.EndButton.gameObject.SetActive(false);
                    }
                    break;
                case QuizType.DESCRIPTION:
                    {
                        popup.CloseButton.gameObject.SetActive(false);
                        popup.NextButton.gameObject.SetActive(true);
                        popup.EndButton.gameObject.SetActive(false);
                    }
                    break;
                case QuizType.SCORE:
                    {
                        popup.CloseButton.gameObject.SetActive(false);
                        popup.NextButton.gameObject.SetActive(false);
                        popup.EndButton.gameObject.SetActive(true);
                    }
                    break;
            }
        }

        void TypeTextChange(QuizType type)
        {
            switch (type)
            {
                case QuizType.QUIZ:
                    {
                        popup.QuizText.gameObject.SetActive(true);
                        popup.Quiz_DescriptionTextobj.SetActive(false);
                        popup.UserAnswerobj.gameObject.SetActive(false);
                        popup.Quiz_Scoreobj.gameObject.SetActive(false);
                    }
                    break;
                case QuizType.DESCRIPTION:
                    {
                        popup.QuizText.gameObject.SetActive(false);
                        popup.Quiz_DescriptionTextobj.SetActive(true);
                        popup.UserAnswerobj.gameObject.SetActive(true);
                        popup.Quiz_Scoreobj.gameObject.SetActive(false);
                    }
                    break;
                case QuizType.SCORE:
                    {
                        popup.QuizText.gameObject.SetActive(false);
                        popup.Quiz_DescriptionTextobj.SetActive(false);
                        popup.UserAnswerobj.gameObject.SetActive(false);
                        popup.Quiz_Scoreobj.gameObject.SetActive(true);
                    }
                    break;
            }
        }

        void ShowStars(int count)
        {
            if (count > popup.StarObjs.Length) return;

            for (int i = 0; i < popup.StarObjs.Length; i++)
            {
                if (i < count)
                {
                    popup.StarObjs[i].SetActive(true);
                }
                else
                {
                    popup.StarObjs[i].SetActive(false);
                }
            }
        }

        #region 다른유저 숨기기/드러내기   
        private List<GameObject> otherPlayers = new List<GameObject>();

        /// <summary>
        /// 타 유저 비활성
        /// </summary>
        void Off_otherPlayer()
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            otherPlayers.Clear();

            foreach (GameObject obj in players)
            {
                if (!obj.GetPhotonView().IsMine)
                {
                    obj.SetActive(false);
                    otherPlayers.Add(obj);
                }
            }
        }

        /// <summary>
        /// 타 유저 활성
        /// </summary>
        void On_otherPlayer()
        {
            foreach (GameObject obj in otherPlayers)
            {
                obj.SetActive(true);
            }
        }
        #endregion
    }
}