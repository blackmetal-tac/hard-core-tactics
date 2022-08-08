using UnityEngine;

public class CollisionAMS : MonoBehaviour
{
    private WPNManager wpnManager;

    void Start()
    {
        wpnManager = GetComponentInParent<WPNManager>();
    }

    private void OnTriggerStay(Collider collider)
    {
        if (wpnManager.targetAMS == null)
        {            
            wpnManager.targetAMS = collider.gameObject;
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        Debug.Log("lost target");
        wpnManager.targetAMS = null;
    }
}
