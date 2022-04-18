using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SongName : MonoBehaviour
{
    private AudioClip song;
    private TextMeshProUGUI textmeshPro;
    private TextMeshProUGUI clonetextmeshPro;

    private RectTransform rectTransform;
    private float scrollPos;
    private Vector3 startPosition;

    // Start is called before the first frame update
    void Awake()
    {
        song = GameObject.Find("BGM").GetComponent<AudioSource>().clip;
        textmeshPro = GetComponent<TextMeshProUGUI>();
        rectTransform = textmeshPro.GetComponent<RectTransform>();
        startPosition = rectTransform.position;
        scrollPos = 0;
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
}
