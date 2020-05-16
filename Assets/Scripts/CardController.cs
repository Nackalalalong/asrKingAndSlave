using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardController : MonoBehaviour
{
    private bool isFacing = true;

    private Card card;
    private Vector3 target;
    private float speed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if ( target != null ){
            float step =  speed * Time.deltaTime; // calculate distance to move
            transform.position = Vector3.MoveTowards(transform.position, target, step);   
        }
    }

    public CARD_RANK GetRank(){
        return card.getRank();;
    }

    public CARD_SUIT GetSuit(){
        return card.getSuit();
    }

    public void SetCard(Card card){
        this.card = card;
    }

    public bool IsFacing(){
        return isFacing;
    }

    public void SetFace(bool b){
        isFacing = b;
    }

    public void SetTarget(Vector3 target){
        this.target = target;
    }

    public void SetSpeed(float speed){
        this.speed = speed;
    }
}
