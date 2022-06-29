using UnityEngine;
using DG.Tweening;

public class ScaleCockpit : MonoBehaviour
{
    //private AudioClip initSound;
    //private AudioSource buttonAudio;

    public float scale = 1f;
    public float time = 1f;
    public float delay = 0f;

    // Start is called before the first frame update
    void Start()
    {
        //buttonAudio = GameObject.Find("MainUI").GetComponent<AudioSource>();
        //initSound = GameObject.Find("AudioManager").GetComponent<AudioSourcesUI>().initButton;

        this.Wait(delay * 0.2f, () => {
            transform.DOScale(scale * Vector3.one, time).SetEase(Ease.OutBack, .5f);
            //buttonAudio.PlayOneShot(initSound);
        });
    }
}
