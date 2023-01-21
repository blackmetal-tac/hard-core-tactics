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
            && collider.transform.parent.transform.localScale != Vector3.zero)
        {            
            _wpnManager.TargetAMS = collider.gameObject;            
        }
        
        if (_wpnManager.TargetAMS == null && !_wpnManager.IsFriend && collider.gameObject.layer == 12
            && collider.transform.parent.transform.localScale != Vector3.zero)
        {            
            _wpnManager.TargetAMS = collider.gameObject;            
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (_wpnManager.TargetAMS == collider.gameObject)
        {            
            _wpnManager.TargetAMS = null;
        }        
    }
}
