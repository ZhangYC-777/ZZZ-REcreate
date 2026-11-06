using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[Serializable]
public class PlayerDashData
{
    [field: SerializeField] [field: Range(1f, 3f)] public float speedModifier { get; private set; } = 2f; 
}
