using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoving : MonoBehaviour
{
    public MovementStateMachines movementStateMachines;     
    void Awake()
    {
        movementStateMachines = new MovementStateMachines();
    }
    void Start()
    {
        movementStateMachines.ChangeState(movementStateMachines.idlingState);
    }
    void Update()
    {
        movementStateMachines.HandleInput();
        movementStateMachines.Update();
    }
    void FixedUpdate()
    {
        movementStateMachines.PhysicsUpdate();
    }
}
