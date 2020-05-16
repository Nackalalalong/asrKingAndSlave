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

    public float dealingCardSpeed = 10.0f;
    public float dealingCardDeltaTime = 0.1f;

    public GameObject sampleCard;
    public Transform player1Hand, player2Hand, player3Hand, player4Hand; 

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

        InitDeck();
    }   

    private void InitDeck(){
        foreach(CARD_RANK rank in System.Enum.GetValues(typeof(CARD_RANK))){
            foreach(CARD_SUIT suit in System.Enum.GetValues(typeof(CARD_SUIT))){
                deck.Add(new Card(rank,suit));
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if ( Input.GetKeyDown(KeyCode.T) ){
            Debug.Log("start dealing cards");
            StartCoroutine(DealCards());
        }
    }

    public void AnimateDealCard(int player){
        GameObject dealingCard = Instantiate(sampleCard);
        SampleCardController sampleCardController = dealingCard.GetComponent<SampleCardController>();
        if ( player % 2 == 0 ){
            dealingCard.transform.localEulerAngles = new Vector3(0,0,90);
        }
        switch (player)
        {
            case 1: 
                sampleCardController.SetTarget(player1Hand);
                break;
            case 2:
                sampleCardController.SetTarget(player2Hand);
                break;
            case 3:
                sampleCardController.SetTarget(player3Hand);
                break;
            case 4:
                sampleCardController.SetTarget(player4Hand);
                break;
        }
        sampleCardController.SetSpeed(dealingCardSpeed);
        dealingCard.SetActive(true);
    }

    private IEnumerator DealCards(){  // แจกไพ่ตอนต้นเกม
        int player = 1;
        System.Random rnd = new System.Random();
        while( deck.Count > 0 ){

            yield return new WaitForSeconds(dealingCardDeltaTime);
            Debug.Log("remaining " + deck.Count);
            int index = rnd.Next(0, deck.Count);
            Card card = deck[index];

            AnimateDealCard(player);

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

    public void LongCard( Card card){   // ลงการ์ด
        board.Add(card);
    }

    private void EndTurn(){

    }


}
