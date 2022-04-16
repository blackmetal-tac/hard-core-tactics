using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private float followVelocity = 1;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        LeanTween.scale(gameObject, new Vector3(1,1.1f,1), 0.1f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //LeanTween.scaleX(gameObject, 1, 0.1f);
    }
}
