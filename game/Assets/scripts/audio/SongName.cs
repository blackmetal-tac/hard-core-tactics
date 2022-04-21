using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SongName : MonoBehaviour
{
    private AudioClip song;
    private TextMeshProUGUI textmeshPro;
    private TextMeshProUGUI clonetextmeshPro;

    public static RectTransform rectTransform;
    public static Vector3 startPosition;
    private float scrollPos;

    // Start is called before the first frame update
    void Start()
    {
        song = GameObject.Find("BGM").GetComponent<AudioSource>().clip;
        textmeshPro = GetComponent<TextMeshProUGUI>();
        rectTransform = textmeshPro.GetComponent<RectTransform>();        
        scrollPos = 0;
        //startPosition = rectTransform.position;

        UpdateStartPos();
    }

    // Update is called once per frame
    void Update()
    {       
        textmeshPro.text = "\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0" + song.name.ToString() +           
        "\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0" + song.name.ToString();

        float width = textmeshPro.preferredWidth / 2;

        textmeshPro.transform.position = new Vector3((-scrollPos % width) + startPosition.x, startPosition.y, startPosition.z);
        scrollPos += 1 * 20 * Time.deltaTime;
    }

    public static void UpdateStartPos()
    {
        startPosition = rectTransform.position;
    }
}
