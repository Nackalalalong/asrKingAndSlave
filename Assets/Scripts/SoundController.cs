using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    public AudioClip cardMove;
    public AudioClip cardSpacing;
    public AudioClip youWin;
    public AudioClip youLose;
    public AudioClip awww;
    public AudioClip newRound;

    private AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayCardMove(){
        audioSource.PlayOneShot(cardMove);
    }

    public void PlayCardSpacing(){
        audioSource.PlayOneShot(cardSpacing);
    }

    public void PlayYouWin(){
        audioSource.PlayOneShot(youWin);
    }

    public void PlayYouLose(){
        audioSource.PlayOneShot(youLose);
    }

    public void PlayAwww(){
        audioSource.PlayOneShot(awww);
    }

    public void PlayNewRound(){
        audioSource.PlayOneShot(newRound);
    }
}
