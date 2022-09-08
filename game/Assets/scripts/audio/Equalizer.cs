using UnityEngine;
using UnityEngine.VFX;
using System;

public class Equalizer : MonoBehaviour
{
    private AudioData _ad;
    //private AudioSource audioSource;
    private VisualEffect VFX;
    private Shield shield;
    private float bass, maxBass, whisle, maxWhisle, C, D, E;
    private float[] bassArray = new float[18];

    // Start is called before the first frame update
    void Start()
    {
        _ad = GameObject.Find("AudioData").GetComponent<AudioData>();
        //audioSource = GameObject.Find("AudioData").GetComponent<AudioSource>();
        VFX = GetComponent<VisualEffect>();
        shield = GetComponent<Shield>();
    }

    void FixedUpdate()
    {
        // Audio waves low to high, need only 6 parameters 
        // {{0-29}(bass!!!)-{30-46}(whisle!!!)} {7-{8-9-10}(hard hits!!!)-{15-24}(electric)-{26-29}(hard hits weak)-{29-44}(electric!!! cleaner 31-32-33)}

        // Bass
        for (int i = 0; i < 17; i++)
        {
            bassArray[i] = _ad.samples[i];
        }
        maxBass = Mathf.Max(bassArray);
        bass = AnimateWave(maxBass, 0.2f, 100, 4);

        // Whisle
        maxWhisle = Mathf.Max(
            _ad.samples[30], _ad.samples[31], _ad.samples[32],
            _ad.samples[33], _ad.samples[34], _ad.samples[35],
            _ad.samples[36], _ad.samples[37], _ad.samples[38],
            _ad.samples[39], _ad.samples[40], _ad.samples[41],
            _ad.samples[42], _ad.samples[43], _ad.samples[44],
            _ad.samples[45], _ad.samples[46], _ad.samples[47]);
        whisle = AnimateWave(maxWhisle, 0.05f, 200, 4);

        //C = 300 * (_ad.samples[8] + _ad.samples[9] + _ad.samples[10]);
        //D = 600 * (_ad.samples[24] + _ad.samples[25] + _ad.samples[26]);
        //E = 600 * (_ad.samples[31] + _ad.samples[32] + _ad.samples[33]);

        //VFX.SetFloat("Whisle", bass * 4);        
        //VFX.SetFloat("Hits", B * 2);
        //VFX.SetFloat("Sparks", C * 4 + D);

        // Set shield scale
        if (shield != null)
        {
            shield.scale = bass;
        }
    }

    private float AnimateWave(float maxValue, float limit, int mult, int speed)
    {
        float wave = 0, multWave;
        if (maxValue > limit)
        {
            multWave = mult * maxBass;
        }
        else
        {
            multWave = 0;
        }
        wave = Mathf.Lerp(wave, multWave, Time.fixedDeltaTime / speed);
        return wave;
    }
}
