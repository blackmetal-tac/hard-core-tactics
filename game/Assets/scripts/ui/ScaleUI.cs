using UnityEngine;
using DG.Tweening;

public class ScaleUI : MonoBehaviour
{
    private AudioClip _initSound;
    private AudioSource _buttonAudio;

    [SerializeField] private float _scale, _time, _delay;
    [SerializeField] private bool _playSound;
    
    // Start is called before the first frame update
    void Start()
    {       
        _buttonAudio = GameObject.Find("MainUI").GetComponent<AudioSource>();
        _initSound = GameObject.Find("AudioManager").GetComponent<AudioSourcesUI>().InitButton;

        this.Wait(_delay * 0.2f, () => {
            transform.DOScale(_scale * Vector3.one, _time).SetEase(Ease.OutBack);
            if (_playSound)
            {
                _buttonAudio.PlayOneShot(_initSound);                    
            }                       
        });     
    }
}
