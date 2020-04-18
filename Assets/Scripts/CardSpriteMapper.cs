using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSpriteMapper : MonoBehaviour
{
    public Sprite[] cardSprite;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void mapCard(GameObject cardGameObject){ // map rank,suit of a card to card sprites index
        int index = 0;
        CARD_RANK rank = cardGameObject.GetComponent<CardControl>().getRank();
        CARD_SUIT suit = cardGameObject.GetComponent<CardControl>().GetSuit();

        index += 13 * ((int)suit);
        index += (int)rank;

        cardGameObject.GetComponent<SpriteRenderer>().sprite = cardSprite[index];
    }
}
