using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SongName : MonoBehaviour
{
    private AudioClip song;
    private static TextMeshProUGUI textmeshPro;
    
    public static RectTransform maskPos;
    public static RectTransform textPos;
    public static Vector3 startPosition;
    private static float width;
    private float scrollPos;

    // Start is called before the first frame update
    void Start()
    {
        song = GameObject.Find("AudioManager").GetComponent<AudioSource>().clip;
        maskPos = GameObject.Find("Mask").GetComponent<RectTransform>();
        textPos = this.gameObject.GetComponent<RectTransform>();
        textmeshPro = GetComponent<TextMeshProUGUI>();    
        scrollPos = 0;

        startPosition = maskPos.position;
    }

    // Update is called once per frame
    void Update()
    {       
        //Double string and spaces
        textmeshPro.text = "\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0" + song.name.ToString() +           
        "\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0" + song.name.ToString();

        //Set reset modifier for animation
        width = textmeshPro.preferredWidth / 2;

        //Text animation
        textmeshPro.transform.position = new Vector3(-scrollPos + startPosition.x, startPosition.y, startPosition.z);
        scrollPos += 1 * 1 * Time.deltaTime / 50;

        if (textPos.localPosition.x <= -width)
        {
            scrollPos = 0;
        }
    }
}
