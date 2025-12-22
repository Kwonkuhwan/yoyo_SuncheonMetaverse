using System;
using System.Collections;
using UniRx;
using UniRx.Triggers;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CardObject : MonoBehaviour
{
    // 카드 앞뒷면 이미지
    [SerializeField]private Image fCardSprite;
    
    // 카드 뒤집기 애니메이션
    private Animation ani;
    
    // 카드 인덱스
    [HideInInspector] public int cardIndex = -1; 
    // 카드 번호
    public int cardNumber;

    // 카드 뒤집기 여부
    private bool bIsOpen = false;

    [HideInInspector] public bool bCorrect;

    [SerializeField] private Material mat;
    
    protected CompositeDisposable disposables;
    

    private void Awake()
    {
        disposables = new CompositeDisposable();
        
        cardIndex = -1;
        
        fCardSprite = transform.GetChild(1).GetComponent<Image>();
        ani = GetComponent<Animation>();
        
        // 카드 선택 뒤집기 애니메이션 수행 이벤트
        GetComponent<Button>().OnClickAsObservable().Subscribe(x =>
        {
            if (bCorrect) return;

            StartCoroutine(BtnOnClick());
        }).AddTo(disposables);
        
        // 12개 모두 오픈 이벤트 수신
        MessageBroker.Default.Receive<AllClear>().Subscribe(evt =>
        {
            CardClear();
        }).AddTo(disposables);
    }

    public void CardClear()
    {
        Animate(false, true);
        cardIndex = -1;
        bCorrect = false;
        bIsOpen = false;
    }

    public void Reset()
    {
        cardIndex = -1;
        bCorrect = false;
        bIsOpen = false;

        transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    /// <summary>
    /// 카드 세팅
    /// </summary>
    public void Init()
    {
        fCardSprite.material = new Material(mat)
        {
            mainTexture = CardSpriteList._instance.GetFrontCardSprite(cardIndex)
        };
    }

    IEnumerator BtnOnClick()
    {
        yield return null;

        if (!bIsOpen)
        {
            yield return StartCoroutine(AnimOpen());
        }
        
        // 카드정보 전달 이벤트 송신
        MessageBroker.Default.Publish(new CardOnSelect()
        {
            nSelectIndex = cardNumber
        });
    }

    public void Animate(bool _isOpen, bool _isClear = false)
    {
        if(_isOpen)
            StartCoroutine(AnimOpen());
        else
            StartCoroutine(AnimClose(_isClear));
    }

    public void ShowAllAnimation()
    {
        StartCoroutine(ShowAll());
    }

    private IEnumerator AnimOpen()
    {
        if (bIsOpen) yield break;
        yield return null;
        ani.Play("OpenCard_Ani");
        bIsOpen = true;

        while (ani.isPlaying)
        {
            yield return null;
        }
    }

    private IEnumerator AnimClose(bool _isClear = false)
    {
        if (!bIsOpen) yield break;
        yield return null;
        ani.Play("CloseCard_Ani");
        bIsOpen = false;
            
        while (ani.isPlaying)
        {
            yield return null;
        }

        if (_isClear)
        {
            MessageBroker.Default.Publish(new CardInitComplete());
        }
            
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private IEnumerator ShowAll()
    {
        GetComponent<Button>().interactable = false;
        yield return new WaitForSeconds(1);
        
        ani.Play("OpenCard_Ani");
        bIsOpen = true;

        while (ani.isPlaying)
        {
            yield return null;
        }

        yield return new WaitForSeconds(1);
        
        ani.Play("CloseCard_Ani");
        bIsOpen = false;
            
        while (ani.isPlaying)
        {
            yield return null;
        }
        GetComponent<Button>().interactable = true;
    }

    private void OnDestroy()
    {
        disposables.Dispose();
        disposables = null;
    }
}