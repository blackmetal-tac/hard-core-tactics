using UnityEngine;

public class Shield : MonoBehaviour
{
    public float scale;
    private float shieldSize;

    void FixedUpdate()
    {
        //shieldSize = Mathf.Lerp(0, scale, Time.fixedDeltaTime * 50);
        transform.localScale = (0.9f + scale) * Vector3.one;
    }
}
