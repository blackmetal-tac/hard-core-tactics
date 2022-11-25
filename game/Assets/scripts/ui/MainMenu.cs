using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string _level;
    private AudioSource _audioUI;
    private AudioClip _buttonClick;

    private static GameObject _optionsScreen;
    private GameObject _startButton, _openOptions, _exitButton, _startBorder, _optionsBorder, _exitBorder;

    public static float ButtonDelay = 0.4f;

    // Start is called before the first frame update
    void Start()
    {
        _optionsScreen = GameObject.Find("MainOptions");
        _startButton = GameObject.Find("StartButton");
        _openOptions = GameObject.Find("OptionsButton");        
        _exitButton = GameObject.Find("CloseButton");

        _startBorder = _startButton.transform.Find("ButtonBorder").gameObject;
        _optionsBorder = _openOptions.transform.Find("ButtonBorder").gameObject;
        _exitBorder = _exitButton.transform.Find("ButtonBorder").gameObject;

        _audioUI = GameObject.Find("MainUI").GetComponent<AudioSource>();
        _buttonClick = GameObject.Find("AudioManager").GetComponent<AudioSourcesUI>().ClickButton;
    }

    //Buttons
    public void StartGame()
    {
        _audioUI.PlayOneShot(_buttonClick);
        BorderAnim(_startBorder, 1.2f, 3);

        //Delay for animation
        this.Wait(ButtonDelay, ()=> {
            BorderAnim(_startBorder, 1f, 1);
            SceneManager.LoadScene(_level);
        });        
    }

    public void OpenMainOptions()
    {
        _audioUI.PlayOneShot(_buttonClick);
        BorderAnim(_optionsBorder, 1.2f, 3);

        //Delay for animation
        this.Wait(ButtonDelay, () => {
            BorderAnim(_optionsBorder, 1f, 1);
        });
        _optionsScreen.transform.DOScale(Vector3.one, ButtonDelay).SetEase(Ease.OutBack);
    }

    public void CloseGame()
    {
        _audioUI.PlayOneShot(_buttonClick);
        BorderAnim(_exitBorder, 1.2f, 3);

        //Delay for animation
        this.Wait(ButtonDelay, () => {
            BorderAnim(_exitBorder, 1f, 1);
            Application.Quit();            
        });        
    }

    public static void BorderAnim(GameObject gameObject, float scaleX, int loops)
    {
        gameObject.transform.DOScaleX(scaleX, 0.1f).SetLoops(loops);
    }

    public static void ScaleDown(GameObject gameObject) 
    {
        gameObject.transform.DOScale(Vector3.zero, 0.2f);
    }
}
