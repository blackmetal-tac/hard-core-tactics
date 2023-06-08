using UnityEngine;

public class LegsController : MonoBehaviour
{
    // Joints    
    private Transform hips, leftHip, rightHip, leftKnee, rightKnee, leftKnee2, rightKnee2, leftFoot, rightFoot;

    [SerializeField] private float legsHeight;

    // Start is called before the first frame update
    void Start()
    {
        hips = GetComponentInChildren<Hips>().transform;
        leftHip = GetComponentInChildren<LeftHip>().transform;
        rightHip = GetComponentInChildren<RightHip>().transform;
        leftKnee = GetComponentInChildren<LeftKnee>().transform;
        rightKnee = GetComponentInChildren<RightKnee>().transform;
        leftKnee2 = GetComponentInChildren<LeftKnee2>().transform;
        rightKnee2 = GetComponentInChildren<RightKnee2>().transform;
        leftFoot = GetComponentInChildren<LeftFoot>().transform;
        rightFoot = GetComponentInChildren<RightFoot>().transform;
    }

    private void SetRotation(Transform joint, int rotation)
    {
        joint.localRotation = Quaternion.Euler(rotation, 0, 0);
    }

    public void Glide(bool left, float speed)
    {
        hips.transform.localPosition = Vector3.Slerp(hips.transform.localPosition, new Vector3(hips.transform.localPosition.x,
            hips.transform.localPosition.y, legsHeight - 0.04f), speed); // z - 0.04

        if (!left)
        {
            RotateJoint(hips, Quaternion.Euler(0, 5, 0), speed);  // y + 5 = right turn
            RotateJoint(leftHip, Quaternion.Euler(-15, 5, 0), speed); // y + 5
            RotateJoint(rightHip, Quaternion.Euler(-15, -5, 0), speed); // y - 5
            RotateJoint(leftKnee, Quaternion.Euler(100, 0, 0), speed); // x - 5
            RotateJoint(rightKnee, Quaternion.Euler(125, 0, 0), speed); // x + 20
            RotateJoint(leftKnee2, Quaternion.Euler(-115, 0, 0), speed); // x + 5
            RotateJoint(rightKnee2, Quaternion.Euler(-140, 0, 0), speed); // x - 20
            RotateJoint(leftFoot, Quaternion.Euler(30, -10, -10), speed); // y - 10 z - 10
            RotateJoint(rightFoot, Quaternion.Euler(30, 0, 0), speed); // ---
        }
        else
        {
            RotateJoint(hips, Quaternion.Euler(0, -5, 0), speed);  
            RotateJoint(leftHip, Quaternion.Euler(-15, 5, 0), speed); 
            RotateJoint(rightHip, Quaternion.Euler(-15, -5, 0), speed); 
            RotateJoint(leftKnee, Quaternion.Euler(125, 0, 0), speed); 
            RotateJoint(rightKnee, Quaternion.Euler(100, 0, 0), speed); 
            RotateJoint(leftKnee2, Quaternion.Euler(-140, 0, 0), speed); 
            RotateJoint(rightKnee2, Quaternion.Euler(-115, 0, 0), speed);
            RotateJoint(leftFoot, Quaternion.Euler(30, 0, 0), speed); 
            RotateJoint(rightFoot, Quaternion.Euler(30, 10, 10), speed);       
        }

    }

    public void Reset(float speed)
    {
        hips.transform.localPosition = Vector3.Slerp(hips.transform.localPosition, new Vector3(hips.transform.localPosition.x,
            hips.transform.localPosition.y, legsHeight), speed); 
        RotateJoint(hips, Quaternion.Euler(0, 0, 0), speed);  
        RotateJoint(leftHip, Quaternion.Euler(-15, 0, 0), speed); 
        RotateJoint(rightHip, Quaternion.Euler(-15, 0, 0), speed); 
        RotateJoint(leftKnee, Quaternion.Euler(105, 0, 0), speed); 
        RotateJoint(rightKnee, Quaternion.Euler(105, 0, 0), speed);
        RotateJoint(leftKnee2, Quaternion.Euler(-120, 0, 0), speed); 
        RotateJoint(rightKnee2, Quaternion.Euler(-120, 0, 0), speed); 
        RotateJoint(leftFoot, Quaternion.Euler(30, 0, 0), speed); 
        RotateJoint(rightFoot, Quaternion.Euler(30, 0, 0), speed); 
    }

    private void RotateJoint(Transform joint, Quaternion rotation, float speed)
    {
        joint.localRotation = Quaternion.Slerp(joint.localRotation, rotation, speed);
    }
}
