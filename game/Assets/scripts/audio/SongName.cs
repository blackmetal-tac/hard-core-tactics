using UnityEngine;
using TMPro;

public class SongName : MonoBehaviour
{
    private AudioClip song;
    private TextMeshProUGUI textmeshPro;
    private float width, scrollPos, startPos, posX;
    private RectTransform rectTransform;

    // Start is called before the first frame update
    void Start()
    {
        song = GameObject.Find("AudioManager").GetComponent<AudioSource>().clip;      
        textmeshPro = GetComponent<TextMeshProUGUI>();
        scrollPos = 0;
        //startPos = textmeshPro.transform.position.x;
        startPos = transform.localPosition.x;
        posX = startPos;
    }

    // Update is called once per frame
    void Update()
    {       
        //Double strings and spaces
        textmeshPro.text = "\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0" + song.name.ToString() +
        "\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0" + song.name.ToString();

        //Set reset modifier for animation
        width = textmeshPro.preferredWidth / 2;        

        //Text animation
        //textmeshPro.transform.position = new Vector3(-scrollPos + posX,
          //  textmeshPro.transform.position.y, textmeshPro.transform.position.z);

        transform.localPosition = new Vector3(-scrollPos + posX,
            transform.localPosition.y, transform.localPosition.z);
        scrollPos += Time.deltaTime / 2000;

        Debug.Log(transform.localPosition.x);

        if (transform.localPosition.x <= -width)
        {
            scrollPos = 0;
            posX = startPos;
        }
    }
}
