using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandController : MonoBehaviour
{
    private string command;

    private int testInput = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void PutDecision(){ // return true if commmand valid for the purpose
        StartCoroutine(waitForKeyPress());
    }

    public bool IsPutDecisionReady(){
        if ( testInput == 3){
            testInput = 0;
            Debug.Log("decision ready");
            return true;
        }
        return false;
    }

    public List<int> GetPutDecision()  // PUT_TYPE, RANK, SUIT
    {
        CommandTester tester =  GetComponent<CommandTester>();
        return new List<int>{tester.putType,tester.rank,tester.suit};
    }

    public void PutOrPass(){ // return true if commmand valid for the purpose
        StartCoroutine(waitForKeyPress());
    }

    public bool IsPutCommand(){
        if ( testInput == 1){
            testInput = 0;
            return true;
        }
        return false;
    }

    public bool IsPassCommand(){
        if ( testInput == 2){
            testInput = 0;
            return true;
        }
        return false;
    }

    private void ShowSpeakAgain(){
        Debug.Log("speak again");
    }

    private IEnumerator waitForKeyPress(){
        bool done = false;
        while(!done) // essentially a "while true", but with a bool to break out naturally
        {
            if(Input.GetKeyDown(KeyCode.Alpha1))
            {
                testInput = 1;
                Debug.Log("test input 1");
                done = true; // breaks the loop
            }
            else if ( Input.GetKeyDown(KeyCode.Alpha2)){
                testInput = 2;
                Debug.Log("test input 2");
                done = true; // breaks the loop
            }
            else if ( Input.GetKeyDown(KeyCode.Alpha3) ){
                testInput = 3;
                Debug.Log("test input 3");
                done = true; // breaks the loop
            }
            yield return null; // wait until next frame, then continue execution from here (loop continues)
        }
    
        // now this function returns
    }

}
