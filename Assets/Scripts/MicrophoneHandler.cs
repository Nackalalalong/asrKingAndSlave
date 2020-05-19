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

    private string command;
    void Start(){
        commandController = gameController.GetComponent<CommandController>();
        foreach (var device in Microphone.devices)
        {
            Debug.Log("Name: " + device);
        }
    }

    public void Gowajee(){
        Debug.Log("recording gowajee");
        record = Microphone.Start(null, false, 6, 16000 );
        StartCoroutine(Wait());
    }

    private IEnumerator Wait(){
        yield return new WaitForSeconds(6);

        SavWav.Save("speak", record);
        string filepath = Path.Combine(Application.dataPath, "speak.wav");
        string textPath = Path.Combine(Application.dataPath, "speak.txt");
        RunCmd(filepath, textPath);
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
            string filepath = Path.Combine(Application.dataPath, "speak.wav");
            string textPath = Path.Combine(Application.dataPath, "speak.txt");
            // var thread = new Thread(delegate () {RunCmd(filepath, textPath);});
            // thread.Start();
            RunCmd(filepath, textPath);
        }
    } 

    private void RunCmd(string filepath, string textPath){
        Debug.Log(textPath);
        CallKardiProcess(filepath, textPath);
        ReadText(textPath);
    }

    private void ReadText(string textPath){
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
