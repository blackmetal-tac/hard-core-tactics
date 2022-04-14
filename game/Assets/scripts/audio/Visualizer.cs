using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Visualizer : MonoBehaviour
{
    public GameObject quadPreflab;
    GameObject[] quad = new GameObject[64];
    private AudioData audioData;
    
    public float maxScale = 1;

    private float nextActionTime = 0.0f;
    private float period = 0.1f;
    // Start is called before the first frame update
    void Start()
    {
        audioData = GameObject.Find("BGM").GetComponent<AudioData>();
        for (int i = 0; i < 64; i++)
        {
            GameObject instanceQuad = (GameObject)Instantiate(quadPreflab);
            instanceQuad.transform.position = this.transform.position;
            instanceQuad.transform.parent = this.transform;
            instanceQuad.name = "Quad" + i;
            instanceQuad.transform.position = new Vector3(0.1f + 0.2f*i, 0, 0);
            quad[i] = instanceQuad;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > nextActionTime)
        {

            for (int i = 0; i < 64; i++)
            {
                if (quad != null)
                {
                    quad[i].transform.localScale = new Vector3(0.1f, audioData.samples[i] * maxScale + 0.5f, 1);
                }
            }
        }
    }
}
