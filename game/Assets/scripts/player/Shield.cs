using UnityEngine;

public class Shield : MonoBehaviour
{
    [HideInInspector] public float scale;
    [HideInInspector] public Material material;

    void Start()
    {
        material = GetComponentInChildren<MeshRenderer>().sharedMaterial;
    }

    void FixedUpdate()
    {        
        transform.localScale = (0.9f + scale) * Vector3.one;
    }
}
