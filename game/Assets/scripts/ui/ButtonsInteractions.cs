using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonsInteractions : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public AudioClip hoverSound;

    // Start is called before the first frame update
    void Start()
    {
        LeanTween.reset();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        LeanTween.scaleX(gameObject, 1.1f, 0.1f);
        gameObject.GetComponent<AudioSource>().PlayOneShot(hoverSound);        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        LeanTween.scaleX(gameObject, 1, 0.1f);
    }
}
