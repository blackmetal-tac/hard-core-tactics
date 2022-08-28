using UnityEngine;
using UnityEngine.VFX;

public class Equalizer : MonoBehaviour
{
    private AudioData _ad;
    private VisualEffect VFX;
    private Shield shield;

    //Waves amplification
    private int amp;

    // Start is called before the first frame update
    void Start()
    {
        amp = 100;
        _ad = GameObject.Find("AudioManager").GetComponent<AudioData>();
        VFX = GetComponent<VisualEffect>();
        shield = GetComponent<Shield>();
    }

    void FixedUpdate()
    {
        //audio waves low to high, need only 6 parameters
        VFX.SetFloat("A", amp * (_ad.samples[0] + _ad.samples[1] + _ad.samples[2] +
            _ad.samples[3] + _ad.samples[4] + _ad.samples[5] +
            _ad.samples[6] + _ad.samples[7] + _ad.samples[8]));

        VFX.SetFloat("B", amp * 2 * (_ad.samples[9] + _ad.samples[10] + _ad.samples[11] +
            _ad.samples[12] + _ad.samples[13] + _ad.samples[14] +
            _ad.samples[15] + _ad.samples[16] + _ad.samples[17]));

        VFX.SetFloat("C", amp * 4 * (_ad.samples[18] + _ad.samples[19] + _ad.samples[20] +
            _ad.samples[21] + _ad.samples[22] + _ad.samples[23] +
            _ad.samples[24] + _ad.samples[25] + _ad.samples[26]));

        VFX.SetFloat("D", amp * 8 * (_ad.samples[27] + _ad.samples[28] + _ad.samples[29] +
            _ad.samples[30] + _ad.samples[31] + _ad.samples[32] +
            _ad.samples[33] + _ad.samples[34] + _ad.samples[35]));

        VFX.SetFloat("E", amp * 16 * (_ad.samples[36] + _ad.samples[37] + _ad.samples[38] +
            _ad.samples[39] + _ad.samples[40] + _ad.samples[41] +
            _ad.samples[42] + _ad.samples[43] + _ad.samples[44]));

        VFX.SetFloat("F", amp * 32 * (_ad.samples[45] + _ad.samples[46] + _ad.samples[47] +
            _ad.samples[48] + _ad.samples[49] + _ad.samples[50] +
            _ad.samples[51] + _ad.samples[52] + _ad.samples[53]));

        // Set shield scale
        if (shield != null)
        {
            //shield.scale = 10 * (_ad.samples[0] + _ad.samples[1] + _ad.samples[2]);
        }
    }
}
