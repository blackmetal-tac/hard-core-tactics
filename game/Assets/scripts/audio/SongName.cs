using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SongName : MonoBehaviour
{
    private AudioClip song;
    private static TextMeshProUGUI textmeshPro;
    private TextMeshProUGUI clonetextmeshPro;

    //public static RectTransform rectTransform;
    public static RectTransform maskPos;
    public static Vector3 startPosition;
    public static float resModifier;
    private static float width;
    private float scrollPos;

    // Start is called before the first frame update
    void Start()
    {
        song = GameObject.Find("BGM").GetComponent<AudioSource>().clip;
        maskPos = GameObject.Find("Mask").GetComponent<RectTransform>();
        textmeshPro = GetComponent<TextMeshProUGUI>();    
        scrollPos = 0;        

        UpdateStartPos();
    }

    // Update is called once per frame
    void Update()
    {       
        textmeshPro.text = "\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0" + song.name.ToString() +           
        "\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0" + song.name.ToString();

        width = textmeshPro.preferredWidth / resModifier;

        //width = maskPos.sizeDelta.x ;

        textmeshPro.transform.position = new Vector3((-scrollPos % width) + startPosition.x, startPosition.y, startPosition.z);
        scrollPos += 1 * 20 * Time.deltaTime;
    }

    public static void UpdateStartPos()
    {
        startPosition = maskPos.position;
        resModifier = 2;
    }
}
