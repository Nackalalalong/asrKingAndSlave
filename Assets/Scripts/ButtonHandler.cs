using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonHandler : MonoBehaviour
{
    public void Restart(){
        SceneManager.LoadScene("PlayScene");
    }

    public void Play(){
        SceneManager.LoadScene("PlayScene");
    }

    public void Home(){
        SceneManager.LoadScene("HomeScene");
    }
}
