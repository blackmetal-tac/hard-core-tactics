using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LoadUI : MonoBehaviour
{
    private Image loadingBar;
    private CanvasGroup canvasGroup;
    private GameObject initText;
    public float alpha = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        loadingBar = GetComponent<Image>();
        canvasGroup = GetComponent<CanvasGroup>();
        initText = GameObject.Find("Initializing");

        this.Progress(2f, () => {
            if (loadingBar.fillAmount < 1f)
            {
                loadingBar.fillAmount += Time.deltaTime * 0.6f;                
            }
            else
            {
                initText.transform.DOScale(Vector3.zero, 1f);
            }

            if (canvasGroup.alpha > alpha)
            {
                canvasGroup.alpha -= Time.deltaTime * 0.5f;
            }
        });
    }

    // Update is called once per frame
    void Update()
    {

    }
}
