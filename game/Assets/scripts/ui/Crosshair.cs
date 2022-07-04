using UnityEngine;
using DG.Tweening;

public class Crosshair : MonoBehaviour
{
    public UnitManager playerManager;
    public float crosshairSize = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        playerManager = GameObject.Find("Player").GetComponentInChildren<UnitManager>();
        this.Wait(.6f, () => 
        {
            transform.localScale = (crosshairSize + playerManager.moveSpeed) * Vector3.one;
            Yoyo();
        });               
    }

    public void Yoyo()
    {
        transform.DOScale((crosshairSize + playerManager.moveSpeed / 5) * Vector3.one, 2f)
            .SetLoops(-1, LoopType.Yoyo);
    }
}
