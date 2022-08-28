using UnityEngine;

public class Shield : MonoBehaviour
{
    public float scale;
    private float shieldSize;

    void FixedUpdate()
    {
        shieldSize = Mathf.Lerp(0, scale, Time.fixedDeltaTime);
        transform.localScale = (0.9f + shieldSize) * Vector3.one;
    }
}
