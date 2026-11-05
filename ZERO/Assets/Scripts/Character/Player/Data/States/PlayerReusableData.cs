using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerReusableData
{
    public Vector2 MovementInput { get; set; }
    public float SpeedModifier { get; set; } = 1f;

    private Vector3 currentTargetRotation;
    private Vector3 timeToReachTargetRotation = new Vector3(0f, 0.14f, 0f);
    private Vector3 dampedTargetRotationCurrentVelocity;
    private Vector3 dampedTargetRotationPassedTime;

    public ref Vector3 CurrentTargetRotation
    {
        get
        {
            return ref currentTargetRotation;
        }
    }

    public ref Vector3 TimeToReachTargetRotation
    {
        get
        {
            return ref timeToReachTargetRotation;
        }
    }

    public ref Vector3 DampedTargetRotationCurrentVelocity
    {
        get
        {
            return ref dampedTargetRotationCurrentVelocity;
        }
    }

    public ref Vector3 DampedTargetRotationPassedTime
    {
        get
        {
            return ref dampedTargetRotationPassedTime;
        }
    }
}
