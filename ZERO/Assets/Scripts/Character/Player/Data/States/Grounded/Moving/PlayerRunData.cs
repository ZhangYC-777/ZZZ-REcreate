using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; 
[Serializable]
public class PlayerRunData
{
    [field: SerializeField, Range(1f, 2f)] public float speedModifier { get; private set; } = 1f;
}
