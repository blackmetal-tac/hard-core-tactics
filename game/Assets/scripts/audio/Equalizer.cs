using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Equalizer : MonoBehaviour
{
    private AudioData audioData;
    private VisualEffect VFX;

    private float nextActionTime = 0.0f;
    public float period = 0.1f;

    public int amp = 200;

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
            
            //audio waves low to high
            VFX.SetFloat("A", audioData.samples[0] + audioData.samples[1] + audioData.samples[2] * amp);
            VFX.SetFloat("B", audioData.samples[3] + audioData.samples[4] + audioData.samples[5] * amp);
            VFX.SetFloat("C", audioData.samples[6] + audioData.samples[7] + audioData.samples[8] * amp);
            VFX.SetFloat("D", audioData.samples[9] + audioData.samples[10] + audioData.samples[11] * amp);

            VFX.SetFloat("E", audioData.samples[12] + audioData.samples[13] + audioData.samples[14] * amp * 2);
            VFX.SetFloat("F", audioData.samples[15] + audioData.samples[16] + audioData.samples[17] * amp * 2);
            VFX.SetFloat("G", audioData.samples[18] + audioData.samples[19] + audioData.samples[20] * amp * 2);
            VFX.SetFloat("H", audioData.samples[21] + audioData.samples[22] + audioData.samples[23] * amp * 2);

            VFX.SetFloat("I", audioData.samples[24] + audioData.samples[25] + audioData.samples[26] * amp * 3);
            VFX.SetFloat("J", audioData.samples[27] + audioData.samples[28] + audioData.samples[29] * amp * 3);
            VFX.SetFloat("K", audioData.samples[30] + audioData.samples[31] + audioData.samples[32] * amp * 3);
            VFX.SetFloat("L", audioData.samples[33] + audioData.samples[34] + audioData.samples[35] * amp * 3);

            VFX.SetFloat("M", audioData.samples[36] + audioData.samples[37] + audioData.samples[38] * amp * 4);
            VFX.SetFloat("N", audioData.samples[39] + audioData.samples[40] + audioData.samples[41] * amp * 4);
            VFX.SetFloat("O", audioData.samples[42] + audioData.samples[43] + audioData.samples[44] * amp * 4);
            VFX.SetFloat("P", audioData.samples[45] + audioData.samples[46] + audioData.samples[47] * amp * 4);
        }        
    }
}
