using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Threading;
using UnityEngine.UI;

public class MicrophoneHandler : MonoBehaviour
{
    private bool isSpeaking = false;

    private AudioClip record;
    public Text commandText;
    public Text buttonText;
    public Image commanderBackground;

    public GameObject gameController;
    private CommandController commandController;

    void Start(){
        commandController = gameController.GetComponent<CommandController>();
    }
   
    public void Press(){
        isSpeaking = !isSpeaking;

        commanderBackground.color = Color.black;
        if ( isSpeaking ){
            Debug.Log("recording");
            record = Microphone.Start ( null, false, 10, 16000 );
            buttonText.text = "หยุด";
            commandText.text = "...";
        }
        else {
            buttonText.text = "พูด";
            Debug.Log("saving");
            SavWav.Save("speak", record);
            Debug.Log("saved");
            RunCmd();
        }
    } 

    private void RunCmd(){
        string filepath = Path.Combine(Application.dataPath, "speak.wav");
        string textPath = Path.Combine(Application.dataPath, "speak.txt");
        Debug.Log(textPath);
        CallKardiProcess(filepath, textPath);
        ReadText();
    }

    private void ReadText(){
        string textPath = Path.Combine(Application.dataPath, "speak.txt");
        string[] lines = System.IO.File.ReadAllLines(textPath);

        string command;
        if ( lines.Length > 0 ){
            command = lines[lines.Length-1];
        }
        else {
            command = "";
        }
        commandText.text = command;
        commandController.SetCommand(command);
    }
 
     private  void CallKardiProcess (string path, string textPath)
     {
         Debug.Log("call kardi process");
         var processInfo = new System.Diagnostics.ProcessStartInfo("CMD.exe", 
            "/C sox " + path + " -t raw - | nc -w 1 localhost 5050 > " + textPath);
         processInfo.CreateNoWindow = true;
         processInfo.UseShellExecute = false;
 
        System.Diagnostics.Process process = System.Diagnostics.Process.Start(processInfo);


        Debug.Log("write speak text");
       
        process.WaitForExit();
        process.Close();
                 
     }

}
