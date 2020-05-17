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

    public void MapCard(GameObject cardGameObject, int player){ // map rank,suit of a card to card sprites index
        CardController cardControl = cardGameObject.GetComponent<CardController>();

        int index = 0;
        int rank = (int)cardControl.GetRank();
        CARD_SUIT suit = cardControl.GetSuit();

        switch (suit)
        {
            case CARD_SUIT.HEART:
                index = 0 ;
                break;
            case CARD_SUIT.CLUB:
                index = 13;
                break;
            case CARD_SUIT.DIAMOND:
                index = 26;
                break;
            case CARD_SUIT.SPRADE:
                index = 39;
                break;
        }

        rank += 2; // refered to enum
        if ( rank > 12 ){
            rank -= 13;
        }
        
        index += rank;
        if ( player != 1){
            cardControl.SetFace(false);
        }
        else {
            cardControl.SetFace(true);
        }
        if ( !cardControl.IsFacing() ){
            cardGameObject.GetComponent<SpriteRenderer>().sprite = backCard;
            cardGameObject.transform.localScale = new Vector3(0.35f, 0.35f, 1);
        }
        else {
            cardGameObject.GetComponent<SpriteRenderer>().sprite = cardSprite[index];
            cardGameObject.transform.localScale = new Vector3(0.5f, 0.5f, 1);
        }

    }
}
