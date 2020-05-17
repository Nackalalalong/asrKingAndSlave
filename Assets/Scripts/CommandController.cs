using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum PUT_TYPE {
    SOLO, PAIR, TRIPPLE, QUARD
}

public class CommandController : MonoBehaviour
{
    private string command;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PutDecision(){ // return true if commmand valid for the purpose
        
    }

    public List<int> GetPutDecision()  // PUT_TYPE, RANK, SUIT
    {
        return new List<int>{1,2,3};
    }

    public void PutOrPass(){ // return true if commmand valid for the purpose
        
    }

    public bool IsPutCommand(){
        return true;
    }

    public bool IsPassCommand(){
        return true;
    }

    private void ShowSpeakAgain(){

    }

}
