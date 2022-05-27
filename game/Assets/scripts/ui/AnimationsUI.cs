using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationsUI : MonoBehaviour
{
    private GameObject playerUI, targetUI;

    public float size = 0.2f;
    public float animOffset = 0.02f;        

    // Start is called before the first frame update
    void Start()
    {
        playerUI = GameObject.Find("PlayerUI");
        targetUI = GameObject.Find("Crosshair");

        //playerUI.transform.localScale = Vector3.one * size;
        targetUI.transform.localScale = Vector3.one * size;

        var leanSeq = LeanTween.sequence();

        //leanSeq.insert(LeanTween.scale(playerUI, new Vector3(size + animOffset,
        //   size + animOffset, size + animOffset), 1f)
        //   .setEaseInOutCubic().setLoopPingPong());

        //leanSeq.insert(LeanTween.scale(targetUI, new Vector3(size + animOffset,
        //   size + animOffset, size + animOffset), 1f)
        //  .setEaseInOutCubic().setLoopPingPong());

        LeanTween.scale(targetUI, new Vector3(size + animOffset,
            size + animOffset, size + animOffset), 1f)
            .setEaseInOutCubic().setLoopPingPong();

        //LeanTween.reset();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
