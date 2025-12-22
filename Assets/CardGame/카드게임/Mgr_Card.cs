using Suncheon;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.CanvasScaler;
using Random = UnityEngine.Random;

#region 이벤트 셋
/// <summary>
/// 카드 버튼 선택 이벤트
/// </summary>
public class CardOnSelect
{
    public int nSelectIndex;
}

/// <summary>
/// 12개 모두 오픈 이벤트
/// </summary>
public class AllClear{}

/// <summary>
/// 카드 초기화 완료 이벤트
/// </summary>
public class CardInitComplete{}
#endregion

// 카드 정보 구조
[System.Serializable]
public struct CardInfo
{
    // 카드 이미지 인덱스
    public int CardImgIndex;
    // 카드 개수
    public int Counts;
    // 할당 카드 인덱스
    public List<int> CardNumbers;
}

public class Mgr_Card : MonoBehaviour
{
    // 카드 생성 부모
    // [SerializeField] private Transform trCardParent;
    
    // 카드 오브젝트
    [SerializeField] private GameObject goCard;
    
    // 카드 리스트 취합
    [SerializeField] private List<CardInfo> cardInfos;
    
    // 카드 생성 취합
    [SerializeField] private List<GameObject> goTotalCards;

    // 카드 생성 수 확인용
    private int nTotalCounts;
    
    // 카드 선택 대기
    private int nSelected = -1;
    
    // 열린 카드 수 확인
    private int nCardOpened = 0;
    
    // 카드 초기화 확인용
    private bool bCardClearChk;
    
    // 시간 추가 게이지
    [SerializeField] private Slider slAddTime;
    
    // 피버 게이지
    [SerializeField] private GameObject goFeverTimer;
    
    // 콤보 가시화
    [SerializeField] private TextMeshProUGUI txtCombo;
    
    // 점수
    [SerializeField] private TextMeshProUGUI txtScore;
    
    // 피버타임
    [SerializeField] private GameObject[] goFevers;
    
    // 타이머
    [SerializeField] private Slider slTimer;
    [SerializeField] private TextMeshProUGUI txtTimer;
    
    [Header("시간")] [SerializeField]
    private float fPlayTimeValue;
    
    [Header("피버 유지 시간")] [SerializeField]
    private float fPlayFeverTimeValue;

    private float fFeverTimer;

    [Header("정답 시 게이지 추가 수치(0 ~ 100_100 도달 시 시간 추가")] [SerializeField]
    private int nAddGageValue;
    
    [Header("게이지 시간 추가 수치")] [SerializeField]
    private int nAddTimeValue;

    public GameObject ResultObj;
    public GameObject SuccessObj;
    public GameObject FailedObj;
    public Button Card_EndButton;

    // 게임 끝
    private bool isEnd = false;
    
    // 점수
    private int nScore = 0;
    private int successScore = 500;

    // 콤보
    private int nCombo = 0;
    
    // 피버 콤보 확인
    private int nFeverComboChk = 0;
    
    // 피버 확인
    private bool bFever;
    
    // 시간 추가 게이지
    private float fAddGage;
    
    // 콤보 유지 타이머
    private float fComboTimer;
    
    protected CompositeDisposable disposables;
    
    private void Start()
    {
        disposables = new CompositeDisposable();

        MessageBroker.Default.Receive<CardOnSelect>().Subscribe(evt =>
        {
            CardSelect(evt.nSelectIndex);
        }).AddTo(disposables);

        MessageBroker.Default.Receive<CardInitComplete>().Subscribe(evt =>
        {
            if (bCardClearChk) return;
            bCardClearChk = true;
            CreateCards();
        }).AddTo(disposables);

        Card_EndButton.onClick.AddListener(OnClickEndButton);
    }

    public void GameReset()
    {
        GameInit();

        foreach (var cardObj in goTotalCards)
        {
            cardObj.GetComponent<CardObject>().Reset();
        }

        cardInfos.Clear();
        CreateCards();
    }

    void GameInit()
    {
        isEnd = false;
        nScore = 0;
        nCombo = 0;
        fAddGage = 0;

        nTotalCounts = 0;
        nSelected = -1;
        nCardOpened = 0;

        fPlayTimeValue = 30;

        fFeverTimer = fPlayFeverTimeValue;
        goFeverTimer.GetComponent<Slider>().maxValue = fPlayFeverTimeValue;
        slTimer.maxValue = fPlayTimeValue;
        UIset();
    }

    void Update()
    {
        if (isEnd) return;

        ChkCombo();

        //ChkFever();
        
        UIset();

        ChkTimer();
        
        ChkAddTime();
    }

    /// <summary>
    /// 추가 시간 게이지 확인
    /// </summary>
    private void ChkAddTime()
    {
        if (!(slAddTime.value >= 100)) return;
        
        fPlayTimeValue += nAddTimeValue;
        if (fPlayTimeValue > slTimer.maxValue)
            fPlayTimeValue = slTimer.maxValue;
        fAddGage = 0;
    }
    
    /// <summary>
    /// 타이머
    /// </summary>
    private void ChkTimer()
    {
        if(fPlayTimeValue <= 0)
        {
            GameEnd();
        }
        fPlayTimeValue -= Time.deltaTime;
    }
    
    /// <summary>
    /// 피버타임 타이머
    /// </summary>
    private void ChkFever()
    {
        if (bFever)
        {
            fFeverTimer -= Time.deltaTime;
            goFeverTimer.SetActive(true);
            goFeverTimer.GetComponent<Slider>().value = fFeverTimer;
            if (fFeverTimer < 0)
            {
                fFeverTimer = fPlayFeverTimeValue;
                nFeverComboChk = 0;
                bFever = false;
            }
        }
        else
        {
            goFeverTimer.SetActive(false);
        }
    }
    
    /// <summary>
    /// 콤보 확인 피버타임
    /// </summary>
    private void ChkCombo()
    {
        if (nFeverComboChk > 29)
        {
            bFever = true;
            foreach (var obj in goFevers)
            {
                obj.SetActive(true);
            }
        }
        else
        {
            //foreach (var obj in goFevers)
            //{
            //    obj.SetActive(false);
            //}
        }
    }
    
    /// <summary>
    /// UI 가시화 설정
    /// </summary>
    private void UIset()
    {
        txtScore.text = nScore.ToString();
        txtCombo.text = nCombo.ToString();
        txtTimer.text = Mathf.FloorToInt(fPlayTimeValue).ToString();
        
        slAddTime.value = fAddGage;
        slTimer.value = fPlayTimeValue;
    }

    
    /// <summary>
    /// 카드 라운드 초기화
    /// </summary>
    void Init()
    {
        cardInfos.Clear();
        nTotalCounts = 0;
        nSelected = -1;
        nCardOpened = 0;
    }
    
    /// <summary>
    /// 카드 생성
    /// </summary>
    /// <param name="_isFever"></param>
    void CreateCards()
    {
        // 카드 12개가 다 생성될동안 
        while (nTotalCounts < 12)
        {
            // 카드 이미지 중복 방지 while
            while (true)
            {
                var choice = Random.Range(0, 12);
                // 카드 이미지 사용 유무 확인 (사용시 true)
                var chk = false;
                for (var i = 0; i < cardInfos.Count; i++)
                {
                    if (cardInfos[i].CardImgIndex == choice)
                    {
                        chk = true;
                        break;
                    }
                }

                if (!chk)
                {
                    // 카드 이미지 지정
                    var card = new CardInfo
                    {
                        CardImgIndex = choice,
                        Counts = 0,
                        CardNumbers = new List<int>(),
                    };

                    // 카드 수 지정(fever시 4장 고정)
                    if (bFever)
                    {
                        card.Counts = 4;
                    }
                    else if (nTotalCounts < 9)
                    {
                        var cnt = Random.Range(0, 2);
                        card.Counts = cnt switch
                        {
                            0 => 2,
                            1 => 4,
                            _ => card.Counts
                        };
                    }
                    else
                    {
                        card.Counts = 2;
                    }

                    // 카드 생성 수 누적
                    nTotalCounts += card.Counts;

                    // 카드 배치 랜덤
                    for (var i = 0; i < card.Counts; i++)
                    {
                        // 카드 배치 중복 방지
                        while (true)
                        {
                            var number = Random.Range(0, 12);
                            var cardSet = goTotalCards[number].GetComponent<CardObject>();
                            
                            if (cardSet.cardIndex != -1) continue;

                            card.CardNumbers.Add(number);
                            cardSet.cardIndex = card.CardImgIndex;
                            cardSet.Init();
                            break;
                        }    
                    }
                    
                    // 카드 정보 저장
                    cardInfos.Add(card);
                    break;
                }
            }
        }

        foreach (var card in goTotalCards)
        {
            card.GetComponent<CardObject>().ShowAllAnimation();
        }
    }


    void CardSelect(int _nSel)
    {
        if (nSelected == -1)
            nSelected = _nSel;
        else if (nSelected == _nSel)
        {
            var card1 = goTotalCards[nSelected].GetComponent<CardObject>();
            card1.Animate(false);
            nSelected = -1;
        }
        else
        {
            var card1 = goTotalCards[nSelected].GetComponent<CardObject>();
            var card2 = goTotalCards[_nSel].GetComponent<CardObject>();

            if (card1.cardIndex == card2.cardIndex)
            {
                // 정답
                card1.bCorrect = true;
                card2.bCorrect = true;

                nCardOpened += 2;

                nScore += 30 + (bFever ? 30 : 0);
                nCombo++;
                nFeverComboChk++;
                fAddGage += nAddGageValue;
            }
            else
            {
                // 오답
                // 다시 뒤집기
                card1.Animate(false);
                card2.Animate(false);

                nCombo = 0;
                nFeverComboChk = 0;
            }
            
            nSelected = -1;
        }

        if (nCardOpened > 11)
        {
            Init();
            bCardClearChk = false;
            MessageBroker.Default.Publish(new AllClear());
        }
    }

    void OnClickEndButton()
    {
        ResultObj.SetActive(false);
        SuccessObj.SetActive(false);
        FailedObj.SetActive(false);
    }

    void GameEnd()
    {
        isEnd = true;
        ResultObj.SetActive(true);
        if(nScore > successScore)
        {
            SuccessObj.SetActive(true);
            FailedObj.SetActive(false);
        }
        else
        {
            SuccessObj.SetActive(false);
            FailedObj.SetActive(true);
        }
    }

    private void OnDestroy()
    {
        disposables.Dispose();
        disposables = null;
    }
}
