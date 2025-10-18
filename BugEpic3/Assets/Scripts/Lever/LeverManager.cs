using UnityEngine;

public class LeverManager : MonoBehaviour
{
    public Lever lever;
    public PushedObject pushedObject;
    public Transform targetPosition; 
    
    public float snapDistanceThreshold = 0.5f;
    
    [Range(0.1f, 5f)]
    public float pushSensitivity = 1f;
    
    public float NegativerotationMultiplier = 2f;
    public float rotationMultiplier = 2f;
    public bool IsSystemActive { get; private set; } = true;

    private void Start()
    {
        lever.Initialize(this);
    }
    
    public bool CheckIfShouldSnap(Vector2 currentPosition)
    {
        if (targetPosition == null) return false;
        float distance = Vector2.Distance(currentPosition, targetPosition.position);
        return distance <= snapDistanceThreshold;
    }

    public void OnLeverRotated(float rotationAngle)
    {
        if (rotationAngle>0)
        {
            float objectRotateAngle = rotationAngle * NegativerotationMultiplier;
            pushedObject.RotateAroundPivot(objectRotateAngle);
        }
        else if (rotationAngle < 0)
        {
            float objectRotateAngle = rotationAngle * rotationMultiplier;
            pushedObject.RotateAroundPivot(objectRotateAngle);
        }
    }

    public void OnLeverSnapped()
    {
        Debug.Log("杠杆已吸附到目标位置");
    }
    
    public void OnLeverReset()
    {
        if (pushedObject != null)
        { 
            pushedObject.ResetState(); 
        }
    }

}
