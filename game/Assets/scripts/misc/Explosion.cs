using UnityEngine;
using DG.Tweening;

public class Explosion : MonoBehaviour
{
    [HideInInspector] public float Damage;

    // Bullet collision 
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.name == "Body")
        {
            collider.GetComponent<UnitManager>().TakeDamage(Damage);
        }
    }

    public void Spawn(float size)
    {
        transform.parent.DOScale(size, 0.1f).SetLoops(2, LoopType.Yoyo);
    }
}
