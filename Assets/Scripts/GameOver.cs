using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    public AudioSource board;
    public GameObject gameOverBackground;
    public GameObject[] hideGameObj;
    public GameObject[] showGameObj;
    // Start is called before the first frame update
    void Start()
    {
        board.Stop();

        gameOverBackground.SetActive(true);
        StartCoroutine(FadeImage());
        foreach (GameObject gameObject in hideGameObj)
        {
            gameObject.SetActive(false);
        }
        foreach (GameObject gameObject in showGameObj)
        {
            gameObject.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator FadeImage()
    {

        // loop over 1 second
        for (float i = 0; i <= 0.7f; i += Time.deltaTime/3)
        {
            // set color with i as alpha
            gameOverBackground.GetComponent<Image>().color = new Color(0, 0, 0, i);
            yield return null;
        }
        
    }

    public void SetWinner(int winner){
        if ( winner == 1 ){
            GetComponent<Text>().text = "YOU WIN!";
        }
        else {
            GetComponent<Text>().text = "YOU LOSE!";
        }
    }
}
