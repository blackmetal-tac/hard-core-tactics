using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SongName : MonoBehaviour
{
    private AudioClip song;
    private static TextMeshProUGUI textmeshPro;
    private TextMeshProUGUI clonetextmeshPro;
    
    public static RectTransform maskPos;
    public static Vector3 startPosition;
    public static float resModifier;
    private static float width;
    private float scrollPos;

    // Start is called before the first frame update
    void Start()
    {
        song = GameObject.Find("AudioManager").GetComponent<AudioSource>().clip;
        maskPos = GameObject.Find("Mask").GetComponent<RectTransform>();
        textmeshPro = GetComponent<TextMeshProUGUI>();    
        scrollPos = 0;        

        UpdateStartPos();
    }

    // Update is called once per frame
    void Update()
    {       
        //Double string and spaces
        textmeshPro.text = "\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0" + song.name.ToString() +           
        "\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0" + song.name.ToString();

        //Set reset modifier for animation
        width = textmeshPro.preferredWidth / resModifier;        

        //Text animation
        textmeshPro.transform.position = new Vector3((-scrollPos % width) + startPosition.x, startPosition.y, startPosition.z);
        scrollPos += 1 * 20 * Time.deltaTime;
    }

    //Changes runnig text position and width to current resolution
    public static void UpdateStartPos()
    {
        startPosition = maskPos.position;

        if (Screen.width == 1920)
        {
            resModifier = 2;
        }
        else
        {
            resModifier = 3;
        }
    }
}
