using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommandController : MonoBehaviour
{
    private string command;

    private int testInput = 0;
    bool done;
    bool forPutOrPass = true;
    private bool isGowajee = false;
    public Text commander;
    public Image commanderBackground;
    public GameObject microphoneObject;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void Gowajee(){
        Debug.Log("go wa jee");
        isGowajee = true;
        microphoneObject.GetComponent<MicrophoneHandler>().Gowajee();
        StartCoroutine(WaitForCommand());
    }

    public bool IsGowajeeDone(){
        if ( done ){
            done = false;
            isGowajee = false;
            return true;
        }
        return false;
    }

    public string GetGowajee(){
        return command;
    }

    public void PutDecision(){ // return true if commmand valid for the purpose
        forPutOrPass = false;
        commander.text = "เลือกไพ่ลง หรือ \"ผ่าน\"";
        StartCoroutine(WaitForCommand());
    }

    public bool IsPutDecisionReady(bool test = false){
        if ( test ){
            if ( testInput == 3){
                testInput = 0;
                Debug.Log("decision ready");
                return true;
            }
            return false;
        }

        if ( done ){
            done = false;
            return true;
        }

        return false;
    }

    public List<int> GetPutDecision(bool test = false)  // PUT_TYPE, RANK, SUIT
    {
        if ( test ){
            CommandTester tester =  GetComponent<CommandTester>();
            int putType = (int)tester.putType;
            int rank = (int)tester.rank;
            int suit1 = (int)tester.suit1;
            int suit2 = (int)tester.suit2;
            int suit3 = (int)tester.suit3;
            
            switch (putType)
            {
                case (int)PUT_TYPE.SOLO:
                    suit2 = 1; suit3 = -1; break;
                case (int)PUT_TYPE.PAIR:
                    suit3 = -1; break;
            }

            return new List<int>{putType, rank, suit1, suit2, suit3};   
        }

        if ( command == "ผ่าน" || command == "ข้าม" ){
            return new List<int>{-1};
        }
        
        return ParseCommand();
    }

    public void PutOrPass(){ // return true if commmand valid for the purpose
        forPutOrPass = true;
        commander.text = "\"ลง\" หรือ \"ผ่าน\"";
        StartCoroutine(WaitForCommand());
    }

    public bool IsPutCommand(bool test = false){
        if ( test ){
                if ( testInput == 1){
                testInput = 0;
                return true;
            }
            return false;
        }
        
        if ( command == "ลง" ){
            command = "";
            return true;
        }

        return false;
    }

    public bool IsPassCommand(bool test = false){
        if ( test ){
                if ( testInput == 1){
                testInput = 0;
                return true;
            }
            return false;
        }
        
        if ( command == "ข้าม" || command == "ผ่าน" ){
            command = "";
            return true;
        }

        return false;
    }

    private void ShowSpeakAgain(){
        Debug.Log("speak again");
        if ( forPutOrPass ){
            commander.text = "พูดอีกครั้ง: \"ลง\" หรือ \"ผ่าน\"";
        }
        else {
            commander.text = "พูดอีกครั้ง: เลือกไพ่ลง หรือ \"ผ่าน\"";
        }
        commanderBackground.color = Color.red;
    }

    private IEnumerator WaitForCommand(){
        done = false;
        while(!done) // essentially a "while true", but with a bool to break out naturally
        {
            yield return null; // wait until next frame, then continue execution from here (loop continues)
        }
    }

    public void SetCommand(string command){
        command = command.Trim();
        this.command = command;
        done = true;

        if ( isGowajee ){
            if ( !command.Contains("โกวาจี") ){
                Debug.Log("command is not go wa jee : " + command);
                done = false;
                command = "";
                Gowajee();
            }
            return ;
        }

        else if ( forPutOrPass ){
            if ( !IsPutCommand() && !IsPassCommand() ){
                done = false;
                command = "";
                ShowSpeakAgain();
            }
        }
        else {
            if ( ParseCommand() == null && (command != "ผ่าน" && command != "ข้าม") ){
                done = false;
                command = "";
                ShowSpeakAgain();
            }
        }
    }

    private List<int> ParseCommand(){

        string[] texts = command.Trim().Split(' ');
        Debug.Log("parse command texts " +command );

        if ( texts[0] == "คู่"){
            if ( texts.Length != 4 ){
                return null;
            }
            return new List<int>{
                (int)PUT_TYPE.PAIR, (int)MapRank(texts[1]), 
                (int)(MapSuit(texts[2])), (int)MapSuit(texts[3]), -1};
        }
        else if ( texts[0] == "ตอง" ){
            if ( texts.Length != 5 ){
                return null;
            }
            return new List<int>{
                (int)PUT_TYPE.TRIPPLE, (int)MapRank(texts[1]), 
                (int)(MapSuit(texts[2])), (int)MapSuit(texts[3]), 
                (int)MapSuit(texts[4])};
        }
        else if ( texts[0] == "โฟธ" ){
            if ( texts.Length != 2 ){
                return null;
            }
            return new List<int>{(int)PUT_TYPE.QUAD, (int)MapRank(texts[1]), -1, -1, -1};
        }
        // else {
        //     if ( texts.Length != 3 ){
        //         return null;
        //     }
        //     return new List<int>{
        //         (int)PUT_TYPE.SOLO, (int)MapRank(texts[1]),
        //         (int)MapSuit(texts[2]), -1, -1};
        // }
        else {
            if ( texts.Length != 2 ){
                return null;
            }
            return new List<int>{
                (int)PUT_TYPE.SOLO, (int)MapRank(texts[0]),
                (int)MapSuit(texts[1]), -1, -1};
        }
    }

    private CARD_RANK MapRank(string rank){
        switch (rank)
        {
            case "สาม":
                return CARD_RANK.THREE;
            case "สี่":
                return CARD_RANK.FOUR;
            case "ห้า":
                return CARD_RANK.FIVE;
            case "หก":
                return CARD_RANK.SIX;
            case "เจ็ด":
                return CARD_RANK.SEVEN;
            case "แปด":
                return CARD_RANK.EIGHT;
            case "เก้า":
                return CARD_RANK.NINE;
            case "สิบ":
                return CARD_RANK.TEN;
            case "แจ็ค":
                return CARD_RANK.JACK;
            case "แหม่ม":
                return CARD_RANK.QUEEN;
            case "ควีน":
                return CARD_RANK.QUEEN;
            case "คิง":
                return CARD_RANK.KING;
            case "เอซ":
                return CARD_RANK.ACE;
            case "เอด":
                return CARD_RANK.ACE;
            case "สอง":
                return CARD_RANK.TWO;
            default:
                return CARD_RANK.TWO;
        }
    }

    private CARD_SUIT MapSuit(string suit){
        switch (suit)
        {
            case "ดอกจิก":
                return CARD_SUIT.SPRADE;
            case "ข้าวหลามตัด":
                return CARD_SUIT.DIAMOND;
            case "โพธิ์แดง":
                return CARD_SUIT.HEART;
            default:
                return CARD_SUIT.CLUB;
        }
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
