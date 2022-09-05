using UnityEngine;

[RequireComponent (typeof(AudioSource))]
public class AudioData : MonoBehaviour
{
    AudioSource audioSource;
    public float[] samples;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void FixedUpdate()
    {
        GetSpectrumAudio();
    }

    //Get audio data from track for visualization
    void GetSpectrumAudio()
    {
        audioSource.GetSpectrumData(samples, 0, FFTWindow.Blackman);
    }
}
