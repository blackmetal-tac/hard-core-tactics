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
        if (_wpnManager.TargetAMS == null)
        {            
            _wpnManager.TargetAMS = collider.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        _wpnManager.TargetAMS = null;
    }
}
