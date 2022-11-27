using UnityEngine;
using DG.Tweening;

public class ScaleCockpit : MonoBehaviour
{
    //private AudioClip initSound;
    //private AudioSource buttonAudio;

    [SerializeField] private float _scale = 1f, _time = 1f, _delay = 0f;

    // Start is called before the first frame update
    void Start()
    {
        //buttonAudio = GameObject.Find("MainUI").GetComponent<AudioSource>();
        //initSound = GameObject.Find("AudioManager").GetComponent<AudioSourcesUI>().initButton;

        this.Wait(_delay * 0.2f, () => {
            transform.DOScale(_scale * Vector3.one, _time).SetEase(Ease.OutBack, .5f);
            //buttonAudio.PlayOneShot(initSound);
        });
    }
}
