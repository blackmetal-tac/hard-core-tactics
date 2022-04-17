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

    // Start is called before the first frame update
    void Awake()
    {
        song = GameObject.Find("BGM").GetComponent<AudioSource>().clip;
        textmeshPro = GetComponent<TextMeshProUGUI>();

        rectTransform = textmeshPro.GetComponent<RectTransform>();

        //clonetextmeshPro = Instantiate(textmeshPro) as TextMeshProUGUI;
        //RectTransform cloneRectTransform = clonetextmeshPro.GetComponent<RectTransform>();
        //cloneRectTransform.SetParent(rectTransform);
        //cloneRectTransform.anchorMin = new Vector2(1, 0.5f);
        //cloneRectTransform.localScale = new Vector3(1, 1, 1);
    }

    // Update is called once per frame
    void Update()
    {
        textmeshPro.text = song.name.ToString();
    }

    IEnumerator Start()
    {
        float width = textmeshPro.preferredWidth;
        Vector3 startPosition = rectTransform.position;

        float scrollPos = 0;

        while (true)
        {
            //if (textmeshPro.havePropertiesChanged)
            {
                //width = textmeshPro.preferredWidth;
                //clonetextmeshPro.text = textmeshPro.text;
            }

            textmeshPro.transform.position = new Vector3((-scrollPos % width) + startPosition.x, startPosition.y, startPosition.z);
            scrollPos += 1 * 20 * Time.deltaTime;

            yield return null;
        }
    }
}
