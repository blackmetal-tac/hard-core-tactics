using UnityEngine;
using UnityEditor;
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
            Yoyo();
        });               
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Yoyo()
    {
        transform.localScale = Vector3.one * (crosshairSize + playerManager.moveSpeed);
        transform.DOScale(Vector3.one * (crosshairSize + playerManager.moveSpeed / 5), 2f)
            .SetLoops(-1, LoopType.Yoyo);
    }
}
