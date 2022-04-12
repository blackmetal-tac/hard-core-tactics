using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equalizer : MonoBehaviour
{
    public int waveLenght;
    public float waveScale;

    private AudioData audioData;

    // Start is called before the first frame update
    void Start()
    {
        audioData = GameObject.Find("BGM").GetComponent<AudioData>();
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<VisualEffect>().CoreSize = audioData.samples[1];
    }
}
