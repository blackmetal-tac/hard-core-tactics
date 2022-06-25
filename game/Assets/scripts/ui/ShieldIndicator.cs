using UnityEngine;
using UnityEngine.UI;

public class ShieldIndicator : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    private Image image;
    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        canvasGroup = GetComponentInParent<CanvasGroup>();
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        canvasGroup.alpha = gameManager.crosshairBars + ((1 - image.fillAmount) / 2);
    }
}
