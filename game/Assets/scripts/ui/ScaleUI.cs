using UnityEngine;
using DG.Tweening;

public class ScaleUI : MonoBehaviour
{
    private AudioClip initSound;
    private AudioSource buttonAudio;

    public float scale = 1f;
    public float time = 1f;
    public float delay = 0f;

    // Start is called before the first frame update
    void Start()
    {       
        buttonAudio = GameObject.Find("MainUI").GetComponent<AudioSource>();
        initSound = GameObject.Find("AudioManager").GetComponent<AudioSourcesUI>().initButton;

        this.Wait(delay * 0.2f, () => {
            transform.DOScale(Vector3.one * scale, time).SetEase(Ease.OutBack);
            buttonAudio.PlayOneShot(initSound);
        });        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
