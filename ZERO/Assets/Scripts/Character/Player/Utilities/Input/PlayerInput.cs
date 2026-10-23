using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
   public PlayerInputMap playerInputMap {get; private set;}
   public PlayerInputMap.PlayerActions playerActions {get; private set;}
   void Awake()
    {
        playerInputMap = new PlayerInputMap();
        playerActions = playerInputMap.Player;
    }
    void OnEnable()
    {
        playerInputMap.Enable();
    }
    void OnDisable()
    {
        playerInputMap.Disable();
    }
}
