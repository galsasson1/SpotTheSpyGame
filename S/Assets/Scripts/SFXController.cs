using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXController : MonoBehaviour
{
    public AudioSource mainAudioSource;
    public AudioClip eliminatePlayerClickSound;
    public AudioClip targetingAnotherPlayer;

    //Plays the player eliminated sound
   public void PlayEliminatePlayerSound()
    {
        mainAudioSource.PlayOneShot(eliminatePlayerClickSound);
    }

    //Plays when you target another player
    public void PlayTargetOtherPlayerSound()
    {
        mainAudioSource.PlayOneShot(targetingAnotherPlayer);
    }
}
