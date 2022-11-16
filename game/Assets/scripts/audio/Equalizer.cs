using UnityEngine;
using UnityEngine.VFX;

public class Equalizer : MonoBehaviour
{
    private AudioData _audioData;
    private VisualEffect _VFX;
    private Shield _shield;    
    private const int arraySize = 18;
    private readonly float[] _bassArray = new float[3];
    private readonly float[] _whisleArray = new float[3];
    private readonly float[] _hitsArray = new float[3];
    private readonly float[] _electricArray = new float[3];    

    // Start is called before the first frame update
    void Start()
    {
        _audioData = GameObject.Find("AudioData").GetComponent<AudioData>();
        _VFX = GetComponent<VisualEffect>();        
        _shield = GetComponent<Shield>();
    }

    void FixedUpdate()
    {
        // Audio waves low to high, need only 6 parameters 
        // {{0-28}(bass) {29-47}(whisle)} {48-251}(hard hits) {252-}(electric)}

        _bassArray[0] = AnimateWave(_bassArray, 0, 0.2f, 10);
        _whisleArray[0] = AnimateWave(_whisleArray, 29, 0.02f, 30);
        _hitsArray[0] = AnimateWave(_hitsArray, 79, 0.02f, 130);
        _electricArray[0] = AnimateWave(_electricArray, 255, 0.005f, 120);

        _VFX.SetFloat("Bass", _bassArray[0]);
        _VFX.SetFloat("Whisle", _whisleArray[0] * 100);       
        _VFX.SetFloat("Hits", _hitsArray[0] * 100);
        _VFX.SetFloat("Sparks", _electricArray[0] * 50);

        if (transform.parent.name == "MenuCore")
        {
            transform.parent.localScale = (2f + _bassArray[0]) * Vector3.one;
        }

        // Set _shield scale
        if (_shield != null)
        {
            _shield.scale = _bassArray[0];
            _shield.material.SetFloat("_Intensity", 2.5f + _whisleArray[0] * 20);
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
                array[i] = _audioData.Samples[i + startIndex];
            }
        }
        else 
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = _audioData.Samples[i + startIndex];
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
