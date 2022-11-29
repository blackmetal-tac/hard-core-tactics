using UnityEngine;
using TMPro;

public class SongName : MonoBehaviour
{
    [SerializeField] private int _textSpeed;
    private AudioClip _song;
    private TextMeshProUGUI _textmeshPro;
    private float _scrollPos;

    // Start is called before the first frame update
    void Start()
    {        
        _song = GameObject.Find("AudioManager").GetComponent<AudioSource>().clip;      
        _textmeshPro = GetComponent<TextMeshProUGUI>();  
        _scrollPos = transform.localPosition.x;
    }

    void FixedUpdate()
    {
        //Double strings and spaces
        _textmeshPro.text = "\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0" + _song.name.ToString() +
            "\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0" + _song.name.ToString();

        transform.localPosition = new Vector3(-_scrollPos,
            transform.localPosition.y, transform.localPosition.z);
        _scrollPos += Time.fixedDeltaTime * _textSpeed;

        if (-_textmeshPro.preferredWidth / 2 >= transform.localPosition.x)
        {
            _scrollPos = 0;
        }
    }
}
