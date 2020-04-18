using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CARD_RANK {
    ACE, TWO, THREE, FOUR, FIVE, SIX, SEVEN, EIGHT, NINE, TEN, JACK, QUEEN, KING
}

public enum CARD_SUIT {
    DIAMOND, CLUB, HEART, SPRADE
}

public class GameControl : MonoBehaviour
{

    private const int card_count = 52;

    private List<Card> deck;
    public List<Card> player1, player2, player3, player4;
    private List<Card> board;

    private int turn = 1; // 1 is player, 2,3,4 are bot

    // Start is called before the first frame update
    void Start()
    {
        board = new List<Card>();
        player1 = new List<Card>();
        player2 = new List<Card>();
        player3 = new List<Card>();
        player4 = new List<Card>();
        deck = new List<Card>();

        initDeck();
    }   

    private void initDeck(){
        foreach(CARD_RANK rank in System.Enum.GetValues(typeof(CARD_RANK))){
            foreach(CARD_SUIT suit in System.Enum.GetValues(typeof(CARD_SUIT))){
                deck.Add(new Card(rank,suit));
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void dealCards(){  // แจกไพ่ตอนต้นเกม
        int player = 1;
        System.Random rnd = new System.Random();
        while( deck.Count > 0 ){
            int index = rnd.Next(0, deck.Count);
            Card card = deck[index];

            if ( player == 1 ){
                player1.Add(card);
            }
            else if ( player == 2 ){
                player2.Add(card);
            }
            else if ( player == 3 ){
                player3.Add(card);
            }
            else {
                player4.Add(card);
            }

            player = ( player % 4 ) + 1;
            deck.RemoveAt(index);
        }
    }

    public void longCard( Card card){   // ลงการ์ด
        board.Add(card);
    }

    private void endTurn(){

    }


}
