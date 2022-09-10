using UnityEngine;
using UnityEngine.VFX;
using System;

public class Equalizer : MonoBehaviour
{
    private AudioData _ad;
    private VisualEffect VFX;
    private Shield shield;    
    private const int arraySize = 18;
    private float[] bassArray = new float[3];
    private float[] whisleArray = new float[3];
    private float[] hitsArray = new float[3];
    private float[] electricArray = new float[3];    

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
        // {{0-28}(bass!!!)-{29-47}(whisle!!!)} {48-230}(hard hits!!!)-{15-24}(electric)-{26-29}(hard hits weak)-{29-44}(electric!!! cleaner 31-32-33)}

        bassArray[0] = AnimateWave(bassArray, 0, 0.2f, 10);
        whisleArray[0] = AnimateWave(whisleArray, 29, 0.02f, 30);
        hitsArray[0] = AnimateWave(hitsArray, 79, 0.02f, 120);
        electricArray[0] = AnimateWave(hitsArray, 231, 0.02f, 120);

        VFX.SetFloat("Whisle", whisleArray[0] * 4);        
        VFX.SetFloat("Hits", hitsArray[0] * 2);
        VFX.SetFloat("Sparks", electricArray[0] * 4);

        // Set shield scale
        if (shield != null)
        {
            shield.scale = electricArray[0];
        }
    }

    private float AnimateWave(float[] waveArray, int startIndex, float limit, int mult)
    {        
        float[] array = new float[arraySize];
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = _ad.samples[i + startIndex];
        }
        float maxValue = Mathf.Max(array);

        if (maxValue > limit)
        {
            waveArray[1] += mult * maxValue;
        }
        if (waveArray[1] > 0)
        {
            waveArray[1] -= Time.fixedDeltaTime + (waveArray[1] * 0.85f);
        }
        waveArray[2] = Mathf.Lerp(waveArray[2], waveArray[1], Time.fixedDeltaTime * 3);
        return waveArray[2];
    }
}
