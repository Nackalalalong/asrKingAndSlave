﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CARD_RANK {
    THREE, FOUR, FIVE, SIX, SEVEN, EIGHT, NINE, TEN, JACK, QUEEN, KING, ACE, TWO, 
}

public enum CARD_SUIT {
    SPRADE, DIAMOND, HEART, CLUB
}

public enum PUT_TYPE {
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
    private bool doneGowajee = false;

    private int winner = 1;
    private  PUT_TYPE boardPutType = PUT_TYPE.ANY;
    private SoundController soundController;

    public float dealingCardSpeed = 10.0f;
    public float dealingCardDeltaTime = 0.1f;
    public float cardInHandOffset = 0.5f;
    public float startTurnDelayTime = 1.5f;
    public float newRoundDelayTime = 2.5f;

    public GameObject sampleCard;
    public GameObject deckForDeal;
    public Transform player1Hand, player2Hand, player3Hand, player4Hand; 
    public Transform boardCenter;
    public GameObject arrowPlayer1, arrowPlayer2, arrowPlayer3, arrowPlayer4;
    public GameObject passPlayer1, passPlayer2, passPlayer3, passPlayer4;
    public GameObject newRoundText;
    public GameObject gameOver;
    public GameObject[] playerSpeakObjects;

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
        soundController = GetComponent<SoundController>();

        InitDeck();
        // StartCoroutine(DealCards());
        commandController.Gowajee();
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
        // if ( Input.GetKeyDown(KeyCode.T) ){
        //     Debug.Log("start dealing cards");
        //     StartCoroutine(DealCards());
        // }
        if ( Input.GetKeyDown(KeyCode.W) ){
            isGameOver = true;
            winner = 1;
            ShowGameOver();
        }
        if ( Input.GetKeyDown(KeyCode.L) ){
            isGameOver = true;
            winner = 2;
            ShowGameOver();
        }

        if ( !doneGowajee && commandController.IsGowajeeDone() ){
            doneGowajee = true;
            playerSpeakObjects[4].GetComponent<UnityEngine.UI.Text>().text = "โกวาจี สลาฟ ออน";
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
        soundController.PlayCardMove();
    }

    private void AddCard(int player, CARD_RANK rank, CARD_SUIT suit){
        for(int i=0; i<deck.Count; ++i){
            if ( deck[i].getRank() == rank && deck[i].getSuit() == suit ){
                AnimateDealCard(player, deck[i]);
                deck.RemoveAt(i);
                return ;
            }
        }
    }

    private IEnumerator DealCards(){  // แจกไพ่ตอนต้นเกม
        yield return new WaitForSeconds(1.5f);
        int player = 1;
        AddCard(1, CARD_RANK.FOUR, CARD_SUIT.HEART);
        AddCard(2, CARD_RANK.FOUR, CARD_SUIT.SPRADE);
        AddCard(3, CARD_RANK.FOUR, CARD_SUIT.CLUB);
        AddCard(4, CARD_RANK.FOUR, CARD_SUIT.DIAMOND);
        AddCard(1, CARD_RANK.EIGHT, CARD_SUIT.HEART);
        AddCard(2, CARD_RANK.EIGHT, CARD_SUIT.SPRADE);
        AddCard(3, CARD_RANK.EIGHT, CARD_SUIT.CLUB);
        AddCard(4, CARD_RANK.EIGHT, CARD_SUIT.DIAMOND);

        AddCard(1, CARD_RANK.NINE, CARD_SUIT.HEART);
        AddCard(2, CARD_RANK.NINE, CARD_SUIT.SPRADE);
        AddCard(3, CARD_RANK.NINE, CARD_SUIT.CLUB);
        AddCard(4, CARD_RANK.NINE, CARD_SUIT.DIAMOND);

        AddCard(1, CARD_RANK.TWO, CARD_SUIT.HEART);
        AddCard(2, CARD_RANK.TWO, CARD_SUIT.SPRADE);
        AddCard(3, CARD_RANK.TWO, CARD_SUIT.CLUB);
        AddCard(4, CARD_RANK.TWO, CARD_SUIT.DIAMOND);
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

        soundController.PlayCardSpacing();

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
            StartCoroutine(NewRound());
        }
        else {
            arrowPlayer1.SetActive(true);
            foreach (GameObject gameObject in playerSpeakObjects){
                gameObject.SetActive(true);
            }
 
            if ( turnState == 0 ){
                turnState = 2;
                // commandController.PutOrPass();
                // Debug.Log("your turn, press 1 to Put press 2 to pass");
            }
            else if ( turnState == 1 ){
                if ( commandController.IsPutCommand() ){
                    turnState = 2;
                    Debug.Log("press 3 to put the decision");
                }
                else if ( commandController.IsPassCommand() ){
                    playerPass[turn-1] = true;
                    soundController.PlayAwww();
                    ShowPlayerHasPassed();
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
                Debug.Log("pudes " + putDecision[0]);
                if ( putDecision.Count == 1 && putDecision[0] == -1 ){ //pass
                    playerPass[turn-1] = true;
                    soundController.PlayAwww();
                    ShowPlayerHasPassed();
                    GoNextTurn();
                }
                else if ( !IsPutDecisionValid(putDecision) ){
                    Debug.Log("decision invalid");
                    ShowPutDecisionInvalid();
                    turnState = 0;
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

        if ( playerPass[turn-1] ){
            ShowPlayerHasPassed();
            GoNextTurn();
        }
        else if ( AreOthersAllPass() ){
            yield return NewRound();
            
        }
        else {
            Debug.Log("Bot play " + turn);
            foreach (GameObject gameObject in playerSpeakObjects){
                gameObject.SetActive(false);
            }

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
           

            List<int> putDecision = new List<int>();
            List<GameObject> hand = GetHand();
            
            if ( boardPutType == PUT_TYPE.ANY ){
                
                boardPutType = PUT_TYPE.SOLO;
                putDecision.Add(0); // putType solo
                putDecision.Add((int)hand[0].GetComponent<CardController>().GetRank());
                putDecision.Add((int)hand[0].GetComponent<CardController>().GetSuit());
                putDecision.Add(-1);
                putDecision.Add(-1);

                PutCard(putDecision);
                GoNextTurn();
            }
            else {
                List<int> decision = GetMinimumPutDecision();

                if ( decision == null ){
                    Debug.Log("Bot " + turn + " pass");
                    playerPass[turn-1] = true;
                    soundController.PlayAwww();
                    ShowPlayerHasPassed();
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

        for(int i=0; i<hand.Count; ++i){
            CardController controller = hand[i].GetComponent<CardController>();
            int rank = (int)controller.GetRank();
            int count = CountCardInHand(hand, rank);

            List<int> decision;

            if ( boardPutType == PUT_TYPE.SOLO && count >= 1){
                decision = new List<int>{(int)boardPutType, rank, (int)controller.GetSuit(), -1, -1};
            }
            else if ( boardPutType == PUT_TYPE.PAIR && count >= 2 && i < hand.Count - 1){
                decision = new List<int>{
                        (int)boardPutType, rank, (int)controller.GetSuit(), 
                        (int)hand[i+1].GetComponent<CardController>().GetSuit(), -1
                    };
            }
            else if ( boardPutType == PUT_TYPE.TRIPPLE && count >= 3 && i < hand.Count - 2){
                decision = new List<int>{
                        (int)boardPutType, rank, (int)controller.GetSuit(), 
                        (int)hand[i+1].GetComponent<CardController>().GetSuit(),
                        (int)hand[i+2].GetComponent<CardController>().GetSuit(),
                    };
            }
            else if( boardPutType == PUT_TYPE.QUAD && count >= 4 && i < hand.Count - 3){
                decision = new List<int>{
                        (int)boardPutType, rank, -1,-1,-1
                    };
            }
            else {
                continue;
            }

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

        Debug.Log(legal + " " + enough);

        return legal && enough; 
    }   

    private bool IsDecisionLegal(List<int> putDecision, List<GameObject> hand){

        if ( boardPutType == PUT_TYPE.ANY ){
            return true;
        }

        int putType = putDecision[0];
        int rank = putDecision[1];
        int playerMaxSuit = Mathf.Max(putDecision[2], putDecision[3], putDecision[4]);

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

        if ( playerMaxSuit > (int)topBoardSuit ){
            return true;
        }

        return false;
    }

    private bool HasEnoughCard(List<int> putDecision, List<GameObject> hand){ // type, rank, suit

        int putType = putDecision[0];
        int rank = putDecision[1];
        int suit1 = putDecision[2];
        int suit2 = putDecision[3];
        int suit3 = putDecision[4];

        if ( putType == (int)PUT_TYPE.SOLO ){   // player
            return CountCardInHand(hand, rank, suit1) == 1;
        }
        else if ( putType == (int)PUT_TYPE.PAIR ){
            return CountCardInHand(hand, rank, suit1) == 1 && CountCardInHand(hand, rank, suit2) == 1;
        }
        else if ( putType == (int)PUT_TYPE.TRIPPLE ){
            return CountCardInHand(hand, rank, suit1) == 1 && CountCardInHand(hand, rank, suit2) == 1 
                    && CountCardInHand(hand, rank, suit3) == 1;
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
        switch (turn)
            {
                case 1:
                    passPlayer1.SetActive(true);
                    break;
                case 2:
                    passPlayer2.SetActive(true);
                    break;
                case 3:
                    passPlayer3.SetActive(true);
                    break;
                case 4:
                    passPlayer4.SetActive(true);
                    break;
            }
    }

    private bool AreOthersAllPass(){
        // Debug.Log(playerPass[0] + " " + playerPass[1] + " " + playerPass[2] + " " + playerPass[3] + " ");
        return !playerPass[turn-1] && playerPass[(turn)%4] && playerPass[(turn+1)%4] && playerPass[(turn+2)%4];
    }

    private IEnumerator NewRound(){
        isPlaying = false;
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

        yield return new WaitForSeconds(startTurnDelayTime);
        newRoundText.SetActive(true);
        soundController.PlayNewRound();
        yield return new WaitForSeconds(newRoundDelayTime);
        newRoundText.SetActive(false); 

        passPlayer1.SetActive(false);
        passPlayer2.SetActive(false);
        passPlayer3.SetActive(false);
        passPlayer4.SetActive(false);
               
        isPlaying = true;
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
        soundController.PlayCardMove();

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
            
            ShowGameOver();
        }
    }

    private void ShowGameOver(){
        gameOver.GetComponent<GameOver>().SetWinner(winner);
        gameOver.SetActive(true);

        if ( winner == 1 ){
            soundController.PlayYouWin();
        }
        else {
            soundController.PlayYouLose();
        }
    }

    private List<GameObject> GetCardsDecision(List<int> putDecision){   // assume that pusDecision is valid

        List<GameObject> hand = GetHand();

        int putType = putDecision[0];
        int rank = putDecision[1];
        int suit1 = putDecision[2];
        int suit2 = putDecision[3];
        int suit3 = putDecision[4];

        List<GameObject> cardsToPut = new List<GameObject>();
        List<int> indexes = new List<int>();

        for(int index=0; index<hand.Count; ++index){
            CardController controller = hand[index].GetComponent<CardController>();
            int suit = (int)controller.GetSuit();
            if ( rank == (int)controller.GetRank() && putType == (int)PUT_TYPE.QUAD ){
                indexes.Add(index);
            }
            else if ( rank == (int)controller.GetRank() && (
                suit == suit1 || suit == suit2 || suit == suit3
            )){
                indexes.Add(index);
            }
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
