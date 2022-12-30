using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class CoreButton : MonoBehaviour
{
    [HideInInspector] public UnitManager PlayerManager;
    private GameManager _gameManager;
    private Button _button;
    private AudioSource _audioUI;
    private AudioClip _buttonClick;
    private GameObject _buttonBorder, _actionMask; 
    private TextMeshProUGUI _switchText;
    private Tweener _tweener;

    // Start is called before the first frame update
    void Start()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(ButtonClick);
        _audioUI = GameObject.Find("MainUI").GetComponent<AudioSource>();
        _buttonClick = GameObject.Find("AudioManager").GetComponent<AudioSourcesUI>().ClickButton;
        _buttonBorder = _button.transform.Find("ButtonBorder").gameObject;
        _switchText = transform.Find("Switch").GetComponentInChildren<TextMeshProUGUI>();
        _tweener = _buttonBorder.transform.DOScaleX(1.1f, 1f).SetLoops(-1);
        _tweener.Pause();
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _actionMask = transform.parent.Find("ActionMask").gameObject;
    }

    void ButtonClick()
    {
        PlayerManager.CoreOverdrive();
        _audioUI.PlayOneShot(_buttonClick);
        _tweener.Pause();
        _buttonBorder.transform.localScale = Vector3.one;
        _buttonBorder.transform.DOScaleX(1.2f, 0.1f).SetLoops(4);

        this.Wait(MainMenu.ButtonDelay, () => 
        {
            _buttonBorder.transform.DOScaleX(1f, 0.1f);
        });   

        if (PlayerManager.CoreSwitch)
        {
            _switchText.text = "On";
            this.Wait(0.5f, () => {
                _buttonBorder.transform.localScale = Vector3.one;
                _tweener.Play();
            });
            
        }
        else
        {
            _switchText.text = "Off";        
        }
    }

    public void UpdateStatus() // ???
    {
        _actionMask.transform.localScale = Vector3.one;
        if (PlayerManager.CoreSwitch && PlayerManager.CoreDownTimer == 5)
        {
            _switchText.text = "On" + "\n" + 2 + " trns";
        }
        else if (PlayerManager.CoreSwitch && PlayerManager.CoreDownTimer == 4)
        {
            _switchText.text = "On" + "\n" + 1 + " turn";
        }
        else if (!PlayerManager.CoreSwitch && PlayerManager.CoreDownTimer > 1)
        {
            _tweener.Pause();
            _switchText.text = "Off" + "\n" + PlayerManager.CoreDownTimer + " trns";        
        }
        else if (!PlayerManager.CoreSwitch && PlayerManager.CoreDownTimer == 1)
        {
            _switchText.text = "Off" + "\n" + PlayerManager.CoreDownTimer + " turn"; 
        }
        else if (!PlayerManager.CoreSwitch && PlayerManager.CoreDownTimer <= 0 && !_gameManager.InAction)
        {
            _actionMask.transform.localScale = Vector3.zero;
            _switchText.text = "Off";
        }       
    }
}
