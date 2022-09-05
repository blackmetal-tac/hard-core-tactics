using UnityEngine;

public class Visualizer : MonoBehaviour
{
    public GameObject quadPrefab;
    private readonly static int samles = 32;
    GameObject[] quad = new GameObject[samles];
    GameObject[] mQuad = new GameObject[samles];
    private AudioData audioData;
    public float width = 0.1f, height = 0.5f, distance = 0.2f, amp = 0.5f;
    public bool mainMenu = true;
    public bool test = false;

    // Start is called before the first frame update
    void Start()
    {
        audioData = GameObject.Find("AudioManager").GetComponent<AudioData>();

        //Spawn waves visual
        if (mainMenu)
        {
            for (int i = 0; i < samles; i++)
            {
                GameObject instanceQuad = (GameObject)Instantiate(quadPrefab);
                instanceQuad.transform.position = this.transform.position;
                instanceQuad.transform.parent = this.transform;
                instanceQuad.name = "Quad" + i;
                instanceQuad.transform.position = new Vector3(transform.position.x + distance * i, transform.position.y,
                    transform.position.z);
                instanceQuad.transform.rotation = transform.rotation;
                quad[i] = instanceQuad;
            }

            for (int i = 0; i < samles; i++)
            {
                GameObject instanceQuad = (GameObject)Instantiate(quadPrefab);
                instanceQuad.transform.position = this.transform.position;
                instanceQuad.transform.parent = this.transform;
                instanceQuad.name = "mQuad" + i;
                instanceQuad.transform.position = new Vector3(transform.position.x - distance * i, transform.position.y,
                    transform.position.z);
                instanceQuad.transform.rotation = transform.rotation;
                mQuad[i] = instanceQuad;
            }
            transform.localScale = Vector3.zero;
        }
        else if (test)
        {
            float angle = 36f / (float)samles;
            for (int i = 0; i < samles; i++)
            {
                GameObject instanceQuad = (GameObject)Instantiate(quadPrefab);
                instanceQuad.transform.parent = this.transform;
                instanceQuad.name = "Quad" + i;

                transform.rotation = Quaternion.Euler(0, 0, 1 * ((i + 1) * angle));
                instanceQuad.transform.position = new Vector3(transform.position.x + distance * i, transform.position.y,
                   transform.position.z);
                instanceQuad.transform.rotation = Quaternion.Euler(transform.rotation.x,
                   transform.rotation.y, -1 * ((i + 1) * angle));

                quad[i] = instanceQuad;
            }
        }
        else
        {
            float angle = 36f / (float)samles;
            for (int i = 0; i < samles; i++)
            {
                GameObject instanceQuad = (GameObject)Instantiate(quadPrefab);
                instanceQuad.transform.parent = this.transform;
                instanceQuad.name = "Quad" + i;

                transform.rotation = Quaternion.Euler(0, 0, 1 * ((i + 1) * angle));
                instanceQuad.transform.position = new Vector3(transform.position.x + distance * i, transform.position.y,
                   transform.position.z);
                instanceQuad.transform.rotation = Quaternion.Euler(transform.rotation.x,
                    transform.rotation.y, -1 * ((i + 1) * angle));

                quad[i] = instanceQuad;
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Animate audio waves
        if (mainMenu)
        {
            for (int i = 0; i < samles; i++)
            {
                if (quad != null)
                {
                    quad[i].transform.localScale = new Vector3(width, audioData.samples[i] * amp + height, 1);
                }
            }

            for (int i = 0; i < samles; i++)
            {
                if (quad != null)
                {
                    mQuad[i].transform.localScale = new Vector3(width, audioData.samples[i] * amp + height, 1);
                }
            }
        }
        else if (test)
        {
            for (int i = 0; i < samles; i++)
            {
                if (quad != null)
                {                                                                                           
                    quad[i].transform.localScale = new Vector3(width, audioData.samples[i] * amp * (i + 1) + height, 1);
                }
            }
        }
        else
        {
            for (int i = 32; i < 64; i++)
            {
                if (quad != null)
                {                                                                            //* (i / 2 + 1)     
                    quad[i - 32].transform.localScale = new Vector3(width, audioData.samples[i] * amp * (i + 1) + height, 1);
                }
            }
        }
    }
}
