using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSpriteList : MonoBehaviour
{
    [SerializeField] private Texture[] bCardSprites;
    [SerializeField] private Texture[] fCardSprites;
    
    public static CardSpriteList _instance = null;

    private void Awake()
    {
        _instance = this;
    }

    // public static CardSpriteManager Instance {
    //     get { return _instance ??= new CardSpriteManager(); }
    // }

    public Texture GetBackCardSprite(int index)
    {
        return bCardSprites[index];
    }

    public Texture GetFrontCardSprite(int index)
    {
        return fCardSprites[index];
    }
}
