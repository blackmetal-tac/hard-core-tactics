using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(AudioSource))]
public class AudioData : MonoBehaviour
{
    AudioSource audioSource;
    public float[] samples = new float[64];

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        GetSpectrumAudio();
    }

    void GetSpectrumAudio()
    {
        audioSource.GetSpectrumData(samples, 0, FFTWindow.Blackman);
    }
}
