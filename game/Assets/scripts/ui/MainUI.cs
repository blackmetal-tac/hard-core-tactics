using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainUI : MonoBehaviour
{
    public AudioClip startUI;
    private AudioSource buttonAudio;

    // Start is called before the first frame update
    void Start()
    {
        buttonAudio = GetComponent<AudioSource>();
        buttonAudio.PlayOneShot(startUI);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
