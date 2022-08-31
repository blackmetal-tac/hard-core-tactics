using UnityEngine;
using UnityEngine.VFX;

public class Equalizer : MonoBehaviour
{
    private AudioData _ad;
    private VisualEffect VFX;
    private Shield shield;

    // Start is called before the first frame update
    void Start()
    {
        _ad = GameObject.Find("AudioManager").GetComponent<AudioData>();
        VFX = GetComponent<VisualEffect>();
        shield = GetComponent<Shield>();
    }

    void FixedUpdate()
    {
        // Audio waves high to low, need only 6 parameters 
        // {0-1-2(high!!!)-3-4-5(whisle!!!)} {7-{8-9-10}(hard hits!!!)-{15-24}(electric)-{26-29}(hard hits weak)-{29-44}(electric!!! cleaner 31-32-33)}
        VFX.SetFloat("Whisle", 100 * (_ad.samples[3] + _ad.samples[4] + _ad.samples[5]));
        VFX.SetFloat("Hits", 200 * (_ad.samples[8] + _ad.samples[9] + _ad.samples[10]));
        VFX.SetFloat("Sparks", 400 * (_ad.samples[31] + _ad.samples[32] + _ad.samples[33]));

        // Set shield scale
        if (shield != null)
        {
            //shield.scale = 10 * (_ad.samples[0] + _ad.samples[1] + _ad.samples[2]);
        }
    }
}
