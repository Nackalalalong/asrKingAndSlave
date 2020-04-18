using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSpriteMapper : MonoBehaviour
{
    public Sprite[] cardSprite;
    public Sprite backCard;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MapCard(GameObject cardGameObject){ // map rank,suit of a card to card sprites index

        CardControl cardControl = cardGameObject.GetComponent<CardControl>();
        if ( !cardControl.IsFacing() ){
            cardGameObject.GetComponent<SpriteRenderer>().sprite = backCard;
        }

        int index = 0;
        CARD_RANK rank = cardControl.GetRank();
        CARD_SUIT suit = cardControl.GetSuit();

        index += 13 * ((int)suit);
        index += (int)rank;

        cardGameObject.GetComponent<SpriteRenderer>().sprite = cardSprite[index];
    }
}
