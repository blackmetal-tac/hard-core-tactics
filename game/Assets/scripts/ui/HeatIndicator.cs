using UnityEngine;
using UnityEngine.UI;

public class HeatIndicator : MonoBehaviour
{
    public GameObject unit;
    private UnitManager unitManager;
    private Image heatImage;
    private CanvasGroup canvasGroup;
    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        canvasGroup = GetComponentInParent<CanvasGroup>();
        unitManager = unit.GetComponent<UnitManager>();
        heatImage = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        heatImage.fillAmount = unitManager.heat;
        canvasGroup.alpha = gameManager.crosshairBars + (unitManager.heat * 0.8f);
    }
}
