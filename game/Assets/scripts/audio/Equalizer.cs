using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Equalizer : MonoBehaviour
{
    private AudioData audioData;
    private VisualEffect VFX;

    //Waves amplification
    public int amp;

    // Start is called before the first frame update
    void Start()
    {
        amp = 100;
        audioData = GameObject.Find("AudioManager").GetComponent<AudioData>();
        VFX = GetComponent<VisualEffect>();
    }

    // Update is called once per frame
    void Update()
    {
            //audio waves low to high, need only 6 parameters
            VFX.SetFloat("A", (audioData.samples[0] + audioData.samples[1] + audioData.samples[2] +
                audioData.samples[3] + audioData.samples[4] + audioData.samples[5] +
                audioData.samples[6] + audioData.samples[7] + audioData.samples[8]) * amp);

            VFX.SetFloat("B", (audioData.samples[9] + audioData.samples[10] + audioData.samples[11] +
                audioData.samples[12] + audioData.samples[13] + audioData.samples[14] +
                audioData.samples[15] + audioData.samples[16] + audioData.samples[17]) * amp * 2);

            VFX.SetFloat("C", (audioData.samples[18] + audioData.samples[19] + audioData.samples[20] +
                audioData.samples[21] + audioData.samples[22] + audioData.samples[23] +
                audioData.samples[24] + audioData.samples[25] + audioData.samples[26]) * amp * 4);

            VFX.SetFloat("D", (audioData.samples[27] + audioData.samples[28] + audioData.samples[29] +
                audioData.samples[30] + audioData.samples[31] + audioData.samples[32] +
                audioData.samples[33] + audioData.samples[34] + audioData.samples[35]) * amp * 8);

            VFX.SetFloat("E", (audioData.samples[36] + audioData.samples[37] + audioData.samples[38] +
                audioData.samples[39] + audioData.samples[40] + audioData.samples[41] +
                audioData.samples[42] + audioData.samples[43] + audioData.samples[44]) * amp * 16);

            VFX.SetFloat("F", (audioData.samples[45] + audioData.samples[46] + audioData.samples[47] +
                audioData.samples[48] + audioData.samples[49] + audioData.samples[50] +
                audioData.samples[51] + audioData.samples[52] + audioData.samples[53]) * amp * 32);    
    }
}
