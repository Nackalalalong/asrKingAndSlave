using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card 
{

    private CARD_RANK rank;
    private CARD_SUIT suit;

    public Card(CARD_RANK rank, CARD_SUIT suit){
        this.rank = rank;
        this.suit = suit;
    }

    public CARD_RANK getRank(){
        return this.rank;
    }

    public CARD_SUIT getSuit(){
        return this.suit;
    }
}
