using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(AudioSource))]
public class AudioData : MonoBehaviour
{
    AudioSource audioSource;
    public float[] samples = new float[64];

    private float nextActionTime = 0.0f;
    private float period = 0.1f;

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
        if (Time.time > nextActionTime)
        {
            audioSource.GetSpectrumData(samples, 0, FFTWindow.Blackman);
        }
    }
}
