using UnityEngine;

[RequireComponent (typeof(AudioSource))]
public class AudioData : MonoBehaviour
{
    private AudioSource _audioSource;
    [HideInInspector] public float[] Samples;

    // Start is called before the first frame update
    void Start()
    {
		Samples = new float[512];
        _audioSource = GetComponent<AudioSource>();		
    }

    void FixedUpdate()
    {
		//Get audio data from track for visualization
        _audioSource.GetSpectrumData(Samples, 0, FFTWindow.Blackman);
    }
}


