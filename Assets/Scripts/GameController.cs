using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CARD_RANK {
    THREE, FOUR, FIVE, SIX, SEVEN, EIGHT, NINE, TEN, JACK, QUEEN, KING, ACE, TWO, 
}

public enum CARD_SUIT {
    SPRADE, DIAMOND, HEART, CLUB
}

public class GameController : MonoBehaviour
{

    private const int card_count = 52;

    private List<Card> deck;
    public List<GameObject> player1, player2, player3, player4;
    private List<GameObject> board;
    private CardSpriteMapper spriteMapper;

    public float dealingCardSpeed = 10.0f;
    public float dealingCardDeltaTime = 0.1f;
    public float cardInHandOffset = 0.5f;

    public GameObject sampleCard;
    public GameObject deckForDeal;
    public Transform player1Hand, player2Hand, player3Hand, player4Hand; 

    private int turn = 1; // 1 is player, 2,3,4 are bot

    // Start is called before the first frame update
    void Start()
    {
        board = new List<GameObject>();
        player1 = new List<GameObject>();
        player2 = new List<GameObject>();
        player3 = new List<GameObject>();
        player4 = new List<GameObject>();
        deck = new List<Card>();

        spriteMapper = GetComponent<CardSpriteMapper>();

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

    public void AnimateDealCard(int player, Card card){
        GameObject dealingCard = Instantiate(sampleCard);
        CardController cardController = dealingCard.GetComponent<CardController>();
        cardController.SetCard(card);
        spriteMapper.MapCard(dealingCard, player);
        if ( player % 2 == 0 ){
            dealingCard.transform.localEulerAngles = new Vector3(0,0,90);
        }
        switch (player)
        {
            case 1: 
                cardController.SetTarget(player1Hand.position);
                player1.Add(dealingCard);
                break;
            case 2:
                cardController.SetTarget(player2Hand.position);
                player2.Add(dealingCard);
                break;
            case 3:
                cardController.SetTarget(player3Hand.position);
                player3.Add(dealingCard);
                break;
            case 4:
                cardController.SetTarget(player4Hand.position);
                player4.Add(dealingCard);
                break;
        }
        cardController.SetSpeed(dealingCardSpeed);
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

            if ( deck.Count == 1){
                deckForDeal.SetActive(false);
            }

            AnimateDealCard(player, card);

            player = ( player % 4 ) + 1;
            deck.RemoveAt(index);
        }

        SortCardsInHand(player1, player1Hand, 1);
        SortCardsInHand(player2, player2Hand, 2);
        SortCardsInHand(player3, player3Hand, 3);
        SortCardsInHand(player4, player4Hand, 4);
    }

    private void SortCardsInHand(List<GameObject> hand, Transform midTf, int player){
        hand.Sort((x,y) => {
            CardController controllerX = x.GetComponent<CardController>();
            CardController controllerY = y.GetComponent<CardController>();

            CARD_RANK rankX = controllerX.GetRank();
            CARD_SUIT suitX = controllerX.GetSuit();
            CARD_RANK rankY = controllerY.GetRank();
            CARD_SUIT suitY = controllerY.GetSuit();

            int res = 0;

            if ( rankX > rankY ){
                res = 1;
            }
            else if ( rankX == rankY ){
                if ( suitX > suitY){
                    res = 1;
                }
                else if ( suitX < suitY ){
                    res = -1;
                }
            }
            else {
                res = -1;
            }

            return res;
        });

        // set sort layer order and spread cards in hand
        int count = hand.Count;
        int mid = count / 2;
        Vector3 mostLeftCenter;
        if ( player % 2 == 1){
            mostLeftCenter = midTf.position - (new Vector3(cardInHandOffset * mid, 0, 0));
        }
        else {
            mostLeftCenter = midTf.position - (new Vector3(0, cardInHandOffset * mid, 0));
        }
        
        for(int index=0; index<count; ++index){
            GameObject gameObject = hand[index];
            gameObject.GetComponent<SpriteRenderer>().sortingOrder = index;

            CardController cardController = gameObject.GetComponent<CardController>();
            Vector3 targetPos;
            if ( player % 2 == 1){
                targetPos = mostLeftCenter + (new Vector3(cardInHandOffset * ( index - 0 ), 0, 0));
            }
            else {
                targetPos = mostLeftCenter + (new Vector3(0, cardInHandOffset * ( index - 0 ), 0));
            }
            
            cardController.SetTarget(targetPos);
        }

    }

    public void LongCard( GameObject card){   // ลงการ์ด
        board.Add(card);
    }

    private void EndTurn(){

    }

    public string GetCommand(){

        

        return "";
    }


}
