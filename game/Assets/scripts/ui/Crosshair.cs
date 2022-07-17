using UnityEngine;
using DG.Tweening;

public class Crosshair : MonoBehaviour
{
    public UnitManager playerManager;
    public float size = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        playerManager = GameObject.Find("Player").GetComponentInChildren<UnitManager>();
        this.Wait(.6f, () => 
        {
            //transform.DOScale((crosshairSize ) * Vector3.one, 2f);
            //Yoyo();+ playerManager.moveSpeed
        });               
    }

    void Update()
    {
        //transform.localScale = (crosshairSize) * Vector3.one;
    }

    public void Yoyo1()
    {
        transform.DOScale((size + playerManager.moveSpeed / 5) * Vector3.one, 2f)
            .SetLoops(-1, LoopType.Yoyo);
    }
}
