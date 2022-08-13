using UnityEngine;
using TMPro;

public class SongName : MonoBehaviour
{
    public int textSpeed;
    private AudioClip song;
    private TextMeshProUGUI textmeshPro;
    private float scrollPos;


    // Start is called before the first frame update
    void Start()
    {
        song = GameObject.Find("AudioManager").GetComponent<AudioSource>().clip;      
        textmeshPro = GetComponent<TextMeshProUGUI>();  
        scrollPos = transform.localPosition.x;
    }

    void FixedUpdate()
    {
        //Double strings and spaces
        textmeshPro.text = "\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0" + song.name.ToString() +
            "\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0" + song.name.ToString();

        transform.localPosition = new Vector3(-scrollPos,
            transform.localPosition.y, transform.localPosition.z);
        scrollPos += Time.fixedDeltaTime * textSpeed;

        if (-textmeshPro.preferredWidth / 2 >= transform.localPosition.x)
        {
            scrollPos = 0;
        }
    }
}
