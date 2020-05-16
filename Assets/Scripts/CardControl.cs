﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardControl : MonoBehaviour
{
    private Card card;
    private bool isFacing = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public CARD_RANK GetRank(){
        return card.getRank();
    }

    public CARD_SUIT GetSuit(){
        return card.getSuit();
    }

    public bool IsFacing(){
        return isFacing;
    }
}