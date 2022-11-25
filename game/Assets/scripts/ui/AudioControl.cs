using UnityEngine;
using UnityEngine.Audio;

public class AudioControl : MonoBehaviour
{
    private static AudioControl _instance;
    [SerializeField] private AudioMixer _mainMixer;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        if (_instance == null)
        {
            _instance = this;
        }
        else 
        {
            Destroy(gameObject);
        }

        //Player audio settings
        if (PlayerPrefs.HasKey("MasterVolume"))
        {
            _mainMixer.SetFloat("MasterVolume", PlayerPrefs.GetFloat("MasterVolume"));
        }
        else 
        {
            _mainMixer.SetFloat("MasterVolume", 10);
        }
    }
}
