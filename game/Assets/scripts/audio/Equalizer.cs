using UnityEngine;
using UnityEngine.VFX;

public class Equalizer : MonoBehaviour
{
    private AudioData audioData;
    private VisualEffect VFX;
    private Shield shield;    
    private const int arraySize = 18;
    private readonly float[] bassArray = new float[3];
    private readonly float[] whisleArray = new float[3];
    private readonly float[] hitsArray = new float[3];
    private readonly float[] electricArray = new float[3];    

    // Start is called before the first frame update
    void Start()
    {
        audioData = GameObject.Find("AudioData").GetComponent<AudioData>();
        VFX = GetComponent<VisualEffect>();        
        shield = GetComponent<Shield>();
    }

    void FixedUpdate()
    {
        // Audio waves low to high, need only 6 parameters 
        // {{0-28}(bass) {29-47}(whisle)} {48-251}(hard hits) {252-}(electric)}

        bassArray[0] = AnimateWave(bassArray, 0, 0.2f, 10);
        whisleArray[0] = AnimateWave(whisleArray, 29, 0.02f, 30);
        hitsArray[0] = AnimateWave(hitsArray, 79, 0.02f, 130);
        electricArray[0] = AnimateWave(electricArray, 255, 0.005f, 120);

        VFX.SetFloat("Bass", bassArray[0]);
        VFX.SetFloat("Whisle", whisleArray[0] * 100);       
        VFX.SetFloat("Hits", hitsArray[0] * 100);
        VFX.SetFloat("Sparks", electricArray[0] * 50);

        if (transform.parent.name == "MenuCore")
        {
            transform.parent.localScale = (2f + bassArray[0]) * Vector3.one;
        }

        // Set shield scale
        if (shield != null)
        {
            shield.scale = bassArray[0];
            shield.material.SetFloat("_Intensity", 2.5f + whisleArray[0] * 20);
        }
    }

    private float AnimateWave(float[] waveArray, int startIndex, float limit, int mult)
    {        
        float[] array = new float[arraySize];
        float maxValue;

        if (mult == 120)
        {
            for (int i = 0; i < 4; i++)
            {
                array[i] = audioData.samples[i + startIndex];
            }
        }
        else 
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = audioData.samples[i + startIndex];
            }
        }

        maxValue = Mathf.Max(array);
        if (mult == 120 && maxValue > 0.01f && maxValue < 0.014f)
        { 
            maxValue *= 5;
        }

        if (maxValue > limit)
        {
            waveArray[1] += mult * maxValue;
        }

        if (waveArray[1] > 0)
        {
            waveArray[1] -= Time.fixedDeltaTime + (waveArray[1] * 0.85f);
        }

        if (mult == 120)
        {            
            waveArray[2] = waveArray[1];
        }
        else 
        {
            waveArray[2] = Mathf.Lerp(waveArray[2], waveArray[1], Time.fixedDeltaTime * 3);
        }
        return waveArray[2];
    }
}
