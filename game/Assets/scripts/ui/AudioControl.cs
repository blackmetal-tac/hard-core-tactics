using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioControl : MonoBehaviour
{
    public static AudioControl instance;
    public AudioMixer mainMixer;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        if (instance == null)
        {
            instance = this;
        }
        else 
        {
            Destroy(gameObject);
        }

        //Player audio settings
        if (PlayerPrefs.HasKey("MasterVolume"))
        {
            mainMixer.SetFloat("MasterVolume", PlayerPrefs.GetFloat("MasterVolume"));
        }
        else 
        {
            mainMixer.SetFloat("MasterVolume", 10);
        }


    }
}
