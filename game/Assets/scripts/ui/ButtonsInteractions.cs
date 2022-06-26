using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ButtonsInteractions : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private AudioClip hoverSound;
    private AudioSource buttonAudio;

    // Start is called before the first frame update
    void Start()
    {
        buttonAudio = GameObject.Find("MainUI").GetComponent<AudioSource>();
        hoverSound = GameObject.Find("AudioManager").GetComponent<AudioSourcesUI>().hoverButton;
    }
    
    //Buttons sound and animation
    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.DOScaleX(1.1f, 0.1f);
        buttonAudio.PlayOneShot(hoverSound);        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOScaleX(1, 0.1f);
    }
}
