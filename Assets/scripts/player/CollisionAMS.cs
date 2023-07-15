using UnityEngine;

public class CollisionAMS : MonoBehaviour
{
    private WPNManager _wpnManager;

    void Start()
    {
        _wpnManager = GetComponentInParent<WPNManager>();
    }

    private void OnTriggerStay(Collider collider)
    {
        if (_wpnManager.TargetAMS == null && _wpnManager.IsFriend && collider.gameObject.layer == 13
            && collider.transform.parent.transform.localScale != Vector3.zero && !collider.GetComponent<Missile>().Locked)
        {            
            _wpnManager.TargetAMS = collider.transform;       
            collider.GetComponent<Missile>().Locked = true;
        }
        
        if (_wpnManager.TargetAMS == null && !_wpnManager.IsFriend && collider.gameObject.layer == 12
            && collider.transform.parent.transform.localScale != Vector3.zero && !collider.GetComponent<Missile>().Locked)
        {            
            _wpnManager.TargetAMS = collider.transform;  
            collider.GetComponent<Missile>().Locked = true;          
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (_wpnManager.TargetAMS == collider.transform)
        {            
            _wpnManager.TargetAMS = null;
        }        
    }
}
