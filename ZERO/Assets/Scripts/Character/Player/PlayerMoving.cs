using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(PlayerInput))]
public class PlayerMoving : MonoBehaviour
{
    public MovementStateMachines movementStateMachines;
    public PlayerInput playerInput{get; private set;}
    public Rigidbody playerRigidbody{get; private set;}
    //引用相机Transform组件
    public Transform cameraTransform{get; private set;}
    

    void Awake()
    {
        movementStateMachines = new MovementStateMachines(this);
        playerInput = GetComponent<PlayerInput>();
        playerRigidbody = GetComponent<Rigidbody>();
        cameraTransform = Camera.main.transform;
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
