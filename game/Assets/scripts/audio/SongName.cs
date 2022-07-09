using UnityEngine;
using TMPro;

public class SongName : MonoBehaviour
{
    public int textSpeed;
    private AudioClip song;
    private TextMeshProUGUI textmeshPro;
    private float scrollPos, startPos, posX;
    private bool isReady;

    // Start is called before the first frame update
    void Start()
    {
        song = GameObject.Find("AudioManager").GetComponent<AudioSource>().clip;      
        textmeshPro = GetComponent<TextMeshProUGUI>();

        this.Wait(1f, () =>
        {
            scrollPos = 0;
            startPos = transform.position.x;
            posX = startPos;
            isReady = true;
        });
    }

    // Update is called once per frame
    void Update()
    {
        //Double strings and spaces
        textmeshPro.text = "\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0" + song.name.ToString() +
            "\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0" + song.name.ToString();

        if (isReady)
        {
            transform.position = new Vector3(-scrollPos + posX,
                transform.position.y, transform.position.z);
            scrollPos += Time.deltaTime / textSpeed;

            if (transform.position.x / 2 >= -scrollPos)
            {
                scrollPos = 0.02f;
                posX = startPos;
            }
        }
    }
}
