using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class CardMatchManager : MonoBehaviour
{
    [SerializeField] private CardObject cardObject;
    
    [SerializeField] private int playTime;
    [SerializeField] private int score;
    [SerializeField] private int comboCount;

    [SerializeField] private int maxMatchCount;

    private CardMatchState curCardMatchState = CardMatchState.None;

    public static CardMatchManager instance;
    

    public enum CardMatchState
    {
        None,
        Init,
        Reset,
        CheckMatch,
        Success,
        Failure,
        Result,
        End,
        
    }

    private void Awake()
    {
        if (null == instance)
            instance = this;
    }

    public void SetCardMatchState(CardMatchState state)
    {
        if (curCardMatchState.Equals(state))
            return;

        curCardMatchState = state; 
        switch(state)
        {
            case CardMatchState.None:
                break;
            case CardMatchState.Init:
                CreateCard();
                break;
            case CardMatchState.Reset:
                ShuffleCard();
                break;
            case CardMatchState.CheckMatch:
                CheckMatch();
                break;
            case CardMatchState.Success:
                Success();
                break;
            case CardMatchState.Failure:
                Failure();
                break;
            case CardMatchState.Result:
                Result();
                break;
            case CardMatchState.End:
                End();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    private void CreateCard()
    {
        
    }

    private void ShuffleCard()
    {
        
    }

    private void CheckMatch()
    {
        
    }

    private void Success()
    {
        
    }

    private void Failure()
    {
        
    }

    private void Result()
    {
        
    }

    private void End()
    {
        
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
