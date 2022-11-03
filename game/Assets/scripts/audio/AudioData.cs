using UnityEngine;

[RequireComponent (typeof(AudioSource))]
public class AudioData : MonoBehaviour
{
    AudioSource audioSource;
    [HideInInspector] public float[] samples;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void FixedUpdate()
    {
        GetAudioSpectrum();
    }

    //Get audio data from track for visualization
    void GetAudioSpectrum()
    {
        audioSource.GetSpectrumData(samples, 0, FFTWindow.Blackman);
    }
}


