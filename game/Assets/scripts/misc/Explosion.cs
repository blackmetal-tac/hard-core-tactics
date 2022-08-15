using UnityEngine;
using DG.Tweening;

public class Explosion : MonoBehaviour
{
    [HideInInspector] public float damage;

    // Bullet collision 
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.name == "Body")
        {
            collider.GetComponent<UnitManager>().TakeDamage(damage);
            Debug.Log(collider.transform.parent.name + " took damage");
        }

        //Debug.Log(collider.name);
    }

    public void Spawn(float size)
    {
        transform.parent.DOScale(size, 0.1f).SetLoops(2, LoopType.Yoyo);
        //transform.parent.localScale = size * Vector3.one;
    }
}
