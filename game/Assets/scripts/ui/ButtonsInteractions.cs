using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ButtonsInteractions : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private AudioClip _hoverSound;
    private AudioSource _buttonAudio;

    // Start is called before the first frame update
    void Start()
    {
        _buttonAudio = GameObject.Find("MainUI").GetComponent<AudioSource>();
        _hoverSound = GameObject.Find("AudioManager").GetComponent<AudioSourcesUI>().HoverButton;
    }
    
    //Buttons sound and animation
    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.DOScaleX(1.1f, 0.1f);
        _buttonAudio.PlayOneShot(_hoverSound);        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOScaleX(1, 0.1f);
    }
}
