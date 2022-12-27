using UnityEngine;
using UnityEngine.UI;

public class CoreButton : MonoBehaviour
{
    public UnitManager PlayerManager;
    private Button _button;

    // Start is called before the first frame update
    void Start()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(ButtonClick);
    }

    void ButtonClick()
    {
        PlayerManager.CoreOverdrive();
    }
}
