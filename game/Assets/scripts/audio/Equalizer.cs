using UnityEngine;
using UnityEngine.VFX;
using System;

public class Equalizer : MonoBehaviour
{
    private AudioData _ad;
    private VisualEffect VFX;
    private Shield shield;
    private float bass, bassAmp, whisle, whisleAmp, C, D, E;
    private const int arraySize = 18;

    // Start is called before the first frame update
    void Start()
    {
        _ad = GameObject.Find("AudioData").GetComponent<AudioData>();
        VFX = GetComponent<VisualEffect>();
        shield = GetComponent<Shield>();
    }

    void FixedUpdate()
    {
        // Audio waves low to high, need only 6 parameters 
        // {{0-28}(bass!!!)-{29-47}(whisle!!!)} {7-{8-9-10}(hard hits!!!)-{15-24}(electric)-{26-29}(hard hits weak)-{29-44}(electric!!! cleaner 31-32-33)}

        bass = AnimateWave(bassAmp, 0, 0.2f, 100, 6);

        // Whisle
        /*maxWhisle = Mathf.Max(
            _ad.samples[30], _ad.samples[31], _ad.samples[32],
            _ad.samples[33], _ad.samples[34], _ad.samples[35],
            _ad.samples[36], _ad.samples[37], _ad.samples[38],
            _ad.samples[39], _ad.samples[40], _ad.samples[41],
            _ad.samples[42], _ad.samples[43], _ad.samples[44],
            _ad.samples[45], _ad.samples[46], _ad.samples[47]);*/
        //whisle = AnimateWave(whisleAmp, 29, 0.04f, 60);

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

    private float AnimateWave(float waveAmp, int startIndex, float limit, int mult, int speed)
    {
        float wave = 0;
        float[] array = new float[arraySize];
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = _ad.samples[i + startIndex];
        }
        float maxValue = Mathf.Max(array);
        
        if (maxValue > limit)
        {
            waveAmp += mult * maxValue;
        }
        if (waveAmp > 0)
        {
            //waveAmp -= (Time.fixedDeltaTime + maxValue) / 2;
        }
        wave = Mathf.Lerp(wave, waveAmp, Time.fixedDeltaTime / speed);
        return wave;
    }
}
