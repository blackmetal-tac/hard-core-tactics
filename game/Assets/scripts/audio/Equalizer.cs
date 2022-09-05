using UnityEngine;
using UnityEngine.VFX;
using System;

public class Equalizer : MonoBehaviour
{
    private AudioData _ad;
    private AudioSource audioSource;
    private VisualEffect VFX;
    private Shield shield;
    private float bass, maxBass, B, C, D, E;

    // Start is called before the first frame update
    void Start()
    {
        _ad = GameObject.Find("AudioManager").GetComponent<AudioData>();
        audioSource = GameObject.Find("AudioManager").GetComponent<AudioSource>();
        VFX = GetComponent<VisualEffect>();
        shield = GetComponent<Shield>();
    }

    void FixedUpdate()
    {
        // Audio waves low to high, need only 6 parameters 
        // {{1-5}(bass!!!)-{30-46}(whisle!!!)} {7-{8-9-10}(hard hits!!!)-{15-24}(electric)-{26-29}(hard hits weak)-{29-44}(electric!!! cleaner 31-32-33)}
        // Bass
        maxBass = Mathf.Max(_ad.samples[0], _ad.samples[1], _ad.samples[2],
            _ad.samples[3], _ad.samples[4], _ad.samples[5]);

        if (maxBass > 0.22f * (1 - audioSource.volume))
        {
            bass = 200 * (maxBass) * (2 - audioSource.volume);
        }
        else
        {
            bass = maxBass;
        }

        
        //bass = 100 * (_ad.samples[0] + _ad.samples[1] + _ad.samples[2]
        //+ _ad.samples[3] + _ad.samples[4] + _ad.samples[5]);

        // Whisle
        B = 200 * (_ad.samples[30] + _ad.samples[31] + _ad.samples[32] 
            + _ad.samples[33] + _ad.samples[34] + _ad.samples[35]
            + _ad.samples[36] + _ad.samples[37] + _ad.samples[38]
            + _ad.samples[39] + _ad.samples[40] + _ad.samples[41]
            + _ad.samples[42] + _ad.samples[43] + _ad.samples[44]
            + _ad.samples[45] + _ad.samples[46] + _ad.samples[47]);

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
}
