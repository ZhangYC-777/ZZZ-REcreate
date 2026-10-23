using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(PlayerInput))]
public class PlayerMoving : MonoBehaviour
{
    public MovementStateMachines movementStateMachines;
    public PlayerInput playerInput{get; private set;}
    void Awake()
    {
        movementStateMachines = new MovementStateMachines();
        playerInput = GetComponent<PlayerInput>();
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
