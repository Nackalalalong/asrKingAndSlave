using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CARD_RANK {
    THREE, FOUR, FIVE, SIX, SEVEN, EIGHT, NINE, TEN, JACK, QUEEN, KING, ACE, TWO, 
}

public enum CARD_SUIT {
    SPRADE, DIAMOND, HEART, CLUB
}

enum PUT_TYPE {
    SOLO, PAIR, TRIPPLE, QUAD, ANY
}

public class GameController : MonoBehaviour
{

    private const int card_count = 52;

    private List<Card> deck;
    public List<GameObject> player1, player2, player3, player4;
    private List<GameObject> board;
    private List<bool> playerPass;
    private CardSpriteMapper spriteMapper;
    private CommandController commandController;
    private int turn = 1; // 1 is player, 2,3,4 are bot
    private int turnState = 0; // 0 -> putOrPass, 1 -> waitForPutOrPass, 2 -> putDecision, 3 -> waitForPutDecision 
    private bool isPlaying = false;
    private bool isBotPlaying = false;
    private bool isGameOver = false;
    private int winner = 1;
    private  PUT_TYPE boardPutType = PUT_TYPE.ANY;

    public float dealingCardSpeed = 10.0f;
    public float dealingCardDeltaTime = 0.1f;
    public float cardInHandOffset = 0.5f;
    public float startTurnDelayTime = 1.5f;

    public GameObject sampleCard;
    public GameObject deckForDeal;
    public Transform player1Hand, player2Hand, player3Hand, player4Hand; 
    public Transform boardCenter;
    public GameObject arrowPlayer1, arrowPlayer2, arrowPlayer3, arrowPlayer4;

    // Start is called before the first frame update
    void Start()
    {
        board = new List<GameObject>();
        player1 = new List<GameObject>();
        player2 = new List<GameObject>();
        player3 = new List<GameObject>();
        player4 = new List<GameObject>();
        deck = new List<Card>();
        playerPass = new List<bool>{false, false, false, false};

        spriteMapper = GetComponent<CardSpriteMapper>();
        commandController = GetComponent<CommandController>();

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

        if ( isGameOver ){

        }
        else {
            if ( isPlaying ){
                if ( turn == 1 ){
                    Play();
                }
                else if ( !isBotPlaying ){
                    isBotPlaying = true;
                    StartCoroutine(BotPlay());
                }
            }
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

        isPlaying = true;
    }

    private void SortCardsInHand(List<GameObject> hand, Transform midTf, int player){
        hand.Sort(CardComparator);

        SpacingCards(hand, midTf, player);
    }

    private void SpacingCards(List<GameObject> cards, Transform midTf, int player = 1, int sortOrderStart = 0){
        // set sort layer order and spread cards in hand
        int count = cards.Count;
        int mid = count / 2;
        Vector3 mostLeftCenter;
        if ( player % 2 == 1){
            mostLeftCenter = midTf.position - (new Vector3(cardInHandOffset * mid, 0, 0));
        }
        else {
            mostLeftCenter = midTf.position - (new Vector3(0, cardInHandOffset * mid, 0));
        }
        
        for(int index=0; index<count; ++index){
            GameObject gameObject = cards[index];
            gameObject.GetComponent<SpriteRenderer>().sortingOrder = index + sortOrderStart;

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

    private int CardComparator(GameObject x, GameObject y){ // 1 -> x > y
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
    }

    private void Play(){
        if ( playerPass[turn-1] ){
            ShowPlayerHasPassed();
            GoNextTurn();
        }
        else if ( AreOthersAllPass() ){
            NewRound();
        }
        else {
            arrowPlayer1.SetActive(true);
            if ( turnState == 0 ){
                commandController.PutOrPass();
                turnState = 1;
                Debug.Log("your turn, press 1 to Put press 2 to pass");
            }
            else if ( turnState == 1 ){
                if ( commandController.IsPutCommand() ){
                    turnState = 2;
                    Debug.Log("press 3 to put the decision");
                }
                else if ( commandController.IsPassCommand() ){
                    playerPass[turn-1] = true;
                    GoNextTurn();
                }
                // wait for command
            } 
            else if ( turnState == 2 ){
                commandController.PutDecision();
                turnState = 3;
            }
            else if ( turnState == 3 && commandController.IsPutDecisionReady() ){
                Debug.Log("processing turn state 3");
                List<int> putDecision = commandController.GetPutDecision();
                if ( !IsPutDecisionValid(putDecision) ){
                    Debug.Log("decision invalid");
                    ShowPutDecisionInvalid();
                    turnState = 2;
                }
                else {
                    Debug.Log("decision valid");
                    boardPutType = (PUT_TYPE)putDecision[0];
                    PutCard(putDecision);
                    GoNextTurn();
                }
            } 
            
        }
    }

    private IEnumerator BotPlay(){

        switch (turn)
        {
            case 1:
                arrowPlayer1.SetActive(true);
                break;
            case 2:
                arrowPlayer2.SetActive(true);
                break;
            case 3:
                arrowPlayer3.SetActive(true);
                break;
            case 4:
                arrowPlayer4.SetActive(true);
                break;
        }

        yield return new WaitForSeconds(startTurnDelayTime); 

        if ( playerPass[turn-1] ){
            ShowPlayerHasPassed();
            GoNextTurn();
        }
        else if ( AreOthersAllPass() ){
            NewRound();
        }
        else {
            Debug.Log("Bot play " + turn);
           

            List<int> putDecision = new List<int>();
            List<GameObject> hand = GetHand();
            
            if ( boardPutType == PUT_TYPE.ANY ){
                
                boardPutType = PUT_TYPE.SOLO;
                putDecision.Add(0); // putType solo
                putDecision.Add((int)hand[0].GetComponent<CardController>().GetRank());
                putDecision.Add(-1);

                PutCard(putDecision);
                GoNextTurn();
            }
            else {
                List<int> decision = GetMinimumPutDecision();

                if ( decision == null ){
                    Debug.Log("Bot " + turn + " pass");
                    playerPass[turn-1] = true;
                    GoNextTurn();
                }
                else {
                    PutCard(decision);
                    GoNextTurn();
                }
                
            }
        }

        
    }

    private List<int> GetMinimumPutDecision(){
        List<GameObject> hand = GetHand();
        List<int> decision;
        
        for(int rank=(int)hand[0].GetComponent<CardController>().GetRank();
                rank<(int)hand[hand.Count-1].GetComponent<CardController>().GetRank();
                ++rank){
                    decision = new List<int>{(int)boardPutType, rank, -1};
                    if ( IsPutDecisionValid(decision) ){
                        return decision;
                    }
                }

        return null;
    }

    private void ShowPutDecisionInvalid(){

    }

    private bool IsPutDecisionValid(List<int> putDecision){

        List<GameObject> hand = GetHand();

        bool legal = IsDecisionLegal(putDecision, hand);
        bool enough = HasEnoughCard(putDecision, hand);

        return legal && enough; 
    }   

    private bool IsDecisionLegal(List<int> putDecision, List<GameObject> hand){

        if ( boardPutType == PUT_TYPE.ANY ){
            return true;
        }

        int putType = putDecision[0];
        int rank = putDecision[1];
        int suit = putDecision[2];

        if ( boardPutType == PUT_TYPE.SOLO && putType == (int)PUT_TYPE.TRIPPLE ){
           return true;
        }
        else if ( boardPutType == PUT_TYPE.PAIR && putType == (int)PUT_TYPE.QUAD ){
            return true;
        }
        else if ( (int)boardPutType != putType ){
            return false;
        }

        // ลงเหมือนกัน
        CARD_RANK topBoardRank = board[board.Count-1].GetComponent<CardController>().GetRank();
        CARD_SUIT topBoardSuit = board[board.Count-1].GetComponent<CardController>().GetSuit();

        if ( (int)topBoardRank > rank ){    // ลงเลขต่ำกว่าบอร์ด
            return false;   
        }
        else if ( (int)topBoardRank < rank ){   // ลงเลขสูงกว่า
            return true; 
        }

        // ลงเลขเท่ากัน
        CARD_SUIT playerMaxSuit = CARD_SUIT.SPRADE;
        for(int i=0; i<hand.Count; ++i){
            CardController controller = hand[i].GetComponent<CardController>();
            if ( (int)controller.GetRank() == rank ){
                if ( controller.GetSuit() > playerMaxSuit ){
                    playerMaxSuit = controller.GetSuit();
                }
            }
        }

        if ( playerMaxSuit > topBoardSuit ){
            return true;
        }

        return false;
    }

    private bool HasEnoughCard(List<int> putDecision, List<GameObject> hand){ // type, rank, suit

        int putType = putDecision[0];
        int rank = putDecision[1];
        int suit = putDecision[2];

        if ( putType == (int)PUT_TYPE.SOLO ){
            return CountCardInHand(hand, rank, suit) == 1;
        }
        else if ( putType == (int)PUT_TYPE.PAIR ){
            return CountCardInHand(hand, rank) > 1;
        }
        else if ( putType == (int)PUT_TYPE.TRIPPLE ){
            return CountCardInHand(hand, rank) > 2;
        }
        else if ( putType == (int)PUT_TYPE.QUAD ){
            return CountCardInHand(hand, rank) == 4;
        }

        return false;
    }

    private int CountCardInHand(List<GameObject> hand, int rank, int suit = -1){
        int count = 0;
        for(int i=0; i<hand.Count; ++i){
            CardController controller = hand[i].GetComponent<CardController>();
            if ( (int)controller.GetRank() == rank ){
                if ( suit >= 0 && (int)controller.GetSuit() == suit){
                    count += 1;
                }
                else if ( suit < 0) {
                    count +=1 ;
                }
            }
        }

        return count;
    }

    private void GoNextTurn(){
            switch (turn)
            {
                case 1:
                    arrowPlayer1.SetActive(false);
                    break;
                case 2:
                    arrowPlayer2.SetActive(false);
                    break;
                case 3:
                    arrowPlayer3.SetActive(false);
                    break;
                case 4:
                    arrowPlayer4.SetActive(false);
                    break;
            }
            turn = (turn % 4) + 1;
            turnState = 0;
            isBotPlaying = false;
            Debug.Log("go next turn: " + turn);
    }

    private void ShowPlayerHasPassed(){

    }

    private bool AreOthersAllPass(){
        // Debug.Log(playerPass[0] + " " + playerPass[1] + " " + playerPass[2] + " " + playerPass[3] + " ");
        return !playerPass[turn-1] && playerPass[(turn)%4] && playerPass[(turn+1)%4] && playerPass[(turn+2)%4];
    }

    private void NewRound(){
        Debug.Log("new round");
        for(int i=0; i<4; ++i){
            playerPass[i] = false;
        }
        turnState = 0;
        boardPutType = PUT_TYPE.ANY;
        for(int index=board.Count - 1; index>=0; --index){
            Destroy(board[index]);
        }
        board.Clear();
        isBotPlaying = false;
    }

    private void PutCard(List<int> putDecision){
        List<GameObject> cardsToPut = GetCardsDecision(putDecision);
        cardsToPut.Sort(CardComparator);
        Debug.Log("cardsToPut count: " + cardsToPut.Count);

        for(int i=0; i<board.Count; ++i){
            board[i].GetComponent<SpriteRenderer>().sortingOrder = -2;
        }

        for(int i=0; i<cardsToPut.Count; ++i){
            cardsToPut[i].transform.localEulerAngles = new Vector3(0,0,0);
            spriteMapper.MapCard(cardsToPut[i], 1);
            board.Add(cardsToPut[i]);
        }
        SpacingCards(cardsToPut, boardCenter);

         for(int i=0; i<board.Count; ++i){
            board[i].GetComponent<SpriteRenderer>().sortingOrder = i;
        }

        CheckWinner();
        SpacingCards(GetHand(), GetHandCenter(), turn);
    }

    private void CheckWinner(){
        List<GameObject> hand = GetHand();
        if ( hand.Count == 0 ){
            isGameOver = true;
            winner = turn;
            Debug.Log("GameOver winner is " + winner);
        }
    }

    private List<GameObject> GetCardsDecision(List<int> putDecision){   // assume that pusDecision is valid

        List<GameObject> hand = GetHand();

        int putType = putDecision[0];
        int rank = putDecision[1];
        int suit = putDecision[2];

        List<GameObject> cardsToPut = new List<GameObject>();
        List<int> indexes = new List<int>();

        for(int index=0; index<hand.Count; ++index){
            CardController controller = hand[index].GetComponent<CardController>();
            if ( rank == (int)controller.GetRank() ){
                indexes.Add(index);
            }
        }

        while( indexes.Count > putType + 1 ){
            indexes.RemoveAt(indexes.Count - 1);
        }

        for(int i=indexes.Count - 1; i>=0; --i){
            int index = indexes[i];
            cardsToPut.Add(hand[index]);
            hand.RemoveAt(index);
        }

        return cardsToPut;

    }

    private List<GameObject> GetHand(){
        List<GameObject> hand = player1;
        switch (turn)
        {
            case 1 : 
                hand = player1;
                break;
            case 2 :
                hand = player2;
                break;
            case 3 :
                hand = player3;
                break;
            case 4 : 
                hand = player4;
                break;
        }

        return hand;
    }

    private Transform GetHandCenter(){
        Transform midTf = player1Hand;
        switch (turn)
        {
            case 1 : 
                midTf = player1Hand;
                break;
            case 2 :
                midTf = player2Hand;
                break;
            case 3 :
                midTf = player3Hand;
                break;
            case 4 : 
                midTf = player4Hand;
                break;
        }

        return midTf;
    }


}
