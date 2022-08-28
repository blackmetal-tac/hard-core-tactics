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
        }
        else
        {
            /*for (int i = 0; i < samles; i++)
            {
                GameObject instanceQuad = (GameObject)Instantiate(quadPreflab);
                instanceQuad.transform.position = this.transform.position;
                instanceQuad.transform.parent = this.transform;
                instanceQuad.transform.rotation = this.transform.rotation;
                instanceQuad.name = "Quad" + i;
                transform.rotation = Quaternion.Euler(0, 0, -1f * (i + 1));
                instanceQuad.transform.position = new Vector3(transform.position.x + distance * i, transform.position.y,
                   transform.position.z);
                quad[i] = instanceQuad;
            }*/

            float angle = 360f / (float)samles;
            for (int i = 0; i < samles; i++)
            {
                Quaternion rotation = Quaternion.AngleAxis(i * angle, Vector3.up);
                Vector3 direction = rotation * Vector3.forward;

                Vector3 position = transform.position + (direction * 10);
                Instantiate(quadPrefab, position, rotation);
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
                    quad[i].transform.localScale = new Vector3(width, audioData.samples[i] * amp * (i + 1) + height, 1);
                }
            }

            for (int i = 0; i < samles; i++)
            {
                if (quad != null)
                {
                    mQuad[i].transform.localScale = new Vector3(width, audioData.samples[i] * amp * (i + 1) + height, 1);
                }
            }
        }
        else 
        {
            for (int i = 0; i < samles; i++)
            {
                if (quad != null)
                {
                    quad[i].transform.localScale = new Vector3(width, audioData.samples[i] * amp * (i / 2 + 1) + height, 1);
                }
            }
        }
    }
}
