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
        this.Wait(0.6f, () => 
        {

        });               
    }



}
