using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LoadUI : MonoBehaviour
{
    private Image _loadingBar;
    private CanvasGroup _canvasGroup;
    private GameObject _initText;
    private GameManager _gameManager;
    [SerializeField] private float _alpha;

    // Start is called before the first frame update
    void Start()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _loadingBar = GetComponent<Image>();
        _canvasGroup = GetComponent<CanvasGroup>();
        _initText = GameObject.Find("Initializing");

        this.Progress(_gameManager.LoadTime, () => {
            if (_loadingBar.fillAmount < 1f)
            {
                _loadingBar.fillAmount += Time.deltaTime * 0.6f;                
            }
            else
            {
                _initText.transform.DOScale(Vector3.zero, 1f);
            }

            if (_canvasGroup.alpha > _alpha)
            {
                _canvasGroup.alpha -= Time.deltaTime * 0.5f;
            }
        });
    }
}
