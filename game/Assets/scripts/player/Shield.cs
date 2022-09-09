using UnityEngine;

public class Shield : MonoBehaviour
{
    public float scale;

    void FixedUpdate()
    {        
        transform.localScale = (0.9f + scale) * Vector3.one;
    }
}
