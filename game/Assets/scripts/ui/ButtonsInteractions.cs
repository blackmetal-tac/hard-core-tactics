using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonsInteractions : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public AudioClip hoverSound;
    private AudioSource buttonAudio;

    // Start is called before the first frame update
    void Start()
    {
        buttonAudio = GameObject.Find("MainUI").GetComponent<AudioSource>();
        LeanTween.reset();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    //Buttons sound and animation
    public void OnPointerEnter(PointerEventData eventData)
    {
        LeanTween.scaleX(gameObject, 1.1f, 0.1f);
        buttonAudio.PlayOneShot(hoverSound);        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        LeanTween.scaleX(gameObject, 1, 0.1f);
    }
}
