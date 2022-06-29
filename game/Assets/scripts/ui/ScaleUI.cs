using UnityEngine;
using DG.Tweening;

public class ScaleUI : MonoBehaviour
{
    private AudioClip initSound;
    private AudioSource buttonAudio;

    public float scale;
    public float time;
    public float delay;

    // Start is called before the first frame update
    void Start()
    {       
        buttonAudio = GameObject.Find("MainUI").GetComponent<AudioSource>();
        initSound = GameObject.Find("AudioManager").GetComponent<AudioSourcesUI>().initButton;

        this.Wait(delay * 0.2f, () => {
            transform.DOScale(scale * Vector3.one, time).SetEase(Ease.OutBack);
            buttonAudio.PlayOneShot(initSound);
        });        
    }
}
