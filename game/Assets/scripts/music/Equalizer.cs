using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Equalizer : MonoBehaviour
{
    private AudioData audioData;
    private VisualEffect VFX;

    private float nextActionTime = 0.0f;
    private float averageWave = 0f;
    private int note = 4;

    public float period = 0.1f;
             
    // Start is called before the first frame update
    void Start()
    {
        audioData = GameObject.Find("BGM").GetComponent<AudioData>();
        VFX = GetComponent<VisualEffect>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > nextActionTime)
        {
             nextActionTime += period;
             //execute block of code here

             averageWave = (audioData.samples[note] + audioData.samples[note + 1] + audioData.samples[note + 2] +
             audioData.samples[note + 3])/4;

             //VFX.SetFloat("CoreSize", 2 + averageWave * 100);
        }        
    }
}
