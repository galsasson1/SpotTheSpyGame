using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMManagerScript : MonoBehaviour
{
    public GameObject soundOnButton;
    public GameObject soundOffButton;
    public AudioSource BGMAudioSource;
    public static bool isVolumeEnabled = true;
    // Start is called before the first frame update
    
    public void TurnOffBGM()
    {
        BGMAudioSource.volume = 0;
        soundOnButton.SetActive(false);
        soundOffButton.SetActive(true);
        isVolumeEnabled = false;
    }

    public void TurnOnBGM()
    {
        BGMAudioSource.volume = 1;
        soundOnButton.SetActive(true);
        soundOffButton.SetActive(false);
        isVolumeEnabled = true;
    }
}
